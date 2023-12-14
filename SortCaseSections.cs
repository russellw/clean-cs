using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseSections: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node) {
		var v = new List<SwitchSectionSyntax>(node.Sections);
		var old = new List<SwitchSectionSyntax>(v);
		v.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (v.SequenceEqual(old))
			return base.VisitSwitchStatement(node);
		return node.WithSections(new SyntaxList<SwitchSectionSyntax>(v));
	}
}
