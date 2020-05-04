Imports System.Xml.Serialization

''' <summary>
''' A node in the <see cref="Trie">Trie</see> representing a character with an array of child characters.
''' </summary>
Public Class TrieNode '<Serializable(), XmlRoot(elementName:="tn")>

#Region " Objects and variables "

	Private Const fmt_ToString As String = "Nodes: [{0}] - IDs: [{1}] (Ends word: {2})"
	Private Const PIPE As String = "|"

	Private m_IDs As HashSet(Of Integer)
	Private m_Nodes As Dictionary(Of Integer, TrieNode)

#End Region

#Region " Properties "

	'<XmlAttribute(attributeName:="e")>
	Public Property EndsWord As Boolean = False

	'<XmlElement(elementName:="ids")>
	Public Property WordIDs As HashSet(Of Integer)
		Get
			Return m_IDs
		End Get
		Set(value As HashSet(Of Integer))
			m_IDs = value
		End Set
	End Property

	'<XmlElement(elementName:="nodes")>
	Public Property Nodes As Dictionary(Of Integer, TrieNode)
		Get
			Return m_Nodes
		End Get
		Set(value As Dictionary(Of Integer, TrieNode))
			m_Nodes = value
		End Set
	End Property

#End Region

#Region " Public methods and functions "

	Public Function Contains(c As Integer) As Boolean

		Return m_Nodes.ContainsKey(c)

	End Function

	Public Function GetChild(c As Integer) As TrieNode

		Return m_Nodes(c)

	End Function

	Public Iterator Function TraversePreOrder() As IEnumerable(Of TrieNode)

		Yield Me
		For Each node As TrieNode In Nodes.Values
			For Each n As TrieNode In node.TraversePreOrder()
				Yield n
			Next
		Next

	End Function

	Public Iterator Function TraversePostOrder() As IEnumerable(Of TrieNode)
		Dim toVisit As New Stack(Of TrieNode)
		Dim visitedAncestors As New Stack(Of TrieNode)

		toVisit.Push(Me)
		Do While toVisit.Count > 0
			Dim node As TrieNode = toVisit.Peek()

			If node.Nodes.Count > 0 Then
				If Not visitedAncestors.PeekOrDefault() Is node Then
					visitedAncestors.Push(node)
					toVisit.PushReverse(node.Nodes.Values)
					Continue Do
				End If
				visitedAncestors.Pop()
			End If
			Yield node
			toVisit.Pop()
		Loop

	End Function

	''' <summary>
	''' Overridden to display a debug-friendly representation of this object.
	''' </summary>
	Public Overrides Function ToString() As String

		Return String.Format(fmt_ToString, String.Join(PIPE, m_Nodes.Keys.Select(Function(k) ChrW(k))), String.Join(PIPE, m_IDs), EndsWord)

	End Function

#End Region

#Region " Construction "

	Public Sub New()

		m_Nodes = New Dictionary(Of Integer, TrieNode)
		m_IDs = New HashSet(Of Integer)

	End Sub

#End Region

End Class