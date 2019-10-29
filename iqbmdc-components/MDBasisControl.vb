Imports iqb.mdc.xml

Public Class MDBasisControl
    Inherits UserControl

    Public Shared ReadOnly LanguagesProperty As DependencyProperty =
        DependencyProperty.Register("Languages", GetType(String), GetType(MDBasisControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property Languages As String
        Get
            Return GetValue(LanguagesProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LanguagesProperty, value)
        End Set
    End Property
End Class