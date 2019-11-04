Imports iqb.md.xml

Public Class MDListControl
    Inherits UserControl
    Public Shared ReadOnly MDChangedEvent As RoutedEvent =
        EventManager.RegisterRoutedEvent("MDChanged", RoutingStrategy.Bubble, GetType(RoutedEventHandler), GetType(MDListControl))

    Public Shared ReadOnly XMDListProperty As DependencyProperty = DependencyProperty.Register("XMDList", GetType(XElement), GetType(MDListControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property XMDList As XElement
        Get
            Return GetValue(XMDListProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XMDListProperty, value)
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

    Public Sub Update()
        Dim myCVS As CollectionViewSource = Me.FindResource("CVSMDList")
        If myCVS IsNot Nothing Then
            Dim be As System.Windows.Data.MultiBindingExpression = BindingOperations.GetMultiBindingExpression(myCVS, CollectionViewSource.SourceProperty)
            If be IsNot Nothing Then be.UpdateTarget()
        End If
    End Sub
End Class
