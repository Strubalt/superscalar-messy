Public Class Form1

    Private Sub btnOpen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseInputFile.Click
        Dim dialog As New OpenFileDialog
        If dialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtInputFile.Text = dialog.FileName
        End If
    End Sub


    Private Sub btnBrowseOutputFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowseOutputFile.Click
        Dim dialog As New SaveFileDialog
        If dialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Me.txtOutputFile.Text = dialog.FileName
        End If
    End Sub

    Private Sub btnAssemble_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAssemble.Click
        If Me.txtInputFile.Text <> "" AndAlso Me.txtOutputFile.Text <> "" Then
            Assembler.Assemble(Me.txtInputFile.Text, Me.txtOutputFile.Text)
        End If
    End Sub
End Class
