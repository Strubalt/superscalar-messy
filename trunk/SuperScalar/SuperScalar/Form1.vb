Public Class Form1

    Private processor As Processor


    Private Sub LoadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadToolStripMenuItem.Click
        Dim dlg As New OpenFileDialog
        If dlg.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim fs As IO.FileStream = Nothing
            Dim content As New List(Of Byte)

            Try
                fs = New IO.FileStream(dlg.FileName, IO.FileMode.Open)

                Dim data As Integer = fs.ReadByte()
                While data <> -1
                    content.Add(CByte(data))
                    data = fs.ReadByte
                End While
                
            Catch ex As Exception
                MsgBox(ex.ToString())
                Exit Sub
            Finally
                If Not fs Is Nothing Then
                    fs.Close()
                End If
            End Try
            If content.Count = 0 Then
                MsgBox("No content")
                Exit Sub
            End If
            Dim predictor As BaseBranchPredictor = New OneBitBranchPredictor(10, 2)
            If Me.rdo3Bits.Checked Then
                predictor = New ThreeBitBranchPredictor(9, 2)
            ElseIf Me.rdoOneBit.Checked Then
                predictor = New OneBitBranchPredictor(10, 2)
            ElseIf Me.rdoTwoBits.Checked Then
                predictor = New TwoBitBranchPredictor(9, 2)
            ElseIf Me.rdo0bit.Checked Then
                predictor = New NullBranchPredictor
            ElseIf Me.rdoAssociative.Checked Then
                predictor = New AssociativeBranchPredictor(7, 2)
            End If
            'Dim oneBit As New OneBitBranchPredictor(10, 2)
            'Dim twoBits As New TwoBitBranchPredictor(9, 2)
            processor = New Processor(New ProcessorParameters(predictor))
            processor.Load(content.ToArray())
            UpdateUI()
        End If


    End Sub

    Private Sub NextCycleToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub UpdateUI()
        Me.txtPC.Text = "PC: " & Me.processor.PC

        Me.txtInstructionBuffer.Text = ""
        Dim buf As FixedSizeQueue(Of FetchedInstruction) = processor.DecodeBuffer
        For i As Integer = 0 To buf.MaxSize - 1
            If buf.OutputValid(i) Then

                Dim decoded As New DecodedInstruction(-1, buf.Output(i))

                Me.txtInstructionBuffer.Text &= Format(i, "00") & ": " & buf.Output(i).ToString() & _
                    vbCrLf & vbTab & decoded.ToString() & vbCrLf
            End If
        Next

        Me.txtIssueBuffer.Text = ""
        Dim iBuf As FixedSizeQueue(Of DecodedInstruction) = processor.IssueBuffer
        For i As Integer = 0 To iBuf.MaxSize - 1
            If iBuf.OutputValid(i) Then
                Me.txtIssueBuffer.Text &= Format(i, "00") & ": " & iBuf.Output(i).ToString & vbCrLf
            End If
        Next

        Me.txtReservation.Text = ""
        Dim rs As ReservationStations = processor.ReservationStation
        For i As Integer = 0 To rs.Entries.Count - 1
            Me.txtReservation.Text &= rs.Entries(i).ToString() & vbCrLf
        Next

        Me.txtRob.Text = ""
        Dim rob As ReorderBuffer = processor.ReorderBuffer
        Dim robTexts As New List(Of String)
        For i As Integer = rob.HeadIndex To rob.HeadIndex + rob.MaxEntries
            Dim idx As Integer = rob.CircularIndex(i)
            If Not rob.Entry(idx) Is Nothing Then
                robTexts.Add(Format(idx, "00") & " " & rob.Entry(idx).ToString & ", " & rob.Entry(idx).Instruction.ToString)
                'Me.txtRob.Text &= Format(idx, "00") & " " & rob.Entry(idx).ToString & vbCrLf
            End If
        Next
        For i As Integer = 0 To robTexts.Count - 1
            Me.txtRob.Text &= robTexts(i) & vbCrLf
        Next

        Me.txtExecution.Text = ""
        For i As Integer = 0 To processor.ExecutionUnits.Count - 1
            Me.txtExecution.Text &= Format(i, "00") & " " & _
                processor.ExecutionUnits(i).ToString & vbCrLf
        Next

        Me.txtStoreQueue.Text = ""
        For i As Integer = 0 To processor.StoreQueue.MaxSize - 1
            If processor.StoreQueue.OutputValid(i) Then
                Me.txtStoreQueue.Text &= Format(i, "00") & " " & _
                    processor.StoreQueue.Output(i).ToString & vbCrLf
            End If
        Next

        Me.txtRegisterFile.Text = ""
        For i As Integer = 0 To processor.RegisterFile.RenameMap.Length - 1
            Me.txtRegisterFile.Text &= "$" & CType(i, ISA.RegisterEncoding).ToString & _
                " " & processor.RegisterFile.RenameMap(i).ToString & vbCrLf
        Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim target As Integer
        If Integer.TryParse(Me.txtRunToTarget.Text, target) Then
            Dim run As Integer = -1
            For i As Integer = 0 To 10000
                processor.StepExecute()
                If processor.PC >= target AndAlso processor.PC <= target + 16 Then
                    run = i
                    Exit For
                End If
            Next
            UpdateUI()
            MsgBox(run & ", Misprediction = " & processor.MisPredictionCount _
                & "(" & processor.TotalBranchCount & ")")
        End If

    End Sub

    Private Sub btnBreak_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBreak.Click
        Dim target As Integer
        If Integer.TryParse(Me.txtRunToTarget.Text, target) Then
            Dim run As Integer = -1
            For i As Integer = 0 To 100000
                processor.StepExecute()
                'If processor.TotalBranchCount = 309 Then
                '    Exit For
                'End If
                If processor.ContainInstructionReadyToRetireInRob(target) Then
                    run = i
                    Exit For
                End If
            Next
            UpdateUI()
            MsgBox(run & ", Misprediction = " & processor.MisPredictionCount _
                & "(" & processor.TotalBranchCount & ")")
        End If
    End Sub

    Private Sub InstructionsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InstructionsToolStripMenuItem.Click
        Dim frm As New InstructionsForm
        frm.processor = Me.processor
        frm.Show()
    End Sub

    Private Sub DataToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub btnNextCycle_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNextCycle.Click
        If Not processor Is Nothing Then
            processor.StepExecute()
            UpdateUI()

        End If
    End Sub
End Class
