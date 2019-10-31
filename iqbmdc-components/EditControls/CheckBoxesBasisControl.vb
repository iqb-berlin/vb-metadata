Imports iqb.mdc.xml

Public MustInherit Class CheckBoxesBasisControl
    Inherits MDBasisControl

    Public Shared CheckboxNameSeparator = "___"
    Public MustOverride Function GetCheckBoxesPanel() As Panel

    Friend CheckBoxData As Dictionary(Of String, Boolean) = Nothing


    Private Sub Me_Loaded() Handles Me.Loaded
        Dim ParentPanel As Panel = Me.GetCheckBoxesPanel()
        ParentPanel.Children.Clear()
        Dim DC = Me.DataContext
        If DC IsNot Nothing AndAlso TypeOf DC Is MDObject Then
            Dim myMDObject As MDObject = DC

            Dim myGUID As String = "ID" + DateTime.Now.Millisecond.ToString 'um eindeutige ID zu haben
            CheckBoxData = New Dictionary(Of String, Boolean)

            For Each MDValue As KeyValuePair(Of String, String) In myMDObject.GetListValues("de")
                CheckBoxData.Add(MDValue.Key, False)
                Dim newCheckbox As New CheckBox With {.Content = New TextBlock() With {.Text = MDValue.Value}, .Name = myGUID + CheckboxNameSeparator + MDValue.Key}

                'erst ChildrenAdd, dann SetBinding, weil sonst beim ersten Anspringen des Converters ParentPanel nicht verfügbar
                ParentPanel.Children.Add(newCheckbox)
                newCheckbox.SetBinding(CheckBox.IsCheckedProperty, New Binding With {.Source = myMDObject,
                                       .Path = New PropertyPath("XMD.Value"), .ConverterParameter = newCheckbox, .Converter = New PanelCheckBoxConverter})
            Next
        End If
    End Sub

End Class
