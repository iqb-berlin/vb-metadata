Public Class CheckBoxesFloatExpanderControl
    Inherits CheckBoxesBasisControl

    Public Overrides Function GetCheckBoxesPanel() As Panel
        Return WPCheckBoxes
    End Function

    Private Sub TgBtnEdit_Unchecked(sender As Object, e As RoutedEventArgs)
        'sichert, dass die Textbox aktualisiert wird, wenn wieder eingeklappt wird
        Dim be As BindingExpression = TBRead.GetBindingExpression(TextBlock.TextProperty)
        If be IsNot Nothing Then be.UpdateTarget()
    End Sub
End Class
