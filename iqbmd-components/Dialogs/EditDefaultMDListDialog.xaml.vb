Imports iqb.md.xml
Public Class EditDefaultMDListDialog

    Public XDefaultMDList As XElement
    Public MDCatList As IEnumerable(Of String)
    Public MDFilters As List(Of MDFilter)

    Private myCatMDList As List(Of MDInfo)
    Private Sub Me_Loaded() Handles Me.Loaded
        'Dim myDefaultMDList As New List(Of MDInfo)
        If XDefaultMDList Is Nothing Then
            BtnOK.IsEnabled = False
        Else
            For Each xe As XElement In XDefaultMDList.Elements
                LBDefaultMDList.Items.Add(MDCFactory.GetMDInfo(xe.@cat, xe.@def))
            Next
        End If


        myCatMDList = MDCFactory.GetMDList(MDCatList, MDFilters)
        'Dim cv As CollectionView = CollectionViewSource.GetDefaultView(LBCatMDDefList.ItemsSource)
        'If cv IsNot Nothing Then
        '    cv.GroupDescriptions.Add(New PropertyGroupDescription("CatLabel"))
        '    cv.SortDescriptions.Add(New ComponentModel.SortDescription("Label", ComponentModel.ListSortDirection.Ascending))
        '    LBCatMDDefList.GroupStyle.Add(New GroupStyle() With {.ContainerStyle = Me.FindResource("gsGroupStyleStandard")})
        'End If
        UpdateCatMDList()
    End Sub

    Private Sub UpdateCatMDList()
        Dim habschon As List(Of String) = (From mdi As MDInfo In LBDefaultMDList.Items Let key As String = mdi.CatId + "##" + mdi.id Select key).ToList
        DPMain.DataContext = (From mdi As MDInfo In myCatMDList
                              Let key As String = mdi.CatId + "##" + mdi.id
                              Where Not habschon.Contains(key)
                              Select mdi).ToList
    End Sub
    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        XDefaultMDList.RemoveNodes()
        For Each mbi As MDInfo In LBDefaultMDList.Items
            XDefaultMDList.Add(<MD cat=<%= mbi.CatId %> def=<%= mbi.id %>/>)
        Next
        Me.DialogResult = True
    End Sub
    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnCancel.Click
        Me.DialogResult = False
    End Sub

    '###############################################################################################
    Private Sub BtnAdd_Clicked(sender As Object, e As RoutedEventArgs)
        If LBCatMDDefList.SelectedItems.Count > 0 Then
            Dim myMDI2Add As MDInfo = LBCatMDDefList.SelectedItem
            LBDefaultMDList.Items.Add(myMDI2Add)
            UpdateCatMDList()
        End If
    End Sub

    Private Sub BtnUp_Clicked(sender As Object, e As RoutedEventArgs)
        If LBDefaultMDList.SelectedItems.Count > 0 Then
            Dim myPos As Integer = LBDefaultMDList.Items.CurrentPosition
            Dim myMDI2Add As MDInfo = LBDefaultMDList.SelectedItem
            If myPos > 0 Then
                LBDefaultMDList.Items.RemoveAt(myPos)
                LBDefaultMDList.Items.Insert(myPos - 1, myMDI2Add)
                LBDefaultMDList.SelectedItem = myMDI2Add
            End If
        End If
    End Sub

    Private Sub BtnDelete_Clicked(sender As Object, e As RoutedEventArgs)
        If LBDefaultMDList.SelectedItems.Count > 0 Then
            LBDefaultMDList.Items.Remove(LBDefaultMDList.SelectedItem)
            UpdateCatMDList()
        End If
    End Sub

    Private Sub BtnDown_Clicked(sender As Object, e As RoutedEventArgs)
        If LBDefaultMDList.SelectedItems.Count > 0 Then
            Dim myPos As Integer = LBDefaultMDList.Items.CurrentPosition
            Dim myMDI2Add As MDInfo = LBDefaultMDList.SelectedItem
            If myPos < LBDefaultMDList.Items.Count - 1 Then
                LBDefaultMDList.Items.RemoveAt(myPos)
                LBDefaultMDList.Items.Insert(myPos + 1, myMDI2Add)
                LBDefaultMDList.SelectedItem = myMDI2Add
            End If
        End If
    End Sub
End Class
