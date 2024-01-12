using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShadowViewer.Analyzer.Attributes
{
    /// <summary>
    /// 自动载入Core版本(MinVersion)
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface)]
    public class AutoCoreVersionAttribute : Attribute
    {
    }
}
