Imports ISA
Public Class StoreUnit
    Inherits OperationUnit


    Private mStoreQueue As FixedSizeQueue(Of StoreInstruction)
    Private mDataCache As DataCache
    Private IsCurrentCycleStall As Boolean

    Public Sub New(ByVal storeQueue As StoreQueue, ByVal dataCache As DataCache)
        Me.mStoreQueue = storeQueue
        Me.mDataCache = dataCache
    End Sub

    Public Property StoreQueue() As StoreQueue
        Get
            Return Me.mStoreQueue
        End Get
        Set(ByVal value As StoreQueue)
            Me.mStoreQueue = value
        End Set
    End Property

    Public Property DataCache() As DataCache
        Get
            Return Me.mDataCache
        End Get
        Set(ByVal value As DataCache)
            Me.mDataCache = value
        End Set
    End Property

    Private mCurrentCommand As Object
    Public Overrides Sub Operate()
        If IsCurrentCycleStall Then
            IsCurrentCycleStall = False
        Else
            If Me.mCurrentCommand Is Nothing Then
                If Me.mStoreQueue.OutputValid(0) Then
                    Dim cmd As StoreInstruction = Me.StoreQueue.Output(0)
                    If cmd.HasRetiredFromRob Then
                        Me.mDataCache.SetData(cmd.TargetMemoryAddress, _
                            cmd.SourceValue, cmd.NumByteToStore)
                        Me.mStoreQueue.ItemsConsumed = 1
                    End If

                End If
            Else
                Debug.Assert(False)
            End If

        End If

    End Sub

    Public Overrides Sub StallCurrentCycle()
        IsCurrentCycleStall = True
    End Sub
End Class
