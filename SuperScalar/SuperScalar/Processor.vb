Public Class Processor

    'Private mFetchWidth As Integer = 3
    'Private mDecodeWidth As Integer = 4
    Const InstructionSize As Integer = 4
    'Public NumFxSimpleExecutionUnit As Integer = 3
    'Public NumFxComplexExecutionUnit As Integer = 3
    'Public NumRobEntries As Integer = 60
    'Const NumFpExecutionUnit As Integer = 3
    Const InstructionBaseAddr As Integer = 0
    Public Parameters As ProcessorParameters

    Private mBranchTagTable As BranchTagTable
    Private mReturnAddressStack As ReturnAddressStack

    Private mBranchPredictionUnit As BranchPredictionUnit
    Private mFetchUnit As FetchUnit
    Private mPCRegisterModule As PCModule
    Private mInstructionCache As InstructionCache
    Private mDecodeBuffer As DecodeBuffer
    Private mDecodeUnit As DecodeUnit
    Private mIssueBuffer As FixedSizeQueue(Of DecodedInstruction)
    Private mIssueUnit As IssueUnit


    Private mReorderBuffer As ReorderBuffer
    Private mRetireUnit As RetireUnit
    Private mRegisterFile As GPPRegisterFile
    Private mStoreQueue As StoreQueue
    Private mStoreUnit As StoreUnit      'execute after retire
    Private mStoreAddressComputeUnit As StoreAddressComputeUnit
    Private mDataCache As DataCache

    Private mReservationStation As ReservationStations
    Private mDispatchUnit As DispatchUnit
    'Private mDispatchedInstrRegisters As New List(Of FixedSizeQueue(Of RenamedInstruction))

    Private mBranchUnit As BranchUnit
    Private mFxALUs As New List(Of ExecutionUnit)
    Private mExceptionUnit As ExceptionProcessUnit

    'Private mFxExecutionUnit(NumFxExecutionUnit - 1) As ExecutionUnit
    'Private mFpExecutionUnit(NumFpExecutionUnit - 1) As ExecutionUnit
    Private mLoadUnit As LoadUnit
    Private mExecutionUnits As New List(Of ExecutionUnit)

    Private mControlUnit As CProcessorControl

    Public ReadOnly Property InstrucitonCache() As InstructionCache
        Get
            Return Me.mInstructionCache
        End Get
    End Property

    Public ReadOnly Property DataCache() As DataCache
        Get
            Return Me.mDataCache
        End Get
    End Property

    Public ReadOnly Property DecodeBuffer() As DecodeBuffer
        Get
            Return Me.mDecodeBuffer
        End Get
    End Property

    Public ReadOnly Property IssueBuffer() As FixedSizeQueue(Of DecodedInstruction)
        Get
            Return Me.mIssueBuffer
        End Get
    End Property

    Public ReadOnly Property ReservationStation() As ReservationStations
        Get
            Return Me.mReservationStation
        End Get
    End Property

    Public ReadOnly Property ReorderBuffer() As ReorderBuffer
        Get
            Return Me.mReorderBuffer
        End Get
    End Property

    Public ReadOnly Property ExecutionUnits() As List(Of ExecutionUnit)
        Get
            Return Me.mExecutionUnits
        End Get
    End Property

    Public ReadOnly Property StoreQueue() As StoreQueue
        Get
            Return Me.mStoreQueue
        End Get
    End Property

    Public ReadOnly Property RegisterFile() As GPPRegisterFile
        Get
            Return Me.mRegisterFile
        End Get
    End Property

    Public ReadOnly Property MaxDCacheAddress() As Integer
        Get
            Return &H7FFFFFF
        End Get
    End Property

    Private Sub CreateAllUnits()
        mControlUnit = New CProcessorControl(Me)

        'Storage
        mDataCache = New DataCache
        mInstructionCache = New InstructionCache(InstructionBaseAddr)

        mStoreQueue = New StoreQueue
        mReturnAddressStack = New ReturnAddressStack
        mBranchTagTable = New BranchTagTable(Me.Parameters.DecodeWidth * 5, 0)
        mRegisterFile = New GPPRegisterFile(Me.mBranchTagTable)
        mDecodeBuffer = New DecodeBuffer(Parameters.DecodeBufferSize)
        mIssueBuffer = New FixedSizeQueue(Of DecodedInstruction)(Parameters.IssueBufferSize)
        mReorderBuffer = New ReorderBuffer(Me.Parameters.NumRobEntry)
        mReservationStation = New ReservationStations(Me.Parameters.NumRsEntry)

        mPCRegisterModule = New PCModule(InstructionSize)
        mPCRegisterModule.ExplicitSetPC(0)

        'OperationUnit
        mRetireUnit = New RetireUnit(Me.Parameters.RetireWidth, Me.mReorderBuffer, _
            Me.mRegisterFile, Me.mStoreQueue, Me.mBranchTagTable)
        mStoreUnit = New StoreUnit(Me.mStoreQueue, Me.mDataCache)      'execute after retire
        mFetchUnit = New FetchUnit(Me.Parameters.FetchWidth, InstructionSize)
        mDecodeUnit = New DecodeUnit(Me.Parameters.DecodeWidth, _
            Me.mReturnAddressStack, InstructionSize, Me.mControlUnit)
        mIssueUnit = New IssueUnit(Me.Parameters.IssueWidth, Me.mBranchTagTable)
        mDispatchUnit = New DispatchUnit()
       
        mBranchPredictionUnit = New BranchPredictionUnit(Me.mPCRegisterModule, _
            Me.Parameters.BranchPredictor, Me.Parameters.FetchWidth, InstructionSize, Me.mDecodeBuffer)


        'ExecutionUnits
        mBranchUnit = New BranchUnit(Me.mReorderBuffer, Me.mReservationStation, InstructionSize, _
            Me.mBranchTagTable, Me.mPCRegisterModule, Me.mBranchPredictionUnit, Me.mControlUnit)
        mExceptionUnit = New ExceptionProcessUnit(Me.mReorderBuffer, Me.mReservationStation)
        mLoadUnit = New LoadUnit(Me.mReorderBuffer, Me.mReservationStation, Me.mStoreQueue, Me.mDataCache)
        mStoreAddressComputeUnit = New StoreAddressComputeUnit(Me.mReorderBuffer, Me.mReservationStation, Me.mStoreQueue)

        mFxALUs.Clear()
        For i As Integer = 0 To Me.Parameters.NumFxComplexExecutionUnit - 1
            mFxALUs.Add(New ALUSimpleFxUnit(Me.mReorderBuffer, Me.mReservationStation))
        Next
        For i As Integer = 0 To Me.Parameters.NumFxSimpleExecutionUnit - 1
            mFxALUs.Add(New ALUComplexFxALU(Me.mReorderBuffer, Me.mReservationStation))
        Next
        mExecutionUnits.Clear()

        mExecutionUnits.Add(mBranchUnit)
        mExecutionUnits.Add(mLoadUnit)
        mExecutionUnits.AddRange(Me.mFxALUs)
        mExecutionUnits.Add(mExceptionUnit)
        mExecutionUnits.Add(mStoreAddressComputeUnit)
        'mExecutionUnits.AddRange(Me.mFpExecutionUnit)

    End Sub

    Public Sub New(ByVal par As ProcessorParameters)
        Me.Parameters = par
        
        CreateAllUnits()
        ConnectOperationUnits()

        Me.mPCRegisterModule.ExplicitSetPC(0)

    End Sub

    Public Property PC() As Integer
        Get
            Return Me.mPCRegisterModule.CurrentPC
        End Get
        Set(ByVal value As Integer)
            Me.mPCRegisterModule.ExplicitSetPC(value)
        End Set
    End Property

    Private Sub ConnectOperationUnits()

        Me.mBranchPredictionUnit.PCRegister = Me.mPCRegisterModule

        Me.mFetchUnit.PCRegister = Me.mPCRegisterModule
        Me.mFetchUnit.InstructionCache = Me.mInstructionCache
        Me.mFetchUnit.OutputBuffer = Me.mDecodeBuffer
        
        Me.mDecodeUnit.DecodeBuffer = Me.mDecodeBuffer
        Me.mDecodeUnit.OutputQueue = Me.mIssueBuffer
        Me.mDecodeUnit.BranchTagTable = Me.mBranchTagTable

        Me.mIssueUnit.IssueBuffer = Me.mIssueBuffer
        Me.mIssueUnit.RegisterFile = Me.mRegisterFile
        Me.mIssueUnit.ReorderBuffer = Me.mReorderBuffer
        Me.mIssueUnit.ReservationStation = Me.mReservationStation

        Me.mDispatchUnit.ALUUnits = Me.mFxALUs
        Me.mDispatchUnit.BranchUnit = Me.mBranchUnit
        Me.mDispatchUnit.ExceptionUnit = Me.mExceptionUnit
        Me.mDispatchUnit.LoadUnit = Me.mLoadUnit
        Me.mDispatchUnit.ReservationStation = Me.mReservationStation
        Me.mDispatchUnit.StoreAddressUnit = Me.mStoreAddressComputeUnit


        'Me.mRegisterFile.ReOrderBuffer = Me.mReorderBuffer

    End Sub

   
    Public Sub Load(ByVal instructions() As Byte)
        Me.CreateAllUnits()
        Me.ConnectOperationUnits()

        Me.mInstructionCache.SetData(instructions)
        Me.mRegisterFile.ExplicitSetValue(ISA.RegisterEncoding.sp, Me.MaxDCacheAddress)
        Me.mRegisterFile.ExplicitSetValue(ISA.RegisterEncoding.fp, Me.MaxDCacheAddress)

    End Sub

    Public Sub StepExecute()
        Operate()
        Me.mControlUnit.Operate()
        Update()

    End Sub

    Public Function ContainInstructionReadyToRetireInRob(ByVal instrAddr As Integer) As Boolean
        Dim indexes As New List(Of Integer)
        Dim instrs As List(Of RobEntry) = Me.mReorderBuffer.GetCanRetireEntries( _
            Me.Parameters.RetireWidth, indexes)
        For Each entry As RobEntry In instrs
            If entry.Instruction.PC = instrAddr Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Sub Operate()
        Me.mStoreUnit.Operate()
        Me.mRetireUnit.Operate()
        For Each eu As ExecutionUnit In Me.mExecutionUnits
            eu.Operate()
        Next
        Me.mDispatchUnit.Operate()
        Me.mIssueUnit.Operate()
        Me.mDecodeUnit.Operate()
        Me.mFetchUnit.Operate()
        Me.mBranchPredictionUnit.Operate()

    End Sub

    Private Sub Update()
        Me.mPCRegisterModule.UpdateValue()
        Me.mDecodeBuffer.UpdateValue()
        Me.mIssueBuffer.UpdateValue()
        Me.mReservationStation.UpdateValue()
        Me.mReorderBuffer.UpdateValue()
        Me.mStoreQueue.UpdateValue()
        Me.mRegisterFile.UpdateValue()
        For Each eu As ExecutionUnit In Me.mExecutionUnits
            eu.InstructionRegister.UpdateValue()
        Next
    End Sub

    Public ReadOnly Property MisPredictionCount() As Integer
        Get
            Return Me.mMistPredictionCount
        End Get
    End Property
    Public ReadOnly Property TotalBranchCount() As Integer
        Get
            Return mTotalBranch
        End Get
    End Property
    Private mMistPredictionCount As Integer = 0
    Private mTotalBranch As Integer = 0

    Public Class CProcessorControl
        Private mProcessor As Processor
        Public Sub New(ByVal processor As Processor)
            Me.mProcessor = processor
        End Sub

        Public Sub Operate()
            If Not mMissedDetect Is Nothing Then
                mProcessor.mPCRegisterModule.SetMissedBranch(mMissedDetect.IsTaken, Me.mMissedDetect.BranchTargetAddr)
                If mMissedDetect.IsTaken Then
                    mProcessor.DecodeBuffer.ClearAll()
                End If
                Me.mProcessor.mBranchPredictionUnit.SetBranchResult(mMissedDetect)
                mMissedDetect = Nothing
            End If

            If Not Me.mBranchResult Is Nothing Then
                mProcessor.mTotalBranch += 1
                If mBranchResult.MissPrediction Then
                    mProcessor.mMistPredictionCount += 1

                End If
                Debug.WriteLine(mBranchResult.BranchInstrAddr & ",Taken=" & mBranchResult.IsTaken & ",misPredict=" & mBranchResult.MissPrediction)
                mProcessor.mPCRegisterModule.SetMispredictBranchTarget( _
                    mBranchResult.MissPrediction, mBranchResult.BranchTargetAddr)
                If Me.mBranchResult.MissPrediction Then
                    Me.mProcessor.DecodeBuffer.ClearAll()
                    Me.mProcessor.IssueBuffer.ClearAll()

                    Dim speculativeTags As List(Of Integer) = mProcessor.mBranchTagTable.GetSpeculativeTags
                    Me.mProcessor.ReservationStation.ClearInstrucitons(speculativeTags)
                    Me.mProcessor.StoreQueue.ClearInstrucitons(speculativeTags)
                    'Reorder buffer is done via Common Data bus

                    Me.mProcessor.RegisterFile.RollBackSpeculativeTags(speculativeTags)
                    Me.mProcessor.mReturnAddressStack.RollBackSpeculativeTags(speculativeTags)
                    ClearEUsSpeculativeOperations(speculativeTags)
                    mProcessor.mBranchTagTable.Clear()
                Else

                    Dim brTag As BranchTag = mProcessor.mBranchTagTable.RemoveHead()
                    Me.mProcessor.RegisterFile.Confirm(Me.mBranchResult.BranchTag)
                    Me.mProcessor.mReturnAddressStack.Confirm(Me.mBranchResult.BranchTag)
                    Debug.Assert(brTag.BranchInstrAddress = mBranchResult.BranchInstrAddr)
                End If
                Me.mProcessor.mBranchPredictionUnit.SetBranchResult(mBranchResult)
                Dim bt As BranchTag = mProcessor.mBranchTagTable.GetEntry(Me.mBranchResult.BranchTag)
                If Not bt Is Nothing Then bt.HasResolved = True
                Me.mBranchResult = Nothing
            End If
        End Sub

        Private Sub ClearEUsSpeculativeOperations(ByVal speculativeTags As List(Of Integer))
            For idx As Integer = speculativeTags.Count - 1 To 0 Step -1
                For Each eu As ExecutionUnit In Me.mProcessor.ExecutionUnits
                    If Not eu.InstructionRegister.Input Is Nothing Then
                        Dim ri As RenamedInstruction = eu.InstructionRegister.Input
                        If ri.BranchTag = speculativeTags(idx) Then
                            eu.InstructionRegister.ClearAll()
                        End If
                    End If
                    eu.ClearExecutingInstruciton(speculativeTags)
                Next
            Next
            For idx As Integer = speculativeTags.Count - 1 To 0 Step -1
                For Each eu As ExecutionUnit In Me.mProcessor.ExecutionUnits
                    eu.ClearExecutingInstruciton(speculativeTags)
                Next
            Next
        End Sub

       

        Private mMissedDetect As BranchResult
        Private mBranchResult As BranchResult

        Public Sub SetMissedDetectedBranch(ByVal brInstrAddr As Integer, _
            ByVal isTaken As Boolean, ByVal branchType As ISA.BranchType, _
            ByVal branchTargetAddr As Integer, ByVal tag As Integer)
            mMissedDetect = New BranchResult(isTaken, brInstrAddr, isTaken, branchType, branchTargetAddr, tag)
        End Sub

        Public Sub SetBranchResult(ByVal misprediction As Boolean, ByVal brInstrAddr As Integer, _
            ByVal isTaken As Boolean, ByVal branchType As ISA.BranchType, _
            ByVal branchTargetAddr As Integer, ByVal tag As Integer)


            mBranchResult = New BranchResult(misprediction, brInstrAddr, isTaken, branchType, branchTargetAddr, tag)
        End Sub
    End Class

End Class
