Public Class RetireUnit
    Inherits OperationUnit

    Private mRetireWidth As Integer
    Public Sub New(ByVal width As Integer, ByVal rob As ReorderBuffer, _
        ByVal rf As GPPRegisterFile, ByVal sq As StoreQueue, ByVal brTagTable As BranchTagTable)
        mRetireWidth = width
        Me.mReorderBuffer = rob
        Me.mRegisterFile = rf
        Me.mStoreQueue = sq
        Me.mBrTagTable = brTagTable
    End Sub

    Private mBrTagTable As BranchTagTable
    Private mReorderBuffer As ReorderBuffer
    Private mRegisterFile As GPPRegisterFile
    Private mStoreQueue As FixedSizeQueue(Of StoreInstruction)
    Private mNumInstructionRetired As Integer

    Public ReadOnly Property NumInstructionRetired() As Integer
        Get
            Return Me.mNumInstructionRetired
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

    Public Property RegisterFile() As GPPRegisterFile
        Get
            Return Me.mRegisterFile
        End Get
        Set(ByVal value As GPPRegisterFile)
            Me.mRegisterFile = value
        End Set
    End Property

    Public Property StoreQueue() As FixedSizeQueue(Of StoreInstruction)
        Get
            Return Me.mStoreQueue
        End Get
        Set(ByVal value As FixedSizeQueue(Of StoreInstruction))
            Me.mStoreQueue = value
        End Set
    End Property

    Public Overrides Sub Operate()
        Dim indexes As New List(Of Integer)
        Dim entries As List(Of RobEntry) = Me.mReorderBuffer.GetCanRetireEntries(Me.mRetireWidth, indexes)

        For i As Integer = 0 To Math.Min(mRetireWidth, entries.Count) - 1
            SaveLog(entries(i))
            If entries(i).HasException Then
                If StoreQueueEmpty() AndAlso i = 0 Then
                    Throw New Exception("exception in instruction, " & entries(i).InstructionAddress & ";" & vbCrLf _
                        & "exception code: " & entries(i).ExceptionCode)
                Else
                    Me.mReorderBuffer.NumItemConsumed = i
                    Exit Sub
                End If
            Else
                If entries(i).Instruction.IsWriteBackToMemory Then

                    SetHasRetired(indexes(i))
                ElseIf entries(i).Instruction.IsWriteBackToRegister Then
                    'Me.mRegisterFile.DeallocateRenamedIndex(indexes(i))

                    Me.mRegisterFile.SetValue(entries(i).Instruction.TargetRegisterIndex, entries(i).Value, indexes(i), _
                        Me.mBrTagTable.IsSpeculative(Me.mBrTagTable.GetCurrentTag))
                End If
            End If
        Next
        Me.mReorderBuffer.NumItemConsumed = entries.Count
    End Sub

    Private Sub SaveLog(ByVal entry As RobEntry)
        mNumInstructionRetired += 1
        'Dim fileName As String = "C:\test.txt"

        'If Not IO.File.Exists(fileName) Then
        '    IO.File.Create(fileName).Close()
        'End If
        'Dim fs As New IO.FileStream(fileName, IO.FileMode.Append)
        'Dim stringWriter As New IO.StreamWriter(fs)
        'stringWriter.WriteLine(Format(entry.Instruction.PC, "0000") & " " & _
        '    entry.ToString & ", " & entry.Instruction.ToString)
        'stringWriter.Close()
        'fs.Close()
    End Sub

    Private Sub SetHasRetired(ByVal robIndex As Integer)
        For i As Integer = 0 To Me.mStoreQueue.MaxSize - 1
            If Me.mStoreQueue.OutputValid(i) Then
                If robIndex = Me.mStoreQueue.Output(i).RobIndex Then
                    Me.mStoreQueue.Output(i).HasRetiredFromRob = True
                    Exit Sub
                End If
            End If
        Next
    End Sub

    Private Function StoreQueueEmpty() As Boolean

        If Me.mStoreQueue.OutputValid(0) Then
            Dim cmd As StoreInstruction = Me.StoreQueue.Output(0)
            If cmd.HasRetiredFromRob Then
                Return False
            Else
                Return True
            End If
        Else
            Return True
        End If


    End Function

    Public Overrides Sub StallCurrentCycle()

    End Sub
End Class
