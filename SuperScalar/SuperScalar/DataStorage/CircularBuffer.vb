Public Class CircularBuffer(Of T)

    Protected mData() As T
    Private head, tail As Integer

    Public Sub New(ByVal size As Integer)
        ReDim mData(size)
    End Sub

    Public Function GetNumEmpty() As Integer
        Return Me.MaxSize - Me.OccupiedCount
    End Function

    Public Overridable Function Add(ByVal t As T) As Boolean
        If GetNumEmpty() = 0 Then Return False
        Debug.Assert(mData(tail) Is Nothing)
        mData(tail) = t
        tail = Me.CircularIndex(tail + 1)
        Return True
    End Function

    Public Function GetNextAddIndest() As Integer
        Return Me.tail
    End Function

    Public ReadOnly Property HeadIndex() As Integer
        Get
            Return Me.head
        End Get
    End Property

    Protected ReadOnly Property TailIndex() As Integer
        Get
            Return Me.tail
        End Get
    End Property

    Protected Function GetIndexBeforeTail() As Integer
        Return Me.CircularIndex(Me.tail - 1 + Me.mData.Length)
    End Function

    Protected ReadOnly Property CircularIndex(ByVal idx As Integer) As Integer
        Get
            Return idx Mod Me.mData.Length
        End Get
    End Property

    Public Function GetItemFromHead(ByVal offset As Integer) As T
        Return Me.mData(CircularIndex(Me.head + offset))
    End Function

    Public Function GetSpeculativeTags() As List(Of Integer)
        Dim results As New List(Of Integer)
        If head = tail Then Return results

        For i As Integer = Me.head To Me.head + Me.MaxSize - 1
            Dim idx As Integer = Me.CircularIndex(i)
            If idx <> tail Then
                results.Add(idx)
            Else
                Exit For
            End If
        Next
        Return results
    End Function

    Public Function GetEntry(ByVal tag As Integer) As T

        Return Me.mData(tag)
    End Function

    Public ReadOnly Property MaxSize() As Integer
        Get
            Return Me.mData.Length - 1
        End Get
    End Property

    Public ReadOnly Property OccupiedCount() As Integer
        Get
            If tail >= head Then
                Return tail - head
            Else
                Return Me.mData.Length - (head - tail)
            End If
        End Get
    End Property

    Public Function RemoveHead() As T
        If Me.head <> Me.tail Then
            Dim t As T = Me.mData(Me.head)
            Me.mData(Me.head) = Nothing
            Me.head = Me.CircularIndex(Me.head + 1)
            Return t
        Else
            Return Nothing
        End If
    End Function

    Protected Sub RemoveFromTail()

        Me.tail = GetIndexBeforeTail()
    End Sub

    Public Sub Clear()
        Me.head = Me.CircularIndex(head + 1)
        Me.tail = Me.head
        For i As Integer = 0 To Me.mData.Length - 1
            mData(i) = Nothing
        Next
    End Sub
End Class
