Public Class Assembler

    
    
    Public Shared Sub Assemble(ByVal inputFile As String, ByVal outputFile As String)
        Dim fs As New IO.FileStream(inputFile, IO.FileMode.Open)
        Dim sr As New IO.StreamReader(fs)

        Dim AsmLabelTable As New Dictionary(Of String, Integer)
        Dim Commands As New List(Of Command)
        Dim numAutoLabels As Integer = 0
        Try
            Dim lineNum As Integer = 1
            Dim line As String = sr.ReadLine
            While Not line Is Nothing
                Dim lineTokens As List(Of Token) = ProcessLine(line)

                AddToCommands(line, lineNum, Commands, AsmLabelTable, lineTokens, numAutoLabels)
                line = sr.ReadLine
                lineNum += 1
            End While

        Catch ex As Exception
            MsgBox(ex.Message)
            Exit Sub
        Finally
            sr.Close()
            fs.Close()
        End Try

#If DEBUG Then
        For idx As Integer = 0 To Commands.Count - 1
            WriteTokens(idx, Commands(idx))
        Next
        For Each key As String In AsmLabelTable.Keys
            Debug.Write(key & "," & AsmLabelTable(key) & vbTab)
            WriteTokens(AsmLabelTable(key), Commands(AsmLabelTable(key)))
        Next
#End If

        ReplaceAsmLabel(Commands, AsmLabelTable)

#If DEBUG Then
        For idx As Integer = 0 To Commands.Count - 1
            WriteTokens(idx, Commands(idx))
        Next
#End If
        Try
            Dim machineCodes As List(Of Byte) = GenerateMachineCode(Commands)
            SaveFile(machineCodes, outputFile)
        Catch ex As Exception
            MsgBox(ex.ToString())
            Exit Sub
        End Try


        
    End Sub


    Private Shared Sub SaveFile(ByVal machineCodes As List(Of Byte), ByVal outputFile As String)
        Dim fs As IO.FileStream = Nothing
        Try
            If IO.File.Exists(outputFile) Then
                IO.File.Delete(outputFile)
            End If
            fs = New IO.FileStream(outputFile, IO.FileMode.CreateNew)
            For i As Integer = 0 To machineCodes.Count - 1
                fs.WriteByte(machineCodes(i))
            Next
        Catch ex As Exception
        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
            fs = Nothing
        End Try

#If DEBUG Then
        Try
            fs = New IO.FileStream(outputFile, IO.FileMode.Open)
            Dim temp As New List(Of Byte)
            Dim r As Integer = fs.ReadByte
            While r <> -1
                temp.Add(r)
                r = fs.ReadByte
            End While
            If temp.Count <> machineCodes.Count Then
                MsgBox("error when writing machine code to file")
                Exit Try
            Else
                For i As Integer = 0 To temp.Count - 1
                    If temp(i) <> machineCodes(i) Then
                        MsgBox("error when writing machine code to file")
                        Exit Try
                    End If
                Next
            End If

        Catch ex As Exception
        Finally
            If Not fs Is Nothing Then
                fs.Close()
            End If
        End Try
#End If
    End Sub

   
    Private Shared Function GenerateMachineCode(ByVal commands As List(Of Command)) As List(Of Byte)

        Dim machineCodes As New List(Of Byte)
        For Each command As Command In commands
            Dim instr As ISA.InstrDecodedInfo = OpStringDecoder.DecodeCommand(command)
            If instr Is Nothing Then
                Throw New Exception("Invalid Command: (" & command.LineNumber & ")" & command.OrgText)
            Else
                Dim code() As Byte = InstructionEncoder.GenerateMachineCode(instr)
                Debug.Assert(Not code Is Nothing)
                machineCodes.AddRange(code)


#If DEBUG Then
                Dim decoded As ISA.InstrDecodedInfo = _
                    ISA.InstructionDecoder.DecodeOneInstruction(code)
                If decoded.opcode <> instr.opcode Then
                    Debug.WriteLine("error------------------------------------------")
                End If
                If decoded.opcode = 0 Then
                    If decoded.shamt_fd <> instr.shamt_fd Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.rs_fmt <> instr.rs_fmt Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.rt_ft <> instr.rt_ft Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.rd_fs <> instr.rd_fs Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.funct <> instr.funct Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                End If
                If decoded.OpType = ISA.OpEncodingType.I Then

                    If decoded.rs_fmt <> instr.rs_fmt Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.rt_ft <> instr.rt_ft Then
                        Debug.WriteLine("error------------------------------------------")
                    End If
                    If decoded.IimmediateSigned <> instr.IimmediateSigned Then
                        Debug.WriteLine("error------------------------------------------")
                    End If

                End If
                If decoded.OpType = ISA.OpEncodingType.J Then

                    If decoded.JImmediateUnsigned <> instr.JImmediateUnsigned Then
                        Debug.WriteLine("error------------------------------------------")
                    End If


                End If
