using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Icris.FormatDetectors.Tests
{
    [TestClass]
    public class DateFormatDetectorTests
    {
        Tuple<string, string[]>[] testsets = new Tuple<string, string[]>[]{
            new Tuple<string, string[]>("ddMMyy", new string[] { "010100", "311299", "150401" }) ,
            new Tuple<string, string[]>("ddMMyyyy", new string[] { "01012000", "31121999", "15042001" }) ,
            new Tuple<string, string[]>("MMddyy", new string[] { "010100", "123199", "041501" }) ,
            new Tuple<string, string[]>("MMddyyyy", new string[] { "01012000", "12311999", "04152001" }) ,
            new Tuple<string, string[]>("yyMMdd", new string[] { "000101", "991231", "010415" }) ,
            new Tuple<string, string[]>("yyyyMMdd", new string[] { "20000101", "19991231", "20010415" }) ,
            new Tuple<string, string[]>("dd-MM-yy", new string[] { "01-01-00", "31-12-99", "15-04-01" }) ,
            new Tuple<string, string[]>("dd-MM-yyyy", new string[] { "01-01-2000", "31-12-1999", "15-04-2001" }) ,
            new Tuple<string, string[]>("MM-dd-yy", new string[] { "01-01-00", "12-31-99", "04-15-01" }) ,
            new Tuple<string, string[]>("MM-dd-yyyy", new string[] { "01-01-2000", "12-31-1999", "04-15-2001" }) ,
            new Tuple<string, string[]>("yy-MM-dd", new string[] { "00-01-01", "99-12-31", "01-04-15" }) ,
            new Tuple<string, string[]>("yyyy-MM-dd", new string[] { "2000-01-01", "1999-12-31", "2001-04-15" }) ,
            new Tuple<string, string[]>("dd/MM/yy", new string[] { "01/01/00", "31/12/99", "15/04/01" }) ,
            new Tuple<string, string[]>("dd/MM/yyyy", new string[] { "01/01/2000", "31/12/1999", "15/04/2001" }) ,
            new Tuple<string, string[]>("MM/dd/yy", new string[] { "01/01/00", "12/31/99", "04/15/01" }) ,
            new Tuple<string, string[]>("MM/dd/yyyy", new string[] { "01/01/2000", "12/31/1999", "04/15/2001" }) ,
            new Tuple<string, string[]>("yy/MM/dd", new string[] { "00/01/01", "99/12/31", "01/04/15" }) ,
            new Tuple<string, string[]>("yyyy/MM/dd", new string[] { "2000/01/01", "1999/12/31", "2001/04/15" }) ,
            new Tuple<string, string[]>("dd.MM.yy", new string[] { "01.01.00", "31.12.99", "15.04.01" }) ,
            new Tuple<string, string[]>("dd.MM.yyyy", new string[] { "01.01.2000", "31.12.1999", "15.04.2001" }) ,
            new Tuple<string, string[]>("MM.dd.yy", new string[] { "01.01.00", "12.31.99", "04.15.01" }) ,
            new Tuple<string, string[]>("MM.dd.yyyy", new string[] { "01.01.2000", "12.31.1999", "04.15.2001" }) ,
            new Tuple<string, string[]>("yy.MM.dd", new string[] { "00.01.01", "99.12.31", "01.04.15" }) ,
            new Tuple<string, string[]>("yyyy.MM.dd", new string[] { "2000.01.01", "1999.12.31", "2001.04.15" }),
            //With hours/minutes
            new Tuple<string, string[]>("ddMMyyHHmm", new string[] { "0101000000", "3112990715", "1504012359" }),
            new Tuple<string, string[]>("MMddyyHHmm", new string[] { "0101000000", "1231990715", "0415012359" }),
            new Tuple<string, string[]>("yyMMddHHmm", new string[] { "0001010000", "9912310715", "0104152359" }),
            new Tuple<string, string[]>("ddMMyyyyHHmm", new string[] { "010120000000", "311219990715", "150420012359" }),
            new Tuple<string, string[]>("MMddyyyyHHmm", new string[] { "010120000000", "123119990715", "041520012359" }),
            new Tuple<string, string[]>("yyyyMMddHHmm", new string[] { "200001010000", "199912310715", "200104152359" }),
            new Tuple<string, string[]>("ddMMyy HHmm", new string[] { "010100 0000", "311299 0715", "150401 2359" }),
            new Tuple<string, string[]>("MMddyy HHmm", new string[] { "010100 0000", "123199 0715", "041501 2359" }),
            new Tuple<string, string[]>("yyMMdd HHmm", new string[] { "000101 0000", "991231 0715", "010415 2359" }),
            new Tuple<string, string[]>("ddMMyyyy HHmm", new string[] { "01012000 0000", "31121999 0715", "15042001 2359" }),
            new Tuple<string, string[]>("MMddyyyy HHmm", new string[] { "01012000 0000", "12311999 0715", "04152001 2359" }),
            new Tuple<string, string[]>("yyyyMMdd HHmm", new string[] { "20000101 0000", "19991231 0715", "20010415 2359" }),
            new Tuple<string, string[]>("ddMMyyTHHmm", new string[] { "010100T0000", "311299 0715", "150401 2359" }),
            new Tuple<string, string[]>("MMddyyTHHmm", new string[] { "010100T0000", "123199 0715", "041501 2359" }),
            new Tuple<string, string[]>("yyMMddTHHmm", new string[] { "000101T0000", "991231 0715", "010415 2359" }),
            new Tuple<string, string[]>("ddMMyyyyTHHmm", new string[] { "01012000T0000", "31121999 0715", "15042001 2359" }),
            new Tuple<string, string[]>("MMddyyyyTHHmm", new string[] { "01012000T0000", "12311999 0715", "04152001 2359" }),
            new Tuple<string, string[]>("yyyyMMddTHHmm", new string[] { "20000101T0000", "19991231 0715", "20010415 2359" }),            
            new Tuple<string, string[]>("dd/MM/yy HH:mm", new string[] { "01/01/00 00:00", "31/12/99 07:15", "15/04/01 23:59" }),
            new Tuple<string, string[]>("MM/dd/yy HH:mm", new string[] { "01/01/00 00:00", "12/31/99 07:15", "04/15/01 23:59" }),
            new Tuple<string, string[]>("yy/MM/dd HH:mm", new string[] { "00/01/01 00:00", "99/12/31 07:15", "01/04/15 23:59" }),
            new Tuple<string, string[]>("dd/MM/yyyy HH:mm", new string[] { "01/01/2000 00:00", "31/12/1999 07:15", "15/04/2001 23:59" }),
            new Tuple<string, string[]>("MM/dd/yyyy HH:mm", new string[] { "01/01/2000 00:00", "12/31/1999 07:15", "04/15/2001 23:59" }),
            new Tuple<string, string[]>("yyyy/MM/dd HH:mm", new string[] { "2000/01/01 00:00", "1999/12/31 07:15", "2001/04/15 23:59" }),
            new Tuple<string, string[]>("dd/MM/yyTHH:mm", new string[] { "01/01/00T00:00", "31/12/99 07:15", "15/04/01 23:59" }),
            new Tuple<string, string[]>("MM/dd/yyTHH:mm", new string[] { "01/01/00T00:00", "12/31/99 07:15", "04/15/01 23:59" }),
            new Tuple<string, string[]>("yy/MM/ddTHH:mm", new string[] { "00/01/01T00:00", "99/12/31 07:15", "01/04/15 23:59" }),
            new Tuple<string, string[]>("dd/MM/yyyyTHH:mm", new string[] { "01/01/2000T00:00", "31/12/1999 07:15", "15/04/2001 23:59" }),
            new Tuple<string, string[]>("MM/dd/yyyyTHH:mm", new string[] { "01/01/2000T00:00", "12/31/1999 07:15", "04/15/2001 23:59" }),
            new Tuple<string, string[]>("yyyy/MM/ddTHH:mm", new string[] { "2000/01/01T00:00", "1999/12/31 07:15", "2001/04/15 23:59" }),
            //With hours/minutes/seconds - no separators
            new Tuple<string, string[]>("ddMMyyHHmmss", new string[] { "010100000000", "311299071515", "150401235959" }),
            new Tuple<string, string[]>("MMddyyHHmmss", new string[] { "010100000000", "123199071515", "041501235959" }),
            new Tuple<string, string[]>("yyMMddHHmmss", new string[] { "000101000000", "991231071515", "010415235959" }),
            new Tuple<string, string[]>("ddMMyyyyHHmmss", new string[] { "01012000000000", "31121999071515", "15042001235959" }),
            new Tuple<string, string[]>("MMddyyyyHHmmss", new string[] { "01012000000000", "12311999071515", "04152001235959" }),
            new Tuple<string, string[]>("yyyyMMddHHmmss", new string[] { "20000101000000", "19991231071515", "20010415235959" }),

            new Tuple<string, string[]>("ddMMyyTHHmmss", new string[] { "010100T000000", "311299T071515", "150401T235959" }),
            new Tuple<string, string[]>("MMddyyTHHmmss", new string[] { "010100T000000", "123199T071515", "041501T235959" }),
            new Tuple<string, string[]>("yyMMddTHHmmss", new string[] { "000101T000000", "991231T071515", "010415T235959" }),
            new Tuple<string, string[]>("ddMMyyyyTHHmmss", new string[] { "01012000T000000", "31121999T071515", "15042001T235959" }),
            new Tuple<string, string[]>("MMddyyyyTHHmmss", new string[] { "01012000T000000", "12311999T071515", "04152001T235959" }),
            new Tuple<string, string[]>("yyyyMMddTHHmmss", new string[] { "20000101T000000", "19991231T071515", "20010415T235959" }),

            new Tuple<string, string[]>("dd/MM/yyTHH:mm:ss", new string[] { "01/01/00T00:00:00", "31/12/99T07:15:15", "15/04/01T23:59:59" }),
            new Tuple<string, string[]>("MM/dd/yyTHH:mm:ss", new string[] { "01/01/00T00:00:00", "12/31/99T07:15:15", "04/15/01T23:59:59" }),
            new Tuple<string, string[]>("yy/MM/ddTHH:mm:ss", new string[] { "00/01/01T00:00:00", "99/12/31T07:15:15", "01/04/15T23:59:59" }),
            new Tuple<string, string[]>("dd/MM/yyyyTHH:mm:ss", new string[] { "01/01/2000T00:00:00", "31/12/1999T07:15:15", "15/04/2001T23:59:59" }),
            new Tuple<string, string[]>("MM/dd/yyyyTHH:mm:ss", new string[] { "01/01/2000T00:00:00", "12/31/1999T07:15:15", "04/15/2001T23:59:59" }),
            new Tuple<string, string[]>("yyyy/MM/ddTHH:mm:ss", new string[] { "2000/01/01T00:00:00", "1999/12/31T07:15:15", "2001/04/15T23:59:59" })
        };



        [TestMethod]
        public void TestFormats()
        {
            testsets.ToList().ForEach(x =>
            {
                Assert.AreEqual(x.Item1, new DateTimeFormatDetector().DetectFromValues(x.Item2).FormatString);
            });
        }
        [TestMethod]
        public void TestDescription()
        {
            string[] examples = new string[] {
                "20180101 130000",
                "20170820 170055",
                "20201210 010101",
                "20250101 000000",
                ""
            };
            var result = new DateTimeFormatDetector().DetectFromValues(examples);
            Assert.AreEqual(new DateTime(2025, 1, 1, 0, 0, 0), result.MaxValue);
            Assert.AreEqual(new DateTime(2017, 8, 20, 17, 0, 55), result.MinValue);
            Assert.AreEqual(true, result.EmptyValues);
            Assert.AreEqual("yyyyMMdd HHmmss",result.FormatString);
        }
    }
}
