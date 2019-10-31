Imports iqb.mdc.xml
Public Class MDConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim myreturn As New List(Of MDObject)
        If values.Count > 1 AndAlso TypeOf values(0) Is XElement Then
            For Each XMD As XElement In CType(values(0), XElement).Elements
                Dim mdo As MDObject = MDCFactory.GetMDObject(XMD)
                AddHandler mdo.XMD.Changed, AddressOf NotifyXMDChanged
                myreturn.Add(mdo)
            Next
        End If
        Return myreturn
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("MDConverter ConvertBack")
    End Function

    Private Sub NotifyXMDChanged(sender As Object, e As System.Xml.Linq.XObjectChangeEventArgs)
        Debug.Print(e.ObjectChange.ToString)
    End Sub

End Class

Public Class TypeSpecTextWrappingConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is Dictionary(Of String, String) Then
            Dim valueDict As Dictionary(Of String, String) = value
            If valueDict.ContainsKey("MultiLineText") AndAlso valueDict.Item("MultiLineText").ToUpper = "TRUE" Then
                Return TextWrapping.Wrap
            End If
        End If
        Return TextWrapping.NoWrap
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TypeSpecTextWrappingConverter")
    End Function
End Class

Public Class TypeSpecAcceptsReturnConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is Dictionary(Of String, String) Then
            Dim valueDict As Dictionary(Of String, String) = value
            If valueDict.ContainsKey("MultiLineText") AndAlso valueDict.Item("MultiLineText").ToUpper = "TRUE" Then
                Return True
            End If
        End If
        Return False
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TypeSpecAcceptsReturnConverter")
    End Function
End Class
Public Class TypeSpecLinkTypeConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is Dictionary(Of String, String) Then
            Dim valueDict As Dictionary(Of String, String) = value
            If valueDict.ContainsKey("type") Then
                If valueDict.Item("type").ToLower = "folderlink" Then
                    Return "folder"
                ElseIf valueDict.Item("type").ToLower = "filelink" Then
                    Return "file"
                End If
            End If
        End If
        Return "linktype unknown"
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in TypeSpecLinkTypeConverter")
    End Function
End Class

Public Class TextMultiLangMergeConverter
    Implements IValueConverter
    Shared ReadOnly Property LanguageNamespace As XNamespace = "http://www.w3.org/XML/1998/namespace"

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        Dim XTextList As New List(Of XElement)
        If value IsNot DependencyProperty.UnsetValue AndAlso TypeOf value Is MDObject Then
            Dim myMDObject As MDObject = value
            Dim LanguagesList As New List(Of String) From {"de"}
            Dim TypeSpecDict As Dictionary(Of String, String) = myMDObject.TypeSpec
            If TypeSpecDict.ContainsKey("Languages") Then
                LanguagesList = TypeSpecDict.Item("Languages").Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
            End If

            Dim XValueList As List(Of XElement) = (From xe As XElement In myMDObject.XMD.Elements).ToList
            Dim TagFilter As String = "T"
            If XValueList.Count > 0 Then TagFilter = XValueList.First.Name.LocalName
            For Each lang As String In LanguagesList
                Dim XValue As XElement = (From xe As XElement In XValueList Where xe.Attribute(TextMultiLangMergeConverter.LanguageNamespace + "lang").Value = lang).FirstOrDefault
                If XValue Is Nothing Then
                    XValue = New XElement(TagFilter)
                    XValue.SetAttributeValue(TextMultiLangMergeConverter.LanguageNamespace + "lang", lang)
                    myMDObject.XMD.Add(XValue)
                End If
                XTextList.Add(XValue)
            Next
        End If
        Return XTextList
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("TextMultiLangMergeConverter ConvertBack")
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
            Dim langshorty As String = xvalue.Attribute(TextMultiLangMergeConverter.LanguageNamespace + "lang").Value
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
            Dim langshorty As String = xvalue.Attribute(TextMultiLangMergeConverter.LanguageNamespace + "lang").Value
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

'####################################################################
Public Class DateStringDateConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If String.IsNullOrEmpty(value) Then
            Return Nothing
        Else
            Dim ParseDate As Date
            If Date.TryParse(value, ParseDate) Then
                Return ParseDate
            Else
                Return Nothing
            End If
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        If TypeOf (value) Is Date Then
            Return CType(value, Date).ToString("yyyy-MM-dd")
        Else
            Return Nothing
        End If
    End Function
End Class

Public Class TimeStringIntegerConverter
    Implements IValueConverter

    Public Const MatchPattern = "^[0-5]{0, 1}[0-9]: [0-5]{0,1}[0-9]0{0,1}$"

    Public Function Convert(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.Convert
        Dim myInteger As Integer = 0
        If value Is Nothing OrElse Not TypeOf (value) Is String OrElse Not Integer.TryParse(value, myInteger) Then
            Return ""
        Else
            Dim ts As TimeSpan = TimeSpan.FromSeconds(myInteger)
            Return ts.ToString("m\:ss")
        End If
    End Function

    Public Function ConvertBack(value As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IValueConverter.ConvertBack
        If value Is Nothing OrElse Not TypeOf (value) Is String OrElse String.IsNullOrEmpty(value) OrElse Not Text.RegularExpressions.Regex.IsMatch(CType(value, String).Trim, MatchPattern) Then
            Return 0
        Else
            Dim myMatch As Text.RegularExpressions.Match = Text.RegularExpressions.Regex.Match(CType(value, String).Trim, MatchPattern)
            Dim minStr As String = myMatch.Value.Substring(0, myMatch.Value.IndexOf(":"))
            Dim secStr As String = myMatch.Value.Substring(myMatch.Value.IndexOf(":") + 1)
            If secStr.Length = 1 AndAlso secStr < "6" Then
                secStr += "0"
            ElseIf secStr.Length = 3 Then
                secStr = secStr.Substring(0, 2)
            End If

            Return Integer.Parse(minStr) * 60 + Integer.Parse(secStr)
        End If
    End Function
End Class