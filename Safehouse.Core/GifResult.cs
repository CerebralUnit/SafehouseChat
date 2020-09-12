using System;
using System.Collections.Generic;
using System.Text;

namespace Safehouse.Core
{
    public class GifResult
    { 
        public string Title { get; set; } 
        public string Url { get; set; } 
        public int Width { get; set;   } 
        public int Height { get; set; }
        public List<string> Tags { get; set; }
    }
}
