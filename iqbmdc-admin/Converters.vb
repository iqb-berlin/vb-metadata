Public Class XTextLanguageMergeConverter
    Implements IMultiValueConverter
    Shared ReadOnly Property LanguageNamespace As XNamespace = "http://www.w3.org/XML/1998/namespace"

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim XTextList As New List(Of XElement)
        If values.Count > 0 AndAlso TypeOf values(0) Is XElement Then
            Dim XMD As XElement = values(0)
            Dim LanguagesList As New List(Of String) From {"de"}
            If values(1) IsNot Nothing AndAlso values(1) IsNot DependencyProperty.UnsetValue Then
                LanguagesList = CType(values(1), String).Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            End If

            Dim XValueList As List(Of XElement)
            Dim TagFilter As String
            If values(2) IsNot Nothing AndAlso values(2) IsNot DependencyProperty.UnsetValue Then
                TagFilter = values(2)
                XValueList = (From xe As XElement In XMD.Elements(TagFilter)).ToList
            Else
                XValueList = (From xe As XElement In XMD.Elements).ToList
                If XValueList.Count > 0 Then
                    TagFilter = XValueList.First.Name.LocalName
                Else
                    TagFilter = "MDText"
                End If
            End If

            For Each lang As String In LanguagesList
                Dim XValue As XElement = (From xe As XElement In XValueList Where xe.Attribute(XTextLanguageMergeConverter.LanguageNamespace + "lang").Value = lang).FirstOrDefault
                If XValue Is Nothing Then
                    XValue = New XElement(TagFilter)
                    XValue.SetAttributeValue(XTextLanguageMergeConverter.LanguageNamespace + "lang", lang)
                    XMD.Add(XValue)
                End If
                XTextList.Add(XValue)
            Next
        End If
        Return XTextList
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("XTextLanguageMergeConverter ConvertBack")
    End Function
End Class

Public Class BooleanTextWrapConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is Boolean Then
            If CType(value, Boolean) Then
                Return TextWrapping.Wrap
            End If
        End If
        Return TextWrapping.NoWrap
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanTextWrapConverter")
    End Function
End Class