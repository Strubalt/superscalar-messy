
Public Class Token

    Public Const RegisterChar As String = "$"
    Public Sub New(ByVal type As TokenType, ByVal content As String)
        Me.mType = type
        Me.mContent = content
        If Char.IsDigit(content(0)) Then
            Dim result As Integer
            Me.mIsInteger = Integer.TryParse(content, result)

        End If
    End Sub
    Private mType As TokenType
    Private mContent As String

    Private mIsInteger As Boolean

    Public Property Content() As String
        Get
            Return Me.mContent
        End Get
        Set(ByVal value As String)
            Me.mContent = value
        End Set
    End Property

    Public ReadOnly Property Type() As TokenType
        Get
            Return Me.mType
        End Get
    End Property

    Public ReadOnly Property IsRegister() As Boolean
        Get
            Return (Me.mContent(0) = RegisterChar)
        End Get
    End Property

    Public ReadOnly Property IsInteger() As Boolean
        Get

        End Get
    End Property

End Class

Public Class Command

    Public OrgText As String
    Public LineNumber As Integer
    Public Tokens As List(Of Token)
    Public Sub New(ByVal orgText As String, ByVal lineNumber As Integer, ByVal tokens As List(Of Token))
        Me.OrgText = orgText
        Me.LineNumber = lineNumber
        Me.Tokens = tokens
    End Sub
End Class

Public Enum TokenType
    AsmLabel
    literal
    Comment
End Enum