﻿Imports System.Runtime.CompilerServices

Public Module Extensions

	<Extension()>
	Public Function Combinations(Of T)(source As IEnumerable(Of T), n As Integer) As IEnumerable(Of IEnumerable(Of T))
		Dim lstResults As New List(Of IEnumerable(Of T))

		If n = 0 Then
			lstResults.Add(Enumerable.Empty(Of T))
		Else
			Dim count As Integer = 1


			For Each item As T In source
				For Each innerSequence In source.Skip(count).Combinations(n - 1)
					lstResults.Add(New T() {item}.Concat(innerSequence))
				Next
				count += 1
			Next
		End If

		Return lstResults

	End Function

	<Extension()>
	Public Function AllCombinations(Of T)(source As IList(Of T)) As IEnumerable(Of IEnumerable(Of T))
		Dim output As IEnumerable(Of IEnumerable(Of T)) = Enumerable.Empty(Of IEnumerable(Of T))()

		For i As Integer = 0 To source.Count
			output = output.Concat(source.Combinations(i))
		Next
		Return output

	End Function

	<Extension()>
	Public Function PeekOrDefault(Of T)(stack As Stack(Of T)) As T

		Return If(stack.Count = 0, Nothing, stack.Peek)

	End Function

	<Extension()>
	Public Sub PushReverse(Of T)(stack As Stack(Of T), items As IEnumerable(Of T))

		For Each it As T In items.Reverse
			stack.Push(it)
		Next

	End Sub


End Module