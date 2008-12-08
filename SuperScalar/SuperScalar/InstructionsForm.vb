Public Class InstructionsForm

    Private mProcessor As Processor
    Public Property processor() As Processor
        Get
            Return Me.mProcessor
        End Get
        Set(ByVal value As Processor)
            Me.mProcessor = value

        End Set
    End Property

    Private Sub ShowInstrucitons()
        If Not Me.mProcessor Is Nothing Then
            Dim builder As New System.Text.StringBuilder
            Dim instr()() As Byte = Me.mProcessor.InstrucitonCache.GetAllInstructions()
            If Not instr Is Nothing Then
                For i As Integer = 0 To instr.Length - 1
                    Dim fetch As New FetchedInstruction(i * 4, instr(i))
                    builder.Append(fetch.ToString & vbCrLf)
                    Dim decoded As New DecodedInstruction(0, fetch)
                    builder.Append(vbTab & decoded.ToString & vbCrLf)
                Next
            End If
            Me.txtInstructions.Text = builder.ToString
        End If
    End Sub

    Private Sub InstructionsForm_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Click
        ShowInstrucitons()
        ShowData()
    End Sub

    Private Sub InstructionsForm_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.GotFocus

    End Sub
    
    Private Sub InstructionsForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ShowInstrucitons()
        ShowData()
    End Sub

    Private Sub ShowData()
        If Not Me.mProcessor Is Nothing Then
            Dim builder As New System.Text.StringBuilder
            For i As Integer = Me.mProcessor.MaxDCacheAddress To Me.mProcessor.MaxDCacheAddress - 1000 Step -4
                Dim addr1 As Integer = i - 4
                builder.Append(Format(addr1, "00000000") & ": " & _
                    Me.mProcessor.DataCache.GetInteger(addr1) & vbCrLf)

            Next
            Me.RichTextBox1.Text = builder.ToString
        End If
    End Sub

    Private Sub InstructionsForm_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        ShowInstrucitons()
        ShowData()
    End Sub
End Class