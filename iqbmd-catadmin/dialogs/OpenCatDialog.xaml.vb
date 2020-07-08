Imports System.Collections.Specialized
Imports System.Xml
Imports iqb.lib.components

Public Class OpenCatDialog
    Private _XCat As XDocument = Nothing
    Private Const configFileName = "IQB-MD-CatAdmin-Config.xml"
    Public selectedUri As Uri = Nothing

    Private Sub Me_Loaded() Handles Me.Loaded
        Dim cfgFileUri As String = Nothing
        If System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed Then
            Dim cfnsource As String = Nothing
            Try
                cfnsource = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.AbsoluteUri
            Catch ex As Exception
                cfnsource = Nothing
            End Try
            If cfnsource IsNot Nothing Then
                If cfnsource.IndexOf("file://") = 0 Then
                    Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
                    cfgFileUri = IO.Path.GetDirectoryName(System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdateLocation.LocalPath) +
                           IO.Path.DirectorySeparatorChar + configFileName
                    If Not IO.File.Exists(cfgFileUri) Then cfgFileUri = Nothing
                End If
            End If
        End If
        If String.IsNullOrEmpty(cfgFileUri) Then
            Dim myMainAssembly As Reflection.Assembly = System.Reflection.Assembly.GetEntryAssembly
            cfgFileUri = IO.Path.GetDirectoryName(myMainAssembly.Location) + IO.Path.DirectorySeparatorChar + configFileName
            If Not IO.File.Exists(cfgFileUri) Then cfgFileUri = Nothing
        End If
        If Not String.IsNullOrEmpty(cfgFileUri) Then
            Dim xCfgDef As XDocument = XDocument.Load(cfgFileUri)
            ICSelection.ItemsSource = xCfgDef.Root.Elements
        End If
    End Sub

    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnCancel.Click
        Me.DialogResult = False
    End Sub

    Private Sub BtnOpen_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOpen.Click
        Dim myType As String = Nothing
        If RBdoi.IsChecked Then
            myType = "doi"
        ElseIf RBfs2006.IsChecked Then
            myType = "fs2006"
        ElseIf RBfs2016.IsChecked Then
            myType = "fs2016"
        ElseIf RBfs2020.IsChecked Then
            myType = "fs2020"
        ElseIf RBhttp.IsChecked Then
            myType = "http"
        ElseIf RBfs2006.IsChecked Then
            myType = "fs2006"
        End If
        If String.IsNullOrEmpty(myType) Then
            DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Bitte Typ angeben!")
        Else
            If String.IsNullOrEmpty(TBUri.Text) Then
                DialogFactory.MsgError(Me, "MD-Katalog öffnen", "Bitte Uri (Adresse/Pfad) angeben!")
            Else
                selectedUri = New Uri With {.label = TBUri.Text, .ref = TBUri.Text, .type = myType}
                Me.DialogResult = True
            End If
        End If
    End Sub

    Private Sub BtnPickFile_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnPickFile.Click
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
            TBUri.Text = filepicker.FileName
        End If
    End Sub
    Private Sub BtnSelection_Click(sender As Object, e As RoutedEventArgs)
        Dim fe As FrameworkElement = sender
        Dim selectedXElement As XElement = fe.GetValue(DataContextProperty)
        selectedUri = New Uri With {.label = selectedXElement.Value, .ref = selectedXElement.@ref, .type = selectedXElement.@type}
        Me.DialogResult = True
    End Sub
End Class
