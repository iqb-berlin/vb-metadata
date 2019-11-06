Imports iqb.lib.components

Public Class ListDefControl
    Shared ReadOnly Property LanguageNamespace As XNamespace = "http://www.w3.org/XML/1998/namespace"

    Private Sub UpdateValueList(Optional ValueKeyToSelect As String = "")
        Dim be As BindingExpression = LBValues.GetBindingExpression(ListBox.ItemsSourceProperty)
        If be IsNot Nothing Then be.UpdateTarget()

        Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBValues.ItemsSource)
        If cv IsNot Nothing Then cv.Refresh()
        If Not String.IsNullOrEmpty(ValueKeyToSelect) Then
            Dim PropToSelect As XElement = (From xp As XElement In LBValues.Items Where xp.@id = ValueKeyToSelect Select xp).FirstOrDefault
            If PropToSelect IsNot Nothing Then
                LBValues.SelectedItem = PropToSelect
                LBValues.ScrollIntoView(PropToSelect)
            End If
        End If
    End Sub

    Private Sub BtnAddValue_Click(sender As Object, e As RoutedEventArgs)
        Dim XMDDef As XElement = StPRoot.DataContext
        If XMDDef IsNot Nothing Then
            Dim NewLabel As String = DialogFactory.InputText(Me, "Neuer Merkmalswert", "Bitte Namen eingeben", "", "")
            If Not String.IsNullOrEmpty(NewLabel) Then
                Dim NewId As Integer = 1
                If LBValues.Items.Count > 0 Then
                    NewId = (From xp As XElement In LBValues.Items Select Integer.Parse(xp.@id)).Max
                    NewId += 1
                End If
                Dim XNewValue As XElement = <Value id=<%= NewId.ToString %>>
                                                <Label xml:lang="de"><%= NewLabel %></Label>
                                            </Value>
                XMDDef.Add(XNewValue)
                UpdateValueList(NewId.ToString)
            End If
        End If
    End Sub

    Private Sub BtnDeleteValue_Click(sender As Object, e As RoutedEventArgs)
        If LBValues.SelectedItems.Count = 0 Then
            DialogFactory.MsgError(Me, "Listenmerkmalswert löschen", "Bitte Listenmerkmalswert auswählen!")
        Else
            Dim XMDDefValue As XElement = LBValues.SelectedItem
            Dim Label As String = "Merkmal #" + XMDDefValue.@id
            Dim XLabel As XElement = (From xe As XElement In XMDDefValue.Elements("Label") Where xe.Attribute(ListDefControl.LanguageNamespace + "lang").Value = "de").FirstOrDefault
            If XLabel Is Nothing Then XLabel = (From xe As XElement In XMDDefValue.Elements("Label")).FirstOrDefault
            If XLabel IsNot Nothing Then Label = XLabel.Value
            If DialogFactory.YesNoCancel(Me, "Löschen Merkmalswert", "Soll der Merkmalswert """ + Label + """ gelöscht werden?") Then
                XMDDefValue.Remove()
                UpdateValueList()
            End If
        End If
    End Sub


    Private Sub BtnMoveUpValue_Click(sender As Object, e As RoutedEventArgs)
        Dim XMDDef As XElement = StPRoot.DataContext
        If XMDDef IsNot Nothing Then
            If LBValues.SelectedItems.Count = 0 Then
                DialogFactory.MsgError(Me, "Listenmerkmal verschieben", "Bitte Listenmerkmal auswählen!")
            Else
                Dim XValueToMove As XElement = LBValues.SelectedItem
                Dim XPrev As XElement = Nothing
                For Each xe As XElement In XMDDef.<Value>
                    If xe.@id = XValueToMove.@id Then
                        Exit For
                    Else
                        XPrev = xe
                    End If
                Next
                If XPrev IsNot Nothing Then
                    XValueToMove.Remove()
                    XPrev.AddBeforeSelf(XValueToMove)
                    UpdateValueList(XValueToMove.@id)
                End If
            End If
        End If
    End Sub

    Private Sub BtnMoveDownValue_Click(sender As Object, e As RoutedEventArgs)
        Dim XMDDef As XElement = StPRoot.DataContext
        If XMDDef IsNot Nothing Then
            If LBValues.SelectedItems.Count = 0 Then
                DialogFactory.MsgError(Me, "Listenmerkmal verschieben", "Bitte Listenmerkmal auswählen!")
            Else
                Dim XValueToMove As XElement = LBValues.SelectedItem
                Dim XAfter As XElement = Nothing
                For Each xe As XElement In XMDDef.Elements
                    If xe.Name.LocalName = "Value" Then
                        If XAfter IsNot Nothing Then
                            XAfter = xe
                            Exit For
                        ElseIf xe.@id = XValueToMove.@id Then
                            XAfter = xe
                        End If
                    End If
                Next
                If XAfter IsNot Nothing AndAlso XAfter.@id <> XValueToMove.@id Then
                    XValueToMove.Remove()
                    XAfter.AddAfterSelf(XValueToMove)
                    UpdateValueList(XValueToMove.@id)
                End If
            End If
        End If
    End Sub

End Class
