Public Class RobEntry

    Private mValue As Long
    Private mIsSpeculative As Boolean
    Private mHasFinished As Boolean
    Private mDecodedInstruction As DecodedInstruction
    Private mHasException As Boolean
    Private mExceptionCode As Byte

    Public Sub New(ByVal instr As DecodedInstruction, ByVal speculative As Boolean)
        mDecodedInstruction = instr
        Me.mIsSpeculative = speculative
    End Sub

    Public Property Value() As Long
        Get
            Return Me.mValue
        End Get
        Set(ByVal value As Long)
            Me.mValue = value
        End Set
    End Property
    Public Property IsSpeculative() As Boolean
        Get
            Return Me.mIsSpeculative
        End Get
        Set(ByVal value As Boolean)
            Me.mIsSpeculative = value
        End Set
    End Property
    Public Property HasFinished() As Boolean
        Get
            Return Me.mHasFinished
        End Get
        Set(ByVal value As Boolean)
            Me.mHasFinished = value
        End Set
    End Property

    Public Property HasException() As Boolean
        Get
            Return Me.mHasException
        End Get
        Set(ByVal value As Boolean)
            Me.mHasException = value
        End Set
    End Property

    Public Property ExceptionCode() As Byte
        Get
            Return Me.mExceptionCode
        End Get
        Set(ByVal value As Byte)
            Me.mExceptionCode = value
        End Set
    End Property

    Public ReadOnly Property Instruction() As DecodedInstruction
        Get
            Return Me.mDecodedInstruction
        End Get
    End Property

    Public ReadOnly Property WriteToRegister() As Boolean
        Get
            Return Me.mDecodedInstruction.IsWriteBackToRegister
        End Get
    End Property

    Public ReadOnly Property InstructionAddress() As Integer
        Get
            Return Me.mDecodedInstruction.PC
        End Get
    End Property

    Public ReadOnly Property BranchTag() As Integer
        Get
            Return Me.mDecodedInstruction.BranchTag
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim txt As String = Format(Me.Instruction.PC, "0000")
        Dim tag As String = "Tag: " & Me.BranchTag
        Dim finish As String = IIf(Me.mHasFinished, "->Finished", "Not Finish") & " "
        Dim speculative As String = IIf(Me.mIsSpeculative, "Speculative", "NonSpec") & " "
        Dim exception As String = IIf(Me.mHasException, "**Exception**", "noExc") & " "

        Dim target As String = ""
        If Me.Instruction.IsWriteBackToRegister Then
            target = Me.Instruction.TargetRegisterIndex.ToString
        Else
            target = "     "
        End If
        txt &= tag & "," & target & "," & Format(Me.mValue, "000000000") & _
            "," & finish & "," & speculative & "," & exception
        Return txt

    End Function

End Class
