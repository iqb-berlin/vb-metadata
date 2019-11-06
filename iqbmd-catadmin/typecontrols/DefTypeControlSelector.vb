Imports System.ComponentModel

'wird im EditMetadataCatUserControl verwendet, um das Control zum Ändern der PropValues zu wählen
Public Class DefTypeControlSelector
    Inherits DataTemplateSelector

    Private Shared ThisAssemblyName As String = System.Reflection.Assembly.GetAssembly(GetType(DefTypeControlSelector)).GetName.Name

    Public Shared Function GetControl(UserControlName As String) As Type
        Try
            Dim PPath As New PropertyPath("iqb.md.catadmin." + UserControlName + "," + DefTypeControlSelector.ThisAssemblyName)
            Return Type.GetType(PPath.Path, True, True)
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Public Overrides Function SelectTemplate(ByVal item As Object, ByVal container As System.Windows.DependencyObject) As System.Windows.DataTemplate
        Dim myControlType As Type = Nothing
        If item IsNot Nothing AndAlso TypeOf (item) Is String Then
            Select Case item
                Case "integer" : myControlType = DefTypeControlSelector.GetControl("IntegerDefControl")
                Case "decimal" : myControlType = DefTypeControlSelector.GetControl("DecimalDefControl")
                Case "listsingleselect",
                     "listmultiselect" : myControlType = DefTypeControlSelector.GetControl("ListDefControl")
                Case "boolean",
                     "folderlink",
                     "filelink",
                     "folderlink",
                     "textde",
                     "date",
                     "textmultilang" : myControlType = DefTypeControlSelector.GetControl("NoSpecDefControl")
                Case Else
                    myControlType = DefTypeControlSelector.GetControl("UnknownTypeDefControl")
            End Select
        End If
        If myControlType Is Nothing Then
            Return Nothing
        Else
            Return New DataTemplate With {.VisualTree = New FrameworkElementFactory(myControlType)}
        End If
    End Function
End Class
