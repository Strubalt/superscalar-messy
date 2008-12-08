Public Class StoreInstruction

    Private mRobIndex, mBranchTag, mSourceValue, mTargetAddress, mNumByte As Integer
    Private mRetiredFromRob As Boolean = False 'Always speculative at the beginning

    Public Sub New(ByVal robIndex As Integer, ByVal brTag As Integer, _
        ByVal srcValue As Integer, ByVal targetAddr As Integer, _
        ByVal numByte As Integer)

        Me.mRobIndex = robIndex
        Me.mBranchTag = brTag
        Me.mSourceValue = srcValue
        Me.mTargetAddress = targetAddr
        Me.mNumByte = numByte
    End Sub

    Public ReadOnly Property RobIndex() As Integer
        Get
            Return Me.mRobIndex
        End Get
    End Property

    Public ReadOnly Property BranchTag() As Integer
        Get
            Return Me.mBranchTag
        End Get
    End Property

    Public Property HasRetiredFromRob() As Boolean
        Get
            Return Me.mRetiredFromRob
        End Get
        Set(ByVal value As Boolean)
            Me.mRetiredFromRob = value
        End Set
    End Property

    Public ReadOnly Property TargetMemoryAddress() As Integer
        Get
            Return Me.mTargetAddress
        End Get
    End Property

    Public ReadOnly Property SourceValue() As Integer
        Get
            Return Me.mSourceValue
        End Get
    End Property

    Public ReadOnly Property NumByteToStore() As Integer
        Get
            Return Me.mNumByte
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim retire As String = " (x)"
        If Me.HasRetiredFromRob Then
            retire = " (o)"
        End If
        Return "Store(" & Me.mNumByte & ") " & Me.mSourceValue & _
            " to " & Me.mTargetAddress & retire

    End Function
End Class
