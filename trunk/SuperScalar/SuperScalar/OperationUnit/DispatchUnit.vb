Public Class DispatchUnit
    Inherits OperationUnit

    'Private mDispatchWidth As Integer

    Public Sub New()
        'Me.mDispatchWidth = dispatchWide
    End Sub

    Private mReservationStation As ReservationStations
    Private mExceptionProcessUnit As ExecutionUnit
    Private mLoadUnit, mStoreUnit As ExecutionUnit
    Private mBranchUnit As ExecutionUnit
    Private mALUUnits As List(Of ExecutionUnit)
    Private mStallCurrentCycle As Boolean

    Public Property ReservationStation() As ReservationStations
        Get
            Return Me.mReservationStation
        End Get
        Set(ByVal value As ReservationStations)
            Me.mReservationStation = value
        End Set
    End Property

    Public Property ALUUnits() As List(Of ExecutionUnit)
        Get
            Return mALUUnits
        End Get
        Set(ByVal value As List(Of ExecutionUnit))
            mALUUnits = value
        End Set
    End Property

    'Public Property ALUSimpleUnit() As List(Of ExecutionUnit)
    '    Get
    '        Return mALUSimpleUnits
    '    End Get
    '    Set(ByVal value As List(Of ExecutionUnit))
    '        mALUSimpleUnits = value
    '    End Set
    'End Property

    Public Property BranchUnit() As ExecutionUnit
        Get
            Return mBranchUnit
        End Get
        Set(ByVal value As ExecutionUnit)
            mBranchUnit = value
        End Set
    End Property

    Public Property StoreAddressUnit() As ExecutionUnit
        Get
            Return mStoreUnit
        End Get
        Set(ByVal value As ExecutionUnit)
            mStoreUnit = value
        End Set
    End Property

    Public Property LoadUnit() As ExecutionUnit
        Get
            Return mLoadUnit
        End Get
        Set(ByVal value As ExecutionUnit)
            mLoadUnit = value
        End Set
    End Property

    Public Property ExceptionUnit() As ExecutionUnit
        Get
            Return mExceptionProcessUnit
        End Get
        Set(ByVal value As ExecutionUnit)
            mExceptionProcessUnit = value
        End Set
    End Property

    Public Overrides Sub Operate()
        If mStallCurrentCycle Then
            mStallCurrentCycle = False
            Exit Sub
        End If
        Dim dispatchingEntries As New List(Of Integer)
        Dim hasLoad As Boolean = False


        Dim allEUs As List(Of ExecutionUnit) = GetUnBusyEUs()
        For i As Integer = 0 To Me.ReservationStation.RenamedInstructionCount - 1
            If Me.ReservationStation.CanExecute(i) Then
                If DispatchToEUs(ReservationStation.RenamedInstruction(i), allEUs) Then
                    dispatchingEntries.Add(i)
                    If allEUs.Count = 0 Then Exit For
                End If
            End If
        Next

        DispatchLoadStore(dispatchingEntries)
        Me.ReservationStation.RemoveEntries(dispatchingEntries)
    End Sub

    Private Function GetUnBusyEUs() As List(Of ExecutionUnit)
        Dim allEUs As New List(Of ExecutionUnit)
        'For Each eu As ExecutionUnit In Me.mALUSimpleUnits
        '    If eu.NumCycleToFinish = 0 Then
        '        allEUs.Add(eu)
        '    End If
        'Next
        If Me.mBranchUnit.NumCycleToFinish = 0 Then
            allEUs.Add(Me.mBranchUnit)
        End If

        If Me.mExceptionProcessUnit.NumCycleToFinish = 0 Then
            allEUs.Add(Me.mExceptionProcessUnit)
        End If
        For Each eu As ExecutionUnit In Me.mALUUnits
            If eu.NumCycleToFinish = 0 Then
                allEUs.Add(eu)
            End If
        Next

        Return allEUs
    End Function

    Private Function DispatchToEUs(ByVal ri As RenamedInstruction, ByVal eus As List(Of ExecutionUnit)) As Boolean
        Dim euIndex As Integer = -1
        For i As Integer = 0 To eus.Count - 1
            Dim eu As ExecutionUnit = eus(i)

            If eu.CanExecuteType(ri.ExecuteType) Then
                euIndex = i
                eu.InstructionRegister.SetInputData(ri, True)
                Exit For
            End If
        Next
        If euIndex <> -1 Then
            eus.RemoveAt(euIndex)
            Return True
        Else
            Return False
        End If
    End Function

    Private Sub DispatchLoadStore(ByRef dispatchedIndexes As List(Of Integer))
        Dim hasStore As Boolean = False
        For i As Integer = 0 To Me.ReservationStation.RenamedInstructionCount - 1

            Dim ri As RenamedInstruction = Me.ReservationStation.RenamedInstruction(i)
            If ri.ExecuteType = ISA.ExecuteType.Store Then
                If Me.ReservationStation.CanExecute(i) Then
                    If hasStore Then
                        Exit Sub
                    Else
                        hasStore = True
                        Me.mStoreUnit.InstructionRegister.SetInputData(ri, True)
                        dispatchedIndexes.Add(i)
                    End If
                Else
                    Exit Sub
                End If

            ElseIf ri.ExecuteType = ISA.ExecuteType.Load Then
                If Me.ReservationStation.CanExecute(i) Then
                    If Me.mLoadUnit.NumCycleToFinish = 0 Then
                        Me.mLoadUnit.InstructionRegister.SetInputData(ri, True)
                        dispatchedIndexes.Add(i)

                    End If
                End If
                Exit Sub
            End If

        Next
    End Sub

    Public Overrides Sub StallCurrentCycle()
        mStallCurrentCycle = True
    End Sub
End Class
