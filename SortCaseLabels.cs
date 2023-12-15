using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseLabels: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node) {
		var labels = new List<SwitchLabelSyntax>(node.Labels);
		var old = new List<SwitchLabelSyntax>(labels);
		labels.Sort((a, b) => string.CompareOrdinal(a.ToString(), b.ToString()));
		if (!labels.SequenceEqual(old))
			node = node.WithLabels(new SyntaxList<SwitchLabelSyntax>(labels));
		return base.VisitSwitchSection(node);
	}
}
