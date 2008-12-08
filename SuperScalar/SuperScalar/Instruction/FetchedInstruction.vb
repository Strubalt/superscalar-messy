Public Class FetchedInstruction

    Private mPC As Integer
    Private mInstr As Byte()
    Private mDetectedAsBranch As Boolean
    Private mTaken As Boolean
    Private mTargetAddress As Integer

    Public Sub New(ByVal pc As Integer, ByVal instr() As Byte)
        Me.mPC = pc
        Me.mInstr = instr
        'Me.mDetectedAsBranch = detectedAsBranch
        'Me.mTaken = taken
        'Me.mTargetAddress = targetAddr
    End Sub

    Public ReadOnly Property PC() As Integer
        Get
            Return Me.mPC
        End Get
    End Property

    Public ReadOnly Property InstrData() As Byte()
        Get
            Return Me.mInstr
        End Get
    End Property

    Public Property DetectedAsBranch() As Boolean
        Get
            Return Me.mDetectedAsBranch
        End Get
        Set(ByVal value As Boolean)
            mDetectedAsBranch = value
        End Set
    End Property

    Public Property IsTaken() As Boolean
        Get
            Return Me.mTaken
        End Get
        Set(ByVal value As Boolean)
            Me.mTaken = value
        End Set
    End Property

    Public Property TargetBranchAddress() As Integer
        Get
            Return mTargetAddress
        End Get
        Set(ByVal value As Integer)
            mTargetAddress = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Dim ret As String = ""
        ret &= Format(Me.mPC, "00000") & " "
        For i As Integer = 3 To 0 Step -1
            Dim temp As String = Format(Me.InstrData(i), "x")
            If temp.Length = 1 Then
                ret &= "0"
            End If
            ret &= temp & " "
        Next
        Return ret

    End Function

End Class
