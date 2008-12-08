Public Class DecodeUnit
    Inherits OperationUnit

    Private mDecodeWidth As Integer
    Private mInstrSize As Integer
    Private mControl As Processor.CProcessorControl

    Public Sub New(ByVal decodeWidth As Integer, ByVal ras As ReturnAddressStack, _
        ByVal instrSize As Integer, ByVal controlUnit As Processor.CProcessorControl)
        Me.mDecodeWidth = decodeWidth
        mInstrSize = instrSize
        Me.mReturnAddressStack = ras
        mControl = controlUnit
    End Sub

    Private mDecodeBuffer As DecodeBuffer
    Private mOutputQueue As FixedSizeQueue(Of DecodedInstruction)
    Private mIsCurrentCycleStall As Boolean
    Private mBranchTagTable As BranchTagTable
    Private mReturnAddressStack As ReturnAddressStack

    Public Property DecodeBuffer() As DecodeBuffer
        Get
            Return Me.mDecodeBuffer
        End Get
        Set(ByVal value As DecodeBuffer)
            Me.mDecodeBuffer = value
        End Set
    End Property

    Public Property OutputQueue() As FixedSizeQueue(Of DecodedInstruction)
        Get
            Return Me.mOutputQueue
        End Get
        Set(ByVal value As FixedSizeQueue(Of DecodedInstruction))
            Me.mOutputQueue = value
        End Set
    End Property

    Public Property BranchTagTable() As BranchTagTable
        Get
            Return Me.mBranchTagTable
        End Get
        Set(ByVal value As BranchTagTable)
            Me.mBranchTagTable = value
        End Set
    End Property

    Public Property ReturnAddressStack() As ReturnAddressStack
        Get
            Return Me.mReturnAddressStack
        End Get
        Set(ByVal value As ReturnAddressStack)
            Me.mReturnAddressStack = value
        End Set
    End Property


    Public Overrides Sub Operate()
        If Me.mOutputQueue.IsFull Then Exit Sub
        If Not Me.mIsCurrentCycleStall Then
            Dim decodedInstructions As New List(Of DecodedInstruction)
            Dim decoded As DecodedInstruction = Nothing
            Dim numConsumed As Integer = 0
            Dim isStall As Boolean
            Dim max As Integer = Math.Min(Me.mBranchTagTable.GetNumEmpty, Me.mOutputQueue.NumEmpty)
            For i As Integer = 0 To Math.Min(Me.mDecodeWidth, max) - 1
                Dim unconBranch As Boolean = False
                If Me.mDecodeBuffer.OutputValid(i) Then
                    numConsumed += 1
                    Dim instr As FetchedInstruction = Me.mDecodeBuffer.Output(i)

                    Dim info As ISA.InstrDecodedInfo = _
                        ISA.InstructionDecoder.DecodeOneInstruction(instr.InstrData)

                    Dim isTaken As Boolean, target As Integer
                    If IsBranchInstruction(info) Then
                        isTaken = instr.IsTaken
                        target = instr.TargetBranchAddress
                        If CheckIfMissDetectedBranch(instr, info) Then

                            Me.StaticPredictBranch(info, instr.PC, isTaken, target, isStall)
                            If Me.IsConditional(info) Then
                                Me.mBranchTagTable.Add(New BranchTag( _
                                    instr.PC, isTaken, target))
                            Else
                                unconBranch = True
                            End If
                            If Not isStall Then
                                OutputMissDetectSignals(instr.PC, isTaken, CType(info.DetailDecodedData.SubExecuteType, ISA.BranchType), target, -1)
                            End If

                        Else
                            If Me.IsConditional(info) Then
                                Me.mBranchTagTable.Add(New BranchTag( _
                                    instr.PC, instr.IsTaken, instr.TargetBranchAddress))
                            End If

                        End If
                    End If
                    If Not unconBranch And Not isStall Then
                        decoded = New DecodedInstruction(Me.mBranchTagTable.GetCurrentTag, instr)
                        decodedInstructions.Add(decoded)
                    End If

                    Me.mOutputQueue.SetInputData(decodedInstructions.ToArray())
                    If isStall Then
                        numConsumed -= 1
                    ElseIf isTaken Then
                        numConsumed += GetNumMoreInstrToDrain(i, target)
                        
                        Exit For
                    End If

                Else
                    Exit For
                End If
            Next
            Me.mDecodeBuffer.ItemsConsumed = numConsumed
        Else
        End If
        mIsCurrentCycleStall = False
    End Sub


    Private Sub OutputMissDetectSignals(ByVal brInstrAddr As Integer, _
        ByVal isTaken As Boolean, ByVal branchType As ISA.BranchType, _
        ByVal branchTargetAddr As Integer, ByVal tag As Integer)
        Me.mControl.SetMissedDetectedBranch(brInstrAddr, isTaken, _
            branchType, branchTargetAddr, tag)

        'Debug.Assert(False)
    End Sub

    Private Function GetNumMoreInstrToDrain(ByVal currentIndex As Integer, ByVal brTargetAddress As Integer) As Integer
        Dim num As Integer = 0
        For idx As Integer = currentIndex + 1 To Me.mDecodeBuffer.MaxSize - 1
            If Me.mDecodeBuffer.OutputValid(idx) Then
                If Me.mDecodeBuffer.Output(idx).PC <> brTargetAddress Then
                    num += 1
                Else
                    Dim min As Integer = Math.Min(Me.mBranchTagTable.GetNumEmpty, Me.mOutputQueue.NumEmpty)
                    min = Math.Min(Me.mDecodeWidth, min)
                    For i As Integer = idx To Math.Min(Me.mDecodeWidth, min) - 1
                        num += 1
                    Next
                    Exit For
                End If
            End If
        Next
        Return num
    End Function

    Private Function IsConditional(ByVal decoded As ISA.InstrDecodedInfo) As Boolean
        Debug.Assert(decoded.DetailDecodedData.ExecuteType = ISA.ExecuteType.Branch)
        Select Case CType(decoded.DetailDecodedData.SubExecuteType, ISA.BranchType)
            Case ISA.BranchType.C_Equal, ISA.BranchType.C_NotEqual
                Return True
            Case Else
                Return False
        End Select

    End Function

    Private Function IsBranchInstruction(ByVal decoded As ISA.InstrDecodedInfo) As Boolean
        Return decoded.DetailDecodedData.ExecuteType = ISA.ExecuteType.Branch 
    End Function

    Private Function CheckIfMissDetectedBranch(ByVal fetched As FetchedInstruction, _
        ByVal decoded As ISA.InstrDecodedInfo) As Boolean

        If IsBranchInstruction(decoded) Then
            Return Not fetched.DetectedAsBranch
        Else
            Return False
        End If
    End Function

    Private Sub StaticPredictBranch(ByVal decoded As ISA.InstrDecodedInfo, _
        ByVal instrAddr As Integer, ByRef isTaken As Boolean, _
        ByRef targetAddr As Integer, ByRef stall As Boolean)

        isTaken = True
        If decoded.DetailDecodedData.ExecuteType = ISA.ExecuteType.Branch Then

            Dim type As ISA.BranchType = decoded.DetailDecodedData.SubExecuteType
            Dim constField As Integer = decoded.GetExtendedImmediate
            Dim isSpeculative As Boolean = Me.mBranchTagTable.IsSpeculative(Me.mBranchTagTable.GetCurrentTag)

            Select Case type
                Case ISA.BranchType.U_Addr, ISA.BranchType.U_JumpAndLink
                    targetAddr = (instrAddr And &HF0000000) Or (constField << 2)
                    If type = ISA.BranchType.U_JumpAndLink Then
                        Me.mReturnAddressStack.Push(isSpeculative, _
                            Me.mBranchTagTable.GetCurrentTag, _
                            instrAddr + Me.mInstrSize)
                    End If
                Case ISA.BranchType.U_Register
                    Dim register As Integer = decoded.GetSourceRegister(0)
                    'Implement Jump Register as return only
                    Debug.Assert(register = CInt(ISA.RegisterEncoding.ra))
                    Dim brTag As Integer
                    If Me.mReturnAddressStack.HasEntry Then
                        Me.mReturnAddressStack.Pop(isSpeculative, Me.mBranchTagTable.GetCurrentTag, brTag, targetAddr)
                    Else
                        stall = True
                    End If

                Case ISA.BranchType.C_Equal, ISA.BranchType.C_NotEqual
                    If constField < 0 Then
                        isTaken = True
                        targetAddr = instrAddr + Me.mInstrSize + constField << 2
                    Else
                        isTaken = False
                        targetAddr = instrAddr + Me.mInstrSize
                    End If
            End Select

        Else
            isTaken = False
        End If
    End Sub


    Public Overrides Sub StallCurrentCycle()
        mIsCurrentCycleStall = True
    End Sub

    
End Class
