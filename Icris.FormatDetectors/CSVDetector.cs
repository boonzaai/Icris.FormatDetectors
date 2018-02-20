using System;
using System.Collections.Generic;
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
            //Make a gues which separator is used.
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
                    if (noOfDelimiters > 0)
                        System.Diagnostics.Debugger.Break();
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


        public Dictionary<string,List<string>> GetColumnData()
        {
            var value = new Dictionary<string, List<string>>();
            foreach(var header in Headers)
            {
                value.Add(header, new List<string>());
            }
            long recordcounter = 0;
            foreach(var record in Lines)
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

        public Dictionary<string,DataDescription> GetColumnSpecs()
        {

        }

    }
}
