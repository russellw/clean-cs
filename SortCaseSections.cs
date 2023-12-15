using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseSections: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node) {
		var sections = new List<SwitchSectionSyntax>(node.Sections);
		var old = new List<SwitchSectionSyntax>(sections);
		sections.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (!sections.SequenceEqual(old))
			node = node.WithSections(new SyntaxList<SwitchSectionSyntax>(sections));
		return base.VisitSwitchStatement(node);
	}
}
