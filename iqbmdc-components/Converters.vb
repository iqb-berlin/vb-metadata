Imports iqb.mdc.xml
Public Class MDConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim myreturn As New List(Of MDObject)
        If values.Count > 1 AndAlso TypeOf values(0) Is IEnumerable(Of XElement) Then
            For Each XMD As XElement In CType(values(0), IEnumerable(Of XElement))
                myreturn.Add(MDCFactory.GetMDObject(XMD))
            Next
        End If
        Return myreturn
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("MDConverter ConvertBack")
    End Function
End Class