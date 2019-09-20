Public Class XTextLanguagePropMergeConverter
    Implements IMultiValueConverter
    Shared ReadOnly Property LanguageNamespace As XNamespace = "http://www.w3.org/XML/1998/namespace"

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim XTextList As New List(Of XElement)
        If values.Count > 2 AndAlso TypeOf values(0) Is XElement Then
            Dim XMD As XElement = values(0)
            Dim LanguagesList As List(Of String)
            If values(1) IsNot Nothing AndAlso values(1) IsNot DependencyProperty.UnsetValue AndAlso Not String.IsNullOrEmpty(values(1)) Then
                Dim Languages As String = values(1)
                LanguagesList = Languages.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            Else
                LanguagesList = New List(Of String) From {"de"}
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
                Dim XValue As XElement = (From xe As XElement In XValueList Where xe.Attribute(XTextLanguagePropMergeConverter.LanguageNamespace + "lang").Value = lang).FirstOrDefault
                If XValue Is Nothing Then
                    XValue = New XElement(TagFilter)
                    XValue.SetAttributeValue(XTextLanguagePropMergeConverter.LanguageNamespace + "lang", lang)
                    XMD.Add(XValue)
                End If
                XTextList.Add(XValue)
            Next
        End If
        Return XTextList
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("XTextLanguagePropMergeConverter ConvertBack")
    End Function
End Class

'####################################################################
'für LanguageVisualUserControl, liefert "de" oder "en" aus der Sprach-Info heraus
'erwartet ein XElement mit einem lang-Attribut
Public Class XLanguageAttribute2LettersConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf value Is XElement Then
            Dim xvalue As XElement = value
            Dim langshorty As String = xvalue.Attribute(XTextLanguagePropMergeConverter.LanguageNamespace + "lang").Value
            If Not String.IsNullOrEmpty(langshorty) Then Return langshorty
        End If
        Return "??"
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in XLanguageAttribute2LettersConverter")
    End Function
End Class

'####################################################################
'für LanguageVisualUserControl, liefert ein Dreieck (PointCollection) zur Visualisierung von "de" (rechter Winkel unten links) oder andere (rechter Winkel oben links)
'erwartet ein XElement mit einem lang-Attribut
Public Class XLanguageAttributePolygonPointsConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf value Is XElement Then
            Dim xvalue As XElement = value
            Dim langshorty As String = xvalue.Attribute(XTextLanguagePropMergeConverter.LanguageNamespace + "lang").Value
            If Not String.IsNullOrEmpty(langshorty) Then
                Dim myreturn As New PointCollection()
                If langshorty = "de" Then
                    myreturn.Add(New System.Windows.Point(0, 0))
                    myreturn.Add(New System.Windows.Point(0, 20))
                    myreturn.Add(New System.Windows.Point(20, 20))
                Else
                    myreturn.Add(New System.Windows.Point(0, 0))
                    myreturn.Add(New System.Windows.Point(0, 20))
                    myreturn.Add(New System.Windows.Point(20, 0))
                End If
                Return myreturn
            End If
        End If
        Return Nothing
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in XLanguageAttributePolygonPointsConverter")
    End Function
End Class


Public Class BooleanTextWrapConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If TypeOf value Is Boolean Then
            If CType(value, Boolean) = True Then
                Return TextWrapping.Wrap
            End If
        End If
        Return TextWrapping.NoWrap
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in BooleanTextWrapConverter")
    End Function
End Class
