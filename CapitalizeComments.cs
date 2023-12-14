using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;

sealed class CapitalizeComments: CSharpSyntaxRewriter {
	public CapitalizeComments(): base(true) {
	}

	public override SyntaxTrivia VisitTrivia(SyntaxTrivia trivia) {
		do {
			if (!trivia.IsKind(SyntaxKind.SingleLineCommentTrivia))
				break;
			var s = trivia.ToString();
			Debug.Assert(s.StartsWith("//"));
			if (!(s.StartsWith("// ")))
				break;
			s = s[3..];
			if (s == "")
				break;
			if (char.IsUpper(s, 0))
				break;
			if (s.StartsWith("http"))
				break;
			s = "// " + char.ToUpperInvariant(s[0]) + s[1..];
			return SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, s);
		} while (false);
		return base.VisitTrivia(trivia);
	}
}
