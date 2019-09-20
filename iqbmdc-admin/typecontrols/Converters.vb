Public Class XMDDefTypeSpecConverter
    Implements IValueConverter

    Public Function Convert(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.Convert
        If value Is Nothing OrElse TypeOf value IsNot XElement Then
            Return Nothing
        Else
            Dim XMDef As XElement = value
            Dim XTypeSpec As XElement = (From xe As XElement In XMDef.Elements("TypeSpec")).FirstOrDefault
            If XTypeSpec Is Nothing Then
                XTypeSpec = <TypeSpec/>
                XMDef.Add(XTypeSpec)
            End If
            Dim TagName As String = parameter
            Dim myXElement As XElement = (From xe As XElement In XTypeSpec.Elements(TagName)).FirstOrDefault
            If myXElement Is Nothing Then
                myXElement = New XElement(TagName)
                XTypeSpec.Add(myXElement)
            End If

            Return myXElement
        End If
    End Function

    Public Function ConvertBack(ByVal value As Object, ByVal targetType As System.Type, ByVal parameter As Object, ByVal culture As Globalization.CultureInfo) As Object Implements System.Windows.Data.IValueConverter.ConvertBack
        Throw New NotImplementedException("ConvertBack in XMDDefTypeSpecConverter")
    End Function
End Class
