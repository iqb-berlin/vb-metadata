Public Class BooleanStringConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If String.IsNullOrEmpty(value) OrElse CType(value, String) = Boolean.FalseString Then
            Return Boolean.FalseString
        Else
            Return Boolean.TrueString
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return value
    End Function
End Class

Public Class MDListValuesDictConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If value Is Nothing OrElse Not TypeOf value Is iqb.mdc.xml.MDObject Then
            Return New Dictionary(Of String, String)
        Else
            Return CType(value, iqb.mdc.xml.MDObject).GetListValues("de")
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Return value
    End Function
End Class

Public Class PanelCheckBoxConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As Boolean = False
        If TypeOf parameter Is CheckBox Then
            Dim myCheckbox As CheckBox = parameter
            Dim MDValueKey As String = myCheckbox.Name.Substring(myCheckbox.Name.IndexOf(CheckBoxesBasisControl.CheckboxNameSeparator) + CheckBoxesBasisControl.CheckboxNameSeparator.Length)
            Dim element As FrameworkElement = myCheckbox
            Do While element IsNot Nothing AndAlso Not TypeOf (element) Is CheckBoxesBasisControl
                element = VisualTreeHelper.GetParent(element)
            Loop
            If element IsNot Nothing AndAlso TypeOf (element) Is CheckBoxesBasisControl Then
                Dim myCBC As CheckBoxesBasisControl = element
                If Not String.IsNullOrEmpty(value) Then
                    Dim MDValues As List(Of String) = CType(value, String).Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
                    myreturn = MDValues.Contains(MDValueKey)
                End If
                myCBC.CheckBoxData.Item(MDValueKey) = myreturn
            End If
        End If

        Return myreturn
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If TypeOf parameter Is CheckBox Then
            Dim PropValueKey As CheckBox = parameter
            Dim element As FrameworkElement = PropValueKey
            Do While element IsNot Nothing AndAlso Not TypeOf (element) Is CheckBoxesBasisControl
                element = VisualTreeHelper.GetParent(element)
            Loop
            If element IsNot Nothing AndAlso TypeOf (element) Is CheckBoxesBasisControl Then
                Dim myCBC As CheckBoxesBasisControl = element
                Dim CheckBoxName As String = PropValueKey.Name.Substring(PropValueKey.Name.IndexOf(CheckBoxesBasisControl.CheckboxNameSeparator) + CheckBoxesBasisControl.CheckboxNameSeparator.Length)

                myCBC.CheckBoxData.Item(CheckBoxName) = CType(value, Boolean)
                myCBC.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
                Return String.Join(" ", From kv As KeyValuePair(Of String, Boolean) In myCBC.CheckBoxData Where kv.Value = True Select kv.Key)
            End If
        End If

        Return Nothing
    End Function
End Class

Public Class PanelRadioButtonConverter
    Implements IValueConverter

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        If TypeOf value Is String AndAlso Not String.IsNullOrEmpty(value) AndAlso TypeOf parameter Is RadioButton Then
            Dim myRadioButton As RadioButton = parameter
            Dim PropValue As String = value
            Dim RadiobuttonName As String = myRadioButton.Name.Substring(myRadioButton.Name.IndexOf(RadioButtonsBasisControl.RadiobuttonNameSeparator) + RadioButtonsBasisControl.RadiobuttonNameSeparator.Length)
            Return PropValue = RadiobuttonName
        End If

        Return False
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If TypeOf parameter Is RadioButton Then
            Dim myRB As RadioButton = parameter
            Dim myRadiobuttonName As String = myRB.Name.Substring(myRB.Name.IndexOf(RadioButtonsBasisControl.RadiobuttonNameSeparator) + RadioButtonsBasisControl.RadiobuttonNameSeparator.Length)
            If CType(value, Boolean) = True Then
                myRB.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
                Return myRadiobuttonName
            Else
                Dim element As FrameworkElement = myRB
                Do While element IsNot Nothing AndAlso Not TypeOf (element) Is Panel
                    element = VisualTreeHelper.GetParent(element)
                Loop
                If TypeOf (element) Is Panel Then
                    Dim myPanel As Panel = element
                    For Each RB As RadioButton In From fe As FrameworkElement In myPanel.Children Where TypeOf (fe) Is RadioButton
                        Dim RadiobuttonName As String = RB.Name.Substring(RB.Name.IndexOf(RadioButtonsBasisControl.RadiobuttonNameSeparator) + RadioButtonsBasisControl.RadiobuttonNameSeparator.Length)
                        If RadiobuttonName <> myRadiobuttonName AndAlso RB.IsChecked Then
                            myRB.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
                            Return RadiobuttonName
                        End If
                    Next
                End If
            End If
        End If
        Return Nothing
    End Function
End Class