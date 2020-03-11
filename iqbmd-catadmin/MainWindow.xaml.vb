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

    Private XDocMCatChanged As Boolean = False
    Private XDocMCatFilename As String = Nothing
    Private MDCoreCats As New List(Of String) From {"DOI:10.5159/IQB_MDR_Core_v1"}
    Private nextMDDefid As Integer = 0
    Private MDDefFilter As iqb.md.xml.MDFilter = Nothing

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

        CommandBindings.Add(New CommandBinding(AppCommands.NewCatalog, AddressOf HandleNewCatalogExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Open, AddressOf HandleOpenExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Save, AddressOf HandleSaveExecuted, AddressOf HandleSaveCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.EditCatData, AddressOf HandleEditCatMetadataExecuted, AddressOf HandleCatLoadedCanExecute))
        CommandBindings.Add(New CommandBinding(AppCommands.SaveAs, AddressOf HandleSaveAsExecuted, AddressOf HandleCatLoadedCanExecute))
        CommandBindings.Add(New CommandBinding(IQBCommands.ReloadObject, AddressOf HandleReloadExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.OpenWebCat, AddressOf HandleOpenWebCatExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.EditMDCoreCats, AddressOf HandleEditMDCoreCatsExecuted))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.[New], AddressOf HandleAddMDDefExecuted, AddressOf HandleCatLoadedCanExecute))
        CommandBindings.Add(New CommandBinding(ApplicationCommands.Delete, AddressOf HandleDeleteMDDefExecuted, AddressOf HandleDeleteMDDefCanExecute))
        CommandBindings.Add(New CommandBinding(IQBCommands.Filter, AddressOf HandleFilterExecuted))
        CommandBindings.Add(New CommandBinding(IQBCommands.FilterRemove, AddressOf HandleFilterRemoveExecuted, AddressOf HandleFilterRemoveCanExecuted))
        CommandBindings.Add(New CommandBinding(AppCommands.NewCatalogFromOld, AddressOf HandleNewCatalogFromOldExecuted))

        'CommandBindings.Add(New CommandBinding(AppCommands.TestMDControls, AddressOf HandleTestMDControlsExecuted))
        If iqb.lib.windows.ADFactory.GetMyName() = "mechtelm" Then CommandBindings.Add(New CommandBinding(AppCommands.SaveTheWorld, AddressOf HandleSaveTheWorldExecuted))

        'AppCommands.TestMDControls.Execute(Nothing, Nothing)
        Me.MDCC.MDCatList = MDCoreCats

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
        Dim XMDef As XElement = item
        If String.IsNullOrEmpty(TBSearchString.Text) Then
            Return MDDefFilter Is Nothing OrElse MDDefFilter.IsMatch(XMDef.<MDDefMetadata>.FirstOrDefault)
        Else
            Dim sstr As String = TBSearchString.Text.ToUpper
            Return XMDef.<Label>.First.Value.ToUpper.IndexOf(sstr) >= 0 AndAlso (MDDefFilter Is Nothing OrElse MDDefFilter.IsMatch(XMDef.<MDDefMetadata>.FirstOrDefault))
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

    Private Sub HandleFilterExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBMetadatadefs.ItemsSource)
        If XDocMCat IsNot Nothing AndAlso cv IsNot Nothing Then
            Dim myMDList As New Dictionary(Of String, List(Of String))
            For Each XMD As XElement In XDocMCat.Root.<MDDef>
                Dim myMDDefMDList As XElement = XMD.<MDDefMetadata>.FirstOrDefault
                If myMDDefMDList IsNot Nothing Then
                    For Each xe As XElement In myMDDefMDList.Elements
                        If iqb.md.xml.MDCFactory.IsListDef(xe.@cat, xe.@def) Then
                            If Not myMDList.ContainsKey(xe.@cat) Then myMDList.Add(xe.@cat, New List(Of String))
                            Dim defs As List(Of String) = myMDList.Item(xe.@cat)
                            If Not defs.Contains(xe.@def) Then defs.Add(xe.@def)
                        End If
                    Next
                End If
            Next
            If myMDList.Count = 0 Then
                DialogFactory.MsgWarning(Me, "Eigenschaften filtern", "Die Definitionen des aktuellen Katalogs haben keine Metadaten. Ein Filter kann daher nicht gesetzt werden.")
            Else
                Dim XDefaults As XElement = <Defaults></Defaults>
                XDefaults.Add(From cat As KeyValuePair(Of String, List(Of String)) In myMDList
                              From def As String In cat.Value
                              Let XDef As XElement = <MD cat=<%= cat.Key %> def=<%= def %>/>
                              Select XDef)
                Dim XFilter As XElement = <FList></FList>
                If MDDefFilter IsNot Nothing Then XFilter = MDDefFilter.ToXML
                Dim myDlg As New iqb.md.components.EditMDListDialog(XFilter, XDefaults,
                                                                    "Bitte zulässige Eigenschaftswerte auswählen! Die Eigenschaften werden beim Filtern mit 'UND', die Werte werden jeweils mit 'ODER' verknüpft") With
                                                                    {.Owner = Me, .Title = "Filter setzen"}

                If myDlg.ShowDialog() Then
                    MDDefFilter = New xml.MDFilter(XFilter)
                    TBFilterScope.Text = "Filter gesetzt"
                    cv.Refresh()
                End If
            End If
        End If
    End Sub

    Private Sub HandleFilterRemoveExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Me.MDDefFilter = Nothing
        TBFilterScope.Text = ""

        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBMetadatadefs.ItemsSource)
        If cv IsNot Nothing Then cv.Refresh()
    End Sub

    Private Function HandleFilterRemoveCanExecuted(ByVal sender As Object, ByVal e As System.Windows.Input.CanExecuteRoutedEventArgs) As Boolean
        e.CanExecute = MDDefFilter IsNot Nothing AndAlso MDDefFilter.Count > 0
        Return e.CanExecute
    End Function
    '############################################################################################
    Private Sub Me_Closing(ByVal sender As Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles Me.Closing
        If XDocMCatChanged AndAlso
            DialogFactory.YesNo(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?") = vbOK Then
            ApplicationCommands.Save.Execute(Nothing, Nothing)
        End If
    End Sub
    Private Sub HandleReloadExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        iqb.md.xml.MDCFactory.ResetCatalogList()
    End Sub

    Private Sub HandleNewCatalogExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim mbresult As MessageBoxResult = MessageBoxResult.OK
        If XDocMCatChanged Then
            mbresult = DialogFactory.YesNoCancel(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?")
            If mbresult = MessageBoxResult.OK Then
                ApplicationCommands.Save.Execute(Nothing, Nothing)
            End If
        End If
        If mbresult <> MessageBoxResult.Cancel Then
            If XDocMCat IsNot Nothing Then
                RemoveHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
            End If
            XDocMCat = <?xml version="1.0" encoding="utf-8"?>
                       <MDCat xsi:noNamespaceSchemaLocation="http://raw.githubusercontent.com/iqb-mdc/core/master/mdc.xsd"
                           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                           id=""
                           version="" versionhistory="">
                           <Label xml:lang="de">Neuer Katalog</Label>
                           <Description xml:lang="de">Neuer Katalog - Beschreibung</Description>
                           <Owner xml:lang="de">IQB - Institut zur Qualitätsentwicklung im Bildungswesen</Owner>
                           <Owner xml:lang="en">IQB - Institute for Educational Quality Improvement</Owner>
                           <License>MIT</License>
                           <DefaultMDDefMetadata/>
                       </MDCat>
            AddHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
            XDocMCatChanged = False
            XDocMCatFilename = Nothing
            Me.Title = My.Application.Info.AssemblyName + " - [neu]"
            UpdateMDDefControls()
        End If
    End Sub

    Private Sub HandleOpenExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim fname As String = ""
        Dim fdir As String = ""
        If Not String.IsNullOrEmpty(My.Settings.lastfile_mdcat) Then
            fname = IO.Path.GetFileName(My.Settings.lastfile_mdcat)
            fdir = IO.Path.GetDirectoryName(My.Settings.lastfile_mdcat)
        End If
        Dim filepicker As New Microsoft.Win32.OpenFileDialog With {.FileName = fname, .Filter = "XML-Dateien|*.xml", .InitialDirectory = fdir,
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
                UpdateMDDefControls()
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
                Dim XDocToSave As New XDocument(XDocMCat)
                'remove empty TypeSpecs
                For Each XMD As XElement In XDocToSave.Root.<MDDef>
                    Dim XTypeSpec As XElement = XMD.<TypeSpec>.FirstOrDefault
                    If XTypeSpec IsNot Nothing Then
                        Dim IsEmpty As Boolean = True
                        Dim XeToRemove As New List(Of XElement)
                        For Each XSpec As XElement In XTypeSpec.Elements
                            If String.IsNullOrEmpty(XSpec.Value) Then
                                XeToRemove.Add(XSpec)
                            Else
                                IsEmpty = False
                            End If
                        Next
                        If IsEmpty Then
                            XTypeSpec.Remove()
                        Else
                            For Each xe As XElement In XeToRemove
                                xe.Remove()
                            Next
                        End If
                    End If
                Next
                XDocToSave.Save(XDocMCatFilename)
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

    Private Sub UpdateMDDefControls()
        Me.MDDefFilter = Nothing
        TBFilterScope.Text = ""

        If XDocMCat Is Nothing Then
            Me.MDCC.XDefaultMDList = Nothing
        Else
            Dim XDefaultMDListElement As XElement = XDocMCat.Root.Element("DefaultMDDefMetadata")
            If XDefaultMDListElement Is Nothing Then
                XDocMCat.Root.Add(<DefaultMDDefMetadata/>)
                XDefaultMDListElement = XDocMCat.Root.Element("DefaultMDDefMetadata")
            End If
            Me.MDCC.XDefaultMDList = XDefaultMDListElement

            Dim tmpint As Integer = 0
            nextMDDefid = 0
            For Each XMDDef As XElement In XDocMCat.Root.<MDDef>
                tmpint = Integer.Parse(XMDDef.@id)
                If tmpint > nextMDDefid Then nextMDDefid = tmpint

                Dim XMDDefMDElement As XElement = XMDDef.Element("MDDefMetadata")
                If XMDDefMDElement Is Nothing Then XMDDef.Add(<MDDefMetadata/>)
            Next
            nextMDDefid += 1
        End If
        UpdateMDDefList()
    End Sub

    Private Sub UpdateMDDefList(Optional NewMDDefId As String = Nothing)
        If XDocMCat Is Nothing Then
            LBMetadatadefs.ItemsSource = Nothing
        Else
            LBMetadatadefs.ItemsSource = XDocMCat.Root.Elements("MDDef")
            Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBMetadatadefs.ItemsSource)
            If cv IsNot Nothing Then
                cv.SortDescriptions.Add(New ComponentModel.SortDescription("Element[Label].Value", ComponentModel.ListSortDirection.Ascending))
                cv.Filter = AddressOf FilterList
                cv.MoveCurrentToFirst()
            End If

            If Not String.IsNullOrEmpty(NewMDDefId) Then LBMetadatadefs.SelectedValue = NewMDDefId
        End If
    End Sub

    Private Sub HandleEditCatMetadataExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim myDlg As New EditCatCoreDataDialog(Me.XDocMCat) With {.Owner = Me, .Title = AppCommands.EditCatData.Text}
        myDlg.ShowDialog()
    End Sub

    Private Sub HandleEditMDCoreCatsExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim myCoreCatsStr As String = String.Join(vbCrLf, MDCoreCats)
        Dim myCoreCatsStrNew As String =
            DialogFactory.InputTextMultiLine(Me,
                                             AppCommands.EditMDCoreCats.Text,
                                             "Pro Katalog bitte eine Zeile verwenden",
                                             myCoreCatsStr,
                                             "Diese Kataloge werden genutzt, um Metadaten für die Definitionen des aktuellen Katalogs auszuwählen." + vbNewLine +
                                             "Achtung: Die Änderungen sind bis zum Beenden der Anwendung wirksam. Nach dem Neustart wird der Standard wiederhergestellt.")
        If Not String.IsNullOrEmpty(myCoreCatsStrNew) Then MDCoreCats = myCoreCatsStrNew.Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries).ToList
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
                        UpdateMDDefControls()
                    Catch ex As Exception
                        DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Konnte Datei nicht öffnen:" + vbNewLine + ex.Message)
                    End Try
                End If
            End If
        End If
    End Sub

    '#######################################################################
    Private Sub HandleAddMDDefExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If Me.XDocMCat IsNot Nothing Then
            Dim NewMDDefLabel As String = DialogFactory.InputText(Me, "Neue MD-Definition", "Bitte Name eingeben", "", "")
            If Not String.IsNullOrEmpty(NewMDDefLabel) Then
                Dim NewMDDefId As String = nextMDDefid.ToString
                nextMDDefid += 1

                Dim XNewMDDef As XElement = <MDDef id=<%= NewMDDefId %> type="">
                                                <Label xml:lang="de"><%= NewMDDefLabel %></Label>
                                                <MDDefMetadata/>
                                                <TypeSpec/>
                                                <DefaultValue/>
                                            </MDDef>
                Me.XDocMCat.Root.Add(XNewMDDef)
                XDocMCatChanged = True
                UpdateMDDefList(NewMDDefId)
            End If
        End If
    End Sub

    Private Function HandleDeleteMDDefCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = Me.XDocMCat IsNot Nothing AndAlso LBMetadatadefs.SelectedItems.Count > 0
        Return e.CanExecute
    End Function

    Private Sub HandleDeleteMDDefExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        If Me.XDocMCat IsNot Nothing AndAlso LBMetadatadefs.SelectedItems.Count > 0 Then
            Dim XDef2Remove As XElement = LBMetadatadefs.SelectedItem
            If DialogFactory.YesNoCancel(Me, "Definition löschen", "Soll Defintion '" + XDef2Remove.<Label>.First.Value + "' mit ID '" + XDef2Remove.@id + "' gelöscht werden?") = MessageBoxResult.OK Then
                XDef2Remove.Remove()
                XDocMCatChanged = True
                UpdateMDDefList()
            End If
        End If
    End Sub

    '#######################################################################
    Private Sub HandleTestMDControlsExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim myDlg As New TestMDControlsDialog With {.Owner = Me}
        myDlg.ShowDialog()
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

    Private Sub HandleNewCatalogFromOldExecuted(ByVal sender As Object, ByVal e As ExecutedRoutedEventArgs)
        Dim mbresult As MessageBoxResult = MessageBoxResult.OK
        If XDocMCatChanged Then
            mbresult = DialogFactory.YesNoCancel(Me, My.Application.Info.AssemblyName, "Der Katalog wurde geändert. Soll er vor dem Schließen gespeichert werden?")
            If mbresult = MessageBoxResult.OK Then
                ApplicationCommands.Save.Execute(Nothing, Nothing)
            End If
        End If
        If mbresult <> MessageBoxResult.Cancel Then
            Dim fname As String = ""
            Dim fdir As String = ""
            If Not String.IsNullOrEmpty(My.Settings.last_oldcatfilename) Then
                fname = IO.Path.GetFileName(My.Settings.last_oldcatfilename)
                fdir = IO.Path.GetDirectoryName(My.Settings.last_oldcatfilename)
            End If


            Dim filepicker As New Microsoft.Win32.OpenFileDialog With {.FileName = fname, .Filter = "XML-ZIP-Dateien|*.zip", .InitialDirectory = fdir,
                                                                           .DefaultExt = "Xml", .Title = "Alten MDR-Katalog öffnen"}
            If filepicker.ShowDialog Then
                My.Settings.last_oldcatfilename = filepicker.FileName
                My.Settings.Save()

                Try
                    XDocMCat = MDR2MDC.TransformOld(filepicker.FileName)

                    If XDocMCat IsNot Nothing Then
                        RemoveHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                    End If

                    AddHandler XDocMCat.Root.Changed, AddressOf Notify_XDocMCatChanged
                    XDocMCatChanged = False
                    XDocMCatFilename = Nothing
                    Me.Title = My.Application.Info.AssemblyName + " - [neu von alt]"
                    UpdateMDDefControls()
                Catch ex As Exception
                    DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Konnte Datei nicht öffnen:" + vbNewLine + ex.Message)
                End Try
            End If
        End If
    End Sub

End Class
