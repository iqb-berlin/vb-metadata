Imports System.ComponentModel
Imports iqb.md.xml

Public Class MDTemplateSelector
    Inherits DataTemplateSelector

    Private Shared ThisAssemblyName As String = System.Reflection.Assembly.GetAssembly(GetType(MDTemplateSelector)).GetName.Name
    Private Shared MDNamespace As String = "iqb.md.components."

    Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As System.Windows.DependencyObject) As System.Windows.DataTemplate
        Dim myControlTypeKey As String = "AnyShowControl"
        Dim IsReadOnly As Boolean = False
        If item IsNot Nothing AndAlso TypeOf (item) Is MDObject Then
            Dim element As FrameworkElement = container
            Do While element IsNot Nothing AndAlso Not TypeOf (element) Is MDListControl
                element = VisualTreeHelper.GetParent(element)
            Loop
            If element IsNot Nothing Then
                IsReadOnly = CType(element, MDListControl).IsReadOnly
            End If

            Dim myTypeDef As Dictionary(Of String, String) = CType(item, MDObject).TypeSpec
            If myTypeDef IsNot Nothing AndAlso myTypeDef.ContainsKey("type") Then
                Select Case myTypeDef.Item("type")
                    Case "textmultilang"
                        If IsReadOnly Then
                            myControlTypeKey = "TextMultiLanguageShowControl"
                        Else
                            myControlTypeKey = "TextMultiLanguageEditControl"
                        End If

                    Case "integer"
                        If Not IsReadOnly Then
                            myControlTypeKey = "NumericEditIntegerControl"
                            If myTypeDef.ContainsKey("Seconds") Then
                                If myTypeDef.Item("Seconds").ToUpper = "TRUE" Then
                                    myControlTypeKey = "SecondsEdit"
                                End If
                            End If
                        End If

                    Case "decimal"
                        If Not IsReadOnly Then myControlTypeKey = "NumericEditDoubleControl"

                    Case "textde"
                        If Not IsReadOnly Then myControlTypeKey = "TextEditControl"

                    Case "filelink", "folderlink"
                        If IsReadOnly Then
                            myControlTypeKey = "LinkShowControl"
                        Else
                            myControlTypeKey = "LinkEditControl"
                        End If

                    Case "date"
                        If Not IsReadOnly Then myControlTypeKey = "DateEditControl"

                    Case "boolean"
                        If IsReadOnly Then
                            myControlTypeKey = "BooleanShowControl"
                        Else
                            myControlTypeKey = "OneCheckBox"
                        End If

                    Case "listsingleselect"
                        If Not IsReadOnly Then
                            myControlTypeKey = "RadioButtonsControl"
                            If myTypeDef.ContainsKey("ListControl") Then
                                Select Case myTypeDef.Item("ListControl")
                                    Case "combobox"
                                        myControlTypeKey = "ComboBoxControl"
                                    Case "float"
                                        myControlTypeKey = "RadioButtonsFloatControl"
                                End Select
                            End If
                        End If

                    Case "listmultiselect"
                        If Not IsReadOnly Then
                            myControlTypeKey = "CheckBoxesListControl"
                            If myTypeDef.ContainsKey("ListControl") Then
                                Select Case myTypeDef.Item("ListControl")
                                    Case "expandable"
                                        myControlTypeKey = "CheckBoxesFloatExpanderControl"
                                    Case "float"
                                        myControlTypeKey = "CheckBoxesFloatControl"
                                End Select
                            End If
                        End If
                End Select
            End If
        End If

        Dim myreturn As DataTemplate = Nothing
        Try
            Dim PPath As New PropertyPath(MDNamespace + myControlTypeKey + "," + ThisAssemblyName)
            Dim myControlType As Type = Type.GetType(PPath.Path, True, True)
            Dim factory As New FrameworkElementFactory(myControlType)
            myreturn = New DataTemplate With {.VisualTree = factory}
        Catch ex As Exception
            Debug.Print("MDTemplateSelector failed: " + ex.Message)
        End Try

        If myreturn Is Nothing Then
            Try
                Dim PPath As New PropertyPath(MDNamespace + "AnyShowControl," + ThisAssemblyName)
                Dim myControlType As Type = Type.GetType(PPath.Path, True, True)
                Dim factory As New FrameworkElementFactory(myControlType)
                myreturn = New DataTemplate With {.VisualTree = factory}
            Catch ex As Exception
                Debug.Print("MDTemplateSelector FallBack failed: " + ex.Message)
            End Try
        End If

        If myreturn IsNot Nothing Then
            'factory.SetBinding(SinglePropertyControl.XPropProperty, New Binding With {.Source = item})
            'If PropContainer IsNot Nothing Then
            '    factory.SetBinding(SinglePropertyControl.MetadataCatalogProperty, New Binding With {.Source = PropContainer.MetadataCatalog, .Mode = BindingMode.OneWay})
            '    factory.SetBinding(SinglePropertyControl.MainLanguageProperty, New Binding With {.Source = PropContainer.MetadataCatalog.MainLanguage, .Mode = BindingMode.OneWay})
            '    factory.SetBinding(SinglePropertyControl.OptionalLanguagesProperty, New Binding With {.Source = PropContainer.MetadataCatalog.OptionalLanguages, .Mode = BindingMode.OneWay})
            'End If
        End If

        Return myreturn
    End Function
End Class
