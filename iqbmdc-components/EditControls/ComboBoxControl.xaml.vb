Public Class ComboBoxControl
    Inherits MDBasisControl

    Private Sub ComboBox_PreviewMouseWheel(sender As Object, e As MouseWheelEventArgs)
        'kein Mausrad unterstützen, um versehentliches Verstellen zu verhindern
        e.Handled = True
    End Sub
End Class
