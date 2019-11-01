Imports Newtonsoft.Json

Public Class MDCFactory
    Public Shared ReadOnly Property MDDefTypes As New Dictionary(Of String, String) From {
        {"integer", "Ganze Zahl"},
        {"decimal", "Dezimalzahl"},
        {"textde", "Text - nur Deutsch"},
        {"textmultilang", "Text mehrsprachig"},
        {"date", "Datum"},
        {"filelink", "Link zu Datei"},
        {"folderlink", "Link zu Datei-Ordner"},
        {"boolean", "Ankreuzen: Ja/Nein-Auswahl"},
        {"listsingleselect", "Auswahlliste - nur eines wählbar"},
        {"listmultiselect", "Auswahlliste - mehrere wählbar"}}
    '{"metadataset", "Metadaten-Kombination"},
    '{"taxonomy", "Hierarchische Liste"}}

    Public Shared ReadOnly Property MDListTypes As New Dictionary(Of String, String) From {
        {"list", "Werte untereinander"},
        {"float", "Werte mit Umbruch fließend"},
        {"combobox", "Klappbox"},
        {"expandable", "Kompakt mit Erweiterungsschalter"}}

    Shared ReadOnly Property LanguageNamespace As XNamespace = "http://www.w3.org/XML/1998/namespace"

    Private Shared MDCatList As New Dictionary(Of String, MDCat)

    Public Shared Sub ResetCatalogList()
        MDCatList.Clear()
    End Sub

    Private Shared Function GetMDC(catId As String) As MDCat
        Dim myreturn As MDCat = Nothing
        If MDCatList.ContainsKey(catId) Then
            myreturn = MDCatList.Item(catId)
        Else
            Dim catId_splits As String() = catId.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
            If catId_splits.Count > 1 Then
                Dim myCat As MDCat = Nothing
                Dim prefix As String = catId_splits(0).ToUpper
                If prefix = "DOI" Then
                    Dim WebCatAddr As String = Nothing
                    Using client As New Net.WebClient()
                        Try
                            Dim responseString As String = client.DownloadString("https://doi.org/api/handles/" + catId_splits(1))
                            Dim responseObject As Linq.JObject = JsonConvert.DeserializeObject(responseString)
                            For Each o As Linq.JObject In From i In responseObject("values")
                                If o("type") = "URL" Then
                                    WebCatAddr = o("data")("value")
                                    Exit For
                                End If
                            Next
                        Catch ex As Exception
                            WebCatAddr = Nothing
                        End Try
                    End Using
                    If Not String.IsNullOrEmpty(WebCatAddr) Then
                        Try
                            Using client As New Net.WebClient()
                                Using mstream As New IO.MemoryStream(client.DownloadData(WebCatAddr))
                                    Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(mstream)
                                        myCat = New MDCat(XDocument.Load(xmlR))
                                    End Using
                                End Using
                            End Using
                        Catch ex As Exception
                            Debug.Print("Failed to open MD-Catalog DOI " + catId + ": " + ex.Message)
                        End Try
                    End If
                ElseIf prefix = "HTTP" OrElse prefix = "HTTPS" Then
                    Try
                        Using client As New Net.WebClient()
                            Using mstream As New IO.MemoryStream(client.DownloadData(catId))
                                Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(mstream)
                                    myCat = New MDCat(XDocument.Load(xmlR))
                                End Using
                            End Using
                        End Using
                    Catch ex As Exception
                        Debug.Print("Failed to open MD-Catalog HTTP/S " + catId + ": " + ex.Message)
                    End Try
                ElseIf prefix = "FILE" Then
                    Try
                        myCat = New MDCat(XDocument.Load(catId_splits(1)))
                    Catch ex As Exception
                        Debug.Print("Failed to open MD-Catalog FILE " + catId + ": " + ex.Message)
                    End Try
                End If

                If myCat IsNot Nothing Then
                    myreturn = myCat
                    MDCatList.Add(catId, myCat)
                End If
            End If
        End If

        Return myreturn
    End Function

    Public Shared Function GetMDCatLabel(catId As String, Optional LanguageKey As String = "de") As String
        Dim myreturn As String = "??"
        Dim myCat As MDCat = GetMDC(catId)
        If myCat IsNot Nothing Then myreturn = myCat.GetLabel()
        Return myreturn
    End Function

    Public Shared Function GetMDLabel(catId As String, defId As String, Optional LanguageKey As String = "de") As String
        Dim myreturn As String = "??"
        Dim myCat As MDCat = GetMDC(catId)
        If myCat IsNot Nothing Then myreturn = myCat.GetMDDefLabel(defId, LanguageKey)

        Return myreturn
    End Function

    Public Shared Function GetMDObject(ByRef XMD As XElement) As MDObject
        Dim myreturn As MDObject = Nothing
        Dim catId As String = XMD.@cat
        Dim myCat As MDCat = MDCFactory.GetMDC(catId)
        If myCat IsNot Nothing Then myreturn = myCat.GetMDObject(XMD)

        Return myreturn
    End Function

    Public Shared Function GetMDList(CatIdList As List(Of String)) As List(Of MDInfo)
        Dim myreturn As New List(Of MDInfo)
        For Each catId As String In CatIdList
            Dim myCat As MDCat = GetMDC(catId)
            If myCat IsNot Nothing Then
                For Each mdi As MDInfo In myCat.GetMDList()
                    mdi.CatId = catId
                    myreturn.Add(mdi)
                Next
            End If
        Next

        Return myreturn
    End Function
End Class

Public Class MDInfo
    Public Label As String
    Public Description As String
    Public CatId As String
    Public CatLabel As String
    Public id As String
End Class
