using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    interface IFormatDetector<T> where T: struct
    {
        DataDescription<T> DetectFromValues(string[] examples);
    }
}
