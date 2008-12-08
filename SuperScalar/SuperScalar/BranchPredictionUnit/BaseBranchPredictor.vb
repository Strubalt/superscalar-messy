Public MustInherit Class BaseBranchPredictor

    Public MustOverride Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult

    Friend MustOverride Sub SetBranchResult(ByVal result As BranchResult)

End Class

Public Class BranchPredictionResult

    Public Shared Function GetNonBranchResult(ByVal instrAddress As Integer) As BranchPredictionResult
        Return New BranchPredictionResult(instrAddress, False, False, -1, -1)
    End Function

    Public Sub New(ByVal instrAddress As Integer, ByVal isBranch As Boolean, _
        ByVal isTaken As Boolean, ByVal TargetAddress_TNT As Integer, ByVal type As ISA.BranchType)
        Me.InstructionAddress = instrAddress
        Me.IsBranchType = isBranch
        Me.IsTakenBranch = isTaken
        Me.targetAddress = TargetAddress_TNT
        Me.BranchType = type
    End Sub

    Public InstructionAddress As Integer
    Public IsBranchType As Boolean
    Public IsTakenBranch As Boolean
    Public targetAddress As Integer
    Public BranchType As ISA.BranchType
End Class

Public Class NullBranchPredictor
    Inherits BaseBranchPredictor

    'Public Sub New(ByVal blockIndexBitNum As Integer, ByVal numWayAssociativeBitNum As Integer)

    'End Sub

    Public Overrides Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult
        Return BranchPredictionResult.GetNonBranchResult(instrAddress)

    End Function

    Friend Overrides Sub SetBranchResult(ByVal result As BranchResult)

    End Sub
End Class

Public Class OneBitBranchPredictor
    Inherits BaseBranchPredictor

    Public Structure BTHEntry
        Public TargetAddress As Integer
        Public IsTaken As Boolean
        Public type As ISA.BranchType
    End Structure

    Dim mCache As Cache(Of BTHEntry)

    Public Sub New(ByVal blockIndexBitNum As Integer, ByVal numWayBitNum As Integer)
        Me.mCache = New Cache(Of BTHEntry)(blockIndexBitNum, numWayBitNum)

    End Sub


    Public Overrides Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult

        Dim isHit As Boolean
        Dim data As BTHEntry
        Me.mCache.GetItem(instrAddress, isHit, data)
        If isHit Then
            Return New BranchPredictionResult(instrAddress, True, _
                data.IsTaken, data.TargetAddress, data.type)
            'isBranch = True
            'isTaken = data.IsTaken
            'TargetAddress_TNT = data.TargetAddress
        Else
            Return BranchPredictionResult.GetNonBranchResult(instrAddress)

        End If
    End Function

    Friend Overrides Sub SetBranchResult(ByVal result As BranchResult)
        Dim entry As New BTHEntry
        entry.TargetAddress = result.BranchTargetAddr
        entry.IsTaken = result.IsTaken
        entry.type = result.BranchType
        Me.mCache.SetItem(result.BranchInstrAddr, entry)
    End Sub

    
End Class

Public Class TwoBitBranchPredictor
    Inherits BaseBranchPredictor

    Public Structure BTHEntry
        
        Public TakenAddress As Integer
        Public CurrentState As Byte
        Public type As ISA.BranchType
    End Structure

    Dim mCache As Cache(Of BTHEntry)

    Public Sub New(ByVal blockIndexBitNum As Integer, ByVal numWayBitNum As Integer)
        Me.mCache = New Cache(Of BTHEntry)(blockIndexBitNum, numWayBitNum)
        'For i As Integer = 0 To mCache.Count - 1
        '    mCache.ExplicitSetItem(i, New BTHEntry(False, True))
        'Next
    End Sub


    Public Overrides Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult

        Dim isHit As Boolean
        Dim data As BTHEntry
        Me.mCache.GetItem(instrAddress, isHit, data)
        If isHit Then
            Return New BranchPredictionResult(instrAddress, True, _
                data.CurrentState > 1, data.TakenAddress, data.type)
            'isBranch = True
            'isTaken = data.IsTaken
            'TargetAddress_TNT = data.TargetAddress
        Else
            Return BranchPredictionResult.GetNonBranchResult(instrAddress)

        End If
    End Function

    Friend Overrides Sub SetBranchResult(ByVal result As BranchResult)
        Dim isHit As Boolean
        Dim entry As BTHEntry
        'Debug.WriteLine(result.BranchInstrAddr)
        Me.mCache.GetItem(result.BranchInstrAddr, isHit, entry)
        If result.IsTaken Then
            entry.TakenAddress = result.BranchTargetAddr
        End If
        If isHit Then
            entry.CurrentState = GetNextState(entry.CurrentState, result.IsTaken)
        Else
            Const InitialTakenState As Integer = 2
            Const InitialNotTakenState As Integer = 1
            If result.IsTaken Then
                entry.CurrentState = InitialTakenState
            Else
                entry.CurrentState = InitialNotTakenState
            End If
        End If
        Me.mCache.SetItem(result.BranchInstrAddr, entry)
    End Sub

    Public Function GetNextState(ByVal currentState As Byte, ByVal isTaken As Boolean) As Byte
        If isTaken Then
            currentState = Math.Min(3, currentState + 1)
        Else
            currentState = Math.Max(0, currentState - 1)
        End If
        Return currentState
    End Function


