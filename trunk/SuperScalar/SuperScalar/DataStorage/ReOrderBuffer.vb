Friend Class ValueUpdate
    Sub New(ByVal idx As Integer, ByVal v As Long)
        Index = idx
        Value = v

    End Sub
    Public Index As Integer
    Public Value As Long
    'Public HasException As Boolean
    'Public ExceptionCode As Byte
End Class
Friend Class ValueUpdateWithException
    Inherits ValueUpdate

    Sub New(ByVal idx As Integer, ByVal hasResult As Boolean, ByVal v As Long, _
        ByVal hasExcept As Boolean, ByVal exceptCode As Byte)
        MyBase.New(idx, v)
        Me.HasResult = hasResult
        Me.HasException = hasExcept
        Me.ExceptionCode = exceptCode
    End Sub
    Public HasResult As Boolean
    Public HasException As Boolean
    Public ExceptionCode As Byte
End Class
Friend Class SpeculationResolve
    Sub New(ByVal idx As Integer, ByVal mispredicted As Boolean, ByVal branchTargetAddr As Integer)
        RobIndex = idx
        MisPrediction = mispredicted
        BranchTargetAddress = branchTargetAddr
    End Sub
    Public RobIndex As Integer
    Public MisPrediction As Boolean
    Public BranchTargetAddress As Integer
