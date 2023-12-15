using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortMembers: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node) {
		if (!NoSort(node)) {
			var members = new List<MemberDeclarationSyntax>(node.Members);
			var old = new List<MemberDeclarationSyntax>(members);
			members.Sort(Compare);
			if (!members.SequenceEqual(old)) {
				MemberDeclarationSyntax? prev = null;
				for (var i = 0; i < members.Count; i++) {
					var member = members[i];
					member = WithoutTrivia(member);
					if (WantBlankLine(prev, member))
						member = PrependNewline(member);
					member = AppendNewline(member);
					members[i] = member;
					prev = member;
				}
				node = node.WithMembers(new SyntaxList<MemberDeclarationSyntax>(members));
			}
		}
		return base.VisitClassDeclaration(node);
	}

	static bool NoSort(ClassDeclarationSyntax node) {
		foreach (var trivia in node.GetLeadingTrivia())
			if (trivia.IsKind(SyntaxKind.SingleLineCommentTrivia) && trivia.ToString().Contains("NO-SORT"))
				return true;
		return false;
	}

	static bool WantBlankLine(MemberDeclarationSyntax? prev, MemberDeclarationSyntax member) {
		if (prev == null)
			return false;
		if (member is BaseMethodDeclarationSyntax)
			return true;
		return GetCategory(prev) != GetCategory(member);
	}

	static MemberDeclarationSyntax PrependNewline(MemberDeclarationSyntax member) {
		var trivia = member.GetLeadingTrivia();
		trivia = trivia.Insert(0, SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n"));
		return member.WithLeadingTrivia(trivia);
	}

	static MemberDeclarationSyntax AppendNewline(MemberDeclarationSyntax member) {
		var trivia = member.GetTrailingTrivia();
		trivia = trivia.Add(SyntaxFactory.SyntaxTrivia(SyntaxKind.EndOfLineTrivia, "\n"));
		return member.WithTrailingTrivia(trivia);
	}

	static MemberDeclarationSyntax WithoutTrivia(MemberDeclarationSyntax member) {
		CheckOnlyWhitespace(member.GetLeadingTrivia());
		CheckOnlyWhitespace(member.GetTrailingTrivia());
		return member.WithoutLeadingTrivia().WithoutTrailingTrivia();
	}

	static void CheckOnlyWhitespace(SyntaxTriviaList trivias) {
		foreach (var trivia in trivias)
			switch (trivia.Kind()) {
			case SyntaxKind.EndOfLineTrivia:
			case SyntaxKind.WhitespaceTrivia:
				break;
			default:
				throw new Exception(trivia.ToString());
			}
	}

	static int Compare(MemberDeclarationSyntax a, MemberDeclarationSyntax b) {
		var c = GetVisibility(a) - GetVisibility(b);
		if (c != 0)
			return c;

		c = GetCategory(a) - GetCategory(b);
		if (c != 0)
			return c;

		c = string.CompareOrdinal(Name(a), Name(b));
		if (c != 0)
			return c;

		return string.CompareOrdinal(a.ToString(), b.ToString());
	}

	static Visibility GetVisibility(MemberDeclarationSyntax member) {
		foreach (var modifier in member.Modifiers)
			switch (modifier.Kind()) {
			case SyntaxKind.PublicKeyword:
				return Visibility.PUBLIC;
			case SyntaxKind.ProtectedKeyword:
				return Visibility.PROTECTED;
			}
		return Visibility.PRIVATE;
	}

	static Category GetCategory(MemberDeclarationSyntax member) {
		if (member.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ConstKeyword)))
			return Category.CONST;
		switch (member) {
		case ConstructorDeclarationSyntax:
			return Category.CONSTRUCTOR;
		case MethodDeclarationSyntax:
			return Category.METHOD;
		case DelegateDeclarationSyntax:
			return Category.DELEGATE;
		case EnumMemberDeclarationSyntax:
		case BaseFieldDeclarationSyntax:
			return Category.FIELD;
		}
		throw new Exception(member.ToString());
	}

	static string Name(MemberDeclarationSyntax member) {
		// https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax?view=roslyn-dotnet-4.7.0
		switch (member) {
		case ConstructorDeclarationSyntax:
			return "";
		case MethodDeclarationSyntax method:
			return method.Identifier.Text;
		case DelegateDeclarationSyntax delegateDeclaration:
			return delegateDeclaration.Identifier.Text;
		case EnumMemberDeclarationSyntax enumMember:
			return enumMember.Identifier.Text;
		case BaseFieldDeclarationSyntax baseField:
			return baseField.Declaration.Variables.First().Identifier.Text;
		}
		throw new Exception(member.ToString());
	}
}
