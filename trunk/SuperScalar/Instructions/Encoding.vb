Public Module Encoding
    Public Const RTypeOpcode As Byte = 0

    Public Enum RType_Function As Byte
        'nop = &H0
        sll = &H0       'R[rd]=R[rs] << shamt
        srl = &H2       'R[rd]=R[rs] >> shamt
        'sra = &H3       'R[rd]=R[rs] >>>shamt ?
        sllv = &H4      'R[rd]=R[rs] << R[rt]
        srlv = &H6      'R[rd]=R[rs] >> R[rt]
        'srav = &H7      'R[rd]=R[rs] >>>R[rt]

        jr = &H8        'PC=R[rs]
        '_jalr = &H9

        div = &H1A      'Lo=R[rs]/R[rt]; Hi=R[rs]%R[rt]
        divu = &H1B     'Lo=R[rs]/R[rt]; Hi=R[rs]%R[rt]
        mult = &H18     '{Hi,Lo}=R[rs]*R[rt]
        multu = &H19    '{Hi,Lo}=R[rs]*R[rt]

        add = &H20      'R[rd]=R[rs]+R[rt]
        addu = &H21     'R[rd]=R[rs]+R[rt]
        [sub] = &H22    'R[rd]=R[rs]-R[rt]
        subu = &H23     'R[rd]=R[rs]-R[rt]
        [and] = &H24    'R[rd]=R[rs]&R[rt]
        [or] = &H25     'R[rd]=R[rs]|R[rt]
        [xor] = &H26    'R[rd]=R[rs]^R[rt]
        nor = &H27      'R[rd]=~(R[rs]|R[rt])
        slt = &H2A      'R[rd]=(R[rs]<R[rt])?1:0
        sltu = &H2B     'R[rd]=(R[rs]<R[rt])?1:0


        mfhi = &H10     'R[rd] = R[hi]
        mflo = &H12     'R[rd] = R[lo]


        break = &HD
    End Enum

    Public Enum OpEncodingType
        R
        I
        J
        'FR
        'FI
        Err = -1
    End Enum

    Public Enum JTypeOpcode As Byte
        j = &H2         'nPC=PC[31:28] JImmediate  [00]
        jal = &H3       'nPC={PC[31:28] JImmediate  [00]}; $ra = PC + 4
    End Enum


    Public Enum ITypeOpcode As Byte


        addi = &H8      'R[rt]=R[rs]+ SignExtImm
        addiu = &H9     'R[rt]=R[rs]+ SignExtImm
        andi = &HC      'R[rt]=R[rs]& ZeroExtImm
        ori = &HD       'R[rt]=R[rs]| ZeroExtImm
        xori = &HE      'R[rt]=R[rs]^ ZeroExtImm
        slti = &HA      'R[rt]=(R[rs]<SignExtImm)?1:0
        'sltiu = &HB     'R[rt]=(R[rs]<SignExtImm)?1:0
        beq = &H4       'if(R[rt]==R[rs]) PC=PC+4+SignExtImm<<2
        bne = &H5       'if(R[rt]!=R[rs]) PC=PC+4+SignExtImm<<2

        lb = &H20       'R[rt]=Signext{24'b0,Mem[R[rs]+SignExtImm](7:0)}
        lh = &H21       'R[rt]=Signext{16'b0,Mem[R[rs]+SignExtImm](15:0)}
        lbu = &H24      'R[rt]={24'b0,Mem[R[rs]+SignExtImm](7:0)}
        lhu = &H25      'R[rt]={16'b0,Mem[R[rs]+SignExtImm](15:0)}
        lw = &H23       'R[rt]={Mem[R[rs]+SignExtImm]}

        lui = &HF       'R[rt]={imm,16'b0}


        sb = &H28       'Mem[R[rs]+SignExtImm](7:0)=R[rt](7:0)
        sh = &H29       'Mem[R[rs]+SignExtImm](15:0)=R[rt](15:0)
        sw = &H2B       'Mem[R[rs]+SignExtImm]=R[rt]


    End Enum

    Public Enum ExceptionCode
        None = 0
        Overflow = 1
        DivideByZero = 7
    End Enum
End Module




