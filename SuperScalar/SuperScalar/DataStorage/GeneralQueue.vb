Public Class FixedSizeQueue(Of T)
    Implements IStorageDevice

    Private mInput As New List(Of T)
    'Private mValidInput As IList(Of Boolean)
    Protected mBuffer As New List(Of T)
    Private mInputsAreInvalid As Boolean
    'Private mOutput As New ArrayList
    Private mMaxSize As Integer = 8
    Private mItemConsumed As Integer
    'Private mMaxOutputPort, mMaxInputPort As Integer
    Protected ReadOnly Property Input() As List(Of T)
        Get
            Return Me.mInput
        End Get
    End Property

    Public Sub New(Optional ByVal maxSize As Integer = 12)
        'mMaxInputPort = maxInputPort
        'mMaxOutputPort = maxOutputPort
        Me.mMaxSize = maxSize
    End Sub

    Protected ReadOnly Property Buffer() As List(Of T)
        Get
            Return Me.mBuffer
        End Get
    End Property

    Public ReadOnly Property MaxSize() As Integer
        Get
            Return Me.mMaxSize
        End Get
    End Property


    Public Sub SetInputData(ByVal inputs() As T)

        Debug.Assert(Not Me.IsFull)

        'Debug.Assert(inputs.Length <= Me.MaxInputPort)

        mInput.Clear()
        'mValidInput = New ArrayList
        For i As Integer = 0 To inputs.Length - 1
            'If isInputValid(i) Then
            mInput.Add(inputs(i))
            'mValidInput.Add(True)
            'End If
        Next
    End Sub

    Public Sub ClearAll()
        Me.mInput.Clear()
        Me.mBuffer.Clear()
        Me.ItemsConsumed = 0

    End Sub

    Public ReadOnly Property NumEmpty() As Integer
        Get
            Return Me.mMaxSize - Me.mBuffer.Count
        End Get
    End Property

    Public Property InputsAreInvalid() As Boolean
        Get
            Return Me.mInputsAreInvalid
        End Get
        Set(ByVal value As Boolean)
            Me.mInputsAreInvalid = value
        End Set
    End Property

    Public Property ItemsConsumed() As Integer
        Get
            Return Me.mItemConsumed
        End Get
        Set(ByVal value As Integer)
            Me.mItemConsumed = value
        End Set
    End Property

    Public Overridable ReadOnly Property IsFull() As Boolean
        Get
            Return Me.mBuffer.Count + Me.mInput.Count >= Me.mMaxSize
        End Get
    End Property

    Public ReadOnly Property Output(ByVal idx As Integer) As T
        Get
            'Debug.Assert(idx < Me.MaxOutputPort)
            If idx < mBuffer.Count Then Return mBuffer(idx)
            Return Nothing
        End Get
    End Property

    'Public ReadOnly Property Outputs() As Generic.ICollection(Of T)
    '    Get
    '        Dim result(Me.MaxOutputPort - 1) As T
    '        Me.mBuffer.CopyTo(0, result, 0, Math.Min(result.Length, Me.mBuffer.Count))
    '        Return result
    '    End Get
    'End Property

    Public ReadOnly Property OutputValid(ByVal idx As Integer) As Boolean
        Get
            'Debug.Assert(idx < Me.MaxOutputPort)
            If idx < mBuffer.Count Then Return True
            Return False
        End Get
    End Property

    'Public ReadOnly Property MaxOutputPort() As Integer
    '    Get
    '        Return Me.mMaxOutputPort
    '    End Get
    'End Property

    'Public ReadOnly Property MaxInputPort() As Integer
    '    Get
    '        Return Me.mMaxInputPort
    '    End Get
    'End Property

    Public Overridable Sub UpdateValue() Implements IStorageDevice.UpdateValue
        If Me.mInputsAreInvalid Then
            'If previous unit stalled, the inputs were invalid (bug)
            Me.mInput.Clear()
        End If
        If Me.ItemsConsumed <> 0 Then

            For i As Integer = 0 To Me.ItemsConsumed - 1
                If Me.mBuffer.Count > 0 Then mBuffer.RemoveAt(0)
            Next
            Me.ItemsConsumed = 0
        End If
        For i As Integer = 0 To Me.mInput.Count - 1
            Me.mBuffer.Add(Me.mInput(i))
        Next
        Me.mInput.Clear()

    End Sub



End Class




