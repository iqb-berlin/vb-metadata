Imports System.ComponentModel
Imports iqb.lib.components

Public Class LinkControl
    Public Shared ReadOnly LinkProperty As DependencyProperty = DependencyProperty.Register("Link", GetType(String), GetType(LinkControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property Link As String
        Get
            Return GetValue(LinkProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LinkProperty, value)
        End Set
    End Property

    Public Shared ReadOnly LinkTypeProperty As DependencyProperty = DependencyProperty.Register("LinkType", GetType(String), GetType(LinkControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property LinkType As String
        Get
            Return GetValue(LinkTypeProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LinkTypeProperty, value)
        End Set
    End Property

    Private Sub HyperlinkClick(sender As Object, e As RoutedEventArgs)
        Dim linkcontrol As Hyperlink = sender
        Dim NavUri As Uri = linkcontrol.NavigateUri
        Select Case LinkType.ToUpper
            Case "FILE"
                If NavUri Is Nothing OrElse String.IsNullOrEmpty(NavUri.OriginalString) Then
                    If DialogFactory.MainWindow Is Nothing Then
                        MsgBox("Link zu Datei ist nicht definiert.")
                    Else
                        DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Link zu Datei ist nicht definiert.")
                    End If
                Else
                    If IO.File.Exists(NavUri.LocalPath) Then
                        Try
                            Process.Start(NavUri.LocalPath)
                        Catch ex As Exception
                            If DialogFactory.MainWindow Is Nothing Then
                                MsgBox("Konnte Link nicht aufrufen: " + ex.Message)
                            Else
                                DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Konnte Link nicht aufrufen: " + ex.Message)
                            End If
                        End Try
                    Else
                        If DialogFactory.MainWindow Is Nothing Then
                            MsgBox("Konnte Datei nicht finden (" + NavUri.LocalPath + ").")
                        Else
                            DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Konnte Datei nicht finden (" + NavUri.LocalPath + ").")
                        End If
                    End If
                End If

            Case "FOLDER"
                If NavUri Is Nothing OrElse String.IsNullOrEmpty(NavUri.OriginalString) Then
                    If DialogFactory.MainWindow Is Nothing Then
                        MsgBox("Link zu Ordner ist nicht definiert.")
                    Else
                        DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Link zu Ordner ist nicht definiert.")
                    End If
                Else
                    If IO.Directory.Exists(NavUri.LocalPath) Then
                        Dim runExplorer As New System.Diagnostics.ProcessStartInfo() With {.FileName = "explorer.exe", .Arguments = """" + NavUri.LocalPath + """"}
                        Try
                            System.Diagnostics.Process.Start(runExplorer)
                        Catch ex As Exception
                            If DialogFactory.MainWindow Is Nothing Then
                                MsgBox("Konnte Link nicht aufrufen: " + ex.Message)
                            Else
                                DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Konnte Link nicht aufrufen: " + ex.Message)
                            End If
                        End Try
                    Else
                        If DialogFactory.MainWindow Is Nothing Then
                            MsgBox("Konnte Ordner nicht finden (" + NavUri.LocalPath + ").")
                        Else
                            DialogFactory.MsgError(DialogFactory.MainWindow, DialogFactory.MainWindow.Title, "Konnte Ordner nicht finden (" + NavUri.LocalPath + ").")
                        End If
                    End If
                End If
        End Select
    End Sub
End Class
