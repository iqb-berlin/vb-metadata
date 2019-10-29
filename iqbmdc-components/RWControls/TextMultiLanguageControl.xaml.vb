Public Class TextMultiLanguageControl
    Inherits UserControl

    Public Shared ReadOnly XMDProperty As DependencyProperty = DependencyProperty.Register("XMD", GetType(XElement), GetType(TextMultiLanguageControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property XMD As XElement
        Get
            Return GetValue(XMDProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XMDProperty, value)
        End Set
    End Property


    Public Shared ReadOnly LanguagesProperty As DependencyProperty = DependencyProperty.Register("Languages", GetType(String), GetType(TextMultiLanguageControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property Languages As String
        Get
            Return GetValue(LanguagesProperty)
        End Get
        Set(ByVal value As String)
            SetValue(LanguagesProperty, value)
        End Set
    End Property

    Public Shared ReadOnly TagFilterProperty As DependencyProperty = DependencyProperty.Register("TagFilter", GetType(String), GetType(TextMultiLanguageControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property TagFilter As String
        Get
            Return GetValue(TagFilterProperty)
        End Get
        Set(ByVal value As String)
            SetValue(TagFilterProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MultilineProperty As DependencyProperty = DependencyProperty.Register("Multiline", GetType(Boolean), GetType(TextMultiLanguageControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property Multiline As Boolean
        Get
            Return GetValue(MultilineProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(MultilineProperty, value)
        End Set
    End Property

    'Public Shared ReadOnly CanChangeProperty As DependencyProperty = DependencyProperty.Register("CanChange", GetType(Boolean), GetType(TextMultiLanguageControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    'Public Property CanChange As Boolean
    '    Get
    '        Return GetValue(CanChangeProperty)
    '    End Get
    '    Set(ByVal value As Boolean)
    '        SetValue(CanChangeProperty, value)
    '    End Set
    'End Property
End Class