End Class
Public Class ReorderBuffer
    Implements IStorageDevice

    'One entry should never be used to avoid overflow

    Private mRobEntries() As RobEntry 'when item is nothing => not allocated
    Private mHeadIndex, mTailIndex As Integer
    Private mInputEntries As New List(Of RobEntry)
    Private mItemConsumed As Integer

    Private mValueUpdates As New List(Of ValueUpdateWithException)
    Private mSpeculationUpdates As New List(Of SpeculationResolve)

    Public ReadOnly Property HeadIndex() As Integer
        Get
            Return Me.mHeadIndex
        End Get
    End Property

    Public ReadOnly Property MaxEntries() As Integer
        Get
            Return mRobEntries.Length - 1
        End Get
    End Property

    Public ReadOnly Property Entry(ByVal idx As Integer) As RobEntry
        Get
            Return Me.mRobEntries(idx)
        End Get
    End Property

    Public Sub New(ByVal numEntry As Integer)
        'declare one more entry to prevent from overflow
        ReDim mRobEntries(numEntry)
    End Sub

    Public Sub GetResult(ByVal index As Integer, ByVal isHigh As Boolean, _
        ByRef isValid As Boolean, ByRef value As Integer)
        'Debug.Assert(Not mRobEntries(index) Is Nothing)
        If Me.mRobEntries(index) Is Nothing Then
            isValid = False
            Exit Sub
        End If
        isValid = mRobEntries(index).HasFinished
        If isHigh Then
            value = (mRobEntries(index).Value And &HFFFFFFFF00000000) >> 32
        Else
            value = mRobEntries(index).Value And &HFFFFFFFF
        End If

    End Sub

    Public Sub SetExeResultValue(ByVal robIndex As Integer, ByVal hasResult As Boolean, _
        ByVal value As Integer, ByVal hasException As Boolean, _
        ByVal exceptionCode As Byte)
        mValueUpdates.Add(New ValueUpdateWithException( _
            robIndex, hasResult, value, hasException, exceptionCode))
    End Sub

    Public Sub SetSpeculationResolved(ByVal robIndex As Integer, _
        ByVal misPrediction As Boolean, ByVal branchTargetAddr As Integer)

        Me.mSpeculationUpdates.Add(New SpeculationResolve(robIndex, misPrediction, branchTargetAddr))
    End Sub

    Public Sub UpdateValue() Implements IStorageDevice.UpdateValue
        For i As Integer = 0 To Me.mInputEntries.Count - 1
            AddEntry(Me.mInputEntries(i))
        Next
        For i As Integer = 1 To mItemConsumed
            RemoveEntry()
        Next
        For Each v As ValueUpdate In Me.mValueUpdates
            If Not Me.mRobEntries(v.Index) Is Nothing Then
                Me.mRobEntries(v.Index).Value = v.Value
                Me.mRobEntries(v.Index).HasFinished = True
            End If
            
        Next
        For Each s As SpeculationResolve In Me.mSpeculationUpdates
            If Not Me.mRobEntries(s.RobIndex) Is Nothing Then
                Me.mRobEntries(s.RobIndex).HasFinished = True
            End If
            UpdateSpeculationResult(s)
            
        Next
        Me.mValueUpdates.Clear()
        Me.mSpeculationUpdates.Clear()
        Me.mItemConsumed = 0
        Me.mInputEntries.Clear()
    End Sub

    Private Sub UpdateSpeculationResult(ByVal s As SpeculationResolve)
        If Not Me.mRobEntries(s.RobIndex) Is Nothing Then
            For i As Integer = 0 To Me.mRobEntries.Length - 1
                If s.MisPrediction Then
                    RemoveFromIndexToTheEnd(s.RobIndex)
                Else
                    UpdateToNonSpeculative(s.RobIndex, Me.mRobEntries(s.RobIndex).BranchTag)
                End If
            Next
        End If
    End Sub

    Private Sub UpdateToNonSpeculative(ByVal startIdx As Integer, ByVal branchTag As Integer)

        For i As Integer = 0 To Me.mRobEntries.Length - 1
            Dim idx As Integer = CircularIndex(startIdx + i)
            If Me.mRobEntries(idx) Is Nothing Then Exit Sub
            If Me.mRobEntries(idx).BranchTag = branchTag Then
                Me.mRobEntries(idx).IsSpeculative = False
            End If
        Next
    End Sub

    Public ReadOnly Property CircularIndex(ByVal idx As Integer) As Integer
        Get
            Return (idx) Mod Me.mRobEntries.Length
        End Get
    End Property

    Private Sub RemoveFromIndexToTheEnd(ByVal robIndex As Integer)
        If Me.mRobEntries(robIndex) Is Nothing Then Exit Sub
        For i As Integer = 0 To Me.mRobEntries.Length - 1
            Dim idx As Integer = CircularIndex(robIndex + i)
            If idx = Me.mTailIndex Then
                Exit For
            Else
                Me.mRobEntries(idx) = Nothing
            End If
        Next
        Me.mTailIndex = robIndex
    End Sub

    Private Sub RemoveEntry()
        Me.mRobEntries(Me.mHeadIndex) = Nothing
        Me.mHeadIndex = (Me.mHeadIndex + 1) Mod Me.mRobEntries.Length
    End Sub

    Private Sub AddEntry(ByVal robEntry As RobEntry)
        mRobEntries(Me.mTailIndex) = robEntry
        Me.mTailIndex = (Me.mTailIndex + 1) Mod Me.mRobEntries.Length

    End Sub

    Public ReadOnly Property IsFull() As Boolean
        Get
            Return Me.GetNumEmptyEntries = 0
        End Get
    End Property

    Public Function GetEmptyEntryIndex(ByVal num As Integer) As List(Of Integer)
        Dim result As New List(Of Integer)
        Dim numReturn As Integer = Math.Min(num, Me.GetNumEmptyEntries)
        For i As Integer = 0 To numReturn - 1
            result.Add((Me.mTailIndex + i) Mod mRobEntries.Length)
        Next
        Return result
    End Function

    Public Sub AddEntry(ByVal newRobEntries As List(Of RobEntry))
        If Not newRobEntries Is Nothing Then
            mInputEntries.AddRange(newRobEntries)
        End If
    End Sub

    Public Function GetNumEmptyEntries() As Integer
        'If Me.mHeadIndex = Me.mTailIndex Then Return Me.NumEntries
        If Me.mHeadIndex <= Me.mTailIndex Then
            Return Me.MaxEntries - (Me.mTailIndex - Me.mHeadIndex)
        Else
            Return Me.mHeadIndex - (1 + Me.mTailIndex)
        End If
        'If Me.mHeadIndex > Me.mTailIndex Then
        '    Return Me.mHeadIndex - (1 + Me.mTailIndex)
        'End If
    End Function




    Public Property NumItemConsumed() As Integer
        Get
            Return Me.mItemConsumed
        End Get
        Set(ByVal value As Integer)
            Me.mItemConsumed = value
        End Set
    End Property

    Public Function GetCanRetireEntries(ByVal num As Integer, _
        ByVal robIndexes As List(Of Integer)) As List(Of RobEntry)
        Debug.Assert(num < Me.MaxEntries)

        Dim results As New List(Of RobEntry)
        For i As Integer = 0 To num - 1
            Dim robIndex As Integer = Me.CircularIndex(Me.mHeadIndex + i)
            If Me.mRobEntries(robIndex) Is Nothing Then
                Exit For
            Else
                If Me.mRobEntries(robIndex).HasFinished AndAlso _
                    Not Me.mRobEntries(robIndex).IsSpeculative Then
                    results.Add(Me.mRobEntries(robIndex))
                    robIndexes.Add(robIndex)
                Else
                    Exit For
                End If
            End If
        Next
        Return results
    End Function


    'Public ReadOnly Property Output(ByVal idx As Integer) As RobEntry
    '    Get

    '    End Get
    'End Property
End Class
