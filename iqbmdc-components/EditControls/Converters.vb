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

