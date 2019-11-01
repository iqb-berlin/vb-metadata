Public Class LinkEditControl
    Inherits MDBasisControl

    Public Shared DefaultFolder As String = ""
    Public Shared ReadOnly DefaultFolderChangedEvent As RoutedEvent =
        EventManager.RegisterRoutedEvent("DefaultFolderChanged", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(LinkEditControl))
    'Public Custom Event DefaultFolderChanged As RoutedEventHandler
    '    AddHandler(value As RoutedEventHandler)
    '        Me.AddHandler(DefaultFolderChangedEvent, value)
    '    End AddHandler

    '    RemoveHandler(value As RoutedEventHandler)
    '        Me.RemoveHandler(DefaultFolderChangedEvent, value)
    '    End RemoveHandler

    '    RaiseEvent(sender As Object, e As RoutedEventArgs)
    '        Me.RaiseEvent(e)
    '    End RaiseEvent
    'End Event

    Private Sub HandleEditObjectExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim myMDObject As iqb.mdc.xml.MDObject = fe.GetValue(DataContextProperty)
        If myMDObject IsNot Nothing AndAlso myMDObject.TypeSpec.ContainsKey("type") Then
            Dim linkValue As String = myMDObject.Value
            Dim myFolder As String = DefaultFolder
            Dim cancelled As Boolean = True
            If myMDObject.TypeSpec.Item("type").ToLower = "folderlink" Then
                If String.IsNullOrEmpty(linkValue) Then linkValue = DefaultFolder
                Dim folderpicker As New System.Windows.Forms.FolderBrowserDialog With {
                                                .Description = "Link zu Datei-Ordner ändern",
                                                .ShowNewFolderButton = False,
                                                .SelectedPath = linkValue}

                If folderpicker.ShowDialog = Forms.DialogResult.OK AndAlso linkValue <> folderpicker.SelectedPath Then
                    linkValue = folderpicker.SelectedPath
                    myFolder = linkValue
                    cancelled = False
                End If
            ElseIf myMDObject.TypeSpec.Item("type").ToLower = "filelink" Then
                If Not String.IsNullOrEmpty(linkValue) Then myFolder = IO.Path.GetDirectoryName(linkValue)

                Dim filepicker As New Microsoft.Win32.OpenFileDialog With {.FileName = IO.Path.GetFileName(linkValue),
                    .Filter = "Alle Dateien|*.*", .Multiselect = False, .Title = "Link zu Datei ändern", .InitialDirectory = myFolder}
                If filepicker.ShowDialog Then
                    linkValue = filepicker.FileName
                    myFolder = IO.Path.GetDirectoryName(linkValue)
                    cancelled = False
                End If
            End If

            If Not cancelled Then
                myMDObject.XMD.Value = linkValue
                If DefaultFolder <> myFolder Then
                    DefaultFolder = myFolder
                    Me.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
                    Me.RaiseEvent(New RoutedEventArgs(DefaultFolderChangedEvent))
                End If
            End If
        End If
    End Sub

    Private Sub HandleDeleteExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim DC = fe.GetValue(DataContextProperty)
        If DC IsNot Nothing AndAlso TypeOf DC Is iqb.mdc.xml.MDObject Then
            Dim myMDObject As iqb.mdc.xml.MDObject = DC
            myMDObject.XMD.Value = ""
        End If
    End Sub

    Private Function HandleDeleteCanExecute(ByVal sender As Object, ByVal e As CanExecuteRoutedEventArgs) As Boolean
        Dim fe As FrameworkElement = sender
        Dim DC = fe.GetValue(DataContextProperty)
        If DC IsNot Nothing AndAlso TypeOf DC Is iqb.mdc.xml.MDObject Then
            Dim myMDObject As iqb.mdc.xml.MDObject = DC
            e.CanExecute = myMDObject IsNot Nothing AndAlso Not String.IsNullOrEmpty(myMDObject.Value)
        Else
            e.CanExecute = False
        End If
        Return e.CanExecute
    End Function

End Class
