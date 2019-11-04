﻿Public Class MDCat
    Private MDDefs As Dictionary(Of String, XElement)
    Private XMDCat As XDocument
    Public Sub New(XDoc As XDocument)
        XMDCat = XDoc
        MDDefs = New Dictionary(Of String, XElement)
        If XDoc IsNot Nothing Then
            For Each xe As XElement In XDoc.Root.<MDDef>
                If Not MDDefs.ContainsKey(xe.@id) Then
                    MDDefs.Add(xe.@id, xe)
                End If
            Next
        End If
    End Sub
    Public Function GetLabel(Optional LanguageKey As String = "de") As String
        Dim myreturn As String = "??"
        If XMDCat IsNot Nothing Then
            Dim XLabel As XElement = (From entry In XMDCat.Root.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
            If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) AndAlso LanguageKey <> "de" Then XLabel = (From entry In XMDCat.Root.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
            myreturn = XLabel.Value
        End If
        Return myreturn
    End Function

    Public Function GetMDDefLabel(id As String, Optional LanguageKey As String = "de") As String
        Dim myreturn As String = "?" + id
        If MDDefs.ContainsKey(id) Then
            Dim md As XElement = MDDefs.Item(id)
            Dim XLabel As XElement = (From entry In md.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
            If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) AndAlso LanguageKey <> "de" Then XLabel = (From entry In md.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
            myreturn = XLabel.Value
        End If
        Return myreturn
    End Function

    Public Function GetMDDefDescription(id As String, Optional LanguageKey As String = "de") As String
        Dim myreturn As String = ""
        If MDDefs.ContainsKey(id) Then
            Dim md As XElement = MDDefs.Item(id)
            Dim XDescription As XElement = (From entry In md.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
            If XDescription Is Nothing OrElse String.IsNullOrEmpty(XDescription.Value) AndAlso LanguageKey <> "de" Then XDescription = (From entry In md.<Description> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
            If XDescription IsNot Nothing Then myreturn = XDescription.Value
        End If
        Return myreturn
    End Function

    Public Function GetMDDefType(id As String) As String
        Dim myreturn As String = ""
        If MDDefs.ContainsKey(id) Then
            Dim md As XElement = MDDefs.Item(id)
            myreturn = md.@type
        End If
        Return myreturn
    End Function

    Public Function GetMDObject(ByRef XMD As XElement) As MDObject
        Dim myreturn As MDObject = Nothing
        If XMD IsNot Nothing AndAlso MDDefs.ContainsKey(XMD.@def) Then myreturn = New MDObject(XMD, MDDefs.Item(XMD.@def))

        Return myreturn
    End Function

    Public Function GetMDList(Optional LanguageKey As String = "de") As List(Of MDInfo)
        Dim myreturn As New List(Of MDInfo)
        If XMDCat IsNot Nothing Then
            Dim XLabel As XElement = (From entry In XMDCat.Root.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = LanguageKey Select entry).FirstOrDefault
            If XLabel Is Nothing OrElse String.IsNullOrEmpty(XLabel.Value) AndAlso LanguageKey <> "de" Then XLabel = (From entry In XMDCat.Root.<Label> Where entry.Attribute(MDCFactory.LanguageNamespace + "lang").Value = "de" Select entry).FirstOrDefault
            Dim CatLabel As String = XLabel.Value
            Dim CatId As String = XMDCat.Root.@id

            For Each md As KeyValuePair(Of String, XElement) In MDDefs
                myreturn.Add(New MDInfo With {
                             .CatId = CatId, .CatLabel = CatLabel, .id = md.Key,
                             .Label = GetMDDefLabel(md.Key, LanguageKey), .Description = GetMDDefDescription(md.Key, LanguageKey)})
            Next
        End If

        Return myreturn
    End Function
End Class