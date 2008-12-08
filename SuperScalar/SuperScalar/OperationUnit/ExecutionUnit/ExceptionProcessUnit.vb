Imports ISA
Public Class ExceptionProcessUnit
    Inherits ExecutionUnit

    Public Sub New(ByVal reorderBuffer As ReorderBuffer, ByVal rs As ReservationStations)
        MyBase.New(reorderBuffer, rs)
    End Sub


   

    Public Overrides Sub Operate()
        Dim ri As RenamedInstruction = Me.InstructionRegister.Output
        If ri Is Nothing Then Exit Sub
        Me.ReorderBuffer.SetExeResultValue(ri.RobIndex, False, 0, True, ri.Immediate)

    End Sub

    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.Exception
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Overrides Function FormatString() As String
        Dim ri As RenamedInstruction = Me.InstructionRegister.Output
        Dim instr As String = ""
        If Not ri Is Nothing Then

            instr = vbCrLf & "(" & Format(ri.RobIndex, "000") & ")" & _
                vbTab & ri.ToString
        End If
        Return "Exception: " & vbTab & instr
    End Function
End Class
