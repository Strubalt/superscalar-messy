Imports ISA

Public MustInherit Class OpStringDecoder


    Private Shared mRType As New RTypeDecoder
    Private Shared mIType As New ITypeDecoder
    Private Shared mJType As New JTypeDecoder
    Private Shared decoders As New List(Of OpStringDecoder)
    Private Shared RegisterEnumNames As New List(Of String)

    Shared Sub New()
        decoders.Add(mRType)
        decoders.Add(mIType)
        decoders.Add(mJType)
        RegisterEnumNames.AddRange([Enum].GetNames(GetType(ISA.RegisterEncoding)))

    End Sub

    'Public MustOverride Function OpcodeBelongsToType(ByVal opcode As Byte) As Boolean

    Public MustOverride ReadOnly Property OpType() As OpEncodingType

    Public Function IsOperationBelongsToType(ByVal op As String) As Boolean
        Return Me.GetAllOperators.Contains(op)
    End Function

    Public Shared Function ComputeImmediate(ByVal op As String, ByVal AsmLabelIndex As Integer, _
        ByVal instrIndex As Integer) As Integer
        If mIType.IsOperationBelongsToType(op) Then
            'branch => nPC = PC+4+imm<<2
            '  AsmLabelIndex = Index + 1 + imm
            Return AsmLabelIndex - instrIndex - 1
        End If
        If mJType.IsOperationBelongsToType(op) Then
            Return AsmLabelIndex
        End If
    End Function

    Public Shared Function DecodeCommand(ByVal command As Command) As InstrDecodedInfo
        For Each decoder As OpStringDecoder In decoders
            If decoder.IsOperationBelongsToType(command.Tokens(0).Content) Then
                Return decoder.Decode(command.Tokens)
            End If
        Next
        Return Nothing
    End Function

    Public Shared Function IsOperationBelongsToType(ByVal type As OpEncodingType, ByVal op As String) As Boolean
        For Each decoder As OpStringDecoder In decoders
            If decoder.OpType = type Then
                Return decoder.IsOperationBelongsToType(op)
            End If
        Next

    End Function

    Public MustOverride Function Decode(ByVal tokens As List(Of Token)) As InstrDecodedInfo

    Public MustOverride Function GetAllOperators() As IList(Of String)

    Public Shared Function IsRegisterName(ByVal registerName As String) As Boolean
        If registerName.Contains("$"c) Then
            registerName = registerName.Substring(1)
        Else
            Return False
        End If
        Dim result As Byte
        registerName.TrimEnd().TrimStart()

        If Byte.TryParse(registerName, result) Then
            Return True
        End If


        If RegisterEnumNames.Contains(registerName) Then
            Return True
        Else
            Return False
        End If
    End Function

    Public Function GetRegisterNumber(ByVal registerName As String, ByRef number As Byte) As Boolean

        If registerName.Contains("$"c) Then
            registerName = registerName.Substring(1)
        Else
            'Register needs $ sign
            Debug.Assert(False, "Register requires $ sign")

            Return False
        End If
        Dim result As Byte
        registerName.TrimEnd().TrimStart()

        If Byte.TryParse(registerName, result) Then
            number = result
            Return True
        End If


        If [Enum].IsDefined(GetType(ISA.RegisterEncoding), registerName) Then
            number = CByte([Enum].Parse(GetType(ISA.RegisterEncoding), registerName))
            Return True
        Else
            Return False
        End If

    End Function


End Class

Public Class RTypeDecoder
    Inherits OpStringDecoder

    'Private Shared operations As New List(Of String)
    Private Shared mOpTable As New Dictionary(Of String, RType_Function)

    Shared Sub New()
        'operations.AddRange([Enum].GetNames(GetType(RType_Function)))
        Dim type As Type = GetType(RType_Function)
        For Each value As Byte In [Enum].GetValues(type)
            mOpTable.Add([Enum].GetName(type, value).ToLower, value)
        Next
    End Sub

    Public Overrides Function Decode(ByVal tokens As System.Collections.Generic.List(Of Token)) As ISA.InstrDecodedInfo
        If IsOperationBelongsToType(tokens(0).Content) Then
            Dim funct As RType_Function = mOpTable(tokens(0).Content)
            Dim instr As New ISA.InstrDecodedInfo
            instr.opcode = 0
            instr.funct = funct

            Dim err As Boolean = True
            Select Case funct
                'Case RType_Function.nop
                'do nothing
                Case RType_Function.break
                    'break code
                    Dim code As Integer
                    err = Integer.TryParse(tokens(1).Content, code)
                    instr.shamt_fd = code Mod 32
                    instr.rd_fs = (code \ 32) Mod 32

                    instr.rt_ft = (code \ 1024) Mod 32
                    instr.rs_fmt = (code \ 32768) Mod 32

                Case RType_Function.div, RType_Function.divu, _
                    RType_Function.mult, RType_Function.multu
                    'op rs, rt
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rs_fmt)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rt_ft)
                Case RType_Function.sll, RType_Function.srl
                    'RType_Function.sra
                    'op rd, rs, shamt
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rd_fs)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rs_fmt)
                    instr.rt_ft = instr.rs_fmt
                    err = err And Byte.TryParse(tokens(3).Content, instr.shamt_fd)

                Case RType_Function.jr
                    'jr rs
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rs_fmt)

                Case RType_Function.sllv, RType_Function.srlv

                    'op rd, rs, rt
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rd_fs)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rs_fmt)
                    err = err And Me.GetRegisterNumber(tokens(3).Content, instr.rt_ft)

                Case RType_Function.mfhi, RType_Function.mflo
                    'op rd
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rd_fs)
                    instr.rt_ft = instr.rd_fs
                    instr.rs_fmt = instr.rd_fs
                Case Else
                    'op rd, rs, rt
                    err = Me.GetRegisterNumber(tokens(1).Content, instr.rd_fs)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rs_fmt)
                    err = err And Me.GetRegisterNumber(tokens(3).Content, instr.rt_ft)

            End Select
            If Not err Then
                Throw New Exception
            End If
            Return instr
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function GetAllOperators() As System.Collections.Generic.IList(Of String)
        Return New List(Of String)(mOpTable.Keys)
    End Function



    Public Overrides ReadOnly Property OpType() As ISA.Encoding.OpEncodingType
        Get
            Return Encoding.OpEncodingType.R
        End Get
    End Property
