Public Class IssueUnit
    Inherits OperationUnit


    Private mIssueBuffer As FixedSizeQueue(Of DecodedInstruction)
    Private mReservationStation As ReservationStations
    Private mReorderBuffer As ReorderBuffer
    Private mRegisterFile As GPPRegisterFile
    Private mIsCurrentCycleStall As Boolean
    Private mIssueWidth As Integer
    Private mBranchTagTable As BranchTagTable

    Public Sub New(ByVal issueWide As Integer, ByVal brTagTable As BranchTagTable)
        Me.mIssueWidth = issueWide
        Me.mBranchTagTable = brTagTable
    End Sub

    Public Property IssueBuffer() As FixedSizeQueue(Of DecodedInstruction)
        Get
            Return mIssueBuffer
        End Get
        Set(ByVal value As FixedSizeQueue(Of DecodedInstruction))
            mIssueBuffer = value
        End Set
    End Property

    Public Property ReservationStation() As ReservationStations
        Get
            Return Me.mReservationStation
        End Get
        Set(ByVal value As ReservationStations)
            Me.mReservationStation = value
        End Set
    End Property

    Public Property RegisterFile() As GPPRegisterFile
        Get
            Return Me.mRegisterFile
        End Get
        Set(ByVal value As GPPRegisterFile)
            Me.mRegisterFile = value
        End Set
    End Property

    Public Property ReorderBuffer() As ReorderBuffer
        Get
            Return Me.mReorderBuffer
        End Get
        Set(ByVal value As ReorderBuffer)
            Me.mReorderBuffer = value
        End Set
    End Property

    Public Overrides Sub Operate()
        If mIsCurrentCycleStall Then
            mIsCurrentCycleStall = False
            Exit Sub
        End If
        
        Dim robEntries As List(Of RobEntry) = CreateNewRobEntries()

        Dim renamedInstructions As List(Of RenamedInstruction) = _
            CreateRenamedInstructions(robEntries)
        
        Me.ReorderBuffer.AddEntry(robEntries)
        Me.ReservationStation.SetInputRenamedInstructions(renamedInstructions)
        Me.mIssueBuffer.ItemsConsumed = renamedInstructions.Count


    End Sub

    Private Function CreateRenamedInstructions(ByVal robEntries As List(Of RobEntry)) As List(Of RenamedInstruction)
        Dim freeRobIndexes As List(Of Integer) = _
            ReorderBuffer.GetEmptyEntryIndex(robEntries.Count)
        Dim results As New List(Of RenamedInstruction)

        For i As Integer = 0 To robEntries.Count - 1
            Dim decoded As DecodedInstruction = robEntries(i).Instruction

            Dim robOp() As Integer = Nothing
            Dim isValid() As Boolean = Nothing
            GetOperandRobIndex(decoded, robOp, isValid)

            Dim renamed As New RenamedInstruction(freeRobIndexes(i), _
                robEntries(i).Instruction, robOp(0), isValid(0), _
                robOp(1), isValid(1), decoded.ImmediateValue)
            results.Add(renamed)
            'Modify Rename Map
            If decoded.IsWriteBackToRegister Then
                Dim isSpeculative As Boolean = Me.mBranchTagTable.IsSpeculative(robEntries(i).BranchTag)
                Me.mRegisterFile.AddRenamedInfo(isSpeculative, robEntries(i).BranchTag, _
                    robEntries(i).Instruction.TargetRegisterIndex, freeRobIndexes(i))
            End If

        Next
        Return results
    End Function

    Private Function CreateNewRobEntries() As List(Of RobEntry)
        Dim numRobEntry As Integer = Me.ReorderBuffer.GetNumEmptyEntries
        Dim numRsEntry As Integer = Me.ReservationStation.GetNumEmptyEntryies
        Dim maxIssueInstr As Integer = Math.Min(Me.mIssueWidth, numRobEntry)

        maxIssueInstr = Math.Min(maxIssueInstr, numRsEntry)

        Dim robEntries As New List(Of RobEntry)

        For i As Integer = 0 To maxIssueInstr - 1
            If Me.mIssueBuffer.OutputValid(i) Then
                Dim instr As DecodedInstruction = Me.mIssueBuffer.Output(i)
                Dim isSpeculative As Boolean = Me.mBranchTagTable.IsSpeculative(instr.BranchTag)
                Dim entry As New RobEntry(instr, isSpeculative)
                robEntries.Add(entry)
            Else
                Exit For
            End If
        Next
        Return robEntries
    End Function

    Private Sub GetOperandRobIndex(ByVal decoded As DecodedInstruction, _
        ByRef robOp() As Integer, ByRef isValid() As Boolean)

        ReDim robOp(1)
        isValid = New Boolean() {True, True} 'set to true if no register is required
        'it will simplify the logic in dispatching


        'Retrieve Operands or ROB Index
        For opIdx As Integer = 0 To decoded.NumSourceRegister - 1
            Me.RegisterFile.GetRegisterValueWithNewRenamed( _
                decoded.SrcRegisterIndex(opIdx), decoded.IsHigher4ByteValue, _
                isValid(opIdx), robOp(opIdx))
            If Not isValid(opIdx) Then
                Dim robResult As Long
                Me.ReorderBuffer.GetResult(robOp(opIdx), _
                     decoded.IsHigher4ByteValue, isValid(opIdx), robResult)
                If isValid(opIdx) Then robOp(opIdx) = robResult
            End If
        Next
    End Sub

    'Private Function GetHigh4BytesOp(ByVal decoded As DecodedInstruction) As Boolean
    '    If decoded.ExecuteType = ISA.ExecuteType.FxSimple Then
    '        If CType(decoded.SubExecuteType, ISA.ALUSimpleType) = ISA.ALUSimpleType.MfHigh Then
    '            Return True
    '        End If
    '    End If
    'End Function

    Public Overrides Sub StallCurrentCycle()
        mIsCurrentCycleStall = True
    End Sub
End Class
