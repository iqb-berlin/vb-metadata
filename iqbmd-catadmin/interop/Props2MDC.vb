Public Class Props2MDC
    Public Shared usedProps As New List(Of String) From {{"p10"}, {"p105"}, {"p106"}, {"p107"}, {"p108"}, {"p109"}, {"p11"}, {"p113"}, {"p115"}, {"p116"}, {"p117"}, {"p118"}, {"p119"}, {"p120"}, {"p121"}, {"p122"}, {"p123"}, {"p124"}, {"p125"}, {"p13"}, {"p133"}, {"p134"}, {"p135"}, {"p136"}, {"p137"}, {"p138"}, {"p139"}, {"p14"}, {"p140"}, {"p141"}, {"p142"}, {"p143"}, {"p144"}, {"p145"}, {"p146"}, {"p147"}, {"p148"}, {"p149"}, {"p15"}, {"p150"}, {"p151"}, {"p152"}, {"p153"}, {"p154"}, {"p155"}, {"p156"}, {"p157"}, {"p158"}, {"p159"}, {"p16"}, {"p160"}, {"p161"}, {"p162"}, {"p163"}, {"p164"}, {"p165"}, {"p166"}, {"p167"}, {"p17"}, {"p18"}, {"p19"}, {"p2"}, {"p20"}, {"p21"}, {"p22"}, {"p23"}, {"p24"}, {"p25"}, {"p26"}, {"p27"}, {"p28"}, {"p29"}, {"p3"}, {"p30"}, {"p31"}, {"p32"}, {"p33"}, {"p34"}, {"p35"}, {"p36"}, {"p37"}, {"p38"}, {"p39"}, {"p40"}, {"p41"}, {"p42"}, {"p43"}, {"p44"}, {"p45"}, {"p46"}, {"p47"}, {"p48"}, {"p49"}, {"p5"}, {"p50"}, {"p51"}, {"p52"}, {"p53"}, {"p54"}, {"p55"}, {"p56"}, {"p57"}, {"p58"}, {"p59"}, {"p6"}, {"p60"}, {"p61"}, {"p62"}, {"p63"}, {"p64"}, {"p65"}, {"p66"}, {"p67"}, {"p68"}, {"p7"}, {"p8"}, {"p82"}, {"p83"}, {"p84"}, {"p85"}, {"p86"}, {"p87"}, {"p88"}}

    Public Shared Function TransformOld(OldFName As String) As XDocument
        Dim myreturn As XDocument = <?xml version="1.0" encoding="utf-8"?>
                                    <MDCat xsi:noNamespaceSchemaLocation=""
                                        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                        id=""
                                        version="1.0.0" versionhistory="">
                                        <Label xml:lang="de">Katalog des IQB</Label>
                                        <Owner xml:lang="de">IQB - Institut zur Qualitätsentwicklung im Bildungswesen</Owner>
                                        <Owner xml:lang="en">IQB - Institute for Educational Quality Improvement</Owner>
                                        <License>MIT</License>
                                    </MDCat>

        Dim XOldCat As XDocument = Nothing
        If IO.Path.GetExtension(OldFName).ToLower = "zip" Then
            Using mstream As New IO.MemoryStream(IO.File.ReadAllBytes(OldFName))
                Using ZipCat As New System.IO.Compression.ZipArchive(mstream, System.IO.Compression.ZipArchiveMode.Read)
                    For Each ze As System.IO.Compression.ZipArchiveEntry In ZipCat.Entries
                        Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(ze.Open())
                            XOldCat = XDocument.Load(xmlR)
                        End Using
                    Next
                End Using
            End Using
        Else
            XOldCat = XDocument.Load(OldFName)
        End If

        If XOldCat.Root.Name.LocalName <> "props" Then Throw New ArgumentException("Root-Element nicht 'props'")

        Dim XSubjectDef As XElement = <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="1"></MD>
        Dim SubjectMap As New Dictionary(Of String, String) From {
            {"dep", "1"},
            {"de1", "2"},
            {"dev6", "3"},
            {"en1", "4"},
            {"frz1", "6"},
            {"map", "7"},
            {"ma1", "8"},
            {"nawi1", "9"}
           }
        Dim XScopeDef As XElement = <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2"></MD>
        Dim ScopeMap As New Dictionary(Of String, String) From {
            {"task", "4"},
            {"item", "11"}
           }

        For Each XProp As XElement In XOldCat.Root.<p>
            If usedProps.Contains(XProp.@key) Then
                Dim XMD As XElement = <MDDef id=<%= XProp.@key.Substring(1) %>>
                                          <Label xml:lang="de"><%= XProp.@lb %></Label>
                                      </MDDef>
                Dim XTypeSpec As XElement = Nothing

                If XProp.@t = "seconds" Then
                    XMD.@type = "integer"
                    XTypeSpec = <TypeSpec>
                                    <Min>0</Min>
                                    <Seconds>true</Seconds>
                                </TypeSpec>
                ElseIf XProp.@t = "textsingleline" Then
                    XMD.@type = "textde"
                ElseIf XProp.@t = "textmultiline" Then
                    XMD.@type = "textde"
                    XTypeSpec = <TypeSpec>
                                    <MultiLineText>true</MultiLineText>
                                </TypeSpec>
                Else
                    XMD.@type = XProp.@t
                    If XMD.@type = "listsingleselect" OrElse XMD.@type = "listmultiselect" Then
                        XTypeSpec = <TypeSpec>
                                        <ListControl>list</ListControl>
                                    </TypeSpec>
                    End If
                End If

                Dim XMDDefMDs As XElement = <MDDefMetadata/>
                Dim XScopes As XElement = XProp.<scope>.FirstOrDefault
                If XScopes IsNot Nothing AndAlso Not String.IsNullOrEmpty(XScopes.Value) Then
                    Dim XMDScopes As New XElement(XScopeDef)
                    For Each s As String In XScopes.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                        If ScopeMap.ContainsKey(s) Then
                            If String.IsNullOrEmpty(XMDScopes.Value) Then
                                XMDScopes.Value = ScopeMap.Item(s)
                            Else
                                XMDScopes.Value = XMDScopes.Value + " " + ScopeMap.Item(s)
                            End If
                        End If
                    Next
                    XMDDefMDs.Add(XMDScopes)
                End If
                Dim XSubjects As XElement = XProp.<pools>.FirstOrDefault
                If XSubjects IsNot Nothing AndAlso Not String.IsNullOrEmpty(XSubjects.Value) Then
                    Dim XMDSubjects As New XElement(XSubjectDef)
                    For Each s As String In XSubjects.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                        If SubjectMap.ContainsKey(s) Then
                            If String.IsNullOrEmpty(XMDSubjects.Value) Then
                                XMDSubjects.Value = SubjectMap.Item(s)
                            Else
                                XMDSubjects.Value = XMDSubjects.Value + " " + SubjectMap.Item(s)
                            End If
                        End If
                    Next
                    XMDDefMDs.Add(XMDSubjects)
                End If
                XMD.Add(XMDDefMDs)
                If XTypeSpec IsNot Nothing Then XMD.Add(XTypeSpec)

                For Each xv As XElement In XProp.<v>
                    Dim xValue As XElement = <Value id=<%= xv.@key.Substring(2) %>>
                                                 <Label xml:lang="de"><%= xv.@lb %></Label>
                                             </Value>
                    If Not String.IsNullOrEmpty(xv) Then xValue.Add(<Description xml:lang="de"><%= xv.Value %></Description>)
                    XMD.Add(xValue)
                Next

                myreturn.Root.Add(XMD)
            End If
        Next

        Return myreturn
    End Function
End Class
