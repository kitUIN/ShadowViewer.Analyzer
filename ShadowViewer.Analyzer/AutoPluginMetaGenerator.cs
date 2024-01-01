using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ShadowViewer.Analyzer;
using System.Xml.Linq;
namespace ShadowViewer
{
    [Generator]
    public class AutoPluginMetaGenerator : ISourceGenerator
    {
        static void LogError(GeneratorExecutionContext context,Exception exception)
        {
            DiagnosticDescriptor InvalidXmlWarning = new DiagnosticDescriptor(id: "Error",
                                                                                       title: "Code Generator Error",
                                                                                       messageFormat: "{0}",
                                                                                       category: "CodeGenerator",
                                                                                       DiagnosticSeverity.Error,
                                                                                       isEnabledByDefault: true);
            context.ReportDiagnostic(Diagnostic.Create(InvalidXmlWarning, Location.None, "[插件元数据生成器]"+exception.Message));
        }
        static string GetMinVersion(GeneratorExecutionContext context)
        {
            var packageReference = context.Compilation.ReferencedAssemblyNames.FirstOrDefault(x => x.Name == "ShadowViewer.Core");
            if (packageReference != null)
            {
                var v = packageReference.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
            throw new Exception("缺少指定nuget包ShadowViewer.Core");
        }
        static string GetRequire(GeneratorExecutionContext context)
        {
            var packageReference = context.Compilation.ReferencedAssemblyNames.Where(x => x.Name.StartsWith("ShadowViewer.Plugin."));
            var requires = new List<string>();
            foreach(var package in packageReference)
            {
                var v = package.Version;
                var name = package.Name.Replace("ShadowViewer.Plugin.", "");
                requires.Add(@$"""{name}={v.Major}.{v.Minor}.{v.Build}.{v.Revision}""");
            }
            return string.Join( ";", requires);
        }
        /// <summary>
        /// 获取核心的版本号
        /// </summary>
        static string GetCoreVersion(GeneratorExecutionContext context)
        {
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                if (Path.GetFileName(file.Path).Equals("ShadowViewer.Core.csproj", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = XDocument.Load(file.Path);
                    var propertyGroups = doc.Root.Elements("PropertyGroup");
                    foreach (var propertyGroup in propertyGroups)
                    {
                        var locationElement = propertyGroup.Element("Version");
                        if (locationElement != null)
                        {
                            return locationElement.Value;
                        }
                    }
                    throw new Exception("未找到当前核心版本");
                }
            }
            throw new Exception("缺少ShadowViewer.Core.csproj文件");
        }
        
        static string GetMeta(GeneratorExecutionContext context)
        {
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                if (Path.GetExtension(file.Path).Equals(".csproj", StringComparison.OrdinalIgnoreCase))
                {
                    var doc = XDocument.Load(file.Path);
                    var fileName = Path.GetFileName(file.Path);
                    var meta = new MetaYaml();
                    if(doc.Root.Element("PropertyGroup") is { } propertyGroup)
                    {
                        if (propertyGroup.Element("Version") is { } version)
                        {
                            if(version.Value.Split('.').Length != 4) 
                                throw new Exception($"{fileName}文件中插件元数据版本号必须为<Version>{{Major}}.{{Minor}}.{{Build}}.{{Revision}}<Version/>");
                            meta.Version = version.Value;
                        }
                        else
                            throw new Exception($"{fileName}文件中缺少插件元数据<Version><Version/>");
                        if (propertyGroup.Element("PackageId") is { } id)
                            meta.Id = id.Value.Replace("ShadowViewer.Plugin.","");
                        else
                            throw new Exception(@$"{fileName}文件中缺少插件元数据<PackageId>ShadowViewer.Plugin.{{插件id}}<PackageId/>");
                        if (propertyGroup.Element("PluginLogo") is { } logo)
                            meta.Logo = logo.Value;
                        else
                            meta.Logo = "";
                        if (propertyGroup.Element("PluginName") is { } name)
                            meta.Name = name.Value;
                        else
                            throw new Exception($"{fileName}文件中缺少插件元数据<PluginName><PluginName/>");
                        
                        if (propertyGroup.Element("PluginLang") is { } lang)
                        {
                            var temp = lang.Value.Split(';');
                            for(int i = 0;i < temp.Length; i++)
                            {
                                temp[i] = @$"""{temp[i]}""";
                            }
                            meta.Lang = temp;
                        }
                        else
                            meta.Lang = new string[] { @"""zh-CN""" };
                        if (propertyGroup.Element("Authors") is { } author)
                            meta.Author = author.Value;
                        else
                            throw new Exception($"{fileName}文件中缺少插件元数据<Authors><Authors/>");
                        if (propertyGroup.Element("Description") is { } description)
                            meta.Description = description.Value;
                        else
                            meta.Description = "";
                        if (propertyGroup.Element("RepositoryUrl") is { } url)
                            meta.WebUri = url.Value;
                        else
                            meta.WebUri = "";
                        meta.MinVersion = GetMinVersion(context);
                        var res = $@"PluginMetaData(
    ""{meta.Id}"",
    ""{meta.Name}"",
    ""{meta.Description}"",
    ""{meta.Author}"", ""{meta.Version}"",
    ""{meta.WebUri}"",
    ""{meta.Logo}"",
    ""{meta.MinVersion}"", 
    new string[]{{{GetRequire(context)}}},
    new string[]{{{String.Join(",", meta.Lang)}}})";
                        return res;
                    }
                    throw new Exception("未在当前项目的.csproj文件中找到PropertyGroup");
                }
            }
            throw new Exception(@"缺少当前项目的.csproj文件,请将以下填入到.csproj文件中
<ItemGroup>
	<AdditionalFiles Include=""你的项目.csproj"" CopyToOutputDirectory=""PreserveNewest"" />
</ItemGroup>");

        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                
                // Get the compilation object
                var compilation = context.Compilation;

                // Get the symbol for the Serializable attribute
                var serializableSymbol = compilation.GetTypeByMetadataName("ShadowViewer.Plugins.AutoPluginMetaAttribute");
                var coreVersionSymbol = compilation.GetTypeByMetadataName("ShadowViewer.Plugins.AutoCoreVersionAttribute");

                // Loop through the syntax trees in the compilation
                foreach (var tree in compilation.SyntaxTrees)
                {
                    // Get the semantic model for the syntax tree
                    var model = compilation.GetSemanticModel(tree);

                    // Find all the class declarations in the syntax tree
                    var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
                    var interfaces = tree.GetRoot().DescendantNodes().OfType<InterfaceDeclarationSyntax>();
                    // Loop through the class declarations
                    foreach (var cls in classes)
                    {
                        // Get the symbol for the class declaration
                        var classSymbol = model.GetDeclaredSymbol(cls);

                        // Check if the class has the Serializable attribute
                        if (classSymbol.GetAttributes().Any(a => a.AttributeClass.Equals(serializableSymbol, SymbolEqualityComparer.Default)))
                        {
                            var meta = GetMeta(context);
                            // Generate a Hello method with the class name
                            var code = $@"
using ShadowViewer.Plugins;
// Automatic Generate From ShadowViewer.Analyzer
namespace {classSymbol.ContainingNamespace.ToDisplayString()}
{{
    [{meta}]
    public partial class {classSymbol.Name}
    {{
        /// <summary>
        /// 插件元数据
        /// </summary>
        public static PluginMetaData Meta {{ get; }} = new {meta};
    }}
}}";
                            // Add the generated code to the compilation

                            context.AddSource($"{classSymbol.Name}.g.cs", code);
                            
                        }
                        
                    }

                    foreach(var it in interfaces)
                    {
                        var interfaceSymbol = model.GetDeclaredSymbol(it);
                        if (interfaceSymbol.GetAttributes().Any(a => a.AttributeClass.Equals(coreVersionSymbol, SymbolEqualityComparer.Default)))
                        {
                            var version = GetCoreVersion(context);
                            // Generate a Hello method with the class name
                            var code = $@"
using ShadowViewer.Plugins;    
// Automatic Generate From ShadowViewer.Analyzer
namespace {interfaceSymbol.ContainingNamespace.ToDisplayString()}
{{
    public partial interface {interfaceSymbol.Name}
    {{
        /// <summary>
        /// 插件最小内核版本
        /// </summary>
        public static string MinVersion => ""{version}"";
    }}
}}";
                            context.AddSource($"{interfaceSymbol.Name}.g.cs", code);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                LogError(context, e);
            }
            
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }

    }
}