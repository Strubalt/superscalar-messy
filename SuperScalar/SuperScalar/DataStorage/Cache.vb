Public Class Cache(Of T)

    Public Structure Entry
        Public IsValid As Boolean
        Public Tag As Integer
        Public Data As T
    End Structure

    Private mData() As Entry

    'Private mOffsetBitNum As Integer
    Private mNWayBitNum As Integer
    Private mBlockIndexBitNum As Integer
    Private mNWay As Integer

    'Tag Index Offset
    Public ReadOnly Property Count() As Integer
        Get
            Return Me.mData.Length
        End Get
    End Property

    Public Sub New(ByVal blockIndexBitNum As Integer, _
        ByVal numWayAssociativeBitNum As Integer)

        Me.mBlockIndexBitNum = blockIndexBitNum
        Me.mNWayBitNum = numWayAssociativeBitNum
        Me.mNWay = 2 ^ Me.mNWayBitNum
        'Me.mOffsetBitNum = offsetBitNum

        Dim size As Integer = 2 ^ (Me.mBlockIndexBitNum + Me.mNWayBitNum)
        ReDim mData(size - 1)

    End Sub

    Public Function GetBlockIndex(ByVal addr As Integer) As Integer
        Dim hasMinusValue As Boolean = False
        Dim value As Integer = 2 ^ (Me.mBlockIndexBitNum - 1)
        If (addr And value) Then
            hasMinusValue = True
            addr -= value
        End If
        Dim temp As Integer = addr << (32 - Me.mBlockIndexBitNum)
        Dim result = temp >> (32 - Me.mBlockIndexBitNum)
        If hasMinusValue Then
            result += value
        End If
        Return result
        'If temp < 0 Then
        '    temp = -temp - 1

        '    Return ((temp And (Not (2 ^ mBlockIndexBitNum - 1))) >> (32 - Me.mBlockIndexBitNum)) Xor (2 ^ mBlockIndexBitNum - 1)
        'Else
        '    Return temp >> (32 - Me.mBlockIndexBitNum)
        'End If

    End Function

    Public Function GetBlockTag(ByVal addr As Integer) As Integer
        Return addr >> Me.mBlockIndexBitNum
    End Function

    Public Sub GetItem(ByVal addr As Integer, ByRef isHit As Boolean, _
        ByRef data As T)

        Dim blockIndex As Integer = Me.GetBlockIndex(addr)
        Dim tag As Integer = Me.GetBlockTag(addr)

        Dim startIndex As Integer = Me.mNWay * blockIndex
        For i As Integer = startIndex To startIndex + Me.mNWay - 1
            If Me.mData(i).IsValid AndAlso tag = Me.mData(i).Tag Then
                isHit = True
                data = Me.mData(i).Data
                Exit Sub
            End If
        Next
        isHit = False

    End Sub

    Public Sub ExplicitSetItem(ByVal idx As Integer, ByVal data As T)
        Me.mData(idx).Data = data

    End Sub

    Public Sub SetItem(ByVal addr As Integer, ByVal data As T)
        Dim blockIndex As Integer = Me.GetBlockIndex(addr)
        Dim tag As Integer = Me.GetBlockTag(addr)

        Dim startIndex As Integer = Me.mNWay * blockIndex

        Sort(addr)
        Dim entry As New Entry
        entry.Data = data
        entry.IsValid = True
        entry.Tag = tag
        For i As Integer = startIndex To startIndex + Me.mNWay - 1
            If Not Me.mData(i).IsValid Then
                Me.mData(i) = entry
                Exit Sub
            End If
        Next
        Debug.Assert(False)
    End Sub

    Private Sub AdvanceOnePlace(ByVal blockIndex As Integer, ByVal startOffset As Integer)
        Dim startIndex As Integer = Me.mNWay * blockIndex
        For i As Integer = startIndex + startOffset To startIndex + Me.mNWay - 2
            Me.mData(i) = Me.mData(i + 1)
        Next
    End Sub

    Private Sub Sort(ByVal addr As Integer)
        Dim blockIndex As Integer = Me.GetBlockIndex(addr)
        Dim tag As Integer = Me.GetBlockTag(addr)

        Dim startIndex As Integer = Me.mNWay * blockIndex
        Dim matchOffset As Integer = -1
        For offset As Integer = 0 To Me.mNWay - 1
            If Me.mData(startIndex + offset).IsValid Then
                If Me.mData(startIndex + offset).Tag = tag Then
                    matchOffset = offset
                    Exit For
                End If
            Else
                Exit Sub
            End If
        Next
        If matchOffset = -1 Then Exit Sub
        AdvanceOnePlace(blockIndex, matchOffset)
    End Sub


End Class
