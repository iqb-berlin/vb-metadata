Imports iqb.mdc.xml
Public Class MDConverter
    Implements IMultiValueConverter

    Public Function Convert(values() As Object, targetType As Type, parameter As Object, culture As Globalization.CultureInfo) As Object Implements IMultiValueConverter.Convert
        Dim myreturn As New List(Of MDObject)
        If values.Count > 1 AndAlso TypeOf values(0) Is XElement Then
            Dim XMDList As XElement = values(0)
            Dim XMD2Add As Dictionary(Of String, XElement) =
                (From xe As XElement In XMDList.Elements).ToDictionary(Function(xe) xe.@cat + "##" + xe.@def, Function(xe) xe)

            Dim StandardXMD2Add As New Dictionary(Of String, XElement)
            If values(1) IsNot Nothing AndAlso TypeOf values(1) Is List(Of XElement) Then
                StandardXMD2Add = (From xe As XElement In CType(values(1), List(Of XElement))).ToDictionary(Function(xe) xe.@cat + "##" + xe.@def, Function(xe) xe)
            End If
            For Each XMD As KeyValuePair(Of String, XElement) In StandardXMD2Add
                If XMD2Add.ContainsKey(XMD.Key) Then
                    myreturn.Add(MDCFactory.GetMDObject(XMD2Add.Item(XMD.Key)))
                    XMD2Add.Remove(XMD.Key)
                Else
                    Dim newXMD As New XElement(XMD.Value)
                    XMDList.Add(newXMD)
                    myreturn.Add(MDCFactory.GetMDObject(newXMD))
                End If
            Next
            For Each XMD As KeyValuePair(Of String, XElement) In XMD2Add
                myreturn.Add(MDCFactory.GetMDObject(XMD.Value))
            Next
        End If
        Return myreturn
    End Function

    Public Function ConvertBack(value As Object, targetTypes() As Type, parameter As Object, culture As Globalization.CultureInfo) As Object() Implements IMultiValueConverter.ConvertBack
        Throw New NotImplementedException("MDConverter ConvertBack")
    End Function

End Class
