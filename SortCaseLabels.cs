using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseLabels: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchSection(SwitchSectionSyntax node) {
		return base.VisitSwitchSection(node);
	}
}
