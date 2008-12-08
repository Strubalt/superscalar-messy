Imports ISA
Public Class ALUSimpleFxUnit
    Inherits ExecutionUnit


    Public Sub New(ByVal reorderBuffer As ReorderBuffer, ByVal rs As ReservationStations)
        MyBase.New(reorderBuffer, rs)
    End Sub


    Public Overrides Sub Operate()
        Dim ri As RenamedInstruction = Me.InstructionRegister.Output
        Dim result As Integer
        Dim hasException As Boolean = False
        If Not ri Is Nothing Then
            Try
                Select Case CType(ri.SubExecuteType, ISA.ALUSimpleType)
                    Case ALUSimpleType.Add
                        result = ri.OpRob(0) + ri.OpRob(1)
                    Case ALUSimpleType.AddI
                        result = ri.OpRob(0) + ri.Immediate
                    Case ALUSimpleType.And
                        result = ri.OpRob(0) And ri.OpRob(1)
                    Case ALUSimpleType.AndI
                        result = ri.OpRob(0) And ri.Immediate
                    Case ALUSimpleType.LUI
                        result = ri.Immediate << 16
                    Case ALUSimpleType.Nor
                        result = Not (ri.OpRob(0) Or ri.OpRob(1))
                    Case ALUSimpleType.OrI
                        result = ri.OpRob(0) Or ri.Immediate
                    Case ALUSimpleType.Or
                        result = ri.OpRob(0) Or ri.OpRob(1)
                    Case ALUSimpleType.SetLessThan
                        result = IIf(ri.OpRob(0) < ri.OpRob(1), 1, 0)
                    Case ALUSimpleType.SetLessThanImm
                        result = IIf(ri.OpRob(0) < ri.Immediate, 1, 0)
                    Case ALUSimpleType.ShiftLeft
                        result = ri.OpRob(0) << ri.OpRob(1)
                    Case ALUSimpleType.ShiftLeftLogic
                        result = ri.OpRob(0) << ri.Immediate
                    Case ALUSimpleType.ShiftRight
                        result = ri.OpRob(0) >> ri.OpRob(1)
                    Case ALUSimpleType.ShiftRightLogic
                        result = ri.OpRob(0) >> ri.Immediate
                    Case ALUSimpleType.Sub
                        result = ri.OpRob(0) - ri.OpRob(1)
                    Case ALUSimpleType.Xor
                        result = ri.OpRob(0) Xor ri.OpRob(1)
                    Case ALUSimpleType.XorI
                        result = ri.OpRob(0) Xor ri.OpRob(1)
                    Case ALUSimpleType.MfHigh
                        result = ri.OpRob(0)
                    Case ALUSimpleType.MfLow
                        result = ri.OpRob(0)
                End Select
            Catch ex As Exception
                hasException = True
            End Try
            Me.ReorderBuffer.SetExeResultValue(ri.RobIndex, True, _
                result, hasException, ISA.ExceptionCode.Overflow)
            Me.ReservationStation.SetExeResultValue(ri.RobIndex, _
                result, hasException, ISA.ExceptionCode.Overflow)
        End If
    End Sub


    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.FxSimple
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
        Return "SimpleAlu: " & vbTab & instr
    End Function
End Class
