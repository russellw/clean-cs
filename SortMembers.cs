using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortMembers: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node) {
		var v = new List<MemberDeclarationSyntax>(node.Members);
		var old = new List<MemberDeclarationSyntax>(v);
		v.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (!v.SequenceEqual(old))
			node = node.WithMembers(new SyntaxList<MemberDeclarationSyntax>(v));
		return base.VisitClassDeclaration(node);
	}
}
