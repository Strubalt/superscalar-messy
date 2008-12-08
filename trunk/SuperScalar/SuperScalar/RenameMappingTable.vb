'Mapping Architectual Register to Rename Register using their indexes
Public Class RenameMappingTable

    'There should be an entry valid bit in the table, but
    'it is replaced by searching if the key is contained in the table

    Private table As New Generic.Dictionary(Of Integer, Integer)

    Public Function IsArchitectualRegisterRenamed(ByVal architectualRegister As Integer) As Boolean
        Return table.ContainsKey(architectualRegister)
    End Function

    Public Function IsRenameRegisterUsed(ByVal renameRegister As Integer) As Boolean
        Return table.ContainsValue(renameRegister)
    End Function

    Public Function GetRenameRegister(ByVal architectualRegister As Integer) As Integer
        Return table(architectualRegister)
    End Function

    Public Sub AddRename(ByVal architectualRegister As Integer, ByVal renameRegister As Integer)
        Debug.Assert(Not IsRenameRegisterUsed(renameRegister))

        If IsArchitectualRegisterRenamed(architectualRegister) Then
            table(architectualRegister) = renameRegister
        Else
            table.Add(architectualRegister, renameRegister)
        End If
    End Sub

    'Used when retiring
    Public Sub CancelRename(ByVal renameRegister As Integer)
        Dim architectualRegister As Integer = -1
        For Each key As Integer In table.Values
            If table(key) = renameRegister Then
                architectualRegister = key
                Exit For
            End If
        Next
        If Not architectualRegister = -1 Then
            table.Remove(architectualRegister)
        End If

    End Sub

    Public Function GetRenameRegisters() As ICollection(Of Integer)
        Return table.Values
    End Function


End Class

