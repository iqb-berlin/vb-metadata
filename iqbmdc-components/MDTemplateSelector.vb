Imports System.ComponentModel
Imports iqb.mdc.xml

Public Class MDTemplateSelector
    Inherits DataTemplateSelector

    Private Shared ThisAssemblyName As String = System.Reflection.Assembly.GetAssembly(GetType(MDTemplateSelector)).GetName.Name
    Private Shared MDNamespace As String = "iqb.mdc.components."

    Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As System.Windows.DependencyObject) As System.Windows.DataTemplate
        Dim myControlTypeKey As String = "AnyShowControl"

        If item IsNot Nothing AndAlso TypeOf (item) Is MDObject Then
            Dim myTypeDef As Dictionary(Of String, String) = CType(item, MDObject).TypeSpec
            If myTypeDef IsNot Nothing AndAlso myTypeDef.ContainsKey("type") Then
                Select Case myTypeDef.Item("type")
                    Case "integer", "decimal", "textde", "date"
                        myControlTypeKey = "doof"
                    Case "filelink", "folderlink"
                        myControlTypeKey = "link"
                    Case "boolean"
                        myControlTypeKey = "boo"
                        '"listsingleselect", "listmultiselect"
                End Select
            End If
        End If
        Dim PPath As New PropertyPath(MDNamespace + myControlTypeKey + "," + ThisAssemblyName)
        Dim myControlType As Type = Type.GetType(PPath.Path, True, True)
        Dim factory As New FrameworkElementFactory(myControlType)
        'factory.SetBinding(SinglePropertyControl.XPropProperty, New Binding With {.Source = item})
        'If PropContainer IsNot Nothing Then
        '    factory.SetBinding(SinglePropertyControl.MetadataCatalogProperty, New Binding With {.Source = PropContainer.MetadataCatalog, .Mode = BindingMode.OneWay})
        '    factory.SetBinding(SinglePropertyControl.MainLanguageProperty, New Binding With {.Source = PropContainer.MetadataCatalog.MainLanguage, .Mode = BindingMode.OneWay})
        '    factory.SetBinding(SinglePropertyControl.OptionalLanguagesProperty, New Binding With {.Source = PropContainer.MetadataCatalog.OptionalLanguages, .Mode = BindingMode.OneWay})
        'End If

        Return New DataTemplate With {.VisualTree = factory}
    End Function
End Class
