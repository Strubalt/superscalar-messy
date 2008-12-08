Public Class PCModule
    Implements IStorageDevice

    'Bytes fetched
    Private mInstrSizeInByte As Integer

    Public Sub New(ByVal instrSizeInByte As Integer)
        mInstrSizeInByte = instrSizeInByte
    End Sub

    Private mCurrentPC As Integer = 0
    Private mNumInstructionsFetched As Integer = 0
    'Private mCurrentContainBranch As Boolean
    'Private mCurrentBranchAddress, mCurrentBrTargetAddress As Integer, mCurrentBranchTaken As Boolean
    Private mBranchAddress As Integer, mBrTaken, mDetectBranch As Boolean, mBrTargetAddress As Integer

    'branch not detected in BTB
    Private mMissedBranch As Boolean, mMissedBrTargetAddress As Integer
    Private mIsMispredicted As Boolean, mCorrectMispredictBrTarget As Integer
    Private mDisableCurrentUpdate As Boolean

    'Public ReadOnly Property CurrentInstructionsContainBranch() As Boolean
    '    Get
    '        Return Me.mCurrentContainBranch
    '    End Get
    'End Property

    'Public ReadOnly Property CurrentBranchInstructionAddress() As Integer
    '    Get
    '        Return Me.mCurrentBranchAddress
    '    End Get
    'End Property

    'Public ReadOnly Property IsCurrentBranchTaken() As Boolean
    '    Get
    '        Return Me.mCurrentBranchTaken
    '    End Get
    'End Property

    'Public ReadOnly Property CurrentBranchTargetAddress() As Integer
    '    Get
    '        Return Me.mCurrentBrTargetAddress
    '    End Get
    'End Property

    Public Sub SetPredictionResult(ByVal detectBranch As Boolean, ByVal branchAddress As Integer, _
        ByVal taken As Boolean, ByVal targetAddress As Integer)
        Me.mBrTaken = taken
        Me.mDetectBranch = detectBranch
        Me.mBrTargetAddress = targetAddress
        Me.mBranchAddress = branchAddress
    End Sub

    Public Sub SetMissedBranch(ByVal hasMissedBranch As Boolean, ByVal missedBrTarget As Integer)
        Me.mMissedBranch = hasMissedBranch
        Me.mMissedBrTargetAddress = missedBrTarget
    End Sub

    Public Sub SetMispredictBranchTarget(ByVal isMispredicted As Boolean, _
        ByVal correctTargetAddress As Integer)

        Me.mIsMispredicted = isMispredicted
        Me.mCorrectMispredictBrTarget = correctTargetAddress
    End Sub

    Public Property NumInstructionsFetched() As Integer
        Get
            Return mNumInstructionsFetched
        End Get
        Set(ByVal value As Integer)
            mNumInstructionsFetched = value
        End Set
    End Property

    Public Sub DisableCurrentUpdate()
        mDisableCurrentUpdate = True
    End Sub

    Public ReadOnly Property CurrentPC() As Integer
        Get
            Return Me.mCurrentPC
        End Get
    End Property

    Public Sub UpdateValue() Implements IStorageDevice.UpdateValue
        'Priority
        'Mispredicted Branch > Missed detected Branch > Stall 
        ' > Branch Prediction > PC ++

        If Me.mIsMispredicted Then
            Me.mCurrentPC = Me.mCorrectMispredictBrTarget
        ElseIf Me.mMissedBranch Then
            Me.mCurrentPC = Me.mMissedBrTargetAddress
        Else
            If Me.mDisableCurrentUpdate Then
                'PC stayed unchanged
            Else
                If Me.mDetectBranch Then
                    'Me.mCurrentContainBranch = True
                    'Me.mCurrentBranchAddress = Me.mBranchAddress
                    'Me.mCurrentBranchTaken = Me.mBrTaken
                    'Me.mCurrentBrTargetAddress = Me.mBrTargetAddress
                    If Me.mBrTaken Then
                        Me.mCurrentPC = Me.mBrTargetAddress
                    Else
                        Me.mCurrentPC += Me.mInstrSizeInByte * mNumInstructionsFetched
                    End If
                Else
                    Me.mCurrentPC += Me.mInstrSizeInByte * mNumInstructionsFetched
                End If

            End If
        End If
        Me.mDetectBranch = False
        mMissedBranch = False
        mIsMispredicted = False
        mNumInstructionsFetched = 0
        mDisableCurrentUpdate = False
    End Sub

    Public Sub ExplicitSetPC(ByVal pc As Integer)
        Me.mCurrentPC = pc
    End Sub
End Class
