

Public Class InstructionDecoder


#Region "Detail Decode Info"
   
    Private Shared RTypeDetailData As New Dictionary(Of RType_Function, DetailDecodedData)
    Private Shared ITypeDetailData As New Dictionary(Of ITypeOpcode, DetailDecodedData)
    Private Shared JTypeDetailData As New Dictionary(Of JTypeOpcode, DetailDecodedData)

    Shared Sub New()
        For Each v As RType_Function In [Enum].GetValues(GetType(RType_Function))
            Dim detail As DetailDecodedData = GetRTypeDetail(v)
            RTypeDetailData.Add(v, detail)
        Next
        For Each v As ITypeOpcode In [Enum].GetValues(GetType(ITypeOpcode))
            ITypeDetailData.Add(v, GetITypeDetail(v))
        Next
        For Each v As JTypeOpcode In [Enum].GetValues(GetType(JTypeOpcode))
            JTypeDetailData.Add(v, GetJTypeDetail(v))
        Next
    End Sub

    Private Shared Function GetITypeDetail(ByVal opcode As ITypeOpcode) As DetailDecodedData
        Select Case opcode
            Case ITypeOpcode.addi, ITypeOpcode.addiu
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    ALUSimpleType.AddI, True, False, _
                    RegisterField.Rt, 1, ConstField.SignExtendedI, _
                    True, (opcode = ITypeOpcode.addi), _
                    RegisterField.Rs, RegisterField.None)
            Case ITypeOpcode.andi, ITypeOpcode.ori, ITypeOpcode.xori
                Dim type As ALUSimpleType = ALUSimpleType.And
                If opcode = ITypeOpcode.ori Then
                    type = ALUSimpleType.OrI
                ElseIf opcode = ITypeOpcode.xori Then
                    type = ALUSimpleType.XorI
                ElseIf opcode = ITypeOpcode.andi Then
                    type = ALUSimpleType.AndI
                Else
                    Debug.Assert(False)
                End If
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    type, True, False, _
                    RegisterField.Rt, 1, ConstField.ZeroExtendedI, _
                    False, False, _
                    RegisterField.Rs, RegisterField.None)
            Case ITypeOpcode.slti
                'R[rt]=(R[rs]<SignExtImm)?1:0
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    ALUSimpleType.SetLessThanImm, True, False, _
                    RegisterField.Rt, 1, ConstField.SignExtendedI, _
                    True, False, RegisterField.Rs, RegisterField.None, False)
            Case ITypeOpcode.beq, ITypeOpcode.bne
                Dim type As BranchType = BranchType.C_Equal
                If opcode = ITypeOpcode.bne Then
                    type = BranchType.C_NotEqual
                End If
                Return New DetailDecodedData(ISA.ExecuteType.Branch, _
                    type, False, False, _
                    RegisterField.None, 2, ConstField.SignExtendedI, _
                    True, False, _
                    RegisterField.Rs, RegisterField.Rt)
            Case ITypeOpcode.lb, ITypeOpcode.lbu
                Return New DetailDecodedData(ISA.ExecuteType.Load, _
                    LoadStoreType.Byte1, True, False, _
                    RegisterField.Rt, 1, ConstField.SignExtendedI, _
                    (opcode = ITypeOpcode.lb), False, _
                    RegisterField.Rs, RegisterField.None)
            Case ITypeOpcode.lh, ITypeOpcode.lhu
                Return New DetailDecodedData(ISA.ExecuteType.Load, _
                    LoadStoreType.Byte2, True, False, _
                    RegisterField.Rt, 1, ConstField.SignExtendedI, _
                    (opcode = ITypeOpcode.lh), False, _
                    RegisterField.Rs, RegisterField.None)
            Case ITypeOpcode.lw
                Return New DetailDecodedData(ISA.ExecuteType.Load, _
                    LoadStoreType.Byte4, True, False, _
                    RegisterField.Rt, 1, ConstField.SignExtendedI, _
                    True, False, _
                    RegisterField.Rs, RegisterField.None)
            Case ITypeOpcode.lui
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    ALUSimpleType.LUI, True, False, _
                    RegisterField.Rt, 0, ConstField.SignExtendedI, _
                    True, False, _
                    RegisterField.None, RegisterField.None)
            
            Case ITypeOpcode.sb, ITypeOpcode.sh, ITypeOpcode.sw
                'op $Rt imm($Rs)
                Dim type As LoadStoreType = LoadStoreType.Byte1
                If opcode = ITypeOpcode.sb Then
                    type = LoadStoreType.Byte1
                ElseIf opcode = ITypeOpcode.sh Then
                    type = LoadStoreType.Byte2
                ElseIf opcode = ITypeOpcode.sw Then
                    type = LoadStoreType.Byte4
                Else
                    Debug.Assert(False, "not implement " & opcode)
                End If
                Return New DetailDecodedData(ISA.ExecuteType.Store, _
                    type, False, True, _
                    RegisterField.None, 2, ConstField.SignExtendedI, _
                    True, False, _
                    RegisterField.Rt, RegisterField.Rs)
                'Case ITypeOpcode.slti, ITypeOpcode.sltiu
                '    Dim imm As ConstField = ConstField.SignExtendedI
                '    If opcode = ITypeOpcode.sltiu Then
                '        imm = ConstField.ZeroExtendedI
                '    End If
                '    Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                '        ALUSimpleType.SetLessThan, True, False, _
                '        RegisterField.Rt, 1, imm, _
                '        (opcode = ITypeOpcode.slti), False)
            Case Else
                Debug.Assert(False, "Not implement" & opcode)
                Return Nothing
        End Select
    End Function

    Private Shared Function GetJTypeDetail(ByVal opcode As JTypeOpcode) As DetailDecodedData
        Select Case opcode
            Case JTypeOpcode.j
                Return New DetailDecodedData(ISA.ExecuteType.Branch, _
                    BranchType.U_Addr, False, False, _
                    RegisterField.None, 0, _
                    ConstField.ZeroExtendedJ, False, False, _
                    RegisterField.None, RegisterField.None)
            Case JTypeOpcode.jal
                Return New DetailDecodedData(ISA.ExecuteType.Branch, _
                    BranchType.U_JumpAndLink, False, False, _
                    RegisterField.None, 0, _
                    ConstField.ZeroExtendedJ, False, False, _
                    RegisterField.None, RegisterField.None)
            Case Else
                Debug.Assert(False, "Not implement" & opcode)
                Return Nothing
        End Select
    End Function

    Private Shared Function GetRTypeDetail(ByVal funct As RType_Function) As DetailDecodedData
        Select Case funct
            Case RType_Function.add, RType_Function.addu
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                     ALUSimpleType.Add, True, False, _
                     RegisterField.Rd, 2, _
                     ConstField.None, _
                     True, (funct = RType_Function.add), _
                    RegisterField.Rs, RegisterField.Rt)

            Case RType_Function.and, RType_Function.nor, _
                RType_Function.or, RType_Function.xor

                Dim type As ALUSimpleType = ALUSimpleType.And
                If funct = RType_Function.nor Then
                    type = ALUSimpleType.Nor
                ElseIf funct = RType_Function.or Then
                    type = ALUSimpleType.Or
                ElseIf funct = RType_Function.xor Then
                    type = ALUSimpleType.Xor
                End If
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    type, True, False, _
                    RegisterField.Rd, 2, _
                    ConstField.None, _
                    False, False, _
                    RegisterField.Rs, RegisterField.Rt)
            Case RType_Function.break
                Return New DetailDecodedData(ISA.ExecuteType.Exception, _
                    -1, False, False, _
                    RegisterField.None, 0, _
                    ConstField.Middle, _
                    False, False, _
                    RegisterField.None, RegisterField.None)
            Case RType_Function.div, RType_Function.divu
                Return New DetailDecodedData(ISA.ExecuteType.FxComplex, _
                    ALUComplexType.Div_Mod, True, False, _
                    RegisterField.Hi_Lo, 2, ConstField.None, _
                    True, (funct = RType_Function.div), _
                    RegisterField.Rs, RegisterField.Rt)
            Case RType_Function.mult, RType_Function.multu
                Return New DetailDecodedData(ISA.ExecuteType.FxComplex, _
                    ALUComplexType.Div_Mod, True, False, _
                    RegisterField.Hi_Lo, 2, ConstField.None, _
                    True, False, _
                    RegisterField.Rs, RegisterField.Rt)
            Case RType_Function.jr
                Return New DetailDecodedData(ISA.ExecuteType.Branch, _
                    BranchType.U_Register, False, False, _
                    RegisterField.None, 1, ConstField.None, _
                    False, False, _
                    RegisterField.Rs, RegisterField.None)
            Case RType_Function.mfhi, RType_Function.mflo
                Dim type As ALUSimpleType = ALUSimpleType.MfHigh
                If funct = RType_Function.mflo Then
                    type = ALUSimpleType.MfLow
                End If
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    type, True, False, _
                    RegisterField.Rd, 2, ConstField.None, _
                    False, False, RegisterField.Hi_Lo, _
                    RegisterField.None, funct = RType_Function.mfhi)
            Case RType_Function.sll, RType_Function.srl, _
                RType_Function.sllv, RType_Function.srlv

                Dim type As ALUSimpleType '= ALUSimpleType.ShiftLeft
                Select Case funct
                    Case RType_Function.sll
                        type = ALUSimpleType.ShiftLeftLogic
                    Case RType_Function.srl
                        type = ALUSimpleType.ShiftRightLogic
                    Case RType_Function.sllv
                        type = ALUSimpleType.ShiftLeft
                    Case RType_Function.srlv
                        type = ALUSimpleType.ShiftRight
                End Select
                
                Dim srcRegNum As Integer = 1
                Dim constField As ConstField = constField.Shamt
                Dim registerRight As RegisterField = RegisterField.None
                If funct = RType_Function.sllv OrElse _
                    funct = RType_Function.srlv Then
                    srcRegNum = 2
                    constField = ExecuteSubType.ConstField.None
                    registerRight = RegisterField.Rt
                End If
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    type, True, False, _
                    RegisterField.Rd, srcRegNum, constField, _
                    False, False, _
                    RegisterField.Rs, registerRight)

            Case RType_Function.slt, RType_Function.sltu
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    ALUSimpleType.SetLessThan, True, False, _
                    RegisterField.Rd, 2, ConstField.None, _
                    (funct = RType_Function.slt), False, _
                    RegisterField.Rs, RegisterField.Rt)
            Case RType_Function.sub, RType_Function.subu
                Return New DetailDecodedData(ISA.ExecuteType.FxSimple, _
                    ALUSimpleType.Sub, True, False, _
                    RegisterField.Rd, 2, ConstField.None, _
                    True, (funct = RType_Function.sub), _
                    RegisterField.Rs, RegisterField.Rt)
            Case Else
                Debug.Assert(False, "Not implement" & funct)
                Return Nothing
        End Select
    End Function
