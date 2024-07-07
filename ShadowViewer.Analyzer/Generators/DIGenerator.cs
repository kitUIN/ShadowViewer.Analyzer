using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowViewer.Analyzer.Model;

namespace ShadowViewer.Analyzer.Generators
{
    [Generator]
    internal class DiGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
 
            try
            {
                var compilation = context.Compilation;
                var diSymbol = compilation.GetTypeByMetadataName("ShadowViewer.Analyzer.Attributes.AutoDiAttribute");
                foreach (var tree in compilation.SyntaxTrees)
                {
                    var model = compilation.GetSemanticModel(tree);
                    var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
                    foreach (var cls in classes)
                    {
                        var classSymbol = model.GetDeclaredSymbol(cls);
                        var da = classSymbol!.GetAttributes().FirstOrDefault(a => a!.AttributeClass!.Equals(diSymbol, SymbolEqualityComparer.Default));
                        if (classSymbol is not null & da is not null)
                        {
                            var b = da!.ConstructorArguments.Select(x => (bool)x.Value!).ToList();
                            var p = "";
                            var init = "";
                            List<string> constructor = new();
                            var di = new List<Di>
                            {
                                new("ICallableService","Caller","caller"),
                                new("ISqlSugarClient","Db","db"),
                                new("CompressService","Compressor","compressService"),
                                new("PluginLoader","PluginService","pluginService"),
                                new("ILogger","Logger","logger"),
                                new("ResponderService","ResponderService","responderService"),
                            };
                            for (var i = 0; i < di.Count; i++)
                            {
                                if (!b[i]) continue;
                                p += $@"
        public {di[i].ClassTypeName} {di[i].ClassName} {{ get; }}";
                                init += $@"
            {di[i].ClassName} = {di[i].ConstructorName};";
                                constructor.Add($"{di[i].ClassTypeName} {di[i].ConstructorName}");
                            }
                            var ac = "public";
                            switch(classSymbol!.DeclaredAccessibility)
                            {
                                case Accessibility.Internal:
                                    ac = "internal";
                                    break;
                                case Accessibility.Private:
                                    ac = "private";
                                    break;
                                case Accessibility.NotApplicable:
                                case Accessibility.ProtectedAndInternal:
                                case Accessibility.Protected:
                                case Accessibility.ProtectedOrInternal:
                                case Accessibility.Public:
                                default:
                                    break;
                            }
                            var code = $@"
using ShadowViewer.Services;
using ShadowViewer;
using Serilog;
using SqlSugar;
namespace {classSymbol!.ContainingNamespace.ToDisplayString()}
{{
    {ac} partial class {classSymbol.Name}
    {{
        {p}
        public {classSymbol.Name}({string.Join(",",constructor)})
        {{
            {init}
        }}
    }}
}}";
                            context.AddSource($"{classSymbol.Name}.g.cs", code);
                        }
                    }

                }
                   
            }
            catch (Exception e)
            {
                LogError(context, e);
            }
        }
        static void LogError(GeneratorExecutionContext context, Exception exception)
        {
            DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Error",
                                                                                       title: "Code Generator Error",
                                                                                       messageFormat: "{0}",
                                                                                       category: "CodeGenerator",
                                                                                       DiagnosticSeverity.Error,
                                                                                       isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, "[国际化生成器]" + exception.Message));
        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
