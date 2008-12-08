Public MustInherit Class BaseState

    Public MustOverride Function Process(ByVal input As Char, _
        ByVal tokens As List(Of Token)) As BaseState
    Public MustOverride Function ProcessEndline(ByVal tokens As List(Of Token)) As BaseState
    Public MustOverride Sub SetParsedText(ByVal txt As String)


    Private Shared mInitialState As New InitialState
    Private Shared mCommentState As New CommentState
    Private Shared mTokenCandidateState As New TokenCandidateState
    
    Public Shared ReadOnly Property CommentState() As BaseState
        Get
            Return mCommentState
        End Get
    End Property

    Public Shared ReadOnly Property InitialState() As BaseState
        Get
            Return mInitialState
        End Get
    End Property

    Public Shared ReadOnly Property TokenCandidateState() As BaseState
        Get
            Return mTokenCandidateState
        End Get
    End Property


    
End Class

Public Class CommentState
    Inherits BaseState

    Private mParsedText As String
    Public Overrides Function Process(ByVal input As Char, ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState
        BaseState.CommentState.SetParsedText(mParsedText & input)
        Return BaseState.CommentState
    End Function

    Public Overrides Sub SetParsedText(ByVal txt As String)
        mParsedText = txt
    End Sub

    Public Overrides Function ProcessEndline(ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState
        tokens.Add(New Token(TokenType.Comment, Me.mParsedText))
        Me.mParsedText = ""
        Return BaseState.InitialState
    End Function
End Class

Public Class TokenCandidateState
    Inherits BaseState

    Private mParsedText As String
    Public Overrides Function Process(ByVal input As Char, ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState
        Select Case input
            Case ":"c
                tokens.Add(New Token(TokenType.AsmLabel, Me.mParsedText))
                Me.mParsedText = ""
                Return BaseState.InitialState
            Case ","c, " "c, vbTab
                tokens.Add(New Token(TokenType.literal, Me.mParsedText))
                Me.mParsedText = ""
                Return BaseState.InitialState
            Case "("
                If Me.mParsedText.Contains("("c) Then
                    Throw New Exception("Do not support nested ().")
                End If
                tokens.Add(New Token(TokenType.literal, Me.mParsedText))
                BaseState.TokenCandidateState.SetParsedText("(")
                Return BaseState.TokenCandidateState

            Case ")"
                If Me.mParsedText.Contains("(") Then
                    If Me.mParsedText.Contains(")"c) Then
                        Throw New Exception("Do not support nested ().")
                    End If
                    BaseState.TokenCandidateState.SetParsedText(Me.mParsedText.Remove(Me.mParsedText.IndexOf("("c), 1))
                    Return BaseState.TokenCandidateState
                Else
                    Throw New Exception("Without corresponding ( for ).")
                End If

            Case Else
                BaseState.TokenCandidateState.SetParsedText(mParsedText & input)
                Return BaseState.TokenCandidateState
        End Select
    End Function

    Public Overrides Sub SetParsedText(ByVal txt As String)
        Me.mParsedText = txt
    End Sub

    Public Overrides Function ProcessEndline(ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState
        tokens.Add(New Token(TokenType.literal, Me.mParsedText))
        Me.mParsedText = ""
        Return BaseState.InitialState
    End Function
End Class



Public Class InitialState
    Inherits BaseState


    Public Overrides Function Process(ByVal input As Char, _
        ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState

        Select Case input
            Case ","c
                Throw New Exception
            Case "."c, "#"c
                BaseState.CommentState.SetParsedText(input)
                Return BaseState.CommentState
            Case " "c, vbCrLf, vbTab
                Return BaseState.InitialState
            Case Else
                BaseState.TokenCandidateState.SetParsedText(input)
                Return BaseState.TokenCandidateState
        End Select
    End Function

    Public Overrides Sub SetParsedText(ByVal txt As String)
        'do nothing
    End Sub

    Public Overrides Function ProcessEndline(ByVal tokens As System.Collections.Generic.List(Of Token)) As BaseState
        Return BaseState.InitialState
    End Function
End Class
