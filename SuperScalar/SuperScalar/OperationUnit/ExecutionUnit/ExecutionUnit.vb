Imports ISA
Public MustInherit Class ExecutionUnit
    'Inherits OperationUnit

    Private mInstructionRegister As New Register(Of RenamedInstruction)
    Private mReorderBuffer As ReorderBuffer
    Private mReservationStation As ReservationStations

    Public Sub New(ByVal reorderBuffer As ReorderBuffer, ByVal rs As ReservationStations)
        Me.mReorderBuffer = reorderBuffer
        Me.mReservationStation = rs
    End Sub


    Public ReadOnly Property InstructionRegister() As Register(Of RenamedInstruction)
        Get
            Return Me.mInstructionRegister
        End Get
        'Set(ByVal value As Register(Of RenamedInstruction))
        '    Me.mInstructionRegister = value
        'End Set
    End Property

    Protected ReadOnly Property CurrentExecuteInstruction() As RenamedInstruction
        Get
            Return Me.mInstructionRegister.Output
        End Get
    End Property

    Public Property ReorderBuffer() As ReorderBuffer
        Get
            Return Me.mReorderBuffer
        End Get
        Set(ByVal value As ReorderBuffer)
            Me.mReorderBuffer = value
        End Set
    End Property

    Public Property ReservationStation() As ReservationStations
        Get
            Return Me.mReservationStation
        End Get
        Set(ByVal value As ReservationStations)
            Me.mReservationStation = value
        End Set
    End Property

    'Public MustOverride ReadOnly Property ExecuteType() As ExecuteType

    Public MustOverride ReadOnly Property NumCycleToFinish() As Integer

    Public MustOverride Sub Operate()

    Public MustOverride Function CanExecuteType(ByVal type As ExecuteType) As Boolean
    Public MustOverride Function FormatString() As String

    Public Overridable Sub ClearExecutingInstruciton(ByVal tags As List(Of Integer))

    End Sub

    Public Overrides Function ToString() As String
        Return FormatString()

    End Function
    'End Sub

    'Public Overrides Sub StallCurrentCycle()

    'End Sub
End Class




