Public Class EditCatCoreDataDialog
    Public XCat As XDocument = Nothing

    Private Sub Me_Loaded() Handles Me.Loaded
        If XCat Is Nothing Then
            Me.StPRoot.IsEnabled = False
        Else
            If String.IsNullOrEmpty(XCat.Root.@versionhistory) Then XCat.Root.@versionhistory = ""
            Me.StPRoot.DataContext = XCat.Root
        End If
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        Me.DialogResult = True
    End Sub

End Class
