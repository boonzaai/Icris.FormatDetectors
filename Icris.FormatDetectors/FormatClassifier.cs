using System;
using System.Collections.Generic;
using System.Text;

namespace Icris.FormatDetectors
{
    public struct FormatClassificationProbability
    {
        public Type Type;
        public double Probability;
    }
    public class FormatClassificationResult
    {
        public FormatClassificationProbability[] Probabilities;
    }
    public class FormatClassifier
    {
        /// <summary>
        /// Determine the datatype of a set of values using statistical attributes.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public FormatClassificationResult ClassifyFromValues(string[] values)
        {

            return new FormatClassificationResult()
            {
                Probabilities = new FormatClassificationProbability[] {
                    new FormatClassificationProbability() { Type = typeof(string), Probability = 1 }
                }
            };

        }
    }
}
