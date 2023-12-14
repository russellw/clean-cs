using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

sealed class SortCaseSections: CSharpSyntaxRewriter {
	public override SyntaxNode? VisitSwitchStatement(SwitchStatementSyntax node) {
		return base.VisitSwitchStatement(node);
	}
}
