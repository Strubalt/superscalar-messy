Public Structure RenameMapItem(Of T)
    Public IsRenamed As Boolean
    Public RenamedIndex As Integer
    Public Value As T

    Public Function Clone() As RenameMapItem(Of T)
        Dim r As New RenameMapItem(Of T)
        r.IsRenamed = IsRenamed
        r.RenamedIndex = RenamedIndex
        r.Value = Value
        Return r
    End Function

    Public Overrides Function ToString() As String
        Dim txt As String = ""
        If IsRenamed Then
            txt = "renamed, "
        Else
            txt = "valid, "
        End If
        txt &= RenamedIndex & "; " & Format(Value, "000000000")
        Return txt
    End Function
End Structure
Public Class GPPRegisterFile
    Implements IStorageDevice

    Private Const GPPRegisterCount As Integer = 32

    Private mGeneralRenameMap() As RenameMapItem(Of Long)
    'Private mSpecialRenameMap() As RenameMapItem(Of Long)
    Private mRollBackTags As New List(Of Integer)
    Private mUpdateBackups As New Dictionary(Of Integer, List(Of Update))
    Private mBranchTagTable As BranchTagTable

    Public Sub RollBackSpeculativeTags(ByVal tags As List(Of Integer))
        mRollBackTags.AddRange(tags)
    End Sub

    Private Sub RollBack(ByVal backups As List(Of RenameBackup))
        For i As Integer = backups.Count - 1 To 0 Step -1
            Me.mGeneralRenameMap(backups(i).regIndex) = backups(i).Value
        Next
    End Sub

    Private mConfirmTag As Integer = -1
    Public Sub Confirm(ByVal tag As Integer)
        mConfirmTag = tag
       
    End Sub


    Public Sub New(ByVal table As BranchTagTable)
        'Simplify program by setting all registers as type long
        ReDim mGeneralRenameMap(ISA.RegisterEncoding.hi_lo)
        mBranchTagTable = table
        'ReDim mSpecialRenameMap(ISA.RegisterEncoding.hi_lo)
    End Sub

    Public Sub ExplicitSetValue(ByVal regIndex As Integer, ByVal value As Integer)
        If regIndex < Me.mGeneralRenameMap.Length Then
            Me.mGeneralRenameMap(regIndex).Value = value
        End If
    End Sub

    Public ReadOnly Property RenameMap() As RenameMapItem(Of Long)()
        Get
            Return Me.mGeneralRenameMap
        End Get
    End Property

    Private mSpeculativeBackup As New Dictionary(Of Integer, List(Of RenameBackup))



    Public Sub UpdateValue() Implements IStorageDevice.UpdateValue
        UpdateNonSpeculativeValue(Me.mValueUpdate, True)
        'Speculation Part
        For Each rename As RenamedInfo In Me.mNewRenamedList
            Dim regIndex As Integer = rename.RegIndex
            'rename.BranchTag 
            If rename.IsSpeculative Then AddBackup(rename.BranchTag, rename.RegIndex)
            Me.mGeneralRenameMap(regIndex).IsRenamed = True
            Me.mGeneralRenameMap(regIndex).RenamedIndex = rename.RenamedIndex

        Next

        'Non speculation Part

        'For i As Integer = 0 To Me.mValueUpdate.Count - 1
        '    Dim regIndex As Integer = Me.mValueUpdate(i).Index
        '    AddUpdateBackup(Me.mValueUpdate(i))
        '    Me.mGeneralRenameMap(regIndex).Value = mValueUpdate(i).Value
        '    If Me.mGeneralRenameMap(regIndex).RenamedIndex = mValueUpdate(i).RenamedIndex Then
        '        Me.mGeneralRenameMap(regIndex).IsRenamed = False
        '    End If
        'Next
        If Not Me.mConfirmTag = -1 Then
            If Me.mSpeculativeBackup.ContainsKey(mConfirmTag) Then
                Me.mSpeculativeBackup.Remove(mConfirmTag)
            End If
            If Me.mUpdateBackups.ContainsKey(mConfirmTag) Then
                mUpdateBackups.Remove(mConfirmTag)
            End If
            Me.mConfirmTag = -1
        End If
       
        RollBack()

        mRollBackTags.Clear()
        'mDeallocatingRenamedIndexes.Clear()
        mNewRenamedList.Clear()
        Me.mValueUpdate.Clear()
    End Sub

    Private Sub RollBack()
        For i As Integer = mRollBackTags.Count - 1 To 0 Step -1
            If Me.mSpeculativeBackup.ContainsKey(mRollBackTags(i)) Then
                Dim backups As List(Of RenameBackup) = Me.mSpeculativeBackup(mRollBackTags(i))
                RollBack(backups)
                Me.mSpeculativeBackup.Remove(mRollBackTags(i))
            End If
        Next
        For i As Integer = 0 To mRollBackTags.Count - 1
            If Me.mUpdateBackups.ContainsKey(Me.mRollBackTags(i)) Then
                Dim updates As List(Of Update) = Me.mUpdateBackups(Me.mRollBackTags(i))
                UpdateNonSpeculativeValue(updates, False)
                Me.mUpdateBackups.Remove(Me.mRollBackTags(i))
            End If
        Next
    End Sub

    Private Sub UpdateNonSpeculativeValue(ByVal updates As List(Of Update), ByVal backup As Boolean)
        For i As Integer = 0 To updates.Count - 1
            Dim regIndex As Integer = updates(i).Index
            If backup AndAlso updates(i).RequireBackup Then AddUpdateBackup(updates(i))
            Me.mGeneralRenameMap(regIndex).Value = updates(i).Value
            If Me.mGeneralRenameMap(regIndex).RenamedIndex = updates(i).RenamedIndex Then
                Me.mGeneralRenameMap(regIndex).IsRenamed = False
            End If
        Next

    End Sub

    Private Class RenameBackup
        Public Sub New(ByVal regIdx As Integer, ByVal v As RenameMapItem(Of Long))
            regIndex = regIdx
            Value = v
        End Sub
        Public Value As RenameMapItem(Of Long)
        Public regIndex As Integer
    End Class

    Private Sub AddBackup(ByVal tag As Integer, ByVal regIndex As Integer)

        Dim backups As List(Of RenameBackup)
        If mSpeculativeBackup.ContainsKey(tag) Then
            backups = Me.mSpeculativeBackup(tag)
        Else
            backups = New List(Of RenameBackup)
            Me.mSpeculativeBackup.Add(tag, backups)
        End If

        Dim item As RenameMapItem(Of Long) = Me.mGeneralRenameMap(regIndex).Clone

        backups.Add(New RenameBackup(regIndex, item))
    End Sub

    Private Sub AddUpdateBackup(ByVal update As Update)
        Dim tag As Integer = Me.mBranchTagTable.GetCurrentTag

        Dim backups As List(Of Update)
        If Me.mUpdateBackups.ContainsKey(tag) Then
            backups = Me.mUpdateBackups(tag)
        Else
            backups = New List(Of Update)
            Me.mUpdateBackups.Add(tag, backups)
        End If
        Dim item As New RenameMapItem(Of Long)
        item.Value = update.Value

        backups.Add(update)

    End Sub


    Public Sub GetRegisterValueWithNewRenamed(ByVal index As Integer, ByVal isMSB As Boolean, _
        ByRef isValid As Boolean, ByRef value As Integer)

        For idx As Integer = mNewRenamedList.Count - 1 To 0 Step -1
            Dim rn As RenamedInfo = Me.mNewRenamedList(idx)
            If rn.RegIndex = index Then
                value = rn.RenamedIndex
                isValid = False
                Exit Sub
            End If
        Next


        isValid = Not RenameMap(index).IsRenamed

        If isValid Then
            If isMSB Then
                value = NumericsConvert.GetHigherInt(Me.mGeneralRenameMap(index).Value)
            Else
                value = NumericsConvert.GetLowerInt(Me.mGeneralRenameMap(index).Value)
            End If

        Else
            value = Me.mGeneralRenameMap(index).RenamedIndex
        End If

    End Sub

    Private Class RenamedInfo
        Public Sub New(ByVal speculative As Boolean, ByVal brTag As Integer, _
            ByVal regIdx As Integer, ByVal renamedIdx As Integer)
            Me.IsSpeculative = speculative
            BranchTag = brTag
            Me.RegIndex = regIdx
            Me.RenamedIndex = renamedIdx
        End Sub
        Public IsSpeculative As Boolean
        Public BranchTag As Integer
        Public RegIndex As Integer
        Public RenamedIndex As Integer
    End Class
    Private mNewRenamedList As New List(Of RenamedInfo)
    Public Sub AddRenamedInfo(ByVal speculative As Boolean, ByVal brTag As Integer, _
        ByVal registerIndex As Integer, ByVal renamedIndex As Integer)
        Dim ri As New RenamedInfo(speculative, brTag, registerIndex, renamedIndex)
        mNewRenamedList.Add(ri)
    End Sub

    'Public Sub GetRegisterValueWithNewRenamed(ByVal index As Integer, ByRef isValid As Boolean, _
    '    ByRef value As Integer)
    '    Debug.Assert(False)
    '    'Debug.Assert(index < Me.mRenameMap.Length)
    '    'isValid = Not RenameMap(index).IsRenamed
    '    'value = RenameMap(index).RenamedIndex
    'End Sub

    'Private mDeallocatingRenamedIndexes As New List(Of Integer)
    'Public Sub DeallocateRenamedIndex(ByVal renamedIndex As Integer)
    '    mDeallocatingRenamedIndexes.Add(renamedIndex)
    'End Sub
    Private Class Update
        Inherits ValueUpdate

        Sub New(ByVal idx As Integer, ByVal v As Long, ByVal renamedIdx As Integer, ByVal backup As Boolean)
            MyBase.New(idx, v)
            Me.RenamedIndex = renamedIdx
            Me.RequireBackup = backup
        End Sub
        Public RenamedIndex As Integer
        Public RequireBackup As Boolean
    End Class

    Private mValueUpdate As New List(Of Update)
    Public Sub SetValue(ByVal regIndex As Integer, ByVal value As Long, _
        ByVal renamedIndex As Integer, ByVal record As Boolean)
        mValueUpdate.Add(New Update(regIndex, value, renamedIndex, record))
        'mDeallocatingRenamedIndexes.Add(renamedIndex)
    End Sub
End Class
