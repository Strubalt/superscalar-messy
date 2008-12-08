Public Class InstrDecodedInfo

    Public Sub New()

    End Sub

    Public Sub Print()
        Debug.WriteLine("opcode = " & opcode)
        Debug.WriteLine("rs_fmt = " & rs_fmt)
        Debug.WriteLine("rt_ft = " & rt_ft)
        Debug.WriteLine("rd_fs = " & rd_fs)
        Debug.WriteLine("shamt_fd = " & shamt_fd)
        Debug.WriteLine("funct = " & funct)
        Debug.WriteLine("IimmediateSigned =   " & IimmediateSigned)
        Debug.WriteLine("IimmediateZeroExtended = " & IimmediateZeroExtended)
        'Debug.WriteLine("JImmediateSigned =   " & JImmediateSigned)
        Debug.WriteLine("JImmediateUnsigned = " & JImmediateUnsigned)
    End Sub

    Public OpType As OpEncodingType
    'Public ExecuteType As ExecuteType

    Public opcode As Byte
    Public rs_fmt As Byte
    Public rt_ft As Byte
    Public rd_fs As Byte
    Public shamt_fd As Byte
    Public funct As RType_Function
    Public IimmediateSigned, IimmediateZeroExtended As Integer
    'Public ITypeUseSigned As Boolean
    Public JImmediateUnsigned As Integer
    Public JTypeUseSigned As Boolean

    Public DetailDecodedData As DetailDecodedData

    Public Function GetRegisterIndex(ByVal field As RegisterField) As RegisterEncoding
        Select Case field
            Case RegisterField.Hi_Lo
                Return ISA.RegisterEncoding.hi_lo
            Case RegisterField.None
                Return ISA.RegisterEncoding._Invalid
            Case RegisterField.Rd
                Return Me.rd_fs
            Case RegisterField.Rs
                Return Me.rs_fmt
            Case RegisterField.Rt
                Return Me.rt_ft
        End Select
    End Function

    Public Function GetTargetRegister() As RegisterEncoding
        Return GetRegisterIndex(DetailDecodedData.TargetRegister)

    End Function

    Public Function GetSourceRegister(ByVal idx As Integer) As RegisterEncoding
        Return GetRegisterIndex(DetailDecodedData.SourceRegister(idx))
        'Select Case DetailDecodedData.SourceRegister(idx)
        '    Case RegisterField.Hi_Lo
        '        Return ISA.RegisterEncoding.hi_lo
        '    Case RegisterField.None
        '        Return ISA.RegisterEncoding._Invalid
        '    Case RegisterField.Rd
        '        Return Me.rd_fs
        '    Case RegisterField.Rs
        '        Return Me.rs_fmt
        '    Case RegisterField.Rt
        '        Return Me.rt_ft
        'End Select
    End Function


    Public Function GetExtendedImmediate() As Integer
        Select Case DetailDecodedData.ConstField
            Case ConstField.None
                Return 0
            Case ConstField.Shamt
                Return Me.shamt_fd
            Case ConstField.SignExtendedI
                Return Me.IimmediateSigned
            Case ConstField.ZeroExtendedI
                Return Me.IimmediateZeroExtended
            Case ConstField.ZeroExtendedJ
                Return Me.JImmediateUnsigned
            Case ConstField.Middle
                Return Me.shamt_fd + Me.rd_fs << 5 + Me.rt_ft << 10 + Me.rs_fmt << 15
        End Select
    End Function
    'Public ReadOnly Property WriteBackToRegister() As Boolean
    '    Get
    '        Select Case Me.ExecuteType
    '            Case ExecuteType.Branch, ExecuteType.Exception, ExecuteType.Store
    '                Return False
    '            Case ExecuteType.FxComplex, ExecuteType.FxSimple, ExecuteType.Load
    '                Return True
    '        End Select
    '    End Get
    'End Property

    'Public ReadOnly Property TargetRegister() As Integer
    '    Get
    '        Select Case Me.OpType
    '            Case ISA.OpType.R
    '                Return Me.rd_fs
    '            Case ISA.OpType.I
    '                Return Me.rt_ft
    '            Case Else
    '                Debug.Assert(False)
    '        End Select
    '    End Get
    'End Property


    'Public ReadOnly Property WriteBackToMemory() As Boolean
    '    Get
    '        Return Me.ExecuteType = ExecuteType.Store
    '    End Get
    'End Property




End Class

Public Class DetailDecodedData

    Sub New(ByVal et As ExecuteType, ByVal SubExeType As Integer, _
        ByVal wb2Reg As Boolean, ByVal wb2Mem As Boolean, _
        ByVal tRegister As RegisterField, ByVal numSrcReg As Integer, _
        ByVal constFld As ConstField, ByVal signed As Boolean, _
        ByVal overflow As Boolean, ByVal srcRegister0 As RegisterField, _
        ByVal srcRegister1 As RegisterField, Optional ByVal isHighValue As Boolean = False)
        Me.ExecuteType = et
        Me.SubExecuteType = SubExeType
        Me.WriteBackToRegister = wb2Reg
        Me.WriteBackToMemory = wb2Mem
        Me.TargetRegister = tRegister
        Me.NumSourceRegister = numSrcReg
        Me.ConstField = constFld
        Me.SignedOperation = signed
        Me.HasOverflow = overflow
        Me.SourceRegister(0) = srcRegister0
        Me.SourceRegister(1) = srcRegister1
        Me.IsHigh = isHighValue
    End Sub
    Public IsHigh As Boolean
    Public ExecuteType As ExecuteType
    Public SubExecuteType As Integer
    Public WriteBackToRegister As Boolean
    Public WriteBackToMemory As Boolean
    Public TargetRegister As RegisterField
    Public NumSourceRegister As Integer
    Public ConstField As ConstField
    Public SignedOperation As Boolean
    Public HasOverflow As Boolean
    Public SourceRegister(1) As RegisterField

End Class


