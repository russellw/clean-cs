using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseLabels: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node) {
		var v = new List<SwitchLabelSyntax>(node.Labels);
		var old = new List<SwitchLabelSyntax>(v);
		v.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (v.SequenceEqual(old))
			return base.VisitSwitchSection(node);
		return node.WithLabels(new SyntaxList<SwitchLabelSyntax>(v));
	}
}
