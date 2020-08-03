Public Class MDR2MDC
    Private Const CatFilename = "cat.xml"
    Private Const UIFilename = "ui.xml"

    Public Shared Function TransformOld(OldFName As String) As XDocument
        Dim myreturn As XDocument = <?xml version="1.0" encoding="utf-8"?>
                                    <MDCat xsi:noNamespaceSchemaLocation="http://raw.githubusercontent.com/iqb-mdc/core/master/mdc.xsd"
                                        xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
                                        id=""
                                        version="1.0.0" versionhistory="">
                                        <Label xml:lang="de">Katalog des IQB</Label>
                                        <Owner xml:lang="de">IQB - Institut zur Qualitätsentwicklung im Bildungswesen</Owner>
                                        <Owner xml:lang="en">IQB - Institute for Educational Quality Improvement</Owner>
                                        <License>MIT</License>
                                    </MDCat>

        Dim XOldCat As XDocument = Nothing
        Dim UIDefinition As XDocument = Nothing
        Using mstream As New IO.MemoryStream(IO.File.ReadAllBytes(OldFName))
            Using ZipCat As New System.IO.Compression.ZipArchive(mstream, System.IO.Compression.ZipArchiveMode.Read)
                For Each ze As System.IO.Compression.ZipArchiveEntry In ZipCat.Entries
                    Select Case ze.Name
                        Case UIFilename
                            Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(ze.Open())
                                UIDefinition = XDocument.Load(xmlR)
                            End Using
                        Case CatFilename
                            Using xmlR As System.Xml.XmlReader = System.Xml.XmlReader.Create(ze.Open())
                                XOldCat = XDocument.Load(xmlR)
                            End Using
                    End Select
                Next
            End Using
        End Using


        If XOldCat.Root.Name.LocalName <> "Catalog" Then Throw New ArgumentException("Root-Element nit 'Catalog'")

        Dim ScopeTranslate As New Dictionary(Of String, XElement) From {
                    {"sj.case", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">7</MD>},
                    {"sj.contact", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">10</MD>},
                    {"sj.study", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">14</MD>},
                    {"sj.applicationcontent", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">15</MD>},
                    {"sj.application", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">1</MD>},
                    {"sj.applicant", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">2</MD>},
                    {"sj.process", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">3</MD>},
                    {"sj.dataset", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">5</MD>},
                    {"sj.survey", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">6</MD>},
                    {"sj.release", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">8</MD>},
                    {"sj.instrument", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">9</MD>},
                    {"sj.project", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">13</MD>},
                    {"sj.person", <MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2">12</MD>}
            }

        Dim XProps As XElement = XOldCat.Root.<Properties>.FirstOrDefault
        If XProps IsNot Nothing Then
            For Each XProp As XElement In XProps.<Property>
                If XProp.@public.ToUpper = "TRUE" AndAlso XProp.@deprecated.ToUpper = "FALSE" Then
                    Dim XMD As XElement = <MDDef/>
                    Dim XTypeSpec As XElement = Nothing
                    Debug.Print(XProp.@key)
                    XMD.@id = XProp.@key.Substring(1)
                    If XProp.@type = "second" Then
                        XMD.@type = "integer"
                        XTypeSpec = <TypeSpec>
                                        <Min>0</Min>
                                        <Seconds>true</Seconds>
                                    </TypeSpec>
                    ElseIf XProp.@type = "linkfolder" Then
                        XMD.@type = "folderlink"
                    Else
                        XMD.@type = XProp.@type
                        If XMD.@type = "listsingleselect" OrElse XMD.@type = "listmultiselect" Then
                            XTypeSpec = <TypeSpec>
                                            <ListControl>list</ListControl>
                                        </TypeSpec>
                        End If
                    End If

                    For Each xe As XElement In XProp.<Metadata>.First.Elements
                        XMD.Add(New XElement(xe))
                    Next

                    Dim XMDDefMDs As XElement = <MDDefMetadata/>
                    For Each s As String In XProp.<ScopeRef>.First.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                        If Not ScopeTranslate.ContainsKey(s) Then Throw New ArgumentException("MDR2MDC.TransformOld: Scope '" + s + "'nicht bekannt.")
                        Dim XMDMD As XElement = ScopeTranslate.Item(s)
                        Dim xhabschon As XElement = (From xe As XElement In XMDDefMDs.Elements Where xe.@cat = XMDMD.@cat AndAlso xe.@def = XMDMD.@def).FirstOrDefault
                        If xhabschon Is Nothing Then
                            XMDDefMDs.Add(New XElement(XMDMD))
                        Else
                            xhabschon.Value = xhabschon.Value + " " + XMDMD.Value
                        End If
                    Next
                    XMD.Add(XMDDefMDs)
                    If XTypeSpec IsNot Nothing Then XMD.Add(XTypeSpec)

                    Dim XMDValues As XElement = XProp.<Values>.FirstOrDefault
                    If XMDValues IsNot Nothing Then
                        For Each xv As XElement In XMDValues.<Value>
                            Dim XMDV As XElement = <Value/>
                            XMDV.@id = xv.@key.Substring(2)
                            For Each xe As XElement In xv.<Metadata>.First.Elements
                                XMDV.Add(New XElement(xe))
                            Next
                            XMD.Add(XMDV)
                        Next
                    End If
                    myreturn.Root.Add(XMD)
                End If
            Next
        End If

        Return myreturn
    End Function
End Class
