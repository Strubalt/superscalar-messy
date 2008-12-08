Public Class DataCache

    Public Const PageOffsetBits As Integer = 16
    Public Const PageSize As Integer = 1 << PageOffsetBits

    Private memoryPages As New List(Of Byte())
    Private mTLB As New Dictionary(Of Integer, Integer)

    Public Function VirtualPageNum(ByVal addr As Integer) As Integer
        Return addr >> PageOffsetBits
    End Function

    Public Function PageOffset(ByVal addr As Integer) As Integer
        Return addr - (VirtualPageNum(addr) << PageOffsetBits)
    End Function

    Private Function GetMemoryPage(ByVal addr As Integer) As Byte()
        Dim pageNum As Integer = VirtualPageNum(addr)

        If Not mTLB.ContainsKey(pageNum) Then
            Dim Page(PageSize - 1) As Byte
            mTLB.Add(pageNum, memoryPages.Count)
            memoryPages.Add(Page)
        End If
        Dim physicalNum As Integer = mTLB(pageNum)
        Return memoryPages(physicalNum)
        
    End Function

    Public Function GetData(ByVal addr As Integer, ByVal numByte As Integer) As Byte()
        Dim data(numByte - 1) As Byte
        For i As Integer = 0 To numByte - 1
            data(i) = GetByte(addr + i)
        Next
        Return data
    End Function



    Public Function GetByte(ByVal addr As Integer) As Byte
        Return GetMemoryPage(addr)(PageOffset(addr))
    End Function

    Public Function GetByteExtended(ByVal addr As Integer, ByVal signedExtend As Boolean) As Integer
        Return NumericsConvert.ExtendByteToInteger(GetByte(addr), signedExtend)
    End Function

    Public Sub SetByte(ByVal addr As Integer, ByVal data As Byte)
        GetMemoryPage(addr)(PageOffset(addr)) = data
        Dim bytes() As Byte = GetMemoryPage(addr)
        Dim offset As Integer = PageOffset(addr)
        Debug.Assert(bytes(offset) = data)
    End Sub

    Public Function GetShortExtended(ByVal addr As Integer, ByVal signedExtend As Boolean) As Integer
        Dim data() As Byte = Me.GetData(addr, 2)
        Return NumericsConvert.ExtendShortToInteger(data, signedExtend)
    End Function

    Public Function GetInteger(ByVal addr As Integer) As Integer
        Dim intData() As Byte = Me.GetData(addr, 4)
        Return NumericsConvert.GetInteger(intData)
        'Dim result As Integer = CInt(intData(3)) << 3 Or CInt(intData(2)) << 2 _
        '    Or CInt(intData(1)) << 1 Or intData(0)
        'Return result
    End Function

    Public Sub SetInteger(ByVal addr As Integer, ByVal data As Integer)
        Dim intData As Byte() = NumericsConvert.ConvertToBytes(data)
        'If data < 0 Then
        '    'for 2's complement  -x = x+1 =>x = -x-1 
        '    intData(3) = CByte((((-data - 1) And &HFF000000) >> 24)) Xor 255
        'Else
        '    intData(3) = (data And &HFF000000) >> 24
        'End If
        'intData(2) = (data And &HFF0000) >> 16
        'intData(1) = (data And &HFF00) >> 8
        'intData(0) = (data And &HFF)
        Me.SetData(addr, intData, 4)
    End Sub

    'sData as integer to prevent overflow problem of MSByte
    Public Sub SetShort(ByVal addr As Integer, ByVal sData As Integer)
        Dim intData() As Byte = NumericsConvert.ConvertToBytes(sData)

        'intData(1) = (sData And &HFF00) >> 8
        'intData(0) = (sData And &HFF)
        Me.SetData(addr, intData, 2)
    End Sub

    Public Sub SetData(ByVal addr As Integer, ByVal data() As Byte, ByVal numByte As Integer)
        For i As Integer = 0 To Math.Min(data.Length, numByte) - 1
            SetByte(addr + i, data(i))
        Next
    End Sub

    Public Sub SetData(ByVal addr As Integer, ByVal data As Integer, ByVal numByte As Integer)
        Dim intData() As Byte = NumericsConvert.ConvertToBytes(data)

      
        Me.SetData(addr, intData, numByte)
    End Sub

End Class
