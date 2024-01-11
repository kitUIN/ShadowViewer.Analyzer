using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ShadowViewer.Analyzer
{
 
    internal class ReswValue
    {
        public string Country { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string? Comment { get; set; } 
    }
}
