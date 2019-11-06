Imports iqb.md.xml
Public Class EditMDListDialog

    Private _XMDList As XElement = Nothing
    Private _ShadowXMDList As XElement = Nothing
    Private _XDefaultMDList As XElement = Nothing
    Private MDhasChanged As Boolean = False
    Private _prompt As String = Nothing

    Public Sub New(ByRef XMDList As XElement, ByRef XDefaultMDList As XElement, Optional prompt As String = Nothing)
        InitializeComponent()
        If XMDList IsNot Nothing Then
            _XMDList = XMDList
            _ShadowXMDList = New XElement(XMDList)
        End If
        If XDefaultMDList IsNot Nothing Then
            _XDefaultMDList = XDefaultMDList
        End If
        _prompt = prompt
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        Me.MDLC.XMDList = _ShadowXMDList
        Me.MDLC.XDefaultMDList = _XDefaultMDList
        Me.AddHandler(MDListControl.MDChangedEvent, New RoutedEventHandler(AddressOf MDChangedEventHandler))
        If Not String.IsNullOrEmpty(_prompt) Then TBPrompt.Text = _prompt
    End Sub
    Private Sub Me_Unloaded(ByVal sender As Object, ByVal e As RoutedEventArgs) Handles Me.Unloaded
        Me.RemoveHandler(MDListControl.MDChangedEvent, New RoutedEventHandler(AddressOf MDChangedEventHandler))
    End Sub

    Private Sub MDChangedEventHandler(sender As Object, e As RoutedEventArgs)
        MDhasChanged = True
    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        If MDhasChanged Then
            _XMDList.RemoveAll()
            _XMDList.Add(From xe As XElement In _ShadowXMDList.Elements)
            Me.DialogResult = True
        Else
            Me.DialogResult = False
        End If
    End Sub
    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnCancel.Click
        Me.DialogResult = False
    End Sub

End Class
