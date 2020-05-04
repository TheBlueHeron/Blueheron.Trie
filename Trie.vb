Imports System.Xml.Serialization

''' <summary>
''' A <see cref="Trie">Trie</see> is a tree like data structure that can be used for fast text search.
''' </summary>
''' <remarks>Adapted from: http://www.kerrywong.com/2006/04/01/implementing-a-trie-in-c/ </remarks>
Public Class Trie '<Serializable(), XmlRoot("trie")>

#Region " Objects and variables "

	Private m_Root As New TrieNode

#End Region

#Region " Properties "

	'<XmlElement(elementName:="r")>
	Public Property RootNode As TrieNode
		Get
			Return m_Root
		End Get
		Set(value As TrieNode)
			m_Root = value
		End Set
	End Property

#End Region

#Region " Public methods and functions "

	Friend Function Contains(s As String, ByRef resultNode As TrieNode) As Boolean
		Dim charArray As Char() = s.ToCharArray()
		Dim node As TrieNode = m_Root

		For Each c As Char In charArray
			node = Contains(c, node)

			If node Is Nothing Then
				Return False
			End If
		Next
		resultNode = node

		Return node.EndsWord

	End Function

	Friend Function Insert(s As String, id As Integer) As TrieNode
		Dim charArray As Char() = s.ToCharArray
		Dim node As TrieNode = m_Root

		For Each c As Char In charArray
			node = Insert(c, node)
		Next
		' not needed when using hashSet:  If Not node.WordIDs.Contains(id)
		node.WordIDs.Add(id)
		node.EndsWord = True

		Return m_Root

	End Function

#End Region

#Region " Private methods and functions "

	Private Function Contains(c As Char, node As TrieNode) As TrieNode
		Dim id As Integer = Asc(c)

		If node.Contains(id) Then
			Return node.GetChild(id)
		Else
			Return Nothing
		End If

	End Function

	Private Function Insert(c As Char, node As TrieNode) As TrieNode
		Dim id As Integer = Asc(c)

		If node.Contains(id) Then
			Return node.GetChild(id)
		Else
			Dim t As New TrieNode

			node.Nodes.Add(id, t)
			Return t
		End If

	End Function

#End Region

End Class