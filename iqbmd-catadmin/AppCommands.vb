Public Class AppCommands
    Public Shared ReadOnly SaveAs As RoutedUICommand = New RoutedUICommand("Speichern unter... (Datei)", "SaveAs", GetType(FrameworkElement))
    Public Shared ReadOnly NewCatalog As RoutedUICommand = New RoutedUICommand("Neuer Katalog", "NewCatalog", GetType(FrameworkElement))
    Public Shared ReadOnly OpenWebCat As RoutedUICommand = New RoutedUICommand("Öffnen Katalog von Web-Adresse", "OpenWebCat", GetType(FrameworkElement))
    Public Shared ReadOnly EditCatData As RoutedUICommand = New RoutedUICommand("Katalog-Kerndaten", "EditCatData", GetType(FrameworkElement))
    Public Shared ReadOnly EditMDCoreCats As RoutedUICommand = New RoutedUICommand("Kataloge für Metadaten der Definitionen ändern", "EditMDCoreCats", GetType(FrameworkElement))

    Public Shared ReadOnly NewCatalogFromOld As RoutedUICommand = New RoutedUICommand("Neuer Katalog von alt", "NewCatalogFromOld", GetType(FrameworkElement))
    Public Shared ReadOnly TestMDControls As RoutedUICommand = New RoutedUICommand("Metadaten-Controls prüfen", "TestMDControls", GetType(FrameworkElement))
    Public Shared ReadOnly SaveTheWorld As RoutedUICommand = New RoutedUICommand("Welt retten", "SaveTheWorld", GetType(FrameworkElement),
                                                                                 New InputGestureCollection From {New KeyGesture(Key.M, ModifierKeys.Control, "Strg+M")})
End Class
