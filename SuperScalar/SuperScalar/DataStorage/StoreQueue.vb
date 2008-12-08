Public Class StoreQueue
    Inherits FixedSizeQueue(Of StoreInstruction)

    Public Sub New()

    End Sub

    Private mClearTags As New List(Of Integer)
    Public Sub ClearInstrucitons(ByVal tags As List(Of Integer))
        Me.mClearTags.AddRange(tags)
    End Sub

    Public Overrides Sub UpdateValue()
        MyBase.UpdateValue()
        For i As Integer = Me.mClearTags.Count - 1 To 0 Step -1
            For riIndex As Integer = Me.mBuffer.Count - 1 To 0 Step -1
                If Me.mBuffer(riIndex).BranchTag = Me.mClearTags(i) Then
                    Me.mBuffer.RemoveAt(riIndex)
                End If
            Next
        Next

        mClearTags.Clear()
    End Sub

    Public Function ContainMemAddr(ByVal addr As Integer) As Boolean
        For Each si As StoreInstruction In Me.Buffer
            If si.TargetMemoryAddress = addr Then
                Return True
            End If
        Next
    End Function

    Public Sub GetMostRecentStoreSourceValue(ByVal addr As Integer, _
        ByRef isMatch As Boolean, ByRef value As Integer)

        For i As Integer = Me.Buffer.Count - 1 To 0 Step -1
            If Buffer(i).TargetMemoryAddress = addr Then
                isMatch = True
                value = Buffer(i).SourceValue
                Exit Sub
            End If
        Next
        'For Each si As StoreInstruction In Me.Buffer
        '    If si.TargetMemoryAddress = addr Then
        '        isMatch = True
        '        value = si.SourceValue
        '    End If
        'Next
    End Sub

    
End Class
