Imports iqb.mdc.xml

Public Class MDContainerBasisControl
    Inherits UserControl

    Public Shared ReadOnly MDProperty As DependencyProperty = DependencyProperty.Register("MD", GetType(MDObject), GetType(MDBasisControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property MD As MDObject
        Get
            Return GetValue(MDProperty)
        End Get
        Set(ByVal value As MDObject)
            SetValue(MDProperty, value)
        End Set
    End Property


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