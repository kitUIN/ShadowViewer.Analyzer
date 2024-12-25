using Microsoft.CodeAnalysis;

namespace ShadowViewer.Analyzer.Logger;

internal static class Logger
{
    public static void Error(GeneratorExecutionContext context, string message)
    {
        DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Error",
            title: "Code Generator Error",
            messageFormat: "{0}",
            category: "CodeGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, "[国际化生成器]" + message));
    }
    public static void Info(GeneratorExecutionContext context, string message)
    {
        DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Info",
            title: "Code Generator Info",
            messageFormat: "{0}",
            category: "CodeGenerator",
            DiagnosticSeverity.Info,
            isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, "[国际化生成器]" + message));
    }
}