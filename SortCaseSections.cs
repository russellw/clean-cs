using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseSections: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node) {
		var v = new List<SwitchSectionSyntax>(node.Sections);
		var old = new List<SwitchSectionSyntax>(v);
		v.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (!v.SequenceEqual(old))
			node = node.WithSections(new SyntaxList<SwitchSectionSyntax>(v));
		return base.VisitSwitchStatement(node);
	}
}
