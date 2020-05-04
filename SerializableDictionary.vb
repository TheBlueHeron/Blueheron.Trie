Imports System.Xml
Imports System.Xml.Serialization

''' <summary>
''' Serializable <see cref="Dictionary(Of TKey, TValue)">Dictionary</see>.
''' </summary>
''' <typeparam name="TKey">The type of the value that will serve as key</typeparam>
''' <typeparam name="TValue">The type of the value that will serve as value</typeparam>
''' <remarks>Adapted from: http://stackoverflow.com/questions/12554186/how-to-serialize-deserialize-to-dictionaryint-string-from-custom-xml-not-us</remarks>
<XmlRoot("dict")>
Public Class SerializableDictionary(Of TKey, TValue)
	Inherits Dictionary(Of TKey, TValue)
	Implements IXmlSerializable

#Region " Objects and variables "

	Private Const _item As String = "i"
	Private Const _key As String = "k"
	Private Const _value As String = "v"

#End Region

#Region " IXmlSerializable Members "

	Public Function GetSchema() As Schema.XmlSchema Implements IXmlSerializable.GetSchema

		Return Nothing

	End Function

	Public Sub ReadXml(reader As XmlReader) Implements IXmlSerializable.ReadXml
		Dim keySerializer As New XmlSerializer(GetType(TKey))
		Dim valueSerializer As New XmlSerializer(GetType(TValue))
		Dim wasEmpty As Boolean = reader.IsEmptyElement

		reader.Read()

		If wasEmpty Then
			Return
		End If

		Do While Not reader.NodeType = System.Xml.XmlNodeType.EndElement
			reader.ReadStartElement(_item)
			reader.ReadStartElement(_key)
			Dim key As TKey = CType(keySerializer.Deserialize(reader), TKey)
			reader.ReadEndElement()
			reader.ReadStartElement(_value)
			Dim value As TValue = CType(valueSerializer.Deserialize(reader), TValue)
			reader.ReadEndElement()
			Add(key, value)
			reader.ReadEndElement()
			reader.MoveToContent()
		Loop
		reader.ReadEndElement()

	End Sub

	Public Sub WriteXml(writer As XmlWriter) Implements IXmlSerializable.WriteXml
		Dim keySerializer As New XmlSerializer(GetType(TKey))
		Dim valueSerializer As New XmlSerializer(GetType(TValue))

		For Each key As TKey In Keys
			Dim value As TValue = Me(key)

			writer.WriteStartElement(_item)
			writer.WriteStartElement(_key)
			keySerializer.Serialize(writer, key)
			writer.WriteEndElement()
			writer.WriteStartElement(_value)
			valueSerializer.Serialize(writer, value)
			writer.WriteEndElement()
			writer.WriteEndElement()
		Next

	End Sub

#End Region

End Class