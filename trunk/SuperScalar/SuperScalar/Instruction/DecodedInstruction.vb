Public Class DecodedInstruction

    Private mPC As Integer

    Private mBranchTag As Integer
    Private mBasicInstrField As BasicInstrFields
    Private mInstrFields As ISA.InstrDecodedInfo
    Public Class BasicInstrFields

        Public Sub New(ByVal fields As ISA.InstrDecodedInfo)
            Me.mInstrFields = fields
        End Sub
        Private mInstrFields As ISA.InstrDecodedInfo

        Public ReadOnly Property OpCode() As Byte
            Get
                Return Me.mInstrFields.opcode
            End Get
        End Property

        Public ReadOnly Property OpType() As ISA.OpEncodingType
            Get
                Return Me.mInstrFields.OpType
            End Get
        End Property

        Public ReadOnly Property RsNum() As Byte
            Get
                Return Me.mInstrFields.rs_fmt
            End Get
        End Property

        Public ReadOnly Property RdNum() As Byte
            Get
                Return Me.mInstrFields.rd_fs
            End Get
        End Property

        Public ReadOnly Property RtNum() As Byte
            Get
                Return Me.mInstrFields.rt_ft
            End Get
        End Property

        Public ReadOnly Property Funct() As ISA.RType_Function
            Get
                Return Me.mInstrFields.funct
            End Get
        End Property

        Public ReadOnly Property Shamt() As Byte
            Get

                Return Me.mInstrFields.shamt_fd
            End Get
        End Property

        Public ReadOnly Property ITypeSignedImm() As Integer
            Get
                Return Me.mInstrFields.IimmediateSigned
            End Get
        End Property

        Public ReadOnly Property ITypeZeroedImm() As Integer
            Get
                Return Me.mInstrFields.IimmediateZeroExtended
            End Get
        End Property

        Public ReadOnly Property JTypeImmUnsigned() As Integer
            Get
                Return Me.mInstrFields.JImmediateUnsigned
            End Get
        End Property
    End Class

    Public Sub New(ByVal branchTag As Integer, _
        ByVal instr As FetchedInstruction)
        Me.mBranchTag = branchTag
        Me.mPC = instr.PC
        mInstrFields = ISA.InstructionDecoder.DecodeOneInstruction(instr.InstrData)

        mBasicInstrField = New BasicInstrFields(mInstrFields)
        
    End Sub

    Public ReadOnly Property PC() As Integer
        Get
            Return Me.mPC
        End Get
    End Property

    Public ReadOnly Property IsHigher4ByteValue() As Boolean
        Get
            Return Me.mInstrFields.DetailDecodedData.IsHigh
        End Get
    End Property

    Public ReadOnly Property IsWriteBackToRegister() As Boolean
        Get
            Return Me.mInstrFields.DetailDecodedData.WriteBackToRegister
        End Get
    End Property

    Public ReadOnly Property IsWriteBackToMemory() As Boolean
        Get
            Return Me.mInstrFields.DetailDecodedData.WriteBackToMemory
        End Get
    End Property

    Public ReadOnly Property TargetRegister() As ISA.RegisterField
        Get
            Return Me.mInstrFields.DetailDecodedData.TargetRegister
        End Get
    End Property

    Public ReadOnly Property ExecuteType() As ISA.ExecuteType
        Get
            Return Me.mInstrFields.DetailDecodedData.ExecuteType
        End Get
    End Property

    Public ReadOnly Property NumSourceRegister() As Integer
        Get
            Return Me.mInstrFields.DetailDecodedData.NumSourceRegister
        End Get
    End Property

    Public ReadOnly Property ConstField() As ISA.ConstField
        Get
            Return Me.mInstrFields.DetailDecodedData.ConstField
        End Get
    End Property

    Public ReadOnly Property BranchTag() As Integer
        Get
            Return Me.mBranchTag
        End Get
    End Property

    Public ReadOnly Property SignedOperation() As Boolean
        Get
            Return Me.mInstrFields.DetailDecodedData.SignedOperation
        End Get
    End Property

    Public ReadOnly Property HasOverflow() As Boolean
        Get
            Return Me.mInstrFields.DetailDecodedData.HasOverflow
        End Get
    End Property

    Public ReadOnly Property SubExecuteType() As Integer
        Get
            Return Me.mInstrFields.DetailDecodedData.SubExecuteType
        End Get
    End Property

    Private ReadOnly Property SrcRegister(ByVal idx As Integer) As ISA.RegisterField
        Get
            Return Me.mInstrFields.DetailDecodedData.SourceRegister(idx)
        End Get
    End Property

    Public ReadOnly Property TargetRegisterIndex() As ISA.RegisterEncoding
        Get
            Return Me.mInstrFields.GetTargetRegister
        End Get
    End Property

    Public ReadOnly Property SrcRegisterIndex(ByVal idx As Integer) As ISA.RegisterEncoding
        Get
            Return Me.mInstrFields.GetSourceRegister(idx)
            'Select Case SrcRegister(idx)
            '    Case ISA.RegisterField.Hi_Lo
            '        Return ISA.RegisterEncoding.hi_lo
            '    Case ISA.RegisterField.Rd
            '        Return Me.mBasicInstrField.RdNum
            '    Case ISA.RegisterField.Rs
            '        Return Me.mBasicInstrField.RsNum
            '    Case ISA.RegisterField.Rt
            '        Return Me.mBasicInstrField.RtNum
            'End Select
        End Get
    End Property

    Public ReadOnly Property ImmediateValue() As Integer
        Get
            Return Me.mInstrFields.GetExtendedImmediate
        End Get
    End Property
  
    Public ReadOnly Property InstrFields() As BasicInstrFields
        Get
            Return Me.mBasicInstrField
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Me.GetInstructionText & vbTab & Me.GetArgumentText
    End Function

    Public Function GetInstructionText() As String
        Select Case Me.mInstrFields.OpType
            Case ISA.OpEncodingType.Err
                Return "err"
            Case ISA.OpEncodingType.I
                Return CType(Me.mInstrFields.opcode, ISA.ITypeOpcode).ToString()
            Case ISA.OpEncodingType.J
                Return CType(Me.mInstrFields.opcode, ISA.JTypeOpcode).ToString()
            Case ISA.OpEncodingType.R
                Return CType(Me.mInstrFields.funct, ISA.RType_Function).ToString()
        End Select
        Return "err"
    End Function

    Private Function GetArgumentText() As String
        Dim txt As String = ""
        If Me.mInstrFields.DetailDecodedData.WriteBackToRegister Then
            txt &= "$" & CType(Me.TargetRegisterIndex, ISA.RegisterEncoding).ToString() & " "
        End If
        If Me.mInstrFields.DetailDecodedData.NumSourceRegister > 0 Then
            txt &= "$" & CType(Me.SrcRegisterIndex(0), ISA.RegisterEncoding).ToString() & " "
        End If

        If Me.ConstField <> ISA.ConstField.None Then
            txt &= "(" & Me.ImmediateValue & ")"
        End If
        If Me.mInstrFields.DetailDecodedData.NumSourceRegister > 1 Then
            txt &= "$" & CType(Me.SrcRegisterIndex(1), ISA.RegisterEncoding).ToString()
        End If
        Return txt
    End Function

End Class
