'Just to pretend there is a instruction cache in the system
Public Class InstructionCache



    Public Const InstructionSize As Integer = 4

    Public Sub New(ByVal baseAddr As Integer)
        Me.mBaseAddress = baseAddr
    End Sub

    Private mBaseAddress As Integer

    Friend Sub SetData(ByVal data() As Byte)
        mData = data
    End Sub

    Private mData() As Byte

    Public Function GetAllInstructions() As Byte()()
        Dim results(Me.mData.Length \ 4 - 1)() As Byte
        For i As Integer = 0 To results.Length - 1
            ReDim results(i)(InstructionSize - 1)
        Next
        For instrIdx As Integer = 0 To results.Length - 1
            Dim startIndex As Integer = instrIdx * InstructionSize
            If startIndex < Me.mData.Length Then
                For i As Integer = 0 To InstructionSize - 1
                    results(instrIdx)(i) = Me.mData(startIndex + i)
                Next

            End If

        Next
        Return results
    End Function
    
    Public Function GetInstructions(ByVal pc As Integer, ByVal numInstr As Integer, _
        ByRef instrValid() As Boolean) As Byte()()

        Dim results(numInstr - 1)() As Byte
        ReDim instrValid(numInstr - 1)
        For i As Integer = 0 To results.Length - 1
            ReDim results(i)(InstructionSize - 1)
        Next

        Dim PcOffsetToBase As Integer = pc - Me.mBaseAddress    'addjust addr to start from 0

        For instrIdx As Integer = 0 To numInstr - 1
            Dim startIndex As Integer = PcOffsetToBase + instrIdx * InstructionSize
            If startIndex < Me.mData.Length Then
                For i As Integer = 0 To InstructionSize - 1
                    results(instrIdx)(i) = Me.mData(startIndex + i)
                Next
                instrValid(instrIdx) = True
            End If

        Next
        
        Return results
    End Function

End Class