End Class

Public Class JTypeDecoder
    Inherits OpStringDecoder

    Private Shared mOpTable As New Dictionary(Of String, ISA.JTypeOpcode)

    Shared Sub New()
        'operations.AddRange([Enum].GetNames(GetType(RType_Function)))
        Dim type As Type = GetType(JTypeOpcode)
        For Each value As Byte In [Enum].GetValues(type)
            mOpTable.Add([Enum].GetName(type, value).ToLower, value)
        Next
    End Sub

    Public Overrides Function Decode(ByVal tokens As System.Collections.Generic.List(Of Token)) As ISA.InstrDecodedInfo
        If Me.IsOperationBelongsToType(tokens(0).Content) Then
            Dim instr As New InstrDecodedInfo
            instr.opcode = mOpTable(tokens(0).Content)
            instr.OpType = Encoding.OpEncodingType.J
            Select Case CType(instr.opcode, JTypeOpcode)
                Case JTypeOpcode.j, JTypeOpcode.jal
                    'op immediate       'without *4
                    If Not Integer.TryParse(tokens(1).Content, instr.JImmediateUnsigned) Then
                        Throw New Exception
                    End If

            End Select
            Return instr
        Else
            Return Nothing
        End If


    End Function

    Public Overrides Function GetAllOperators() As System.Collections.Generic.IList(Of String)
        Return New List(Of String)(mOpTable.Keys)
    End Function



    Public Overrides ReadOnly Property OpType() As ISA.Encoding.OpEncodingType
        Get
            Return Encoding.OpEncodingType.J
        End Get
    End Property
End Class

Public Class ITypeDecoder
    Inherits OpStringDecoder

    Private Shared mOpTable As New Dictionary(Of String, ISA.ITypeOpcode)

    Shared Sub New()
        'operations.AddRange([Enum].GetNames(GetType(RType_Function)))
        Dim type As Type = GetType(ITypeOpcode)
        For Each value As Byte In [Enum].GetValues(type)
            mOpTable.Add([Enum].GetName(type, value).ToLower, value)
        Next
    End Sub

    'Ignore the cases that Immediate is to large
    Public Overrides Function Decode(ByVal tokens As System.Collections.Generic.List(Of Token)) As ISA.InstrDecodedInfo
        If Me.IsOperationBelongsToType(tokens(0).Content) Then
            Dim instr As New InstrDecodedInfo
            instr.opcode = mOpTable(tokens(0).Content)
            instr.OpType = Encoding.OpEncodingType.I
            Dim err As Boolean = True
            Select Case CType(instr.opcode, ITypeOpcode)
                Case ITypeOpcode.lb, ITypeOpcode.lbu, _
                    ITypeOpcode.lh, ITypeOpcode.lhu, _
                    ITypeOpcode.lw, ITypeOpcode.sb, _
                    ITypeOpcode.sh, ITypeOpcode.sw
                    'op $rt, imm($rs)
                    err = err And Me.GetRegisterNumber(tokens(1).Content, instr.rt_ft)
                    err = err And Integer.TryParse(tokens(2).Content, instr.IimmediateSigned)
                    err = err And Me.GetRegisterNumber(tokens(3).Content, instr.rs_fmt)

                Case ITypeOpcode.lui
                    'op $rt, imm
                    err = err And Me.GetRegisterNumber(tokens(1).Content, instr.rt_ft)
                    err = err And Integer.TryParse(tokens(2).Content, instr.IimmediateSigned)

                Case ITypeOpcode.beq, ITypeOpcode.bne
                    'op $rs, $rt, imm
                    err = err And Me.GetRegisterNumber(tokens(1).Content, instr.rs_fmt)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rt_ft)
                    err = err And Integer.TryParse(tokens(3).Content, instr.IimmediateSigned)

                    'Case ITypeOpcode.slti, ITypeOpcode.sltiu
                    '    'op $rt, $rs, imm
                    '    err = err And Me.GetRegisterNumber(tokens(1).Content, instr.rt_ft)
                    '    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rs_fmt)
                    '    err = err And Integer.TryParse(tokens(3).Content, instr.IimmediateSigned)


                Case Else
                    'op $rt, $rs, imm
                    err = err And Me.GetRegisterNumber(tokens(1).Content, instr.rt_ft)
                    err = err And Me.GetRegisterNumber(tokens(2).Content, instr.rs_fmt)
                    err = err And Integer.TryParse(tokens(3).Content, instr.IimmediateSigned)


            End Select
            instr.IimmediateZeroExtended = instr.IimmediateSigned
            If Not err Then
                Throw New Exception
            End If
            Return instr
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function GetAllOperators() As System.Collections.Generic.IList(Of String)
        Return New List(Of String)(mOpTable.Keys)
    End Function


    Public Overrides ReadOnly Property OpType() As ISA.Encoding.OpEncodingType
        Get
            Return Encoding.OpEncodingType.I
        End Get
    End Property
End Class
