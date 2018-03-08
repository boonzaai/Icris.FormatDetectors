using System;
using System.Collections.Generic;
using System.Linq;
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
        public DataDescription MostProbableFormat { get; set; }
        public FormatClassificationProbability[] Probabilities;
    }
    public class FormatClassifier
    {
        /// <summary>
        /// Determine the probability a set of values belongs to a certain datatype.
        /// Each value will be parsed to either an int, double, boolean, or date value.
        /// The number of successful attempts for each type will be returned as a fraction of the total amount.
        /// Keep in mind that multiple types can fit (e.g. double or int) so the probabilities can amount up to
        /// a number greater than 1.0.
        /// </summary>
        /// <param name="values">Values  that should be evaluated</param>
        /// <returns>Classificationresult</returns>
        public FormatClassificationResult ClassifyFromValues(string[] values)
        {
            var boolProbability = (double)values.Select(x =>
            {
                bool value;
                return bool.TryParse(x, out value) ? 1 : 0;
            }).Sum() / (double)values.Length;

            var intProbability = (double)values.Select(x =>
            {
                int value;
                return int.TryParse(x, out value) ? 1 : 0;
            }).Sum() / (double)values.Length;

            var doubleProbability = (double)values.Select(x =>
            {
                double value;
                return double.TryParse(x, out value) ? 1 : 0;
            }).Sum() / (double)values.Length;

            var dateProbability = 0.0;

            try
            {
                //Guess the datetimeformat.
                var description = new DateTimeFormatDetector().DetectFromValues(values);
                if (description.FoundAny)
                    dateProbability = (double)description.Values.Where(x => x != null).Count() / (double)values.Length;
            }
            catch (Exception e)
            {
                //No date format found.
            }
            //var dateProbability = (double)values.Select(x =>
            //{
            //    DateTime value;
            //    return DateTime.TryParse(x, out value) ? 1 : 0;
            //}).Sum() / (double)values.Length;

            
            return new FormatClassificationResult()
            {
                Probabilities = new FormatClassificationProbability[] {
                    new FormatClassificationProbability() { Type = typeof(bool), Probability = boolProbability },
                    new FormatClassificationProbability() { Type = typeof(int), Probability = intProbability },
                    new FormatClassificationProbability() { Type = typeof(double), Probability = doubleProbability },
                    new FormatClassificationProbability() { Type = typeof(DateTime), Probability = dateProbability }
                }
            };

        }
    }
}
