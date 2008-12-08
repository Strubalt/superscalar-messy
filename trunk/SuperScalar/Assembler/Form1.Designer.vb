<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form 覆寫 Dispose 以清除元件清單。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    '為 Windows Form 設計工具的必要項
    Private components As System.ComponentModel.IContainer

    '注意: 以下為 Windows Form 設計工具所需的程序
    '可以使用 Windows Form 設計工具進行修改。
    '請不要使用程式碼編輯器進行修改。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnBrowseInputFile = New System.Windows.Forms.Button
        Me.txtInputFile = New System.Windows.Forms.TextBox
        Me.txtOutputFile = New System.Windows.Forms.TextBox
        Me.btnBrowseOutputFile = New System.Windows.Forms.Button
        Me.btnAssemble = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'btnBrowseInputFile
        '
        Me.btnBrowseInputFile.Location = New System.Drawing.Point(342, 28)
        Me.btnBrowseInputFile.Name = "btnBrowseInputFile"
        Me.btnBrowseInputFile.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseInputFile.TabIndex = 0
        Me.btnBrowseInputFile.Text = "Browse"
        Me.btnBrowseInputFile.UseVisualStyleBackColor = True
        '
        'txtInputFile
        '
        Me.txtInputFile.Location = New System.Drawing.Point(19, 30)
        Me.txtInputFile.Name = "txtInputFile"
        Me.txtInputFile.Size = New System.Drawing.Size(293, 22)
        Me.txtInputFile.TabIndex = 1
        '
        'txtOutputFile
        '
        Me.txtOutputFile.Location = New System.Drawing.Point(19, 58)
        Me.txtOutputFile.Name = "txtOutputFile"
        Me.txtOutputFile.Size = New System.Drawing.Size(293, 22)
        Me.txtOutputFile.TabIndex = 3
        '
        'btnBrowseOutputFile
        '
        Me.btnBrowseOutputFile.Location = New System.Drawing.Point(342, 56)
        Me.btnBrowseOutputFile.Name = "btnBrowseOutputFile"
        Me.btnBrowseOutputFile.Size = New System.Drawing.Size(75, 23)
        Me.btnBrowseOutputFile.TabIndex = 2
        Me.btnBrowseOutputFile.Text = "Browse"
        Me.btnBrowseOutputFile.UseVisualStyleBackColor = True
        '
        'btnAssemble
        '
        Me.btnAssemble.Location = New System.Drawing.Point(342, 84)
        Me.btnAssemble.Name = "btnAssemble"
        Me.btnAssemble.Size = New System.Drawing.Size(75, 23)
        Me.btnAssemble.TabIndex = 4
        Me.btnAssemble.Text = "Assemble"
        Me.btnAssemble.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(449, 119)
        Me.Controls.Add(Me.btnAssemble)
        Me.Controls.Add(Me.txtOutputFile)
        Me.Controls.Add(Me.btnBrowseOutputFile)
        Me.Controls.Add(Me.txtInputFile)
        Me.Controls.Add(Me.btnBrowseInputFile)
        Me.Name = "Form1"
        Me.Text = "VerySimpleAssembler"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnBrowseInputFile As System.Windows.Forms.Button
    Friend WithEvents txtInputFile As System.Windows.Forms.TextBox
    Friend WithEvents txtOutputFile As System.Windows.Forms.TextBox
    Friend WithEvents btnBrowseOutputFile As System.Windows.Forms.Button
    Friend WithEvents btnAssemble As System.Windows.Forms.Button

End Class
