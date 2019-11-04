Public Class IntegerValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse String.IsNullOrEmpty(value) Then
            myreturn = ValidationResult.ValidResult
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If Text.RegularExpressions.Regex.IsMatch(myInputString, "^-?[0-9]+$") Then
                myreturn = ValidationResult.ValidResult
            Else
                myreturn = New ValidationResult(False, "Darf nur eine ganze Zahl enthalten!")
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class

Public Class DecimalValidationRule
    Inherits ValidationRule

    Public Overrides Function Validate(ByVal value As Object, ByVal cultureInfo As System.Globalization.CultureInfo) As System.Windows.Controls.ValidationResult
        Dim myreturn As System.Windows.Controls.ValidationResult
        If value Is Nothing OrElse String.IsNullOrEmpty(value) Then
            myreturn = ValidationResult.ValidResult
        ElseIf TypeOf (value) Is String Then
            Dim myInputString As String = value
            If Text.RegularExpressions.Regex.IsMatch(myInputString, "^-?[0-9]+(,[0-9]+)?$") Then
                myreturn = ValidationResult.ValidResult
            Else
                myreturn = New ValidationResult(False, "Darf nur eine Zahl enthalten!")
            End If
        Else
            myreturn = New ValidationResult(False, "Ungültige Eingabe!")
        End If
        Return myreturn
    End Function
End Class
