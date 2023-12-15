﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortMembers: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node) {
		var v = new List<MemberDeclarationSyntax>(node.Members);
		var old = new List<MemberDeclarationSyntax>(v);
		v.Sort(Compare);
		if (!v.SequenceEqual(old))
			node = node.WithMembers(new SyntaxList<MemberDeclarationSyntax>(v));
		return base.VisitClassDeclaration(node);
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

	static Visibility GetVisibility(MemberDeclarationSyntax a) {
		foreach (var modifier in a.Modifiers)
			switch (modifier.Kind()) {
			case SyntaxKind.PublicKeyword:
				return Visibility.PUBLIC;
			case SyntaxKind.ProtectedKeyword:
				return Visibility.PROTECTED;
			}
		return Visibility.PRIVATE;
	}

	static Category GetCategory(MemberDeclarationSyntax a) {
		switch (a) {
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
		throw new Exception(a.ToString());
	}

	static string Name(MemberDeclarationSyntax a) {
		// https://learn.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntax.memberdeclarationsyntax?view=roslyn-dotnet-4.7.0
		switch (a) {
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
		throw new Exception(a.ToString());
	}
}
