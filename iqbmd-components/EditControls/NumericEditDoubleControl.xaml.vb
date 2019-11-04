Public Class NumericEditDoubleControl
    Inherits MDBasisControl

    Private Sub Me_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Dim myMDObject As iqb.md.xml.MDObject = Me.DataContext
        AddHandler myMDObject.XMD.Changed, AddressOf NotifyXMDChanged
    End Sub
    Private Sub NotifyXMDChanged(sender As Object, e As System.Xml.Linq.XObjectChangeEventArgs)
        Dim ChangeToken As String = e.ObjectChange.ToString
        If ChangeToken <> "Remove" AndAlso ChangeToken <> "Value" Then Me.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
    End Sub

    Private Sub Me_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Unloaded
        If TypeOf Me.DataContext Is iqb.md.xml.MDObject Then
            Dim myMDObject As iqb.md.xml.MDObject = Me.DataContext
            RemoveHandler myMDObject.XMD.Changed, AddressOf NotifyXMDChanged
        End If
    End Sub
End Class
