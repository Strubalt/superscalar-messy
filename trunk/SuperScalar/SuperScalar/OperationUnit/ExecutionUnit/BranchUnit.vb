Public Class BranchUnit
    Inherits ExecutionUnit


    Private mInstructionSize As Integer
    Private mBranchTagTable As BranchTagTable
    Private mPCModule As PCModule
    Private mBranchPredicitonUnit As BranchPredictionUnit
    Private mControl As Processor.CProcessorControl

    Public Sub New(ByVal reorderBuffer As ReorderBuffer, ByVal rs As ReservationStations, ByVal instrSize As Integer, _
        ByVal branchTagTable As BranchTagTable, ByVal pcRegisterModule As PCModule, _
        ByVal brPredictUnit As BranchPredictionUnit, ByVal control As Processor.CProcessorControl)
        MyBase.New(reorderBuffer, rs)
        Me.mInstructionSize = instrSize
        Me.mBranchTagTable = branchTagTable
        Me.mPCModule = pcRegisterModule
        Me.mBranchPredicitonUnit = brPredictUnit
        Me.mControl = control
    End Sub


    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Property PCModule() As PCModule
        Get
            Return Me.mPCModule
        End Get
        Set(ByVal value As PCModule)
            Me.mPCModule = value
        End Set
    End Property

    Public Property BranchTagTable() As BranchTagTable
        Get
            Return mBranchTagTable
        End Get
        Set(ByVal value As BranchTagTable)
            mBranchTagTable = value
        End Set
    End Property

    Public Property BranchPredictionUnit() As BranchPredictionUnit
        Get
            Return mBranchPredicitonUnit
        End Get
        Set(ByVal value As BranchPredictionUnit)
            mBranchPredicitonUnit = value
        End Set
    End Property

    Public Overrides Sub Operate()
        Dim instr As RenamedInstruction = Me.InstructionRegister.Output
        If instr Is Nothing Then Exit Sub

        Dim addr As Integer
        Dim isTaken As Boolean = False
        Select Case CType(instr.SubExecuteType, ISA.BranchType)
            Case ISA.BranchType.C_Equal
                isTaken = IIf(instr.OpRob(0) = instr.OpRob(1), 1, 0)
                addr = ComputeConditionalTargetAddr(instr, isTaken)


            Case ISA.BranchType.C_NotEqual
                isTaken = IIf(instr.OpRob(0) <> instr.OpRob(1), 1, 0)
                addr = ComputeConditionalTargetAddr(instr, isTaken)

            Case ISA.BranchType.U_Addr, ISA.BranchType.U_JumpAndLink, ISA.BranchType.U_Register
                Debug.Assert(False)
                Exit Sub
        End Select
        'Dim bt As BranchTag = Me.mBranchTagTable.GetEntry(instr.BranchTag)
        Dim bt As BranchTag = Me.mBranchTagTable.GetEntry(instr.BranchTag)


        Dim misPrediction As Boolean = bt.PredictIsBranchTaken <> isTaken
        Me.mControl.SetBranchResult(misPrediction, instr.DecodedInstruction.PC, _
            isTaken, CType(instr.SubExecuteType, ISA.BranchType), addr, instr.BranchTag)

        Me.ReorderBuffer.SetSpeculationResolved(instr.RobIndex, _
            misPrediction, addr)
    End Sub

    Private Function ComputeConditionalTargetAddr(ByVal instr As RenamedInstruction, ByVal isTaken As Boolean) As Integer
        If isTaken Then
            Return instr.DecodedInstruction.PC + _
                        mInstructionSize + (instr.Immediate << 2)
        Else
            Return instr.DecodedInstruction.PC + _
                        mInstructionSize
        End If

    End Function

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.Branch
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Overrides Function FormatString() As String
        Dim ri As RenamedInstruction = Me.InstructionRegister.Output
        Dim instr As String = ""
        If Not ri Is Nothing Then
            instr = vbCrLf & "(" & Format(ri.RobIndex, "000") & ")" & _
                vbTab & ri.ToString
        End If
        Return "Branch: " & vbTab & instr
    End Function

    
End Class
