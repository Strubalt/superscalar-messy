Imports ISA
Public Class StoreAddressComputeUnit
    Inherits ExecutionUnit

    Public Sub New(ByVal reorderBuffer As ReorderBuffer, _
        ByVal rs As ReservationStations, ByVal sq As StoreQueue)
        MyBase.New(reorderBuffer, rs)
        Me.mStoreQueue = sq
    End Sub

    'Compute Memory address and add to StoreQueue

    Private mStoreQueue As StoreQueue
    Public Property StoreQueue() As StoreQueue
        Get
            Return mStoreQueue
        End Get
        Set(ByVal value As StoreQueue)
            mStoreQueue = value
        End Set
    End Property

    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            Return 0
        End Get
    End Property

    Public Overrides Sub Operate()
        Dim instr As RenamedInstruction = Me.CurrentExecuteInstruction
        If Not instr Is Nothing Then

            Dim addr As Integer = instr.OpRob(1) + instr.Immediate
            Dim numByte As Integer
            Select Case CType(instr.DecodedInstruction.SubExecuteType, ISA.LoadStoreType)
                Case LoadStoreType.Byte1
                    numByte = 1
                Case LoadStoreType.Byte2
                    numByte = 2
                Case LoadStoreType.Byte4
                    numByte = 4
            End Select
            Dim storeInstr As New StoreInstruction(instr.RobIndex, _
                instr.BranchTag, instr.OpRob(0), addr, numByte)
            Me.mStoreQueue.SetInputData( _
                New StoreInstruction() {storeInstr})
            Me.ReorderBuffer.SetExeResultValue(instr.RobIndex, False, 0, False, 0)
        End If
    End Sub

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.Store
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
        Return "Store Addr: " & vbTab & instr
    End Function

End Class
