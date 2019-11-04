'#####################################################
Public Class TimeValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing Then
            myreturn = ValidationResult.ValidResult
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If String.IsNullOrEmpty(myInputString) Then
                myreturn = ValidationResult.ValidResult
            Else
                If Text.RegularExpressions.Regex.IsMatch(myInputString.Trim, TimeStringIntegerConverter.MatchPattern) Then
                    myreturn = ValidationResult.ValidResult
                Else
                    myreturn = New ValidationResult(False, "Bitte eine Zeitangabe in der Form 'mm:ss' eingeben!")
                End If
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class

'#####################################################
Public Class DoubleValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse Not TypeOf (value) Is String OrElse String.IsNullOrEmpty(value) Then
            myreturn = ValidationResult.ValidResult
        Else
            Dim myNumeric As Double
            Try
                Dim strvalue As String = CType(value, String).Replace(",", ".")
                myNumeric = Double.Parse(strvalue, cultureInfo.NumberFormat)
                myreturn = ValidationResult.ValidResult
            Catch ex As Exception
                myreturn = New ValidationResult(False, "Bitte eine Zahl eingeben")
            End Try
        End If
        Return myreturn
    End Function
End Class

'#####################################################
Public Class IntegerValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse Not TypeOf (value) Is String OrElse String.IsNullOrEmpty(value) Then
            myreturn = ValidationResult.ValidResult
        Else
            If Text.RegularExpressions.Regex.IsMatch(CType(value, String).Trim, "^-?[0-9]+$") Then
                myreturn = ValidationResult.ValidResult
            Else
                myreturn = New ValidationResult(False, "Bitte eine ganze Zahl eingeben")
            End If
        End If
        Return myreturn
    End Function
End Class
