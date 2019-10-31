Public Class TestMDControlsDialog
    'Public XCat As XDocument = Nothing

    Private Sub Me_Loaded() Handles Me.Loaded
        Me.AddHandler(iqb.mdc.components.LinkEditControl.DefaultFolderChangedEvent, New RoutedEventHandler(AddressOf HandleDefaultFolderChanged))
        TBInfo.Text = iqb.mdc.components.LinkEditControl.DefaultFolder
    End Sub

    Private Sub HandleDefaultFolderChanged()
        TBInfo.Text = iqb.mdc.components.LinkEditControl.DefaultFolder
    End Sub
    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        Me.DialogResult = True
    End Sub

    Private Sub BtnGo_Clicked(sender As Object, e As RoutedEventArgs)
        Me.DPX.DataContext = XElement.Parse(Me.TBXML.Text)
    End Sub

    Private Sub BtnDebugChange_Clicked(sender As Object, e As RoutedEventArgs)
        Dim XMD As XElement = Me.DPX.DataContext
        For Each xe As XElement In XMD.Elements
            If xe.Value = "True" Then
                xe.Value = "False"
            Else
                xe.Value = "True"
            End If
        Next
    End Sub
    Private Sub BtnDebugView_Clicked(sender As Object, e As RoutedEventArgs)
        Dim XMD As XElement = Me.DPX.DataContext
        Me.TBDebug.Text = XMD.ToString
    End Sub
End Class
