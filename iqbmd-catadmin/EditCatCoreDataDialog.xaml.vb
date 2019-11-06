Public Class EditCatCoreDataDialog
    Private _XCat As XDocument = Nothing

    Public Sub New(XCat As XDocument)
        InitializeComponent()
        _XCat = XCat
    End Sub

    Private Sub Me_Loaded() Handles Me.Loaded
        If _XCat Is Nothing Then
            Me.StPRoot.IsEnabled = False
            Me.BtnOK.IsEnabled = False
        Else
            TBID.Text = _XCat.Root.@id
            Dim xe As XElement = _XCat.Root.<License>.FirstOrDefault
            If xe IsNot Nothing Then TBLicense.Text = xe.Value
            TBVersion.Text = _XCat.Root.@version
            TBVersionHistory.Text = _XCat.Root.@versionhistory
            Dim XLabel As XElement = <n></n>
            XLabel.Add(_XCat.Root.<Label>)
            TMLCName.XMD = New XElement(XLabel)
            Dim XDescr As XElement = <d></d>
            XDescr.Add(_XCat.Root.<Description>)
            TMLCDescr.XMD = New XElement(XDescr)
            Dim XOwner As XElement = <o></o>
            XOwner.Add(_XCat.Root.<Owner>)
            TMLCOwner.XMD = New XElement(XOwner)
        End If

    End Sub

    Private Sub BtnOK_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnOK.Click
        _XCat.Root.@id = TBID.Text
        _XCat.Root.@version = TBVersion.Text
        _XCat.Root.@versionhistory = TBVersionHistory.Text

        Dim XList As List(Of XElement) = _XCat.Root.<License>.ToList
        XList.AddRange(_XCat.Root.<Owner>.ToList)
        XList.AddRange(_XCat.Root.<Label>.ToList)
        XList.AddRange(_XCat.Root.<Description>.ToList)
        For Each xe As XElement In XList
            xe.Remove()
        Next

        'Reihenfolge beachten!
        _XCat.Root.AddFirst(<License><%= TBLicense.Text %></License>)
        _XCat.Root.AddFirst(From xe As XElement In TMLCOwner.XMD.Elements)
        _XCat.Root.AddFirst(From xe As XElement In TMLCDescr.XMD.Elements)
        _XCat.Root.AddFirst(From xe As XElement In TMLCName.XMD.Elements)

        Me.DialogResult = True
    End Sub

    Private Sub BtnCancel_Clicked(sender As Object, e As RoutedEventArgs) Handles BtnCancel.Click
        Me.DialogResult = False
    End Sub

End Class
