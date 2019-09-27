Imports iqb.lib.components
Imports Newtonsoft.Json

Class MainWindow
    Public Shared ReadOnly ClearSearchString As RoutedUICommand = New RoutedUICommand("Filter löschen", "ClearSearchString", GetType(MainWindow))

    Public Shared ReadOnly XDocMCatProperty As DependencyProperty = DependencyProperty.Register("XDocMCat", GetType(XDocument), GetType(MainWindow))
    Public Property XDocMCat As XDocument
        Get
            Return GetValue(XDocMCatProperty)
        End Get
        Set(ByVal value As XDocument)
            SetValue(XDocMCatProperty, value)
        End Set
    End Property

    Private NewMDDefId As String = Nothing
    Private XDocMCatChanged As Boolean = False
    Private XDocMCatFilename As String = Nothing

    Private Sub MainApplication_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf MyUnhandledExceptionEventHandler

        Me.Title = My.Application.Info.AssemblyName
        Me.XDocMCat = Nothing

        DialogFactory.MainWindow = Me

        Dim SettingsOK As Boolean = True
        Dim ErrMsg As String = "Es gibt ein Problem bei dem Versuch, die alten Programmeinstellungen zu laden. Bitte deinstallieren Sie die Anwendung über die Systemsteuerung und installieren Sie sie dann erneut!"
        Dim UserConfigFilename As String = ""
        Try
            'neue Programmversion -> alte Settings holen
            If Not My.Settings.updated Then
                My.Settings.Upgrade()
                My.Settings.updated = True
                My.Settings.Save()
            End If
        Catch ex As Configuration.ConfigurationException
            SettingsOK = False
            If ex.InnerException Is Nothing Then
                Debug.Print("Configuration.ConfigurationException ohne InnerException")
            Else
                ErrMsg += " Alternativ können Sie die unten genannte Datei löschen (Achtung: Apps ist ein verstecktes Verzeichnis)." + vbNewLine + vbNewLine + ex.InnerException.Message
                Debug.Print(ex.InnerException.Message)
                Dim pos As Integer = ex.InnerException.Message.IndexOf("(")
                If pos > 0 Then
                    UserConfigFilename = ex.InnerException.Message.Substring(pos + 1)
                    pos = UserConfigFilename.IndexOf("\user.config ")
                    If pos > 0 Then
                        UserConfigFilename = UserConfigFilename.Substring(0, pos) + "\user.config"
                        Debug.Print(">>" + UserConfigFilename + "<<")
                    Else
                        UserConfigFilename = ""
                    End If
                End If
            End If
        End Try

        If Not SettingsOK Then
            If Not String.IsNullOrEmpty(UserConfigFilename) AndAlso
                UserConfigFilename.IndexOfAny(IO.Path.GetInvalidFileNameChars()) < 0 AndAlso
                IO.File.Exists(UserConfigFilename) Then
                Try
                    IO.File.Delete(UserConfigFilename)
                    ErrMsg = "Die lokalen Programmeinstellungen mussten gelöscht werden. Bitte starten Sie die Anwendung erneut!"
                Catch ex As Exception
                    ErrMsg += vbNewLine + vbNewLine + "Löschen gescheitert: " + ex.Message
                End Try
            End If
            DialogFactory.MsgError(Me, Me.Title, ErrMsg)
            Me.Close()
        End If


        CommandBindings.Add(New CommandBinding(MainWindow.ClearSearchString, AddressOf HandleClearSearchStringExecuted))

        CommandBindings.Add(New CommandBinding(ApplicationCommands.Open, AddressOf HandleOpenExecuted)) 'Studie wählen
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Save, AddressOf HandleSaveExecuted, AddressOf HandleSaveCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.EditCatMetadata, AddressOf HandleEditCatMetadataExecuted, AddressOf HandleCatLoadedCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.SaveAs, AddressOf HandleSaveAsExecuted, AddressOf HandleCatLoadedCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.OpenWebCat, AddressOf HandleOpenWebCatExecuted))
        If iqb.lib.windows.ADFactory.GetMyName() = "mechtelm" Then CommandBindings.Add(New CommandBinding(AppCommands.SaveTheWorld, AddressOf HandleSaveTheWorldExecuted))

        ApplicationCommands.Open.Execute(Nothing, Nothing)
    End Sub

    Private Sub MyUnhandledExceptionEventHandler(sender As Object, e As UnhandledExceptionEventArgs)
        Dim MsgText As String = "??"
        If TypeOf (e.ExceptionObject) Is System.Exception Then
            Dim myException As System.Exception = e.ExceptionObject
            MsgText = myException.Message
            If myException.InnerException IsNot Nothing Then MsgText += "; " + myException.InnerException.Message
            If Not String.IsNullOrEmpty(myException.StackTrace) Then
                If myException.StackTrace.Length > 500 Then
                    MsgText += vbNewLine + myException.StackTrace.Substring(0, 500) + "..."
                Else
                    MsgText += vbNewLine + myException.StackTrace
                End If
            End If
        ElseIf TypeOf (e.ExceptionObject) Is Runtime.CompilerServices.RuntimeWrappedException Then
            Dim myException As Runtime.CompilerServices.RuntimeWrappedException = e.ExceptionObject
            If myException.InnerException IsNot Nothing Then MsgText += "; " + myException.InnerException.Message
            If Not String.IsNullOrEmpty(myException.StackTrace) Then
                If myException.StackTrace.Length > 500 Then
                    MsgText += vbNewLine + myException.StackTrace.Substring(0, 500) + "..."
                Else
                    MsgText += vbNewLine + myException.StackTrace
                End If
            End If
        End If

        DialogFactory.MsgError(Me, "Absturz " + My.Application.Info.AssemblyName, "Die Anwendung hat einen unerwarteten Abbruch erlitten. Folgende Informationen könnten bei der Fehlersuche helfen:" +
                               vbNewLine + vbNewLine + MsgText)

        Me.Close()
    End Sub
    '############################################################################################
    Public Function FilterList(item As Object) As Boolean
        If String.IsNullOrEmpty(TBSearchString.Text) Then
            Return True
        Else
            Dim sstr As String = TBSearchString.Text.ToUpper
            If item IsNot Nothing AndAlso TypeOf (item) Is XElement Then
                Return CType(item, XElement).Element("Label").Value.ToUpper.IndexOf(sstr) >= 0
            Else
                Return False
            End If
        End If
    End Function

    Private Sub TBSearchString_Changed(sender As Object, e As TextChangedEventArgs)
        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBMetadatadefs.ItemsSource)
        If cv IsNot Nothing Then cv.Refresh()
    End Sub

    Private Sub HandleClearSearchStringExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        TBSearchString.Text = ""
        TBSearchString.Focus()
    End Sub
    Private Sub Me_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If XDocMCatChanged AndAlso
            DialogFactory.YesNo(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?") = vbOK Then
            ApplicationCommands.Save.Execute(Nothing, Nothing)
        End If
    End Sub

    Private Sub HandleOpenExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim filepicker As New Microsoft.Win32.OpenFileDialog With {.FileName = My.Settings.lastfile_mdcat, .Filter = "XML-Dateien|*.xml",
                                                                           .DefaultExt = "Xml", .Title = "MD-Katalog öffnen"}
        If filepicker.ShowDialog Then
            My.Settings.lastfile_mdcat = filepicker.FileName
            My.Settings.Save()

            Try
                If XDocMCat IsNot Nothing Then
                    RemoveHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                End If
                XDocMCat = XDocument.Load(filepicker.FileName)
                AddHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                XDocMCatChanged = False
                Dim fp As String = IO.Path.GetDirectoryName(filepicker.FileName)
                If iqb.lib.windows.ADFactory.canWriteToFolder(fp) Then
                    XDocMCatFilename = filepicker.FileName
                Else
                    XDocMCatFilename = Nothing
                End If
                Me.Title = My.Application.Info.AssemblyName + " - " + IO.Path.GetFileName(XDocMCatFilename)
                LoadMDDefList()
            Catch ex As Exception
                DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Konnte Datei nicht öffnen:" + vbNewLine + ex.Message)
            End Try
        End If
    End Sub

    Private Sub Notify_XDocMCatChanged(sender As Object, e As System.Xml.Linq.XObjectChangeEventArgs)
        XDocMCatChanged = True
    End Sub

    Private Function HandleSaveCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = XDocMCatChanged
        Return e.CanExecute
    End Function

    Private Function HandleCatLoadedCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = Me.XDocMCat IsNot Nothing
        Return e.CanExecute
    End Function

    Private Sub SaveFile(ForceFileSelectDialog As Boolean)
        If String.IsNullOrEmpty(XDocMCatFilename) OrElse ForceFileSelectDialog Then
            Dim filepicker As New Microsoft.Win32.SaveFileDialog With {.FileName = My.Settings.lastfile_mdcat, .Filter = "MD-Katalog|*.xml",
                                                                           .DefaultExt = "xml", .Title = "Metadatenkatalog speichern"}
            If filepicker.ShowDialog Then
                My.Settings.lastfile_mdcat = filepicker.FileName
                My.Settings.Save()

                XDocMCatFilename = My.Settings.lastfile_mdcat
                Me.Title = My.Application.Info.AssemblyName + " - " + IO.Path.GetFileName(XDocMCatFilename)
            End If
        End If
        If Not String.IsNullOrEmpty(XDocMCatFilename) Then
            Try
                XDocMCat.Save(XDocMCatFilename)
                XDocMCatChanged = False
            Catch ex As Exception
                DialogFactory.MsgError(Me, "Speichern Metadatenkatalog '" + IO.Path.GetFileName(XDocMCatFilename) + "'", "Konnte nicht speichern:" + vbNewLine + ex.Message)
            End Try
        End If
    End Sub

    Private Sub HandleSaveExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If String.IsNullOrEmpty(XDocMCatFilename) Then
            SaveFile(True)
        Else
            If XDocMCatChanged Then SaveFile(False)
        End If
    End Sub

    Private Sub HandleSaveAsExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        SaveFile(True)
    End Sub

    Private Sub LoadMDDefList(Optional PreSelectedMDDefId As String = Nothing)
        If XDocMCat Is Nothing Then
            LBMetadatadefs.ItemsSource = Nothing
        Else
            LBMetadatadefs.ItemsSource = XDocMCat.Root.Elements("MDDef")
        End If

        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBMetadatadefs.ItemsSource)
        If cv IsNot Nothing Then
            'cv.GroupDescriptions.Add(New PropertyGroupDescription("Attribute[group].Value"))
            cv.SortDescriptions.Add(New ComponentModel.SortDescription("Element[Label].Value", ComponentModel.ListSortDirection.Ascending))
            cv.Filter = AddressOf FilterList
            cv.MoveCurrentToFirst()
        End If

        If Not String.IsNullOrEmpty(NewMDDefId) Then
            PreSelectedMDDefId = NewMDDefId
            NewMDDefId = Nothing
        End If

        If Not String.IsNullOrEmpty(PreSelectedMDDefId) Then
            LBMetadatadefs.SelectedValue = PreSelectedMDDefId
        End If

    End Sub

    Private Sub HandleEditCatMetadataExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim myDlg As New EditCatCoreDataDialog With {.Owner = Me, .Title = AppCommands.EditCatMetadata.Text, .XCat = Me.XDocMCat}
        myDlg.ShowDialog()
    End Sub

    Private Sub HandleOpenWebCatExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim mbresult As MessageBoxResult = MessageBoxResult.OK
        If XDocMCatChanged Then
            mbresult = DialogFactory.YesNoCancel(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?")
            If mbresult = MessageBoxResult.OK Then
                ApplicationCommands.Save.Execute(Nothing, Nothing)
            End If
        End If
        If mbresult <> MessageBoxResult.Cancel Then
            Dim WebCatAddr As String = DialogFactory.InputText(Me, AppCommands.OpenWebCat.Text, "Bitte Url eingeben", My.Settings.last_webcat, "")
            If Not String.IsNullOrEmpty(WebCatAddr) Then
                My.Settings.last_webcat = WebCatAddr
                My.Settings.Save()

                Dim newAppTitle As String = My.Application.Info.AssemblyName + " - "

                Dim WebCatAddr_splits As String() = WebCatAddr.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
                If WebCatAddr_splits.Count > 1 Then
                    Dim prefix As String = WebCatAddr_splits(0).ToUpper
                    If prefix = "DOI" Then
                        Using client As New Net.WebClient()
                            Try
                                Dim responseString As String = client.DownloadString("https://doi.org/api/handles/" + WebCatAddr_splits(1))
                                Dim responseObject As Linq.JObject = JsonConvert.DeserializeObject(responseString)
                                For Each o As Linq.JObject In From i In responseObject("values")
                                    If o("type") = "URL" Then
                                        WebCatAddr = o("data")("value")
                                        Exit For
                                    End If
                                Next
                                newAppTitle = newAppTitle + WebCatAddr + " (" + WebCatAddr_splits(1) + ")"
                            Catch ex As Exception
                                WebCatAddr = Nothing
                                DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Konnte DOI nicht auflösen:" + vbNewLine + ex.Message)
                            End Try
                        End Using
                    Else
                        newAppTitle = newAppTitle + WebCatAddr
                    End If
                Else
                    newAppTitle = newAppTitle + WebCatAddr
                End If
                If Not String.IsNullOrEmpty(WebCatAddr) Then
                    Try
                        If XDocMCat IsNot Nothing Then
                            RemoveHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                        End If
                        Using client As New Net.WebClient()
                            Using mstream As New IO.MemoryStream(client.DownloadData(WebCatAddr))
                                Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(mstream)
                                    XDocMCat = XDocument.Load(xmlR)
                                End Using
                            End Using
                        End Using
                        AddHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                        XDocMCatChanged = False
                        XDocMCatFilename = Nothing
                        Me.Title = newAppTitle
                        LoadMDDefList()
                    Catch ex As Exception
                        DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Konnte Datei nicht öffnen:" + vbNewLine + ex.Message)
                    End Try
                End If
            End If
        End If
    End Sub

    Private Sub HandleSaveTheWorldExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim doi As String = DialogFactory.InputText(Me, "DOI-Check", "Bitte DOI eingeben", My.Settings.last_savetheworld, "")
        If Not String.IsNullOrEmpty(doi) Then
            My.Settings.last_savetheworld = doi
            My.Settings.Save()

            Dim myResponse As String = "?"
            Using client As New Net.WebClient()
                Try
                    Dim responseString As String = client.DownloadString("https://doi.org/api/handles/" + doi)
                    Dim responseObject As Linq.JObject = JsonConvert.DeserializeObject(responseString)
                    For Each o As Linq.JObject In From i In responseObject("values")
                        If o("type") = "URL" Then myResponse = o("data")("value")
                    Next
                Catch ex As Exception
                    myResponse = ex.Message
                End Try
            End Using

            DialogFactory.Msg(Me, "DOI-Check", "Antwort: " + vbNewLine + myResponse)
        End If
    End Sub

End Class
