namespace ShadowViewer.Analyzer.Attributes
{
    /// <summary>
    /// 自动载入插件的元数据(从plugin.yml中)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoPluginMetaAttribute : Attribute
    {
    }
}
