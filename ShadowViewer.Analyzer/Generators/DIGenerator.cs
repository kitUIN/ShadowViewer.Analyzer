using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using ShadowViewer.Analyzer;
using ShadowViewer.Analyzer.Attributes;
using System.Diagnostics;
namespace ShadowViewer.Analyzer.Generators
{
    [Generator]
    internal class DIGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
 
            try
            {
                var compilation = context.Compilation;
                var diSymbol = compilation.GetTypeByMetadataName("ShadowViewer.Analyzer.Attributes.AutoDIAttribute");
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
                            var b = da!.ConstructorArguments.Select(x => (bool)x.Value).ToList();
                            string p = "";
                            string init = "";
                            List<string> constructor = new();
                            for (int i = 0; i < 5; i++)
                            {
                                switch (i)
                                {
                                    case 1:
                                        if (b[i])
                                        {
                                            p += $@"
        protected ICallableService Caller {{ get; }}";
                                            init += $@"
            Caller = caller;";
                                            constructor.Add("ICallableService caller");
                                        }
                                        break;
                                    case 2:
                                        if (b[i])
                                        {
                                            p += $@"
        protected ISqlSugarClient Db {{ get; }}"; 
                                            init += $@"
            Db = db;";
                                            constructor.Add("ISqlSugarClient db");
                                        }
                                        break;
                                    case 3:
                                        if (b[i])
                                        {
                                            p += $@"
        protected CompressService CompressServices {{ get; }}";
                                            init += $@"
            CompressServices = compressService;";
                                            constructor.Add("CompressService compressService");
                                        }
                                        break;
                                    case 0:
                                        if (b[i])
                                        {
                                            p += $@"
        protected IPluginService PluginService {{ get; }}";
                                            init += $@"
            PluginService = pluginService;";
                                            constructor.Add("IPluginService pluginService");
                                        }
                                        break;
                                    case 4:
                                        if (b[i])
                                        {
                                            p += $@"
        protected ILogger Logger {{ get; }}";
                                            init += $@"
            Logger = logger;";
                                            constructor.Add("ILogger logger");
                                        }
                                        break;
                                }
                            }
                            string ac = "public";
                            switch(classSymbol!.DeclaredAccessibility)
                            {
                                case Accessibility.Internal:
                                    ac = "internal";
                                    break;
                                case Accessibility.Private:
                                    ac = "private";
                                    break;
                                default:
                                    break;
                            }
                            var code = $@"
using ShadowViewer.Services;
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
