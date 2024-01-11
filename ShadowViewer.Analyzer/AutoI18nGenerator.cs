using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ShadowViewer.Analyzer
{
    /// <summary>
    /// 自动加载I18N相关类
    /// </summary>
    [Generator]
    internal class AutoI18nGenerator : ISourceGenerator
    {
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
        static Dictionary<string, List<ReswValue>> GetReswDatas(GeneratorExecutionContext context)
        {
            var  reswDatas = new Dictionary<string, List<ReswValue>>();
            foreach (AdditionalText file in context.AdditionalFiles)
            {
                if (Path.GetExtension(file.Path).Equals(".resw", StringComparison.OrdinalIgnoreCase))
                {
                    var country = Path.GetFileNameWithoutExtension(Path.GetDirectoryName(file.Path));
                    var doc = XDocument.Load(file.Path);
                    var datas = doc.Root.Elements("data");
                    foreach (var data in datas)
                    { 
                        var name = data.Attribute("name");
                        if (name == null) continue;
                        var comment = data.Element("comment");
                        var value = data.Element("value");
                        if (!reswDatas.ContainsKey(name.Value))
                            reswDatas.Add(name.Value, new List<ReswValue>());
                        reswDatas[name.Value].Add(new ReswValue
                        {
                            Country = country ?? string.Empty,
                            Value = value?.Value ?? string.Empty,
                            Comment = comment?.Value ?? null
                        });
                    }
                }
            }
            return reswDatas;
        }
        public void Execute(GeneratorExecutionContext context)
        {
            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.RootNamespace", out var currentNamespace);
            var isPlugin = currentNamespace?.StartsWith("ShadowViewer.Plugin") ?? false;
            var isCore = false;
            if (currentNamespace == "ShadowViewer.Core")
            {
                currentNamespace = "ShadowViewer";
                isCore = true;
            }
            else if (currentNamespace == "ShadowViewer")
            {
                isCore = false;
            }
            var resws = GetReswDatas(context);
            string keys = string.Empty;
            
            if (resws.Count > 0)
            {
                var keyList = new List<string>();
                foreach (var resw in resws)
                {
                    List<string> r = new();
                    string comments = string.Empty;
                    foreach(var x in resw.Value)
                    {
                        var a = $"{x.Country}:{x.Value}";
                        if (x.Comment != null)
                        {
                            a += $"({x.Comment})";
                        }
                        r.Add(a);
                    }
                    var enums = string.Join("\n        ///",r);
                    var s = $@"
        /// <summary>
        /// {enums}
        /// </summary>
        {resw.Key}";
                    keyList.Add(s);
                }
                keys = string.Join(",", keyList);
            }
            var reswEnumCode = $@"
namespace {currentNamespace}.Enums
{{
    internal enum ResourceKey
    {{
        {keys}
    }}
}}";
            context.AddSource($"ResourceKey.g.cs", reswEnumCode);
            string? resourcesHelperCode;
            if (isPlugin)
            {
                resourcesHelperCode = $@"
using CustomExtensions.WinUI;
using Windows.ApplicationModel.Resources.Core;
using {currentNamespace}.Enums;
using System;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        private static readonly ResourceMap ResourceManager;
        static ResourcesHelper()
        {{
            var map = ApplicationExtensionHost.GetResourceMapForAssembly();
            if (map is not null) ResourceManager = map;
            else
                throw new NotImplementedException();
        }}
        public static string GetString(string key) 
        {{
            return ResourceManager.GetValue(key).ValueAsString;
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";
            }
            else
            {
                if (isCore)
                {
                    resourcesHelperCode = $@"
using {currentNamespace}.Enums;
using Microsoft.Windows.ApplicationModel.Resources;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        private static readonly ResourceManager resourceManager = new();
        public static string GetString(string key)
        {{
            return resourceManager.MainResourceMap.GetValue(""ShadowViewer.Core/Resources/"" + key).ValueAsString;
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";
                }
                else
                {
                    resourcesHelperCode = $@"
using {currentNamespace}.Enums;
using Microsoft.Windows.ApplicationModel.Resources;

namespace {currentNamespace}.Helpers
{{
    internal static class ResourcesHelper
    {{
        private static readonly ResourceLoader resourceLoader = new ResourceLoader();
        public static string GetString(string key)
        {{
            return resourceLoader.GetString(key);
        }}
        public static string GetString(ResourceKey key)
        {{
            return GetString(key.ToString());
        }}
    }}
}}";
                }
            }
            context.AddSource($"ResourcesHelper.g.cs", resourcesHelperCode);
            var localeExtensionCode = $@"
using Microsoft.UI.Xaml.Markup;
using {currentNamespace}.Helpers;
using {currentNamespace}.Enums;
namespace {currentNamespace}.Extensions
{{
    /// <summary>
    /// 多语言本地化
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(string))]
    internal sealed class LocaleExtension : MarkupExtension
    {{
        
        /// <summary>
        /// 键值
        /// </summary>
        public ResourceKey Key {{ get; set; }}

        /// <inheritdoc/>
        protected override object ProvideValue()
        {{
            return ResourcesHelper.GetString(Key);
        }}
    }}
}}";
            context.AddSource($"LocaleExtension.g.cs", localeExtensionCode);

        }
        public void Initialize(GeneratorInitializationContext context)
        {
        }
    }
}
