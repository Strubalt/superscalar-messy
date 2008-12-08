Imports ISA
Public Class LoadUnit
    Inherits ExecutionUnit

    '1st cycle compute address
    '2nd cycle load data

    'Private mNumCycleToFinish As Integer
    Private mCurrentInstruction As RenamedInstruction
    Private mStoreQueue As StoreQueue
    Private mDataCache As DataCache


    Public Sub New(ByVal reorderBuffer As ReorderBuffer, _
        ByVal rs As ReservationStations, ByVal sq As StoreQueue, ByVal dc As DataCache)
        MyBase.New(reorderBuffer, rs)
        Me.mStoreQueue = sq
        Me.mDataCache = dc
    End Sub

    Public Property DataCache() As DataCache
        Get
            Return Me.mDataCache
        End Get
        Set(ByVal value As DataCache)
            Me.mDataCache = value
        End Set
    End Property

    Public Property StoreQueue() As StoreQueue
        Get
            Return Me.mStoreQueue
        End Get
        Set(ByVal value As StoreQueue)
            Me.mStoreQueue = value
        End Set
    End Property

    Public Overrides Sub Operate()
        If Not Me.mCurrentInstruction Is Nothing Then 'Cycle2
            Dim addr As Integer = Me.mCurrentInstruction.OpRob(0) + Me.mCurrentInstruction.Immediate


            Dim result As Integer
            If Not GetFromStoreQueue(addr, result) Then
                Select Case CType(Me.mCurrentInstruction.SubExecuteType, ISA.LoadStoreType)
                    Case LoadStoreType.Byte1
                        result = Me.mDataCache.GetByteExtended(addr, Me.mCurrentInstruction.IsSigned)
                    Case LoadStoreType.Byte2
                        result = Me.mDataCache.GetShortExtended(addr, Me.mCurrentInstruction.IsSigned)
                    Case LoadStoreType.Byte4
                        result = Me.mDataCache.GetInteger(addr)
                End Select
            End If


            Me.ReorderBuffer.SetExeResultValue(Me.mCurrentInstruction.RobIndex, _
                True, result, False, 0)
            Me.ReservationStation.SetExeResultValue(Me.mCurrentInstruction.RobIndex, _
                result, False, ISA.ExceptionCode.Overflow)
            Me.mCurrentInstruction = Nothing
            'Debug.Assert(Me.InstructionRegister.Output Is Nothing, "error")
        End If
        Me.mCurrentInstruction = Me.InstructionRegister.Output  'Cycle1
        'do nothing to simulate 2-cycle execution
        'If Me.mCurrentInstruction Is Nothing Then



        'Else

        'End If

    End Sub

    Private Function GetFromStoreQueue(ByVal addr As Integer, ByRef result As Integer) As Boolean
        Dim isMatch As Boolean
        Me.mStoreQueue.GetMostRecentStoreSourceValue(addr, isMatch, result)
        Return isMatch
    End Function


    Public Overrides ReadOnly Property NumCycleToFinish() As Integer
        Get
            Return 0
            'If Me.mCurrentInstruction Is Nothing Then
            '    Return 0
            'Else
            '    Return 1
            'End If
        End Get
    End Property

    Public Overrides Function CanExecuteType(ByVal type As ISA.ExecuteType) As Boolean
        Select Case type
            Case ISA.ExecuteType.Load
                Return True
            Case Else
                Return False
        End Select
    End Function

    Public Overrides Sub ClearExecutingInstruciton(ByVal tags As System.Collections.Generic.List(Of Integer))
        MyBase.ClearExecutingInstruciton(tags)
        If Me.mCurrentInstruction Is Nothing Then Exit Sub
        For i As Integer = tags.Count - 1 To 0 Step -1
            If Me.mCurrentInstruction.BranchTag = tags(i) Then
                Me.mCurrentInstruction = Nothing
                Exit Sub
            End If
        Next

    End Sub

    Public Overrides Function FormatString() As String
        Dim ri1 As RenamedInstruction = mCurrentInstruction
        Dim ri As RenamedInstruction = Me.InstructionRegister.Output
       
        Dim instr1 As String = ""
        If Not ri Is Nothing Then
            instr1 = "(" & Format(ri.RobIndex, "000") & ")" & _
                vbTab & ri.ToString & "(new)"
        End If
        Dim instr2 As String = ""
        If Not ri1 Is Nothing Then
            instr2 &= "(" & Format(ri1.RobIndex, "000") & ")" & _
                vbTab & ri1.ToString & "(old)"
        End If
        Dim ret As String = "Load: "
        If instr2.Length <> 0 Then
            ret &= vbCrLf & instr2
        End If
        If instr1.Length <> 0 Then
            ret &= vbCrLf & instr1
        End If
        
        Return ret
    End Function
End Class
