Public Class MDFilter
    Inherits Dictionary(Of String, Dictionary(Of String, List(Of String)))

    Public Sub New(XFilter As XElement)
        MyBase.New
        If XFilter IsNot Nothing Then
            For Each xf As XElement In XFilter.Elements
                If Not String.IsNullOrEmpty(xf.Value) Then
                    If Not Me.ContainsKey(xf.@cat) Then Me.Add(xf.@cat, New Dictionary(Of String, List(Of String)))
                    Dim myDefs As Dictionary(Of String, List(Of String)) = Me.Item(xf.@cat)
                    If Not myDefs.ContainsKey(xf.@def) Then myDefs.Add(xf.@def, xf.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList)
                End If
            Next
        End If
    End Sub

    Public Function IsMatch(XMD As XElement) As Boolean
        If Me.Count = 0 Then
            Return True
        Else
            If XMD Is Nothing OrElse Not XMD.HasElements Then
                Return False
            Else
                For Each f In
                        From fcatkv As KeyValuePair(Of String, Dictionary(Of String, List(Of String))) In Me
                        From fdefkv As KeyValuePair(Of String, List(Of String)) In fcatkv.Value
                        Let fcat As String = fcatkv.Key, fdef As String = fdefkv.Key, fvalues As List(Of String) = fdefkv.Value
                        Select fcat, fdef, fvalues
                    Dim myMData As XElement = (From xe As XElement In XMD.Elements Where f.fcat = xe.@cat AndAlso f.fdef = xe.@def).FirstOrDefault
                    If myMData Is Nothing Then
                        Return False
                    Else
                        Dim ValueListMD As List(Of String) = myMData.Value.Split({" "}, StringSplitOptions.RemoveEmptyEntries).ToList
                        Dim ValueFound As Boolean = False
                        For Each FS As String In f.fvalues
                            If ValueListMD.Contains(FS) Then
                                ValueFound = True
                                Exit For
                            End If
                        Next
                        If Not ValueFound Then Return False

                    End If
                Next
                Return True
            End If
        End If
    End Function

    Public Function ToXML() As XElement
        Dim myreturn As XElement = <FList></FList>
        myreturn.Add(From fcatkv As KeyValuePair(Of String, Dictionary(Of String, List(Of String))) In Me
                     From fdefkv As KeyValuePair(Of String, List(Of String)) In fcatkv.Value
                     Let xf As XElement = <F cat=<%= fcatkv.Key %> def=<%= fdefkv.Key %>><%= String.Join(" ", fdefkv.Value) %></F>
                     Select xf)
        Return myreturn
    End Function
End Class
