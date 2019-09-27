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

    Public Function ResolveCatSource(catId As String, catVersion As String, source As String) As String
        Dim myreturn As String = ""



        Return myreturn
    End Function
End Class
