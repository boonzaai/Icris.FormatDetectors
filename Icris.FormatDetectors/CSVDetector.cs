using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Icris.FormatDetectors
{
    public class CSVDetector
    {
        char[] separators = new char[] { '\t', ',', ';' };
        char[] delimiters = new char[] { '"', '\'' };
        IEnumerable<string> Lines;
        int FirstDataRecord;
        public List<string> Headers { get; private set; }
        public char Separator { get; private set; }
        public char Delimiter { get; private set; }

        string file;
        public CSVDetector(string file)
        {
            this.file = file;
            GuessSeparatorDelimiterAndHeaderAndResetLines();
        }

        void GuessSeparatorDelimiterAndHeaderAndResetLines()
        {
            this.Lines = File.ReadLines(file);
            var evaluationset = Lines.Take(200).ToList();
            char guessedSeparator = ' ';
            char guessedDelimiter = '\0';
            int occurrences = 0;
            //Make a guess which separator is used.
            foreach (var sep in separators)
            {
                var sepcount = evaluationset.Select(x => x.Split(sep).Length - 1).Max();
                if (sepcount > occurrences)
                {
                    guessedSeparator = sep;
                    occurrences = sepcount;
                }
            }
            Separator = guessedSeparator;
            var headline = evaluationset.Where(x => x.Split(Separator).Length - 1 == occurrences).First();
            FirstDataRecord = evaluationset.IndexOf(headline) + 1;
            while (evaluationset[FirstDataRecord].Split(Separator).Length - 1 != occurrences)
                FirstDataRecord++;

            foreach (var delimiter in delimiters)
            {
                var linesWithEvenDelimiters = evaluationset.Where(x =>
                {
                    var noOfDelimiters = x.Where(c => c == delimiter).Count();
                    //if (noOfDelimiters > 0)
                    //    System.Diagnostics.Debugger.Break();
                    return noOfDelimiters > 0 && noOfDelimiters % 2 == 0;
                }).ToList();
                //Find lines with pairs of delimiters.
                guessedDelimiter = linesWithEvenDelimiters.Count > 0 ? delimiter : guessedDelimiter;
            }
            this.Delimiter = guessedDelimiter;
            Headers = headline.Split(Separator).Select(x => x.Trim()).ToList();
            this.Lines = File.ReadLines(file).Skip(FirstDataRecord);
        }

        public List<string> Messages { get; set; }

        void AddMessage(string message)
        {
            if (Messages == null)
                Messages = new List<string>();
            Messages.Add(message);
        }

        /// <summary>
        /// Retrieve a dictionairy with all columns
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<string>> GetColumnData()
        {
            var value = new Dictionary<string, List<string>>();
            foreach (var header in Headers)
            {
                value.Add(header, new List<string>());
            }
            long recordcounter = 0;
            foreach (var record in Lines)
            {
                var values = record.Split(this.Separator);
                if (values.Count() != Headers.Count)
                {
                    AddMessage($"Irregular record encountered: {recordcounter}. Skipping it.");
                    //Irregular record.
                }
                else
                {
                    for (var i = 0; i < values.Length; i++)
                    {
                        value[value.Keys.ToArray()[i]].Add(values[i].Trim(delimiters));
                    }
                    recordcounter++;
                }
            }
            return value;
        }

        public Dictionary<string, DataDescription> GetColumnSpecs()
        {
            var specs = new Dictionary<string, DataDescription>();
            var columns = GetColumnData();
            foreach (var col in columns)
            {
                var result = new FormatClassifier().ClassifyFromValues(col.Value.ToArray());
                var type = result.Probabilities.OrderBy(x => x.Probability).Last();
                DataDescription description = new DataDescription();
                description.Type = type.Type;
                specs.Add(col.Key, description);
            }
            return specs;
        }

        public IEnumerable<JObject> Rows
        {
            get
            {
                return Lines.Skip(this.FirstDataRecord).Select(x =>
                {
                    Dictionary<string, object> record = new Dictionary<string, object>();
                    var fields = x.Split(this.Separator);
                    foreach (var header in this.Headers)
                    {
                        double doublevalue;
                        int intvalue;
                        string stringvalue = fields[this.Headers.IndexOf(header)];
                        if (int.TryParse(stringvalue, NumberStyles.Integer, CultureInfo.InvariantCulture, out intvalue))
                            record.Add(header, intvalue);
                        else if (double.TryParse(stringvalue, NumberStyles.Number, CultureInfo.InvariantCulture, out doublevalue))
                            record.Add(header, doublevalue);
                        else
                            record.Add(header, stringvalue);
                    }
                    return JObject.FromObject(record);
                });
            }
        }

    }
}
