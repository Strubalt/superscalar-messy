Public Enum ExecuteType
    FxSimple
    FxComplex
    Load
    Store
    Branch
    Exception
End Enum

Public Module ExecuteSubType
    Public Enum RegisterField As Byte
        None
        Rs
        Rt
        Rd
        Hi_Lo
    End Enum

    Public Enum ConstField As Byte
        None
        Shamt
        SignExtendedI
        ZeroExtendedI
        ZeroExtendedJ
        Middle  'for break code
    End Enum

    Public Enum ALUSimpleType
        Add
        AddI
        [Sub]
        [And]
        AndI
        [Or]
        OrI
        [Xor]
        XorI
        [Nor]
        ShiftLeft
        ShiftLeftLogic
        ShiftRight
        ShiftRightLogic
        LUI
        SetLessThan
        SetLessThanImm
        MfHigh
        MfLow
    End Enum

    Public Enum ALUComplexType
        Mult
        Div_Mod
    End Enum

    Public Enum LoadStoreType
        Byte1
        Byte2
        Byte4
    End Enum

   

    Public Enum BranchType
        C_Equal
        C_NotEqual
        U_Addr
        U_Register
        U_JumpAndLink
    End Enum
End Module
