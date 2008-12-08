Public Class NumericsConvert

    Public Shared Function GetInteger(ByVal intData() As Byte) As Integer

        Dim result As Integer = CInt(intData(3)) << 24 Or CInt(intData(2)) << 16 _
            Or CInt(intData(1)) << 8 Or intData(0)
        Return result
    End Function


    Public Shared Function ExtendShortToInteger(ByVal intData() As Byte, ByVal signedExtend As Boolean) As Integer
        Dim extendValue As Integer = 0
        If signedExtend AndAlso intData(1) > 127 Then
            extendValue = &HFFFF0000
        End If
        Dim result As Integer = extendValue Or CInt(intData(1)) << 1 Or intData(0)

        Return result
    End Function

    Public Shared Function ExtendByteToInteger(ByVal data As Byte, ByVal signedExtend As Boolean) As Integer
        Dim extendValue As Integer = 0
        If signedExtend AndAlso data > 127 Then
            extendValue = &HFFFFFF00
        End If
        Dim result As Integer = extendValue Or data

        Return result

    End Function

    Public Shared Function GetHigherInt(ByVal data As Long) As Integer
        If data < 0 Then
            Return CInt(((-data - 1) And &HFFFFFFFF00000000) >> 32) Xor &HFFFFFFFF
        Else
            Return (data And &HFFFFFFFF00000000) >> 32
        End If
    End Function

    Public Shared Function GetLowerInt(ByVal data As Long) As Integer
        Return (data And &HFFFFFFFF)
    End Function


    Public Overloads Shared Function ConvertToBytes(ByVal intValue As Integer) As Byte()
        Dim intData(3) As Byte
        If intValue < 0 Then
            'for 2's complement  -x = x+1 =>x = -x-1 
            intData(3) = CByte((((-intValue - 1) And &HFF000000) >> 24)) Xor 255
        Else
            intData(3) = (intValue And &HFF000000) >> 24
        End If
        intData(2) = (intValue And &HFF0000) >> 16
        intData(1) = (intValue And &HFF00) >> 8
        intData(0) = (intValue And &HFF)
        Return intData
    End Function

    Public Overloads Shared Function ConvertLower2Bytes(ByVal shortValue As Integer) As Byte()
        Dim data(2) As Byte

        data(1) = (shortValue And &HFF00) >> 8
        data(0) = (shortValue And &HFF)
        Return data

    End Function

End Class
