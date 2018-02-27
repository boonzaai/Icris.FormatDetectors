using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    interface IFormatDetector
    {
        DataDescription DetectFromValues(string[] examples);
    }
}