End Class

Public Class AssociativeBranchPredictor
    Inherits BaseBranchPredictor

    Public Structure BTHEntry

        Public TakenAddress As Integer
        Public CurrentState As Byte
        Public type As ISA.BranchType
    End Structure

    Dim mCache(3) As Cache(Of BTHEntry)
    Private GlobalPredictor As Byte = 1

    Public Sub New(ByVal blockIndexBitNum As Integer, ByVal numWayBitNum As Integer)
        For i As Integer = 0 To mCache.Length - 1
            Me.mCache(i) = New Cache(Of BTHEntry)(blockIndexBitNum, numWayBitNum)
        Next

        'For i As Integer = 0 To mCache.Count - 1
        '    mCache.ExplicitSetItem(i, New BTHEntry(False, True))
        'Next
    End Sub


    Public Overrides Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult

        Dim isHit As Boolean
        Dim data As BTHEntry
        Me.mCache(GlobalPredictor).GetItem(instrAddress, isHit, data)
        If isHit Then
            Return New BranchPredictionResult(instrAddress, True, _
                data.CurrentState > 1, data.TakenAddress, data.type)
            'isBranch = True
            'isTaken = data.IsTaken
            'TargetAddress_TNT = data.TargetAddress
        Else
            Return BranchPredictionResult.GetNonBranchResult(instrAddress)

        End If
    End Function

    Friend Overrides Sub SetBranchResult(ByVal result As BranchResult)
        Dim isHit As Boolean
        Dim entry As BTHEntry
        GlobalPredictor = Me.GetNextState(Me.GlobalPredictor, result.IsTaken)
        Me.mCache(GlobalPredictor).GetItem(result.BranchInstrAddr, isHit, entry)
        If result.IsTaken Then
            entry.TakenAddress = result.BranchTargetAddr
        End If
        If isHit Then
            entry.CurrentState = GetNextState(entry.CurrentState, result.IsTaken)
        Else
            Const InitialTakenState As Integer = 2
            Const InitialNotTakenState As Integer = 1
            If result.IsTaken Then
                entry.CurrentState = InitialTakenState
            Else
                entry.CurrentState = InitialNotTakenState
            End If
        End If
        Me.mCache(GlobalPredictor).SetItem(result.BranchInstrAddr, entry)
    End Sub

    Public Function GetNextState(ByVal currentState As Byte, ByVal isTaken As Boolean) As Byte
        If isTaken Then
            currentState = Math.Min(3, currentState + 1)
        Else
            currentState = Math.Max(0, currentState - 1)
        End If
        Return currentState
    End Function

End Class

Public Class ThreeBitBranchPredictor
    Inherits BaseBranchPredictor

    Public Structure BTHEntry

        Public TakenAddress As Integer
        Public CurrentState As Byte
        Public type As ISA.BranchType
    End Structure

    Dim mCache As Cache(Of BTHEntry)

    Public Sub New(ByVal blockIndexBitNum As Integer, ByVal numWayBitNum As Integer)
        Me.mCache = New Cache(Of BTHEntry)(blockIndexBitNum, numWayBitNum)
        'For i As Integer = 0 To mCache.Count - 1
        '    mCache.ExplicitSetItem(i, New BTHEntry(False, True))
        'Next
    End Sub

    Private Function IsTaken(ByVal state As Byte) As Boolean
        If state = 7 Or state = 5 Or state = 3 Or state = 6 Then
            Return True
        Else
            Return False
        End If
    End Function


    Public Overrides Function Predict(ByVal instrAddress As Integer) As BranchPredictionResult

        Dim isHit As Boolean
        Dim data As BTHEntry
        Me.mCache.GetItem(instrAddress, isHit, data)
        If isHit Then
            Return New BranchPredictionResult(instrAddress, True, _
                IsTaken(data.CurrentState), data.TakenAddress, data.type)
            'isBranch = True
            'isTaken = data.IsTaken
            'TargetAddress_TNT = data.TargetAddress
        Else
            Return BranchPredictionResult.GetNonBranchResult(instrAddress)

        End If
    End Function

    Friend Overrides Sub SetBranchResult(ByVal result As BranchResult)
        Dim isHit As Boolean
        Dim entry As BTHEntry
        Me.mCache.GetItem(result.BranchInstrAddr, isHit, entry)
        If result.IsTaken Then
            entry.TakenAddress = result.BranchTargetAddr
        End If
        If isHit Then
            entry.CurrentState = GetNextState(entry.CurrentState, result.IsTaken)
        Else
            Const InitialTakenState As Integer = 5
            Const InitialNotTakenState As Integer = 1
            If result.IsTaken Then
                entry.CurrentState = InitialTakenState
            Else
                entry.CurrentState = InitialNotTakenState
            End If
        End If
        Me.mCache.SetItem(result.BranchInstrAddr, entry)
    End Sub

    Public Function GetNextState(ByVal currentState As Byte, ByVal isTaken As Boolean) As Byte
        currentState = (currentState << 1) And &H7
        If isTaken Then
            currentState = currentState Or 1
            'Else
            'currentState = Math.Max(0, currentState - 1)
        End If
        Return currentState
    End Function


End Class