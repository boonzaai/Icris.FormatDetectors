using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    public class DataDescription<T> where T:struct
    {
        public T MinValue { get; set; }
        public T MaxValue { get; set; }
        public bool EmptyValues { get; set; }
        public bool FoundAny { get; set; }
        public string FormatString { get; set; }
        public Nullable<T>[] Values { get; set; }
    }
}
