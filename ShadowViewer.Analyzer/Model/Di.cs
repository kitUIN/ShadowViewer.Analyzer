using System;
using System.Collections.Generic;
using System.Text;

namespace ShadowViewer.Analyzer.Model
{
    /// <summary>
    /// 
    /// </summary>
    public class Di
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classTypeName"></param>
        /// <param name="className"></param>
        /// <param name="constructorName"></param>
        /// <param name="comment"></param>
        public Di(string classTypeName, string className, string constructorName,string comment = "")
        {
            ClassTypeName = classTypeName;
            ClassName = className;
            ConstructorName = constructorName;
            Comment = comment;
        }

        /// <summary>
        /// 类类型名
        /// </summary>
        public string ClassTypeName { get; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; }
        /// <summary>
        /// 构造函数名称
        /// </summary>
        public string ConstructorName { get;}
        /// <summary>
        /// 注释
        /// </summary>
        public string Comment { get;}

    }
}
