Imports ISA

Public Class InstructionEncoder

    Public Const InstrSize As Integer = 4
    Public Shared Function GenerateMachineCode(ByVal instr As InstrDecodedInfo) As Byte()
        Select Case instr.OpType
            Case OpEncodingType.I
                Return GenerateITypeMachineCode(instr)
            Case OpEncodingType.J
                Return GenerateJTypeMachineCode(instr)
            Case OpEncodingType.R
                Return GenerateRTypeMachineCode(instr)
        End Select

        Return Nothing
    End Function



    Public Shared Function GenerateRTypeMachineCode(ByVal instr As InstrDecodedInfo) As Byte()
        Dim result(InstrSize - 1) As Byte
        result(3) = (instr.opcode << 2) Or (instr.rs_fmt >> 3)
        result(2) = (instr.rs_fmt << 5) Or (instr.rt_ft)
        result(1) = (instr.rd_fs << 3) Or (instr.shamt_fd >> 2)
        result(0) = (instr.shamt_fd << 6) Or (instr.funct)
        Return result
    End Function

    Public Shared Function GenerateITypeMachineCode(ByVal instr As InstrDecodedInfo) As Byte()
        Dim result(InstrSize - 1) As Byte
        result(3) = (instr.opcode << 2) Or (instr.rs_fmt >> 3)
        result(2) = (instr.rs_fmt << 5) Or (instr.rt_ft)
        Dim imm As Integer = instr.IimmediateSigned
        result(1) = (imm And &HFF00) >> 8
        result(0) = imm And &HFF
        Return result
    End Function

    Public Shared Function GenerateJTypeMachineCode(ByVal instr As InstrDecodedInfo) As Byte()

        Dim result(InstrSize - 1) As Byte
        result(3) = (instr.opcode << 2) Or (instr.JImmediateUnsigned >> 24)
        result(2) = (instr.JImmediateUnsigned And &HFF0000) >> 16
        result(1) = (instr.JImmediateUnsigned And &HFF00) >> 8
        result(0) = (instr.JImmediateUnsigned And &HFF)
        Return result
    End Function



End Class
