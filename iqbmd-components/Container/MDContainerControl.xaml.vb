Imports iqb.lib.components
Imports iqb.md.xml

Public Class MDContainerControl
    Inherits UserControl

    Public Shared ReadOnly AddMD As RoutedUICommand = New RoutedUICommand("Neue Eigenschaft", "AddMD", GetType(MDContainerControl))
    Public Shared ReadOnly RemoveMD As RoutedUICommand = New RoutedUICommand("Eigenschaft entfernen", "RemoveMD", GetType(MDContainerControl))
    Public Shared ReadOnly EditDefault As RoutedUICommand = New RoutedUICommand("Standard-Eigenschaften ändern", "EditDefault", GetType(MDContainerControl))

    Public Shared ReadOnly XMDListProperty As DependencyProperty = DependencyProperty.Register("XMDList", GetType(XElement), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property XMDList As XElement
        Get
            Return GetValue(XMDListProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XMDListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly XDefaultMDListProperty As DependencyProperty = DependencyProperty.Register("XDefaultMDList", GetType(XElement), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property XDefaultMDList As XElement
        Get
            Return GetValue(XDefaultMDListProperty)
        End Get
        Set(ByVal value As XElement)
            SetValue(XDefaultMDListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly IsReadOnlyProperty As DependencyProperty = DependencyProperty.Register("IsReadOnly", GetType(Boolean), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property IsReadOnly As Boolean
        Get
            Return GetValue(IsReadOnlyProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(IsReadOnlyProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MDCatListProperty As DependencyProperty = DependencyProperty.Register("MDCatList", GetType(IEnumerable(Of String)), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property MDCatList As IEnumerable(Of String)
        Get
            Return GetValue(MDCatListProperty)
        End Get
        Set(ByVal value As IEnumerable(Of String))
            SetValue(MDCatListProperty, value)
        End Set
    End Property

    Public Shared ReadOnly MDFilterProperty As DependencyProperty = DependencyProperty.Register("MDFilter", GetType(MDFilter), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property MDFilter As MDFilter
        Get
            Return GetValue(MDFilterProperty)
        End Get
        Set(ByVal value As MDFilter)
            SetValue(MDFilterProperty, value)
        End Set
    End Property

    Public Shared ReadOnly CanDefaultEditProperty As DependencyProperty = DependencyProperty.Register("CanDefaultEdit", GetType(Boolean), GetType(MDContainerControl), New FrameworkPropertyMetadata() With {.BindsTwoWayByDefault = False})
    Public Property CanDefaultEdit As Boolean
        Get
            Return GetValue(CanDefaultEditProperty)
        End Get
        Set(ByVal value As Boolean)
            SetValue(CanDefaultEditProperty, value)
        End Set
    End Property
    '#################################################################################################
    Public Sub Me_Loaded() Handles Me.Loaded
        CommandBindings.Add(New CommandBinding(MDContainerControl.AddMD, AddressOf HandleAddMDExecuted, AddressOf HandleChangeMDCanExecute))
        CommandBindings.Add(New CommandBinding(MDContainerControl.RemoveMD, AddressOf HandleRemoveMDExecuted, AddressOf HandleChangeMDCanExecute))
        CommandBindings.Add(New CommandBinding(MDContainerControl.EditDefault, AddressOf HandleEditDefaultExecuted, AddressOf HandleChangeMDCanExecute))
    End Sub

    Private Function HandleChangeMDCanExecute(sender As System.Object, e As System.Windows.Input.CanExecuteRoutedEventArgs)
        e.CanExecute = Not IsReadOnly
        Return e.CanExecute
    End Function

    Private Sub HandleAddMDExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        If XMDList Is Nothing Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neue Eigenschaft", "Datenfehler: Datenobjekt leer.")
        ElseIf MDCatList Is Nothing OrElse MDCatList.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neue Eigenschaft", "Es sind keine zulässigen Kataloge zugewiesen.")
        Else
            Dim HabSchonMDList As List(Of String) = (From xs As XElement In XMDList.Elements Select xs.@cat + "##" + xs.@def).ToList
            Dim AvailableMDList As List(Of XElement) = (From MDI As MDInfo In MDCFactory.GetMDList(MDCatList, MDFilter)
                                                        Let MDKey As String = MDI.CatId + "##" + MDI.id,
                                                            XMD As XElement = <p key=<%= MDKey %> cat=<%= MDI.CatLabel %>><%= MDI.Label %></p>
                                                        Where Not HabSchonMDList.Contains(MDKey)
                                                        Order By MDI.Label
                                                        Select XMD).ToList
            If AvailableMDList.Count = 0 Then
                DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Neue Eigenschaft", "Keine weitere Eigenschaft verfügbar.")
            Else
                Dim myDlg As New XSelectionDialog With {.Owner = DialogFactory.GetParentWindow(Me),
                    .XSelectionList = AvailableMDList, .GroupAttributePath = "cat", .MultipleSelection = True,
                    .Title = "Neue Eigenschaft"}
                If myDlg.ShowDialog Then
                    For Each md As String In myDlg.Selected.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                        Dim mdSplits As String() = md.Split({"##"}, StringSplitOptions.RemoveEmptyEntries)
                        XMDList.Add(<MD cat=<%= mdSplits(0) %> def=<%= mdSplits(1) %>/>)
                    Next

                    MDLC.Update()
                End If
            End If
        End If
    End Sub

    Private Sub HandleRemoveMDExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        If XMDList Is Nothing OrElse XMDList.Elements.Count = 0 Then
            DialogFactory.MsgError(DialogFactory.GetParentWindow(Me), "Eigenschaft(en) löschen", "Keine Eigenschaft verfügbar.")
        Else
            Dim XSelectionList As New List(Of XElement)
            For Each xp As XElement In XMDList.Elements
                XSelectionList.Add(<p key=<%= xp.@cat + "##" + xp.@def %> cat=<%= MDCFactory.GetMDCatLabel(xp.@cat) %>><%= MDCFactory.GetMDLabel(xp.@cat, xp.@def) %></p>)
            Next

            Dim SelectedID As String = Nothing
            If XSelectionList.Count = 1 Then
                If DialogFactory.YesNoCancel(Me, "Löschen von Eigenschaften", "Möchten Sie die Eigenschaft '" + XSelectionList.First.Value + "' löschen?") = MessageBoxResult.OK Then _
                        SelectedID = XSelectionList.First.@key
            Else
                Dim myDlg As New XSelectionDialog With {.Owner = DialogFactory.GetParentWindow(Me),
                    .XSelectionList = XSelectionList, .GroupAttributePath = "cat", .MultipleSelection = True,
                    .Title = "Löschen von Eigenschaften"}
                If myDlg.ShowDialog Then SelectedID = myDlg.Selected
            End If
            If Not String.IsNullOrEmpty(SelectedID) Then
                For Each md As String In SelectedID.Split({" "}, StringSplitOptions.RemoveEmptyEntries)
                    Dim mdSplits As String() = md.Split({"##"}, StringSplitOptions.RemoveEmptyEntries)
                    Dim XToDelete As XElement = (From xp As XElement In XMDList.Elements Where xp.@cat = mdSplits(0) AndAlso xp.@def = mdSplits(1)).FirstOrDefault
                    If XToDelete IsNot Nothing Then XToDelete.Remove()
                Next
                MDLC.Update()
                Me.RaiseEvent(New RoutedEventArgs(MDListControl.MDChangedEvent))
            End If
        End If
    End Sub

    Private Sub HandleEditDefaultExecuted(sender As Object, e As ExecutedRoutedEventArgs)
        Dim myDlg As New EditDefaultMDListDialog With {.Owner = iqb.lib.components.DialogFactory.GetParentWindow(Me),
            .XDefaultMDList = Me.XDefaultMDList, .MDCatList = Me.MDCatList, .MDFilter = Me.MDFilter}
        If myDlg.ShowDialog Then MDLC.Update()
    End Sub
End Class
