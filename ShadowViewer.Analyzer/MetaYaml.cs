namespace ShadowViewer.Analyzer
{
    internal class MetaYaml
    {
        /// <summary>
        /// 标识符(大小写不敏感)
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 显示的名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string? Author { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string? Version { get; set; }
        /// <summary>
        /// 项目地址
        /// </summary>
        public string? WebUri { get; set; }

        /// <summary>
        /// 图标<br/>
        /// 1.本地文件,以ms-appx://开头<br/>
        /// 2.FontIcon,以font://开头<br/>
        /// 3.FluentRegularIcon,以fluent://regular开头
        /// 4.FluentFilledIcon,以fluent://filled
        /// <example><br/>
        /// 1.ms-appx:///Assets/Icons/Logo.png<br/>
        /// 2.font://\uE714<br/>
        /// 3.fluent://regular/\uE714
        /// 4.fluent://filled/\uE714
        /// </example>
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// 支持的插件管理器版本,该版本即为ShadowViewer.Core的发行版
        /// </summary>
        public string? MinVersion { get; set; }
        /// <summary>
        /// 支持的语言
        /// </summary>
        public string[]? Lang { get; set; }
        /// <summary>
        /// 依赖的插件ID
        /// </summary>
        public string[]? Require { get; set; }
    }
}
