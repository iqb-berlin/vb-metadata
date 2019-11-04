Public Class MDInfo
    Private _Label As String
    Public Property Label() As String
        Get
            Return _Label
        End Get
        Set(ByVal value As String)
            _Label = value
        End Set
    End Property
    Private _Description As String
    Public Property Description() As String
        Get
            Return _Description
        End Get
        Set(ByVal value As String)
            _Description = value
        End Set
    End Property
    Private _CatId As String
    Public Property CatId() As String
        Get
            Return _CatId
        End Get
        Set(ByVal value As String)
            _CatId = value
        End Set
    End Property
    Private _CatLabel As String
    Public Property CatLabel() As String
        Get
            Return _CatLabel
        End Get
        Set(ByVal value As String)
            _CatLabel = value
        End Set
    End Property
    Private _id As String
    Public Property id() As String
        Get
            Return _id
        End Get
        Set(ByVal value As String)
            _id = value
        End Set
    End Property
    Private _typeLabel As String
    Public Property typeLabel() As String
        Get
            Return _typeLabel
        End Get
        Set(ByVal value As String)
            _typeLabel = value
        End Set
    End Property
End Class
