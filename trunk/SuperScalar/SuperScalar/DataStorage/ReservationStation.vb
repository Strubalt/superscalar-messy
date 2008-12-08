''' <summary>
''' Thiss class simulate multiple reservation stations for 
''' different types of instructions, including 
''' ALU, Load/Store, Branch,
''' </summary>
''' <remarks></remarks>
Public Class ReservationStations
    Implements IStorageDevice

    Private mMaxSize As Integer

    Public Sub New(ByVal maxSize As Integer)
        mMaxSize = maxSize
    End Sub

    Private mRenamedInstructions As New List(Of RenamedInstruction)
    Private mInput As New List(Of RenamedInstruction)
    Private mValueUpdates As New List(Of ValueUpdate)
    Private mRemovingEntries As New List(Of Integer)
    Public Sub SetInputRenamedInstructions(ByVal instrs As IList(Of RenamedInstruction))
        mInput.Clear()
        mInput.AddRange(instrs)
    End Sub
    Public Sub SetExeResultValue(ByVal robIndex As Integer, ByVal value As Integer, _
        ByVal hasException As Boolean, ByVal exceptionCode As Byte)
        mValueUpdates.Add(New ValueUpdate(robIndex, value))
    End Sub

    Public Sub RemoveEntries(ByVal indexes As List(Of Integer))
        mRemovingEntries.AddRange(indexes)
        mRemovingEntries.Sort()
    End Sub

    Private mClearTags As New List(Of Integer)
    Public Sub ClearInstrucitons(ByVal tags As List(Of Integer))
        Me.mClearTags.AddRange(tags)
    End Sub


    Public Sub UpdateValue() Implements IStorageDevice.UpdateValue
        'Debug.Assert(False)
        For i As Integer = mRemovingEntries.Count - 1 To 0 Step -1
            Me.mRenamedInstructions.RemoveAt(mRemovingEntries(i))
        Next
        Me.mRenamedInstructions.AddRange(mInput)
        For Each vu As ValueUpdate In mValueUpdates
            For Each ri As RenamedInstruction In Me.mRenamedInstructions
                For i As Integer = 0 To 1
                    If Not ri.OperandValid(i) AndAlso ri.OpRob(i) = vu.Index Then
                        ri.OpRob(i) = vu.Value
                        ri.OperandValid(i) = True
                    End If
                Next
            Next
        Next
        ClearSpeculativeInstructions()

        mClearTags.Clear()
        mInput.Clear()
        mValueUpdates.Clear()
        mRemovingEntries.Clear()
    End Sub

    Private Sub ClearSpeculativeInstructions()
        For i As Integer = Me.mClearTags.Count - 1 To 0 Step -1
            For riIndex As Integer = Me.mRenamedInstructions.Count - 1 To 0 Step -1
                If Me.mRenamedInstructions(riIndex).BranchTag = Me.mClearTags(i) Then
                    Me.mRenamedInstructions.RemoveAt(riIndex)
                End If
            Next
        Next
    End Sub

    Public ReadOnly Property Entries() As List(Of RenamedInstruction)
        Get
            Return Me.mRenamedInstructions
        End Get
    End Property

    Public Function GetNumEmptyEntryies() As Integer
        Return Me.mMaxSize - Me.mRenamedInstructions.Count
    End Function

    Public ReadOnly Property RenamedInstructionCount() As Integer
        Get
            Return mRenamedInstructions.Count
        End Get
    End Property

    Public ReadOnly Property CanExecute(ByVal idx As Integer) As Boolean
        Get
            If idx > Me.mRenamedInstructions.Count Then Return False
            Dim ri As RenamedInstruction = RenamedInstruction(idx)
            If ri.OperandValid(0) AndAlso ri.OperandValid(1) Then
                Return True
            End If
            Return False
        End Get
    End Property

    Public ReadOnly Property RenamedInstruction(ByVal idx As Integer) As RenamedInstruction
        Get
            Return mRenamedInstructions(idx)
        End Get
    End Property

End Class

