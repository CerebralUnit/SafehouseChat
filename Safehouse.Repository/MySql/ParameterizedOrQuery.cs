using System;
using System.Collections.Generic;
using System.Text;

namespace Safehouse.Repository.MySql
{
    public class ParameterizedOrQuery
    {
        public string WhereQuery { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
