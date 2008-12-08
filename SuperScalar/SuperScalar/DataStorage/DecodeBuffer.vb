Public Class DecodeBuffer
    Inherits FixedSizeQueue(Of FetchedInstruction)

    Public Sub New(ByVal maxSize As Integer)
        MyBase.New(maxSize)
    End Sub

    Private mResults As New List(Of BranchPredictionResult)
    Public Sub SetBranchPreditionResult(ByVal results As List(Of BranchPredictionResult))
        mResults.Clear()
        mResults.AddRange(results)
    End Sub

    Public Overrides Sub UpdateValue()
        If Not Me.mResults.Count = 0 Then
            For i As Integer = 0 To Me.Input.Count - 1
                Debug.Assert(Me.Input(i).PC = mResults(i).InstructionAddress)
                Me.Input(i).DetectedAsBranch = mResults(i).IsBranchType
                Me.Input(i).IsTaken = mResults(i).IsTakenBranch
                Me.Input(i).TargetBranchAddress = mResults(i).targetAddress
                If mResults(i).IsTakenBranch Then Exit For
            Next
        End If

        MyBase.UpdateValue()

    End Sub



End Class
