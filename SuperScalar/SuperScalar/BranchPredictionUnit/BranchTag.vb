Public Class BranchTag

    Public Sub New(ByVal brInstrAddr As Integer, ByVal isTaken As Boolean, _
        ByVal brTargetAddr As Integer)

        Me.BranchInstrAddress = brInstrAddr
        Me.PredictIsBranchTaken = isTaken
        Me.PredictBranchTargetAddress = brTargetAddr

    End Sub
    Public BranchInstrAddress As Integer
    Public PredictIsBranchTaken As Boolean
    Public PredictBranchTargetAddress As Integer
    Public HasResolved As Boolean
    'Public IsBranchTaken As Boolean
    'Public BranchTargetAddress As Integer
End Class

Public Class BranchTagTable
    Inherits CircularBuffer(Of BranchTag)


    Private mCurrentResolvedBranch As BranchTag

    Public Sub New(ByVal size As Integer, ByVal firstInstructionAddr As Integer)
        MyBase.New(size)

        'Add a virtual branch before any instruction starts to run
        'Remove it to show that it's resolved
        Dim initialTag As New BranchTag(firstInstructionAddr, False, 0)
        initialTag.HasResolved = True
        Me.Add(initialTag)

        Me.RemoveHead()

        'Me.Add()
    End Sub

    Public Overrides Function Add(ByVal t As BranchTag) As Boolean

        Return MyBase.Add(t)

    End Function

    Public Function GetCurrentTag() As Integer
        Return GetIndexBeforeTail()
    End Function

    Public Function IsSpeculative(ByVal tag As Integer) As Boolean
        If Me.mData(tag) Is Nothing Then Return False
        Return Not Me.mData(tag).HasResolved
    End Function

    'Public Sub SetBranchResult(ByVal brTag As Integer, ByVal isTaken As Boolean, _
    '    ByVal targetAddr As Integer)
    '    Dim entry As BranchTag = Me.GetEntry(brTag)
    '    Debug.Assert(Not entry Is Nothing)

    '    entry.HasResolved = True
    '    entry.IsBranchTaken = isTaken
    '    entry.BranchTargetAddress = targetAddr

    'End Sub

    
    
End Class