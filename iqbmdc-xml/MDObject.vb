Public Class MDObject
    Private _XMD As XElement
    Public ReadOnly Property XMD As XElement
        Get
            Return _XMD
        End Get
    End Property

    Private _XMDDef As XElement

    Public ReadOnly Property TypeSpec() As Dictionary(Of String, String)
        Get
            Dim myreturn As New Dictionary(Of String, String) From {{"type", _XMDDef.@type}}
            Dim XTypeSpec As XElement = (From xe As XElement In _XMDDef.Elements Where xe.Name.LocalName = "TypeSpec").FirstOrDefault
            If XTypeSpec IsNot Nothing Then
                For Each xe As XElement In XTypeSpec.Elements
                    If Not String.IsNullOrEmpty(xe.Value) AndAlso Not myreturn.ContainsKey(xe.Name.LocalName) Then myreturn.Add(xe.Name.LocalName, xe.Value)
                Next
            End If
            Return myreturn
        End Get
    End Property

    Public ReadOnly Property Value() As String
        Get
            Return _XMD.Value
        End Get
    End Property
    Public ReadOnly Property Label() As String
        Get
            Return GetLabel("de")
        End Get
    End Property

    Public Sub New(ByRef XMDSource As XElement, ByRef XMDDefSource As XElement)
        _XMD = XMDSource
        _XMDDef = XMDDefSource
    End Sub

    Public Function GetLabel(LanguageKey As String) As String
        Dim myreturn As String = "?"
        Dim XLabel As XElement = (From entry In _XMDDef.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
        If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) AndAlso LanguageKey <> "de" Then XLabel = (From entry In _XMDDef.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
        myreturn = XLabel.Value
        Return myreturn
    End Function

    Public Function GetDescription(LanguageKey As String) As String
        Dim myreturn As String = "?"
        Dim XDescription As XElement = (From entry In _XMDDef.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
        If XDescription Is Nothing OrElse String.IsNullOrEmpty(XDescription.Value) AndAlso LanguageKey <> "de" Then XDescription = (From entry In _XMDDef.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
        myreturn = XDescription.Value
        Return myreturn
    End Function

    Public Function GetValueAsText(ListValueSeparator As String, IncludeDescriptions As Boolean, LanguageKey As String, fallback_to_de As Boolean) As String
        Dim myreturn As String = ""
        If _XMD IsNot Nothing Then
            myreturn = _XMD.Value
            If _XMDDef IsNot Nothing Then
                Select Case _XMDDef.@type.ToLower

                    'Case "textde", "folderlink", "filelink"
                    '   myreturn = XMD.Value

                    Case "textmultilang"
                        If _XMD.HasElements Then
                            Dim xlang As XElement = (From entry As XElement In _XMD.Elements Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey).FirstOrDefault
                            If (xlang Is Nothing OrElse String.IsNullOrEmpty(xlang)) AndAlso LanguageKey <> "de" AndAlso fallback_to_de Then xlang = (From entry As XElement In _XMD.Elements Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de").FirstOrDefault
                            If xlang IsNot Nothing Then myreturn = xlang.Value
                        End If

                    Case "date"
                        If String.IsNullOrEmpty(_XMD.Value) Then
                            myreturn = "-"
                        Else
                            Dim ParseDate As Date
                            If Date.TryParse(_XMD.Value, ParseDate) Then
                                myreturn = ParseDate.ToString("dd.MM.yyyy")
                            Else
                                myreturn = "ungültiges Datum"
                            End If
                        End If

                    Case "boolean"
                        If _XMD.Value.ToLower = "true" Then
                            If LanguageKey = "de" Then
                                myreturn = "Ja"
                            Else
                                myreturn = "Yes"
                            End If
                        Else
                            If LanguageKey = "de" Then
                                myreturn = "Nein"
                            Else
                                myreturn = "No"
                            End If
                        End If

                    Case "integer"
                        If String.IsNullOrEmpty(_XMD.Value) Then
                            myreturn = "0"
                        Else
                            Dim myInteger As Integer
                            If Integer.TryParse(_XMD.Value, myInteger) Then
                                myreturn = myInteger.ToString

                                Dim XTypeSpec As XElement = (From entry As XElement In _XMD.<TypeSpec>).FirstOrDefault
                                If XTypeSpec IsNot Nothing Then
                                    Dim XSeconds As XElement = (From entry As XElement In XTypeSpec.<Seconds>).FirstOrDefault
                                    If XSeconds IsNot Nothing AndAlso XSeconds.Value.ToUpper = "TRUE" Then
                                        Dim ts As TimeSpan = TimeSpan.FromSeconds(myInteger)
                                        myreturn = ts.ToString("m\:ss")
                                    End If
                                End If
                            End If
                        End If

                    Case "decimal"
                        If String.IsNullOrEmpty(_XMD.Value) Then
                            myreturn = "0"
                        Else
                            Dim myDecimal As Double
                            If Double.TryParse(_XMD.Value.Replace(".", ","), myDecimal) Then
                                Dim digits As Integer = 2

                                Dim XTypeSpec As XElement = (From entry As XElement In _XMDDef.<TypeSpec>).FirstOrDefault
                                If XTypeSpec IsNot Nothing Then
                                    Dim XDigits As XElement = (From entry As XElement In XTypeSpec.<Digits>).FirstOrDefault
                                    If XDigits IsNot Nothing AndAlso Integer.TryParse(XDigits.Value, digits) Then
                                        If digits < 0 OrElse digits > 9 Then digits = 2
                                    End If
                                End If
                                myreturn = myDecimal.ToString("F" + digits.ToString) ', Globalization.CultureInfo.InvariantCulture)
                            Else
                                myreturn = "?" + _XMD.Value
                            End If
                        End If

                    Case "listsingleselect", "listmultiselect"
                        Dim XMDValueIds As List(Of String) = (From id As String In _XMD.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries)).ToList
                        Dim myreturnList As New List(Of String)
                        For Each XValue As XElement In _XMDDef.<Value>
                            If XMDValueIds.Contains(XValue.@id) Then
                                Dim XLabel As XElement = (From entry As XElement In XValue.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey).FirstOrDefault
                                If (XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value)) AndAlso LanguageKey <> "de" Then XLabel = (From entry As XElement In XValue.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de").FirstOrDefault
                                If XLabel IsNot Nothing Then
                                    Dim myText As String = XLabel.Value
                                    If IncludeDescriptions Then
                                        Dim XDescription As XElement = (From entry As XElement In XValue.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey).FirstOrDefault
                                        If (XDescription Is Nothing OrElse String.IsNullOrEmpty(XDescription.Value)) AndAlso LanguageKey <> "de" Then XDescription = (From entry As XElement In XValue.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de").FirstOrDefault
                                        If XDescription IsNot Nothing AndAlso Not String.IsNullOrEmpty(XDescription.Value) Then myText = myText + " (" + XDescription.Value + ")"
                                    End If
                                    myreturnList.Add(myText)
                                End If
                            End If
                        Next
                        myreturn = String.Join(ListValueSeparator, myreturnList)

                    Case Else
                        myreturn = _XMD.Value
                End Select
            End If
        End If

        Return myreturn
    End Function

    Public Function GetValueAsBoolean() As Boolean
        Dim myreturn As Boolean = False
        If _XMD IsNot Nothing AndAlso _XMDDef IsNot Nothing Then
            If _XMDDef.@type.ToLower = "boolean" Then
                myreturn = _XMD.Value.ToLower = "true"
            End If
        End If

        Return myreturn
    End Function

    Public Function GetListValues(LanguageKey As String) As Dictionary(Of String, String)
        Dim myreturn As New Dictionary(Of String, String)
        For Each xListValue As XElement In _XMDDef.<Value>
            Dim XLabel As XElement = (From entry In xListValue.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
            If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) AndAlso LanguageKey <> "de" Then XLabel = (From entry In xListValue.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
            If Not myreturn.ContainsKey(xListValue.@id) Then myreturn.Add(xListValue.@id, XLabel.Value)
        Next

        Return myreturn
    End Function
End Class
