Public Class RenamedInstruction



    Private mOp_Rob() As Integer    'Operand or RobIndex

    Private mOpValid() As Boolean   'True=> operand; False=> RobIndex
    'Private mHighInt() As Boolean
    Private mRobIndex As Integer
    Private mDecodedInstr As DecodedInstruction
    Private mImmediate As Integer

    Public Sub New(ByVal robIndex As Integer, ByVal decoded As DecodedInstruction, _
        ByVal operand_rob1 As Integer, ByVal opValid1 As Boolean, _
        ByVal operand_rob2 As Integer, ByVal opValid2 As Boolean, _
        ByVal immediate As Integer)

        Me.mRobIndex = robIndex
        Me.mDecodedInstr = decoded
        mOp_Rob = New Integer() {operand_rob1, operand_rob2}
        mOpValid = New Boolean() {opValid1, opValid2}

        Me.mImmediate = immediate
    End Sub

    Public ReadOnly Property DecodedInstruction() As DecodedInstruction
        Get
            Return Me.mDecodedInstr
        End Get
    End Property

    Public ReadOnly Property Immediate() As Integer
        Get
            Return Me.mImmediate
        End Get
    End Property

    Public ReadOnly Property RobIndex() As Integer
        Get
            Return Me.mRobIndex
        End Get
    End Property

    Public ReadOnly Property BranchTag() As Integer
        Get
            Return Me.mDecodedInstr.BranchTag
        End Get
    End Property

    Public ReadOnly Property ExecuteType() As ISA.ExecuteType
        Get
            Return Me.mDecodedInstr.ExecuteType
        End Get
    End Property

    Public ReadOnly Property SubExecuteType() As Integer
        Get
            Return Me.mDecodedInstr.SubExecuteType
        End Get
    End Property

    Public ReadOnly Property IsSigned() As Boolean
        Get
            Return Me.mDecodedInstr.SignedOperation
        End Get
    End Property

    Public ReadOnly Property HasOverflow() As Boolean
        Get
            Return Me.mDecodedInstr.HasOverflow
        End Get
    End Property


    'Public ReadOnly Property DecodedInstr() As DecodedInstruction
    '    Get
    '        Return Me.mDecodedInstr
    '    End Get
    'End Property

    Public Property OperandValid(ByVal idx As Integer) As Boolean
        Get
            Return Me.mOpValid(idx)
        End Get
        Set(ByVal value As Boolean)
            Me.mOpValid(idx) = value
        End Set
    End Property
    'Public Function GetOpRob(ByVal idx As Integer) As Byte()
    '    Return Me.mOp_Rob(idx)
    'End Function

    'Public Function GetOpRobInt(ByVal idx As Integer) As Integer
    '    Return Me.mOp_Rob(idx)
    '    'Return NumericsConvert.GetInteger(Me.mOp_Rob(idx))
    'End Function
    Public Property OpRob(ByVal idx As Integer) As Integer
        Get
            Return Me.mOp_Rob(idx)
        End Get
        Set(ByVal value As Integer)
            Me.mOp_Rob(idx) = value
        End Set
    End Property

    Public Overrides Function ToString() As String


        Dim txt As String = "(" & Format(Me.mRobIndex, "00") & ")-(" & Format(Me.mDecodedInstr.PC, "0000") & ")" & Me.mDecodedInstr.ToString & vbCrLf & vbTab
        For i As Integer = 0 To 1
            txt &= Format(Me.OpRob(i), "00000000")
            If Me.OperandValid(i) Then
                txt &= "(o), "
            Else
                txt &= "(x), "
            End If
        Next
        txt &= Me.mImmediate & "(i)"

        Return txt

    End Function

    'Public ReadOnly Property OpRobRight() As Integer
    '    Get
    '        Return Me.mOp_Rob(1)
    '    End Get
    'End Property
   

End Class
