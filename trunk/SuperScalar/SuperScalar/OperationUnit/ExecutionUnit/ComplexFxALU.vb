Imports ISA
Public Class ALUComplexFxALU
    Inherits ExecutionUnit




    Public Sub New(ByVal reorderBuffer As ReorderBuffer, ByVal rs As ReservationStations)
        MyBase.New(reorderBuffer, rs)
    End Sub

    Private mInstruction As RenamedInstruction

    Public Overrides Sub Operate()
        If mInstruction Is Nothing Then
            Me.mInstruction = Me.InstructionRegister.Output
        Else
            Dim result As Long
            Select Case CType(Me.mInstruction.SubExecuteType, ISA.ALUComplexType)
                Case ALUComplexType.Div_Mod
                    result = ((Me.mInstruction.OpRob(0) Mod Me.mInstruction.OpRob(1)) << 32) Or _
                        (Me.mInstruction.OpRob(0) \ Me.mInstruction.OpRob(1))
                Case ALUComplexType.Mult
                    result = Me.mInstruction.OpRob(0) * Me.mInstruction.OpRob(1)
            End Select
            Me.ReorderBuffer.SetExeResultValue(Me.mInstruction.RobIndex, _
                True, result, False, ISA.ExceptionCode.Overflow)
            Me.ReservationStation.SetExeResultValue(Me.mInstruction.RobIndex, _
                result, False, ISA.ExceptionCode.Overflow)
            mInstruction = Nothing
        End If
    End Sub


    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            If mInstruction Is Nothing Then
                Return 0
            Else
                Return 1
            End If
        End Get
    End Property

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.FxComplex
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Overrides Function FormatString() As String
        Dim ri As RenamedInstruction = mInstruction
        If ri Is Nothing Then
            ri = Me.InstructionRegister.Output
        End If
        Dim instr As String = ""
        If Not ri Is Nothing Then
            instr = vbCrLf & "(" & Format(ri.RobIndex, "000") & ")" & _
                vbTab & ri.ToString

        End If
        Return "ComplexAlu: " & vbTab & instr
    End Function

    Public Overrides Sub ClearExecutingInstruciton(ByVal tags As System.Collections.Generic.List(Of Integer))
        MyBase.ClearExecutingInstruciton(tags)
        If Me.mInstruction Is Nothing Then Exit Sub
        For i As Integer = tags.Count - 1 To 0 Step -1
            If Me.mInstruction.BranchTag = tags(i) Then
                Me.mInstruction = Nothing
                Exit Sub
            End If
        Next

    End Sub
    
End Class
