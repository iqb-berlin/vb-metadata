Imports iqb.md.xml

Public Class MDValueToStringConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As String = "??"

        If value IsNot Nothing OrElse value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is MDObject Then
            Dim MD As MDObject = value
            myreturn = MD.GetValueAsText(", ", False, "de", False)
        End If

        Return myreturn
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("MDValueToStringConverter ConvertBack")
    End Function
End Class

Public Class MDValueToBooleanPathConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As String = "M 2,6 C 2,6 9,6 9,6"
        If value IsNot Nothing OrElse value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is MDObject Then
            Dim MD As MDObject = value
            If MD.GetValueAsBoolean() Then myreturn = "M 2,4 C 2,4 3,5 5,13 C 5,13 5,3 12,0"
        End If

        Return myreturn
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("MDValueToBooleanPathConverter ConvertBack")
    End Function
End Class

Public Class LinkLabelConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myreturn As String = ""
        If Not String.IsNullOrEmpty(value) AndAlso value IsNot DependencyProperty.UnsetValue Then
            Dim Link As String = value
            If TypeOf (parameter) Is String AndAlso CType(parameter, String).ToUpper = "RIGHT" Then
                myreturn = IO.Path.GetFileName(Link)
            Else 'LEFT
                Link = IO.Path.GetDirectoryName(Link)
                If Link.EndsWith("\") Then
                    myreturn = Link.Substring(0, Link.Length - 1)
                Else
                    myreturn = Link
                End If
            End If
        End If
        Return myreturn
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        Throw New NotImplementedException("LinkLabelConverter ConvertBack")
    End Function
End Class