#End If


            End If


        Next
        Return machineCodes
    End Function

    Private Shared Sub ReplaceAsmLabel(ByVal Commands As List(Of Command), _
        ByVal AsmLabelTable As Dictionary(Of String, Integer))

        For pc As Integer = 0 To Commands.Count - 1
            Dim command As Command = Commands(pc)
            For Each token As Token In command.Tokens
                If AsmLabelTable.ContainsKey(token.Content) Then
                    token.Content = OpStringDecoder.ComputeImmediate(command.Tokens(0).Content, AsmLabelTable(token.Content), pc)
                End If
            Next
        Next
    End Sub

    Private Shared Sub ReplaceGeneratedBranchLabel(ByVal commands As List(Of Command), _
        ByVal oldLabel As String, ByVal newLabel As String)


        For i As Integer = commands.Count - 1 To 0 Step -1
            If commands(i).Tokens.Count = 4 AndAlso commands(i).Tokens(3).Content = oldLabel Then
                commands(i).Tokens(3).Content = newLabel
            End If
        Next
    End Sub

    Private Shared Sub AddToCommands(ByVal orgText As String, ByVal lineNum As Integer, _
        ByVal Commands As List(Of Command), ByVal AsmLabelTable As Dictionary(Of String, Integer), _
        ByVal tokensOfaLine As List(Of Token), ByRef numAutoLabels As Integer)

        If IsEndMainDirective(tokensOfaLine) Then
            Dim preText As String = Commands(Commands.Count - 1).OrgText
            If preText.Contains("$31") Or preText.Contains("$ra") Then
                Commands.RemoveAt(Commands.Count - 1)
            End If
        End If
        RemoveComment(tokensOfaLine)

        If tokensOfaLine.Count > 0 AndAlso tokensOfaLine(0).Type = TokenType.AsmLabel Then

            Dim temp As Integer
            If Integer.TryParse(tokensOfaLine(0).Content.TrimEnd(":"c), temp) Then
                tokensOfaLine(0).Content = "$MyLabel" & numAutoLabels
                ReplaceGeneratedBranchLabel(Commands, temp & "f", tokensOfaLine(0).Content)
                numAutoLabels += 1
            End If
            AsmLabelTable.Add(tokensOfaLine(0).Content, Commands.Count)
            tokensOfaLine.RemoveAt(0)
        End If
        If tokensOfaLine.Count > 0 Then

            Dim trueMips As List(Of List(Of Token)) = GetTrueMips(tokensOfaLine)
            For Each mips As List(Of Token) In trueMips
                Select Case mips(0).Type
                    Case TokenType.Comment
                        'ignore comment
                    Case TokenType.literal
                        Commands.Add(New Command(orgText, lineNum, mips))
                    Case TokenType.AsmLabel
                        '
                        'AddToCommands(orgText, lineNum, Commands, AsmLabelTable, lineTokens)
                End Select
            Next

        End If
    End Sub


    Private Shared Sub WriteTokens(ByVal idx As Integer, _
        ByVal cmd As Command)
        Debug.Write(Format(idx, "0000") & " ")
        For i As Integer = 0 To cmd.Tokens.Count - 1
            Debug.Write(cmd.Tokens(i).Content)
            If cmd.Tokens(i).IsRegister Then
                Debug.Write("(reg)")
            End If
            If cmd.Tokens(i).IsInteger Then
                Debug.Write("(int)")
            End If
            Debug.Write(",")
        Next
        Debug.WriteLine("")
    End Sub

    'Ignore the cases that Immediate is to large
    Private Shared Function GetTrueMips(ByVal lineTokens As List(Of Token)) As List(Of List(Of Token))
        Dim result As New List(Of List(Of Token))

        Select Case lineTokens(0).Content
            Case "break"
                result.Add(lineTokens)

            Case "nop"
                'nop
                lineTokens(0).Content = "sll"
                lineTokens.Add(New Token(TokenType.literal, "$0"))
                lineTokens.Add(New Token(TokenType.literal, "$0"))
                lineTokens.Add(New Token(TokenType.literal, "$0"))
            Case "subu"
                If lineTokens(3).Content.Contains("$") Then
                Else
                    lineTokens(0).Content = "addiu"
                    lineTokens(3).Content = -Integer.Parse(lineTokens(3).Content)
                End If
                result.Add(lineTokens)
            Case "addu"
                If OpStringDecoder.IsRegisterName(lineTokens(3).Content) Then
                Else
                    lineTokens(0).Content = "addiu"
                    lineTokens(3).Content = Integer.Parse(lineTokens(3).Content)

                End If
                result.Add(lineTokens)
            Case "j"
                'j addr;    j $r
                If OpStringDecoder.IsRegisterName(lineTokens(1).Content) Then
                    lineTokens(0).Content = "jr"
                Else

                End If
                result.Add(lineTokens)
            Case "li"
                'li $R, imm  => addiu $R, $0, imm
                lineTokens(0).Content = "addiu"
                lineTokens.Add(New Token(TokenType.literal, lineTokens(2).Content))
                lineTokens(2).Content = "$0"
                result.Add(lineTokens)
            Case "move", "mov"
                'move $r1, $r2 => add $r1, $r2, $0
                lineTokens(0).Content = "add"
                lineTokens.Add(New Token(TokenType.literal, "$0"))
                result.Add(lineTokens)
            Case "slt"
                'slt $r,$r,$r;   slti $r,$r, imm
                If OpStringDecoder.IsRegisterName(lineTokens(3).Content) Then
                Else
                    lineTokens(0).Content = "slti"
                    lineTokens(3).Content = Integer.Parse(lineTokens(3).Content)

                End If
                result.Add(lineTokens)
            Case "bgez"
                Dim tokens1 As New List(Of Token)
                Dim tokens2 As New List(Of Token)
                tokens1.AddRange(New Token() { _
                    New Token(TokenType.literal, "slt"), _
                    New Token(TokenType.literal, "$at"), _
                    New Token(TokenType.literal, lineTokens(1).Content), _
                    New Token(TokenType.literal, "$0")})
                tokens2.AddRange(New Token() { _
                    New Token(TokenType.literal, "beq"), _
                    New Token(TokenType.literal, "$at"), _
                    New Token(TokenType.literal, "$0"), _
                    New Token(TokenType.literal, lineTokens(2).Content)})
                result.Add(tokens1)
                result.Add(tokens2)
            Case "blez"
                'blez $R, AsmLabel  =>
                'slt $at, $0, $R;   beq $at, $0, AsmLabel
                Dim tokens1 As New List(Of Token)
                Dim tokens2 As New List(Of Token)
                tokens1.AddRange(New Token() { _
                    New Token(TokenType.literal, "slt"), _
                    New Token(TokenType.literal, "$at"), _
                    New Token(TokenType.literal, "$0"), _
                    New Token(TokenType.literal, lineTokens(1).Content)})
                tokens2.AddRange(New Token() { _
                    New Token(TokenType.literal, "beq"), _
                    New Token(TokenType.literal, "$at"), _
                    New Token(TokenType.literal, "$0"), _
                    New Token(TokenType.literal, lineTokens(2).Content)})


                result.Add(tokens1)
                result.Add(tokens2)
            Case "jal"
                'filter jal __main
                If lineTokens(1).Content = "__main" Then
                    Debug.WriteLine("filter jal __main")
                Else
                    result.Add(lineTokens)
                End If
            Case Else
                result.Add(lineTokens)
        End Select



        Return result
    End Function


    Private Shared Function IsEndMainDirective(ByVal linetokens As List(Of Token)) As Boolean
        If linetokens.Count > 0 Then
            If linetokens(0).Content.Contains("end") AndAlso _
                linetokens(0).Content.Contains(".end	main") Then

                Return True
            End If
        Else
            Return False
        End If
    End Function


    Private Shared Sub RemoveComment(ByVal lineTokens As List(Of Token))
        Dim commentIdx As Integer = lineTokens.Count
        For i As Integer = 0 To lineTokens.Count - 1
            If lineTokens(i).Type = TokenType.Comment Then
                commentIdx = i
                Exit For
            End If
        Next
        For i As Integer = commentIdx To lineTokens.Count - 1
            lineTokens.RemoveAt(lineTokens.Count - 1)
        Next
    End Sub

    Private Shared Function ProcessLine(ByVal line As String) As List(Of Token)
        Dim state As BaseState = BaseState.InitialState
        Dim tokens As New List(Of Token)
        For i As Integer = 0 To line.Length - 1
            state = state.Process(line(i), tokens)
        Next
        state = state.ProcessEndline(tokens)
        Return tokens
    End Function


End Class
