Public Class TextMultiLanguageEditControl
    Inherits MDBasisControl

    Public Shared ReadOnly MultilineProperty As DependencyProperty = DependencyProperty.Register("Multiline", GetType(Boolean), GetType(TextMultiLanguageEditControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property Multiline As Boolean
        Get
            If Me.DataContext IsNot Nothing AndAlso TypeOf Me.DataContext Is iqb.mdc.xml.MDObject Then
                Dim typespecDict As Dictionary(Of String, String) = CType(Me.DataContext, iqb.mdc.xml.MDObject).TypeSpec
                Return typespecDict.ContainsKey("MultiLineText") AndAlso typespecDict.Item("MultiLineText").ToUpper = "True"
            Else
                Return False
            End If
        End Get
        Set(ByVal value As Boolean)
            Debug.Print("SetValue(MultilineProperty, " + value.ToString + ")")
        End Set
    End Property
End Class
