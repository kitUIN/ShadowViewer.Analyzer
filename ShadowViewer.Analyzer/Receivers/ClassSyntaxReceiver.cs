using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ShadowViewer.Analyzer.Receivers;

/// <summary>
/// 
/// </summary>
public class ClassSyntaxReceiver : ISyntaxReceiver
{
    /// <summary>
    /// 
    /// </summary>
    public List<ClassDeclarationSyntax> Classes { get; } = [];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="syntaxNode"></param>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration) return;
        Classes.Add(classDeclaration);
    }
}