#End Region

    Public Shared Sub SetDetailDecodedData(ByVal instr As InstrDecodedInfo)
        Select Case instr.OpType
            Case OpEncodingType.I
                instr.DetailDecodedData = ITypeDetailData(CType(instr.opcode, ITypeOpcode))
            Case OpEncodingType.J
                instr.DetailDecodedData = JTypeDetailData(CType(instr.opcode, JTypeOpcode))
            Case OpEncodingType.R
                instr.DetailDecodedData = RTypeDetailData(instr.funct)
            Case OpEncodingType.Err
                instr.DetailDecodedData = Nothing
            Case Else
                instr.DetailDecodedData = Nothing
        End Select
    End Sub

    Private Shared Function GetOpType(ByVal opcode As Byte) As OpEncodingType
        If opcode = Encoding.RTypeOpcode Then
            Return OpEncodingType.R
        Else
            For Each value As Byte In [Enum].GetValues(GetType(JTypeOpcode))
                If value = opcode Then Return OpEncodingType.J
            Next
            For Each value As Byte In [Enum].GetValues(GetType(ITypeOpcode))
                If value = opcode Then Return OpEncodingType.I
            Next
        End If
        Return OpEncodingType.Err
    End Function

    Public Shared Function DecodeOneInstruction(ByVal instr() As Byte) As InstrDecodedInfo
        Dim result As New InstrDecodedInfo
        Debug.Assert(instr.Length = 4)
        result.opcode = instr(3) >> 2
        result.OpType = GetOpType(result.opcode)
        result.rs_fmt = ((instr(3) And &H3) << 3) Or ((instr(2) And &HE0) >> 5)
        result.rt_ft = (instr(2) And &H1F)
        result.rd_fs = (instr(1) And &HF8) >> 3
        result.shamt_fd = ((instr(1) And &H7) << 2) Or ((instr(0) And &HC0) >> 6)
        result.funct = instr(0) And &H3F

        Dim signedExtention As Integer = &HFFFF0000
        If (instr(1) And &H80) = 0 Then
            signedExtention = 0
        End If
        Const factor3 As Integer = 256 << 16
        Const factor2 As Integer = 256 << 8
        Const factor1 As Integer = 256

        result.IimmediateSigned = signedExtention Or (instr(1) * factor1 + instr(0))
        result.IimmediateZeroExtended = (instr(1) * 16 + instr(0))

        signedExtention = &HFC000000
        If (instr(3) And &H2) = 0 Then
            signedExtention = 0
        End If
        'result.ITypeUseSigned = IsITypeImmediateSignExtension(result.opcode)


        Dim temp As Integer = (instr(3) And &H3) * factor3 + _
                instr(2) * factor2 + instr(1) * factor1 + instr(0)
        'result.JImmediateSigned = signedExtention Or temp
        result.JImmediateUnsigned = temp
        SetDetailDecodedData(result)
        'result.ExecuteType = DecideExecutionType(result)
        Return result
    End Function

    'Public Shared Function IsITypeImmediateSignExtension(ByVal opcode As ITypeOpcode) As Boolean
    '    Select Case opcode
    '        Case ISA.ITypeOpcode.andi, ISA.ITypeOpcode.xori
    '            Return False
    '        Case Else
    '            Return True
    '    End Select

    'End Function


    'Public Shared Function DecideExecutionType(ByVal instr As ISA.InstrProcessing) As ExecuteType
    '    Select Case instr.OpType
    '        Case ISA.OpEncodingType.Err
    '            Return ExecuteType.Exception
    '        Case ISA.OpEncodingType.I
    '            Return GetITypeExecuteType(instr)
    '        Case ISA.OpEncodingType.J
    '            Return GetJTypeExecuteType(instr)
    '        Case ISA.OpEncodingType.R
    '            Return GetRTypeExecuteType(instr)
    '    End Select
    'End Function

    'Private Shared Function GetRTypeExecuteType(ByVal instr As ISA.InstrProcessing) As ExecuteType
    '    Select Case instr.funct
    '        Case ISA.RType_Function.add, ISA.RType_Function.addu, _
    '            ISA.RType_Function.and, ISA.RType_Function.nor, _
    '            ISA.RType_Function.or, ISA.RType_Function.sll, _
    '            ISA.RType_Function.sllv, ISA.RType_Function.slt, _
    '            ISA.RType_Function.sltu, ISA.RType_Function.srl, _
    '            ISA.RType_Function.srlv, ISA.RType_Function.sub, _
    '            ISA.RType_Function.subu, ISA.RType_Function.xor
    '            Return ExecuteType.FxSimple
    '        Case ISA.RType_Function.break
    '            Return ExecuteType.Exception
    '        Case ISA.RType_Function.div, ISA.RType_Function.divu, _
    '            ISA.RType_Function.mult, ISA.RType_Function.multu
    '            Return ExecuteType.FxComplex
    '        Case ISA.RType_Function.jr
    '            Return ExecuteType.Branch
    '        Case ISA.RType_Function.mfhi, ISA.RType_Function.mflo
    '            Return ExecuteType.FxSimple
    '    End Select
    'End Function

    'Private Shared Function GetJTypeExecuteType(ByVal instr As ISA.InstrProcessing) As ExecuteType
    '    Select Case CType(instr.opcode, ISA.Encoding.JTypeOpcode)
    '        Case ISA.JTypeOpcode.j, ISA.JTypeOpcode.jal
    '            Return ExecuteType.Branch
    '        Case Else
    '            Debug.Assert(False, "Unexpected Instruction")
    '    End Select
    'End Function

    'Private Shared Function GetITypeExecuteType(ByVal instr As ISA.InstrProcessing) As ExecuteType
    '    Select Case CType(instr.opcode, ISA.Encoding.ITypeOpcode)
    '        Case ISA.ITypeOpcode.addi, ISA.ITypeOpcode.addiu, ISA.ITypeOpcode.andi, ISA.ITypeOpcode.ori, ISA.ITypeOpcode.xori
    '            ', ISA.ITypeOpcode.slti, ISA.ITypeOpcode.sltiu
    '            Return ExecuteType.FxSimple
    '        Case ISA.ITypeOpcode.lui
    '            Return ExecuteType.FxSimple
    '        Case ISA.ITypeOpcode.beq, ISA.ITypeOpcode.bne, ISA.ITypeOpcode.xori
    '            Return ExecuteType.Branch
    '        Case ISA.ITypeOpcode.lb, ISA.ITypeOpcode.lbu, ISA.ITypeOpcode.lh, ISA.ITypeOpcode.lhu, ISA.ITypeOpcode.lw
    '            Return ExecuteType.Load
    '        Case ISA.ITypeOpcode.sb, ISA.ITypeOpcode.sh, ISA.ITypeOpcode.sw
    '            Return ExecuteType.Store
    '        Case Else
    '            Debug.Assert(False, "Unexpected Instruction")
    '    End Select
    'End Function
End Class
