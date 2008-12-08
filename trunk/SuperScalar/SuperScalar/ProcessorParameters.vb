Public Class ProcessorParameters

    Public FetchWidth As Integer = 3
    Public DecodeWidth As Integer = 4
    Public IssueWidth As Integer = 4
    Public DispatchWidth As Integer = 4
    Public RetireWidth As Integer = 4
    Public NumRsEntry As Integer = 40
    Public NumRobEntry As Integer = 60
    Public DecodeBufferSize As Integer = 12
    Public IssueBufferSize As Integer = 12
    Public NumFxSimpleExecutionUnit As Integer = 3
    Public NumFxComplexExecutionUnit As Integer = 3
    Public BranchPredictor As BaseBranchPredictor
    'Public BranchPredictTableSize As Integer = 4096

    'Public NumLoadUnit As Integer = 1
    'Public NumStoreUnit As Integer = 1
    'Const NumFpExecutionUnit As Integer = 3

    Public Sub New(ByVal predictor As BaseBranchPredictor, Optional ByVal fetchWide As Integer = 4, _
        Optional ByVal decodeWide As Integer = 4, Optional ByVal IssueWide As Integer = 4, _
        Optional ByVal DispatchWide As Integer = 5, Optional ByVal RetireWide As Integer = 5, Optional ByVal NumRS As Integer = 40, _
        Optional ByVal NumRob As Integer = 60, Optional ByVal DecodeBufSize As Integer = 12, _
        Optional ByVal IssueBufSize As Integer = 12, Optional ByVal NumFxSimple As Integer = 3, _
        Optional ByVal NumFxComplex As Integer = 3)
        Me.FetchWidth = fetchWide
        Me.DecodeWidth = decodeWide
        Me.IssueWidth = IssueWide
        Me.DispatchWidth = DispatchWide
        Me.NumRsEntry = NumRS
        Me.NumRobEntry = NumRob
        Me.DecodeBufferSize = DecodeBufSize
        Me.IssueBufferSize = IssueBufSize
        Me.NumFxSimpleExecutionUnit = NumFxSimple
        Me.NumFxComplexExecutionUnit = NumFxComplex
        Me.RetireWidth = RetireWide
        Me.BranchPredictor = predictor
    End Sub
End Class
