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
            Dim XDefaultMDDefMetadata As XElement = _XCat.Root.<DefaultMDDefMetadata>.FirstOrDefault
            If XDefaultMDDefMetadata IsNot Nothing Then
                For Each xmd As XElement In XDefaultMDDefMetadata.Elements
                    If xmd.@cat = "DOI:10.5159/IQB_MDR_Core_v1" AndAlso xmd.@def = "2" Then
                        ChBMDCoreScope.IsChecked = True
                    ElseIf xmd.@cat = "DOI:10.5159/IQB_MDR_Core_v1" AndAlso xmd.@def = "1" Then
                        ChBMDCoreSubject.IsChecked = True
                    End If
                Next
            End If
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
        XList.AddRange(_XCat.Root.<DefaultMDDefMetadata>.ToList)
        For Each xe As XElement In XList
            xe.Remove()
        Next

        'Reihenfolge beachten (s. XSD): Letztes zuerst
        Dim XDefaultMDDefMetadata As XElement = <DefaultMDDefMetadata/>
        If ChBMDCoreScope.IsChecked Then XDefaultMDDefMetadata.Add(<MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="2"></MD>)
        If ChBMDCoreSubject.IsChecked Then XDefaultMDDefMetadata.Add(<MD cat="DOI:10.5159/IQB_MDR_Core_v1" def="1"></MD>)
        _XCat.Root.AddFirst(XDefaultMDDefMetadata)
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
