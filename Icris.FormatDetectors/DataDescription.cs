using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    public class DataDescription
    {
        public Type Type { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public bool EmptyValues { get; set; }
        public bool FoundAny { get; set; }
        public string FormatString { get; set; }
        public object[] Values { get; set; }
    }
}
