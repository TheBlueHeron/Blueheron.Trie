Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports System.Xml

''' <summary>
''' Serializable store for individual words that are efficiently stored for fast text search.
''' </summary>
Public Class WordStore '<Serializable(), XmlRoot("store")>

#Region " Objects and variables "

	Friend Const _asciiMatch As String = "[^\u0000-\u007F]"
	Friend Const _spc As Char = " "c

	Private m_RegExAscii As New Regex(_asciiMatch, RegexOptions.Compiled Or RegexOptions.CultureInvariant Or RegexOptions.Singleline)
	Private m_TotalWordCount As Integer = 0
	Private m_OriginalTree As Trie
	Private m_SortedTree As Trie
	Private m_Words As Dictionary(Of Integer, String)

	Public Event Progress As System.ComponentModel.ProgressChangedEventHandler

#End Region

#Region " Properties "

	'<XmlAttribute(attributeName:="cs")>
	Public Property CaseSensitive As Boolean

	'<XmlAttribute(attributeName:="tc")>
	Public Property TotalWordCount As Integer
		Get
			Return m_TotalWordCount
		End Get
		Set(value As Integer)
			m_TotalWordCount = value
		End Set
	End Property

	'<XmlElement(elementName:="otree")>
	Public Property OriginalTree As Trie
		Get
			Return m_OriginalTree
		End Get
		Set(value As Trie)
			m_OriginalTree = value
		End Set
	End Property

	'<XmlElement(elementName:="stree")>
	Public Property SortedTree As Trie
		Get
			Return m_SortedTree
		End Get
		Set(value As Trie)
			m_SortedTree = value
		End Set
	End Property

	'<XmlElement(elementName:="words")>
	Public Property Words As Dictionary(Of Integer, String)
		Get
			Return m_Words
		End Get
		Set(value As Dictionary(Of Integer, String))
			m_Words = value
		End Set
	End Property

#End Region

#Region " Public methods and functions "

	Public Function AsciiCorrect(strWord As String) As String

		Return m_RegExAscii.Replace(If(CaseSensitive, strWord, strWord.ToLower), String.Empty)

	End Function

	Public Sub Clear()

		m_OriginalTree = Nothing
		m_SortedTree = Nothing
		m_Words.Clear()
		m_TotalWordCount = 0
		GC.Collect(3, GCCollectionMode.Forced, True)

		m_OriginalTree = New Trie
		m_SortedTree = New Trie

	End Sub

	Public Function Contains(strWord As String, ByRef resultNode As TrieNode) As Boolean

		Return m_SortedTree.Contains(AsciiCorrect(strWord), resultNode)

	End Function

	Public Function ContainsExact(strWord As String, ByRef resultNode As TrieNode) As Boolean

		Return m_OriginalTree.Contains(AsciiCorrect(strWord), resultNode)

	End Function

	Public Function Insert(strWord As String) As TrieNode
		strWord = strWord.Trim

		If Not String.IsNullOrEmpty(strWord) Then
			Dim strCorrectedWord As String = AsciiCorrect(strWord)
			Dim hv As Integer = strCorrectedWord.GetHashCode

			If Not m_Words.ContainsKey(hv) Then
				m_Words.Add(hv, strCorrectedWord)
				m_OriginalTree.Insert(strCorrectedWord, hv)
				m_SortedTree.Insert(SortLetters(strCorrectedWord), hv)
				m_TotalWordCount += 1
				If m_TotalWordCount Mod 3000 = 0 Then
					RaiseEvent Progress(Me, New System.ComponentModel.ProgressChangedEventArgs(m_TotalWordCount, Nothing))
				End If
			End If
		End If

		Return Nothing

	End Function

	'Public Shared Function LoadXml(strPath As String) As WordStore

	'	Try
	'		Dim serializer As New XmlSerializer(GetType(WordStore))

	'		If File.Exists(strPath) Then
	'			Using stream As Stream = File.Open(strPath, FileMode.Open, FileAccess.Read, FileShare.None)
	'				Return CType(serializer.Deserialize(stream), WordStore)
	'			End Using
	'		End If
	'	Catch ex As Exception

	'	End Try

	'	Return New WordStore

	'End Function

	'Public Sub SaveXml(strPath As String)

	'	Try
	'		Dim serializer As New XmlSerializer(GetType(WordStore))

	'		If Not Directory.Exists(Path.GetDirectoryName(strPath)) Then
	'			Directory.CreateDirectory(Path.GetDirectoryName(strPath))
	'		End If

	'		Using stream As Stream = File.Open(strPath, FileMode.Create)
	'			Dim settings As New XmlWriterSettings With {.CheckCharacters = True, .CloseOutput = True, .ConformanceLevel = ConformanceLevel.Document, .Indent = False, .NewLineOnAttributes = False, .OmitXmlDeclaration = True, .Encoding = System.Text.Encoding.ASCII, .IndentChars = String.Empty, .NewLineChars = String.Empty}

	'			Using writer As XmlWriter = XmlWriter.Create(stream, settings)
	'				serializer.Serialize(writer, Me)
	'			End Using
	'		End Using
	'	Catch ex As Exception

	'	End Try

	'End Sub

	Public Function SortLetters(strWord As String) As String

		Return strWord.OrderBy(Function(s) s).ToArray

	End Function

#End Region

#Region " Construction "

	Public Sub New()

		m_OriginalTree = New Trie
		m_SortedTree = New Trie
		m_Words = New Dictionary(Of Integer, String)

	End Sub

#End Region

End Class