Imports ISA
Public Class BranchPredictionUnit
    Inherits OperationUnit

    'Inherits ExecutionUnit

    'Private mIsCurrentCycleStall As Boolean
    Private mBranchPredictor As BaseBranchPredictor
    Private mFetchSize As Integer
    Private mInstructionSize As Integer
    Private mDecodeBuffer As DecodeBuffer
    Public Sub New(ByVal pcModule As PCModule, ByVal predictor As BaseBranchPredictor, _
        ByVal fetchSize As Integer, ByVal instructSize As Integer, _
        ByVal decodeBuffer As DecodeBuffer)
        Me.mPCRegisterModule = pcModule
        Me.mBranchPredictor = predictor
        Me.mInstructionSize = instructSize
        Me.mFetchSize = fetchSize
        Me.mDecodeBuffer = decodeBuffer
    End Sub

    Private mPCRegisterModule As PCModule

    Public Property PCRegister() As PCModule
        Get
            Return Me.mPCRegisterModule
        End Get
        Set(ByVal value As PCModule)
            Me.mPCRegisterModule = value
        End Set
    End Property



    'Only processor conditional branch
    Friend Sub SetBranchResult(ByVal result As BranchResult)
        If result.BranchType = BranchType.C_Equal OrElse result.BranchType = BranchType.C_NotEqual Then
            Me.mBranchPredictor.SetBranchResult(result)
        End If
    End Sub

    Public Overrides Sub Operate()
        'If mIsStall Then
        '    mIsStall = False
        '    Exit Sub
        'End If
        Dim currentPC As Integer = Me.mPCRegisterModule.CurrentPC
        Dim results As New List(Of BranchPredictionResult)
        For i As Integer = 0 To Me.mFetchSize - 1
            Dim instrAddr As Integer = currentPC + i * Me.mInstructionSize
            Dim result As BranchPredictionResult = Me.mBranchPredictor.Predict(instrAddr)
            results.Add(result)
            If result.IsBranchType AndAlso result.IsTakenBranch Then
                Me.mPCRegisterModule.SetPredictionResult(True, _
                    instrAddr, result.IsTakenBranch, result.targetAddress)
                Exit For
            End If
        Next
        Me.mDecodeBuffer.SetBranchPreditionResult(results)

    End Sub

    Private mIsStall As Boolean
    Public Overrides Sub StallCurrentCycle()
        mIsStall = True
    End Sub
End Class

Friend Class BranchResult

    Public Sub New(ByVal misPredict As Boolean, ByVal brInstrAddr As Integer, _
        ByVal isTaken As Boolean, ByVal branchType As ISA.BranchType, _
        ByVal branchTargetAddr As Integer, ByVal tag As Integer)

        Me.MissPrediction = misPredict
        Me.BranchInstrAddr = brInstrAddr
        Me.IsTaken = isTaken
        Me.BranchType = branchType
        Me.BranchTargetAddr = branchTargetAddr
        Me.BranchTag = tag
    End Sub
    Public BranchTag As Integer
    Public MissPrediction As Boolean
    Public BranchInstrAddr As Integer
    Public IsTaken As Boolean
    Public BranchType As ISA.BranchType
    Public BranchTargetAddr As Integer
End Class
