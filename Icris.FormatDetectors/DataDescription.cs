using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    public class DataDescription<T>
    {
        public T MinValue { get; set; }
        public T MaxValue { get; set; }
        public bool EmptyValues { get; set; }
        public bool FoundAny { get; set; }
        public string FormatString { get; set; }
    }
}
