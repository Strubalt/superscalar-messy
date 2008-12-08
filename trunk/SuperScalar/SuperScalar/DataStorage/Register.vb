Public Interface IStorageDevice

    'Sub SetInputData(ByVal inputs() As Object, _
    '    ByVal isInputValid() As Boolean)

    'Sub ClearAll()
    Sub UpdateValue()

    'ReadOnly Property Outputs() As ICollection

End Interface

Public Class Register(Of T)
    Implements IStorageDevice


    Private mInput, mOutput As T

    Public Sub SetInputData(ByVal input As T, _
        ByVal isInputValid As Boolean)

        If isInputValid Then
            mInput = input
        Else
            mInput = Nothing
        End If
    End Sub

    Public ReadOnly Property Output() As T
        Get
            Return mOutput
        End Get
    End Property

    Public Sub ClearAll()
        Me.mOutput = Nothing
        Me.mInput = Nothing
    End Sub

    Public ReadOnly Property Input() As T
        Get
            Return Me.mInput
        End Get
    End Property

    Public Sub UpdateValue() Implements IStorageDevice.UpdateValue

        Me.mOutput = Me.mInput
        Me.mInput = Nothing

    End Sub
End Class
