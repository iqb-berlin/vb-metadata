Imports iqb.mdc.xml

Public MustInherit Class MDListControl
    Inherits UserControl

    Public Shared ReadOnly MDListProperty As DependencyProperty = DependencyProperty.Register("MDList", GetType(IEnumerable(Of XElement)), GetType(MDListControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property MDList As IEnumerable(Of XElement)
        Get
            Return GetValue(MDListProperty)
        End Get
        Set(ByVal value As IEnumerable(Of XElement))
            SetValue(MDListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MDDefaultListProperty As DependencyProperty = DependencyProperty.Register("MDDefaultList", GetType(IEnumerable(Of XElement)), GetType(MDListControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property MDDefaultList As IEnumerable(Of XElement)
        Get
            Return GetValue(MDDefaultListProperty)
        End Get
        Set(ByVal value As IEnumerable(Of XElement))
            SetValue(MDDefaultListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register("IsReadOnly", GetType(Boolean), GetType(MDListControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property IsReadOnly As Boolean
        Get
            Return GetValue(IsReadOnlyProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property
End Class
