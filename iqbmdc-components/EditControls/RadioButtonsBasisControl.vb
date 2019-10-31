Imports iqb.mdc.xml

Public MustInherit Class RadioButtonsBasisControl
    Inherits MDBasisControl

    Public MustOverride Function GetRadioButtonsPanel() As Panel

    Public Shared RadiobuttonNameSeparator = "___"

    Private Sub Me_Loaded() Handles Me.Loaded
        Dim ParentPanel As Panel = Me.GetRadioButtonsPanel()
        ParentPanel.Children.Clear()
        Dim DC = Me.DataContext
        If DC IsNot Nothing AndAlso TypeOf DC Is MDObject Then
            Dim myMDObject As MDObject = DC
            Dim habschonnamen As New List(Of String)
            Dim myGUID As String = "ID" + DateTime.Now.Millisecond.ToString 'um eindeutige ID zu haben
            Dim GroupId As String = Guid.NewGuid.ToString
            For Each MDValue As KeyValuePair(Of String, String) In myMDObject.GetListValues("de")
                If Not habschonnamen.Contains(MDValue.Key) Then
                    habschonnamen.Add(MDValue.Key)
                    Dim newRadioButton As New RadioButton With {.Content = New TextBlock() With {.Text = MDValue.Value}, .Name = myGUID + RadiobuttonNameSeparator + MDValue.Key, .GroupName = GroupId}
                    newRadioButton.SetBinding(RadioButton.IsCheckedProperty, New Binding With {.Source = myMDObject,
                                           .Path = New PropertyPath("XMD.Value"), .ConverterParameter = newRadioButton, .Converter = New PanelRadioButtonConverter})
                    ParentPanel.Children.Add(newRadioButton)
                End If
            Next
        End If
    End Sub

End Class
