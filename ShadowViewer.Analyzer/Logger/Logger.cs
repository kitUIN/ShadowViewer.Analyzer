using Microsoft.CodeAnalysis;

namespace ShadowViewer.Analyzer.Logger;

internal class Logger(string category, GeneratorExecutionContext context)
{

    public void Log(string id, string title, string message, DiagnosticSeverity severity)
    {
        var invalidXmlWarning = new DiagnosticDescriptor(id: id,
            title: title,
            messageFormat: "[{0}] {1}",
            category: category,
            severity,
            isEnabledByDefault: true);
        context.ReportDiagnostic(Diagnostic.Create(invalidXmlWarning, Location.None, category, message));
    }
    public void Info(string id, string message)
    {
        Log(id, $"{category} Info", message, DiagnosticSeverity.Info);
    }
    public void Warning(string id, string message)
    {
        Log(id, $"{category} Warning", message, DiagnosticSeverity.Warning);
    }
    public void Error(string id, string message)
    {
        Log(id, $"{category} Error", message, DiagnosticSeverity.Error);
    }
}