using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    interface IFormatDetector<T>
    {
        DataDescription<T> DetectFromValues(string[] examples);
    }
}
