Public Class AppCommands
    Public Shared ReadOnly SaveAs As RoutedUICommand = New RoutedUICommand("Speichern unter...", "SaveAs", GetType(FrameworkElement))
    Public Shared ReadOnly OpenWebCat As RoutedUICommand = New RoutedUICommand("Öffnen Katalog von Web-Adresse", "OpenWebCat", GetType(FrameworkElement))
    Public Shared ReadOnly EditCatMetadata As RoutedUICommand = New RoutedUICommand("Katalog-Kerndaten", "EditCatMetadata", GetType(FrameworkElement))
    Public Shared ReadOnly SaveTheWorld As RoutedUICommand = New RoutedUICommand("Welt retten", "SaveTheWorld", GetType(FrameworkElement),
                                                                                 New InputGestureCollection From {New KeyGesture(Key.M, ModifierKeys.Control, "Strg+M")})
End Class
