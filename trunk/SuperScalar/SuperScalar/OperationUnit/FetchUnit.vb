Imports ISA
Public Class FetchUnit
    Inherits OperationUnit



    Private mPCRegister As PCModule
    Private mInstructionCache As InstructionCache
    Private mOutputBuffer As DecodeBuffer

    Private mFetchWide As Integer
    Private mInstructionSize As Integer
    Private mIsCurrentCycleStall As Boolean

    Public Sub New(ByVal fetchWide As Integer, ByVal instrSize As Integer)
        mFetchWide = fetchWide
        Me.mInstructionSize = instrSize
    End Sub

    Public Property PCRegister() As PCModule
        Get
            Return mPCRegister
        End Get
        Set(ByVal value As PCModule)
            mPCRegister = value
        End Set
    End Property

    Public Property InstructionCache() As InstructionCache
        Get
            Return Me.mInstructionCache
        End Get
        Set(ByVal value As InstructionCache)
            Me.mInstructionCache = value
        End Set
    End Property

    Public Property OutputBuffer() As DecodeBuffer
        Get
            Return Me.mOutputBuffer
        End Get
        Set(ByVal value As DecodeBuffer)
            Me.mOutputBuffer = value
        End Set
    End Property

    Public Overrides Sub StallCurrentCycle()
        mIsCurrentCycleStall = True
    End Sub


    Public Overrides Sub Operate()
        If Me.OutputBuffer.IsFull Then Exit Sub
        If Not mIsCurrentCycleStall Then
            Dim instrValid() As Boolean = Nothing
            Dim instrBytes()() As Byte = Me.mInstructionCache.GetInstructions( _
                PCRegister.CurrentPC, Me.mFetchWide, instrValid)

            Dim instrs As New List(Of FetchedInstruction)
            'Dim hasBranch As Boolean = PCRegister.CurrentInstructionsContainBranch

            For i As Integer = 0 To Me.mFetchWide - 1
                If instrValid(i) Then
                    Dim instrAddress As Integer = PCRegister.CurrentPC + i * Me.mInstructionSize
                    Dim instr As New FetchedInstruction( instrAddress, instrBytes(i))

                    instrs.Add(instr)
                End If
            Next
            PCRegister.NumInstructionsFetched = instrs.Count
            Me.mOutputBuffer.SetInputData(instrs.ToArray())
        Else
            mIsCurrentCycleStall = False
        End If
    End Sub
End Class
