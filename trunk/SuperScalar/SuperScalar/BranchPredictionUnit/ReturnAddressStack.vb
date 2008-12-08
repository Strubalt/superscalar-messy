Public Class ReturnAddressStack

    Protected Class ReturnAddressEntry

        Public BranchTag As Integer
        Public ReturnAddress As Integer
        Public CallTime As Integer


    End Class
    Private Class Action

        Public Sub New(ByVal pushNotPop As Boolean, ByVal tag As Integer, ByVal retAddr As Integer)
            Me.IsPush = pushNotPop
            Me.BranchTag = tag
            Me.ReturnAddr = retAddr
        End Sub
        Public BranchTag As Integer
        Public IsPush As Boolean
        Public ReturnAddr As Integer
    End Class


    Private mStack As New Stack(Of ReturnAddressEntry)
    Private mBackupActions As New Dictionary(Of Integer, List(Of Action))

    Public Sub RollBackSpeculativeTags(ByVal tags As List(Of Integer))
        'Debug.Assert(False)
        If mBackupActions.Count = 0 Then Exit Sub
        For i As Integer = tags.Count - 1 To 0 Step -1
            If Me.mBackupActions.ContainsKey(tags(i)) Then
                RollBack(Me.mBackupActions(tags(i)))
                mBackupActions.Remove(tags(i))
            End If
        Next
    End Sub

    Public Sub Confirm(ByVal tag As Integer)
        If Me.mBackupActions.ContainsKey(tag) Then
            Me.mBackupActions.Remove(tag)
        End If
    End Sub

    Private Sub RollBack(ByVal actions As List(Of Action))
        For i As Integer = actions.Count - 1 To 0 Step -1
            Dim tempTag, tempAddr As Integer
            'when pushed => pop
            'when poped  => push
            If actions(i).IsPush Then
                Me.Pop(False, -1, tempTag, tempAddr)
            Else
                Me.Push(False, actions(i).BranchTag, actions(i).ReturnAddr)
            End If
        Next
    End Sub

    

    Private Sub AddBackupAcitons(ByVal speculativeTag As Integer, ByVal action As Action)

        Dim actions As List(Of Action) = Nothing
        If Me.mBackupActions.ContainsKey(speculativeTag) Then
            actions = Me.mBackupActions(speculativeTag)
        Else
            actions = New List(Of Action)
            Me.mBackupActions.Add(speculativeTag, actions)
        End If

        actions.Add(action)
    End Sub

    Public Sub Push(ByVal speculative As Boolean, ByVal branchTag As Integer, ByVal returnAddress As Integer)
        If speculative Then AddBackupAcitons(branchTag, New Action(True, branchTag, returnAddress))
        If mStack.Count > 0 Then
            Dim entry As ReturnAddressEntry = mStack.Peek
            If entry.ReturnAddress = returnAddress AndAlso _
                entry.BranchTag = branchTag Then
                entry.CallTime += 1
            Else
                PushOneEntry(branchTag, returnAddress)
            End If
        Else
            PushOneEntry(branchTag, returnAddress)
        End If
    End Sub

    Private Sub PushOneEntry(ByVal branchTag As Integer, ByVal returnAddress As Integer)
        Dim entry As New ReturnAddressEntry
        entry.BranchTag = branchTag
        entry.ReturnAddress = returnAddress
        entry.CallTime = 1
        Me.mStack.Push(entry)
    End Sub

    Public Function HasEntry() As Boolean
        Return Me.mStack.Count <> 0
    End Function

    Public Sub Pop(ByVal speculative As Boolean, ByVal currentTag As Integer, ByRef branchTag As Integer, ByRef returnAddress As Integer)

        Dim entry As ReturnAddressEntry = Me.mStack.Peek
        If speculative Then AddBackupAcitons(currentTag, New Action(False, entry.BranchTag, entry.ReturnAddress))
        branchTag = entry.BranchTag
        returnAddress = entry.ReturnAddress
        If entry.CallTime = 1 Then
            Me.mStack.Pop()
        Else
            Debug.Assert(entry.CallTime > 1)
            entry.CallTime -= 1
        End If
    End Sub
End Class
