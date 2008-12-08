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
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.txtInstructionBuffer = New System.Windows.Forms.RichTextBox
        Me.txtPC = New System.Windows.Forms.TextBox
        Me.txtIssueBuffer = New System.Windows.Forms.RichTextBox
        Me.txtReservation = New System.Windows.Forms.RichTextBox
        Me.txtRob = New System.Windows.Forms.RichTextBox
        Me.txtExecution = New System.Windows.Forms.RichTextBox
        Me.txtStoreQueue = New System.Windows.Forms.RichTextBox
        Me.txtRegisterFile = New System.Windows.Forms.RichTextBox
        Me.txtRunToTarget = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.btnBreak = New System.Windows.Forms.Button
        Me.rdoOneBit = New System.Windows.Forms.RadioButton
        Me.rdoTwoBits = New System.Windows.Forms.RadioButton
        Me.rdo3Bits = New System.Windows.Forms.RadioButton
        Me.rdo0bit = New System.Windows.Forms.RadioButton
        Me.InstructionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.rdoAssociative = New System.Windows.Forms.RadioButton
        Me.btnNextCycle = New System.Windows.Forms.Button
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem, Me.InstructionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(1016, 24)
        Me.MenuStrip1.TabIndex = 0
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(34, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.LoadToolStripMenuItem.Text = "Load"
        '
        'txtInstructionBuffer
        '
        Me.txtInstructionBuffer.Location = New System.Drawing.Point(12, 56)
        Me.txtInstructionBuffer.Name = "txtInstructionBuffer"
        Me.txtInstructionBuffer.Size = New System.Drawing.Size(191, 148)
        Me.txtInstructionBuffer.TabIndex = 1
        Me.txtInstructionBuffer.Text = ""
        '
        'txtPC
        '
        Me.txtPC.Location = New System.Drawing.Point(12, 28)
        Me.txtPC.Name = "txtPC"
        Me.txtPC.Size = New System.Drawing.Size(133, 22)
        Me.txtPC.TabIndex = 2
        '
        'txtIssueBuffer
        '
        Me.txtIssueBuffer.Location = New System.Drawing.Point(223, 56)
        Me.txtIssueBuffer.Name = "txtIssueBuffer"
        Me.txtIssueBuffer.Size = New System.Drawing.Size(305, 148)
        Me.txtIssueBuffer.TabIndex = 3
        Me.txtIssueBuffer.Text = ""
        '
        'txtReservation
        '
        Me.txtReservation.Location = New System.Drawing.Point(223, 210)
        Me.txtReservation.Name = "txtReservation"
        Me.txtReservation.Size = New System.Drawing.Size(305, 504)
        Me.txtReservation.TabIndex = 4
        Me.txtReservation.Text = ""
        '
        'txtRob
        '
        Me.txtRob.Location = New System.Drawing.Point(534, 56)
        Me.txtRob.Name = "txtRob"
        Me.txtRob.Size = New System.Drawing.Size(470, 288)
        Me.txtRob.TabIndex = 5
        Me.txtRob.Text = ""
        '
        'txtExecution
        '
        Me.txtExecution.Location = New System.Drawing.Point(534, 350)
        Me.txtExecution.Name = "txtExecution"
        Me.txtExecution.Size = New System.Drawing.Size(408, 276)
        Me.txtExecution.TabIndex = 6
        Me.txtExecution.Text = ""
        '
        'txtStoreQueue
        '
        Me.txtStoreQueue.Location = New System.Drawing.Point(534, 632)
        Me.txtStoreQueue.Name = "txtStoreQueue"
        Me.txtStoreQueue.Size = New System.Drawing.Size(230, 82)
        Me.txtStoreQueue.TabIndex = 7
        Me.txtStoreQueue.Text = ""
        '
        'txtRegisterFile
        '
        Me.txtRegisterFile.Location = New System.Drawing.Point(12, 210)
        Me.txtRegisterFile.Name = "txtRegisterFile"
        Me.txtRegisterFile.Size = New System.Drawing.Size(191, 504)
        Me.txtRegisterFile.TabIndex = 8
        Me.txtRegisterFile.Text = ""
        '
        'txtRunToTarget
        '
        Me.txtRunToTarget.Location = New System.Drawing.Point(646, 29)
        Me.txtRunToTarget.Name = "txtRunToTarget"
        Me.txtRunToTarget.Size = New System.Drawing.Size(133, 22)
        Me.txtRunToTarget.TabIndex = 9
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(785, 28)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 10
        Me.Button1.Text = "Run To"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'btnBreak
        '
        Me.btnBreak.Location = New System.Drawing.Point(866, 28)
        Me.btnBreak.Name = "btnBreak"
        Me.btnBreak.Size = New System.Drawing.Size(75, 23)
        Me.btnBreak.TabIndex = 12
        Me.btnBreak.Text = "Break"
        Me.btnBreak.UseVisualStyleBackColor = True
        '
        'rdoOneBit
        '
        Me.rdoOneBit.AutoSize = True
        Me.rdoOneBit.Location = New System.Drawing.Point(347, 30)
        Me.rdoOneBit.Name = "rdoOneBit"
        Me.rdoOneBit.Size = New System.Drawing.Size(59, 16)
        Me.rdoOneBit.TabIndex = 13
        Me.rdoOneBit.Text = "One Bit"
        Me.rdoOneBit.UseVisualStyleBackColor = True
        '
        'rdoTwoBits
        '
        Me.rdoTwoBits.AutoSize = True
        Me.rdoTwoBits.Checked = True
        Me.rdoTwoBits.Location = New System.Drawing.Point(412, 31)
        Me.rdoTwoBits.Name = "rdoTwoBits"
        Me.rdoTwoBits.Size = New System.Drawing.Size(65, 16)
        Me.rdoTwoBits.TabIndex = 14
        Me.rdoTwoBits.TabStop = True
        Me.rdoTwoBits.Text = "Two Bits"
        Me.rdoTwoBits.UseVisualStyleBackColor = True
        '
        'rdo3Bits
        '
        Me.rdo3Bits.AutoSize = True
        Me.rdo3Bits.Location = New System.Drawing.Point(483, 31)
        Me.rdo3Bits.Name = "rdo3Bits"
        Me.rdo3Bits.Size = New System.Drawing.Size(71, 16)
        Me.rdo3Bits.TabIndex = 15
        Me.rdo3Bits.Text = "Three Bits"
        Me.rdo3Bits.UseVisualStyleBackColor = True
        '
        'rdo0bit
        '
        Me.rdo0bit.AutoSize = True
        Me.rdo0bit.Location = New System.Drawing.Point(254, 29)
        Me.rdo0bit.Name = "rdo0bit"
        Me.rdo0bit.Size = New System.Drawing.Size(87, 16)
        Me.rdo0bit.TabIndex = 16
        Me.rdo0bit.TabStop = True
        Me.rdo0bit.Text = "No prediction"
        Me.rdo0bit.UseVisualStyleBackColor = True
        '
        'InstructionsToolStripMenuItem
        '
        Me.InstructionsToolStripMenuItem.Name = "InstructionsToolStripMenuItem"
        Me.InstructionsToolStripMenuItem.Size = New System.Drawing.Size(69, 20)
        Me.InstructionsToolStripMenuItem.Text = "Cache Info"
        '
        'rdoAssociative
        '
        Me.rdoAssociative.AutoSize = True
        Me.rdoAssociative.Location = New System.Drawing.Point(560, 31)
        Me.rdoAssociative.Name = "rdoAssociative"
        Me.rdoAssociative.Size = New System.Drawing.Size(75, 16)
        Me.rdoAssociative.TabIndex = 17
        Me.rdoAssociative.TabStop = True
        Me.rdoAssociative.Text = "Associative"
        Me.rdoAssociative.UseVisualStyleBackColor = True
        '
        'btnNextCycle
        '
        Me.btnNextCycle.Location = New System.Drawing.Point(151, 28)
        Me.btnNextCycle.Name = "btnNextCycle"
        Me.btnNextCycle.Size = New System.Drawing.Size(75, 23)
        Me.btnNextCycle.TabIndex = 18
        Me.btnNextCycle.Text = "Next Cycle"
        Me.btnNextCycle.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1016, 726)
        Me.Controls.Add(Me.btnNextCycle)
        Me.Controls.Add(Me.rdoAssociative)
        Me.Controls.Add(Me.rdo0bit)
        Me.Controls.Add(Me.rdo3Bits)
        Me.Controls.Add(Me.rdoTwoBits)
        Me.Controls.Add(Me.rdoOneBit)
        Me.Controls.Add(Me.btnBreak)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.txtRunToTarget)
        Me.Controls.Add(Me.txtRegisterFile)
        Me.Controls.Add(Me.txtStoreQueue)
        Me.Controls.Add(Me.txtExecution)
        Me.Controls.Add(Me.txtRob)
        Me.Controls.Add(Me.txtReservation)
        Me.Controls.Add(Me.txtIssueBuffer)
        Me.Controls.Add(Me.txtPC)
        Me.Controls.Add(Me.txtInstructionBuffer)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Superscalar Test Form"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents txtInstructionBuffer As System.Windows.Forms.RichTextBox
    Friend WithEvents txtPC As System.Windows.Forms.TextBox
    Friend WithEvents txtIssueBuffer As System.Windows.Forms.RichTextBox
    Friend WithEvents txtReservation As System.Windows.Forms.RichTextBox
    Friend WithEvents txtRob As System.Windows.Forms.RichTextBox
    Friend WithEvents txtExecution As System.Windows.Forms.RichTextBox
    Friend WithEvents txtStoreQueue As System.Windows.Forms.RichTextBox
    Friend WithEvents txtRegisterFile As System.Windows.Forms.RichTextBox
    Friend WithEvents txtRunToTarget As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents btnBreak As System.Windows.Forms.Button
    Friend WithEvents rdoOneBit As System.Windows.Forms.RadioButton
    Friend WithEvents rdoTwoBits As System.Windows.Forms.RadioButton
    Friend WithEvents rdo3Bits As System.Windows.Forms.RadioButton
    Friend WithEvents rdo0bit As System.Windows.Forms.RadioButton
    Friend WithEvents InstructionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents rdoAssociative As System.Windows.Forms.RadioButton
    Friend WithEvents btnNextCycle As System.Windows.Forms.Button

End Class
