using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowViewer.Analyzer.Attributes
{
    /// <summary>
    /// 自动载入DI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoDiAttribute : Attribute
    {
        /// <summary>
        /// 是否启用插件服务类
        /// </summary>
        public bool Plugin { get;  } 
        /// <summary>
        /// 是否启用事件通知服务类
        /// </summary>
        public bool Caller { get;  } 
        /// <summary>
        /// 是否启用数据库服务类
        /// </summary>
        public bool Db { get;  }
        /// <summary>
        /// 是否启用压缩服务类
        /// </summary>
        public bool Compress {  get; }

        /// <summary>
        /// 是否启用日志服务类
        /// </summary>
        public bool Logger { get; }
        /// <summary>
        /// 是否启用触发器服务类
        /// </summary>
        public bool Responder { get; }

        /// <summary>
        /// 自动载入DI
        /// </summary>
        /// <param name="plugin">是否启用插件服务类</param>
        /// <param name="caller">是否启用事件通知服务类</param>
        /// <param name="db">是否启用数据库服务类</param>
        /// <param name="compress">是否启用压缩服务类</param>
        /// <param name="logger">是否启用日志服务类</param>
        /// <param name="responder">是否启用触发器服务类</param>
        public AutoDiAttribute(bool plugin = true, bool caller = true, bool db = true, bool compress = true, bool logger = true, bool responder = true)
        {
            Plugin = plugin;
            Caller = caller;
            Db = db;
            Compress = compress;
            Logger = logger;
            Responder = responder;
        }
    }
}
