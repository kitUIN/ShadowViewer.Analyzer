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
        public Di(string classTypeName, string className, string constructorName)
        {
            ClassTypeName = classTypeName;
            ClassName = className;
            ConstructorName = constructorName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ClassTypeName { get; }
        /// <summary>
        /// 
        /// </summary>
        public string ClassName { get; }
        /// <summary>
        /// 
        /// </summary>
        public string ConstructorName { get;}

    }
}
