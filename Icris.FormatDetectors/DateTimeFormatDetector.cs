using System;
using System.Collections.Generic;
using System.Linq;

namespace Icris.FormatDetectors
{
    public class DateTimeFormatDetector : IFormatDetector<DateTime>
    {
        public DataDescription<DateTime> DetectFromValues(string[] examples)
        {
            //Step 1. Minimum viable data payload should be at least 4 digits. 1-1-11 or 1/1/11
            var maxset = examples.Where(x => x.Select(c => char.IsDigit(c)).Count() >= 4).Select(y => y.Trim());
            //Step 2. Has the possible set a fixed width?
            var fixedwidth = maxset.Select(x => x.Length).Max() == maxset.Select(x => x.Length).Min();
            string determinedformat = "";
            if (fixedwidth)
            {
                return new DataDescription<DateTime>()
                {
                    FormatString = DetermineFixedWidthFormat(maxset)
                };

            }
            else
            {
                //No fixed width. We need to find the separators then. Usual suspects: / . - : T + and space.
                throw new Exception("To do");
            }
        }

        /// <summary>
        /// One big 'decision tree' to decide what format we have giving the input set of date values.
        /// </summary>
        /// <param name="maxset"></param>
        /// <returns></returns>
        private string DetermineFixedWidthFormat(IEnumerable<string> maxset)
        {
            //Find groups of digits. We can safely assume that dd MM and yy are present, possibly yyyy, HH, mm, ss.
            var width = maxset.Select(x => x.Length).Max();
            var payloaddigits = maxset.Select(x => x.Where(c => char.IsDigit(c)).Count()).Max();
            //ddMMyy
            if (width == 6 && payloaddigits == 6)
            {
                //Format must be ddMMyy or MMddyy or yyMMdd.
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                if (this.CouldBeDayPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    //ddMMyy
                    return "ddMMyy";
                }
                if (this.CouldBeMonthPart(firstgroup) && this.CouldBeDayPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    //MMddyy
                    return "MMddyy";
                }
                if (this.CouldBeYearPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeDayPart(thirdgroup))
                {
                    //yyMMdd
                    return "yyMMdd";
                }
            }
            //ddMMyyyy
            if (width == 8 && payloaddigits == 8)
            {

                //assume 4-digit year at the end
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(4, 4)).ToArray();
                //ddMMyyyy
                if (this.CouldBeDayPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    return "ddMMyyyy";
                }
                //MMddyyyy
                if (this.CouldBeMonthPart(firstgroup) && this.CouldBeDayPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    return "MMddyyyy";
                }
                //assume 4-digit year at the start
                firstgroup = maxset.Select(x => x.Substring(0, 4)).ToArray();
                secondgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                thirdgroup = maxset.Select(x => x.Substring(6, 2)).ToArray();
                if (this.CouldBeYearPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeDayPart(thirdgroup))
                {
                    return "yyyyMMdd";
                }
            }
            //dd-MM-yy
            if (width == 8 && payloaddigits == 6)
            {
                //Format must be dd-MM-yy or any other order of these groups.
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(3, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(6, 2)).ToArray();
                var separator = maxset.Select(x => x.Substring(2, 1)).First();

                if (this.CouldBeDayPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeYearPart(secondgroup))
                {
                    //dd-MM-yy
                    return "dd" + separator + "MM" + separator + "yy";
                }
                if (this.CouldBeMonthPart(firstgroup) && this.CouldBeDayPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    //MM-dd-yy
                    return "MM" + separator + "dd" + separator + "yy";
                }
                if (this.CouldBeYearPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeDayPart(thirdgroup))
                {
                    //yy-MM-dd
                    return "yy" + separator + "MM" + separator + "dd";
                }
            }
            //dd-MM-yyyy
            if (width == 10 && payloaddigits == 8)
            {
                //We can determine the yearpart's place by evaluating if there's a separator at index 2.
                var separator = maxset.Select(x => x.Substring(2, 1)).First();
                if (char.IsDigit(separator[0]))
                {
                    separator = maxset.Take(1).First().Substring(4, 1);
                    //year is first group: yyyy-MM-dd
                    return "yyyy" + separator + "MM" + separator + "dd";
                }
                //Format must be dd-MM-yyyy or something like that
                //assume 4-digit year at the end
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(3, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(6, 4)).ToArray();
                //dd-MM-yyyy
                if (this.CouldBeDayPart(firstgroup) && this.CouldBeMonthPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    return "dd" + separator + "MM" + separator + "yyyy";
                }
                //MM-dd-yyyy
                if (this.CouldBeMonthPart(firstgroup) && this.CouldBeDayPart(secondgroup) && this.CouldBeYearPart(thirdgroup))
                {
                    return "MM" + separator + "dd" + separator + "yyyy";
                }
            }
            //ddMMyyHHmm
            if (width == 10 && payloaddigits == 10)
            {
                //smallest format possible to contain time data also.
                //Format must be ddMMyyHHmm or something like that
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                var fourthgroup = maxset.Select(x => x.Substring(6, 2)).ToArray();
                var fifthgroup = maxset.Select(x => x.Substring(8, 2)).ToArray();
                if (this.CouldBeDayPart(firstgroup) &&
                    this.CouldBeMonthPart(secondgroup) &&
                    this.CouldBeYearPart(thirdgroup) &&
                    this.CouldBeHourPart(fourthgroup) &&
                    this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "ddMMyyHHmm";
                }
                if (this.CouldBeMonthPart(firstgroup) &&
                    this.CouldBeDayPart(secondgroup) &&
                    this.CouldBeYearPart(thirdgroup) &&
                    this.CouldBeHourPart(fourthgroup) &&
                    this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "MMddyyHHmm";
                }
                if (this.CouldBeYearPart(firstgroup) &&
                    this.CouldBeMonthPart(secondgroup) &&
                    this.CouldBeDayPart(thirdgroup) &&
                    this.CouldBeHourPart(fourthgroup) &&
                    this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "yyMMddHHmm";
                }
            }
            //ddMMyy HHmm
            if (width == 11 && payloaddigits == 10)
            {
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                var fourthgroup = maxset.Select(x => x.Substring(7, 2)).ToArray();
                var fifthgroup = maxset.Select(x => x.Substring(9, 2)).ToArray();
                var datetimeseparator = maxset.Select(x => x.Substring(6, 1)).Take(1).First();

                if (this.CouldBeDayPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "ddMMyy" + datetimeseparator + "HHmm";
                }
                if (this.CouldBeMonthPart(firstgroup) &&
                        this.CouldBeDayPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "MMddyy" + datetimeseparator + "HHmm";
                }
                if (this.CouldBeYearPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeDayPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return "yyMMdd" + datetimeseparator + "HHmm";
                }
            }
            //ddMMyyHHmmss
            //OR ddMMyyyyHHmm
            if (width == 12 && payloaddigits == 12)
            {
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                var fourthgroup = maxset.Select(x => x.Substring(6, 2)).ToArray();
                var fifthgroup = maxset.Select(x => x.Substring(8, 2)).ToArray();
                var sixthgroup = maxset.Select(x => x.Substring(10, 2)).ToArray();

                if (this.CouldBeDayPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup) &&
                        this.CouldBeSecondOrMinutePart(sixthgroup))
                {
                    return "ddMMyyHHmmss";
                }
                if (this.CouldBeMonthPart(firstgroup) &&
                    this.CouldBeDayPart(secondgroup) &&
                    this.CouldBeYearPart(thirdgroup) &&
                    this.CouldBeHourPart(fourthgroup) &&
                    this.CouldBeSecondOrMinutePart(fifthgroup) &&
                    this.CouldBeSecondOrMinutePart(sixthgroup))
                {
                    return "MMddyyHHmmss";
                }
                if (this.CouldBeYearPart(firstgroup) &&
                    this.CouldBeMonthPart(secondgroup) &&
                    this.CouldBeDayPart(thirdgroup) &&
                    this.CouldBeHourPart(fourthgroup) &&
                    this.CouldBeSecondOrMinutePart(fifthgroup) &&
                    this.CouldBeSecondOrMinutePart(sixthgroup))
                {
                    return "yyMMddHHmmss";
                }
                //ddMMyyyyHHmm
                //1. Determine year: 4,4 or 0,4
                bool fourDigitYearFirst = CouldBeYearPart(maxset.Select(x => x.Substring(0, 4)).ToArray());
                //yyyyMMddHHmm
                if (fourDigitYearFirst)
                {
                    if (CouldBeMonthPart(maxset.Select(x => x.Substring(4, 2)).ToArray()) &&
                        CouldBeDayPart(maxset.Select(x => x.Substring(6, 2)).ToArray()) &&
                        CouldBeHourPart(maxset.Select(x => x.Substring(8, 2)).ToArray()) &&
                        CouldBeSecondOrMinutePart(maxset.Select(x => x.Substring(10, 2)).ToArray()))
                    {
                        return "yyyyMMddHHmm";
                    }
                }
                //ddMMyyyyHHmm or MMddyyyyHHmm
                if (CouldBeMonthPart(maxset.Select(x => x.Substring(2, 2)).ToArray()) &&
                        CouldBeYearPart(maxset.Select(x => x.Substring(4, 4)).ToArray()) &&
                        CouldBeDayPart(maxset.Select(x => x.Substring(0, 2)).ToArray()) &&
                        CouldBeHourPart(maxset.Select(x => x.Substring(8, 2)).ToArray()) &&
                        CouldBeSecondOrMinutePart(maxset.Select(x => x.Substring(10, 2)).ToArray()))
                {
                    return "ddMMyyyyHHmm";
                }
                if (CouldBeMonthPart(maxset.Select(x => x.Substring(0, 2)).ToArray()) &&
                        CouldBeYearPart(maxset.Select(x => x.Substring(4, 4)).ToArray()) &&
                        CouldBeDayPart(maxset.Select(x => x.Substring(2, 2)).ToArray()) &&
                        CouldBeHourPart(maxset.Select(x => x.Substring(8, 2)).ToArray()) &&
                        CouldBeSecondOrMinutePart(maxset.Select(x => x.Substring(10, 2)).ToArray()))
                {
                    return "MMddyyyyHHmm";
                }
            }
            //ddMMyy HHmmss OR ddMMyyyy HHmm
            if (width == 13 && payloaddigits == 12)
            {
                var datetimeseparator = maxset.Select(x => x.Substring(6, 1)).Take(1).First();
                //ddMMyyyy HHmm
                if (char.IsDigit(datetimeseparator[0]))
                {
                    datetimeseparator = maxset.Select(x => x.Substring(8, 1)).Take(1).First();
                    var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                    var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                    var thirdgroup = maxset.Select(x => x.Substring(4, 4)).ToArray();
                    var fourthgroup = maxset.Select(x => x.Substring(9, 2)).ToArray();
                    var fifthgroup = maxset.Select(x => x.Substring(11, 2)).ToArray();
                    if (this.CouldBeDayPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"ddMMyyyy{datetimeseparator}HHmm";
                    }
                    if (this.CouldBeDayPart(secondgroup) &&
                        this.CouldBeMonthPart(firstgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"MMddyyyy{datetimeseparator}HHmm";
                    }
                    if (this.CouldBeYearPart(maxset.Select(x => x.Substring(0, 4)).ToArray()) &&
                        this.CouldBeMonthPart(maxset.Select(x => x.Substring(4, 2)).ToArray()) &&
                        this.CouldBeDayPart(maxset.Select(x => x.Substring(6, 2)).ToArray()) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"yyyyMMdd{datetimeseparator}HHmm";
                    }
                }
                //ddMMyy HHmmss
                else
                {
                    var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                    var secondgroup = maxset.Select(x => x.Substring(2, 2)).ToArray();
                    var thirdgroup = maxset.Select(x => x.Substring(4, 2)).ToArray();
                    var fourthgroup = maxset.Select(x => x.Substring(7, 2)).ToArray();
                    var fifthgroup = maxset.Select(x => x.Substring(9, 2)).ToArray();
                    var sixthgroup = maxset.Select(x => x.Substring(11, 2)).ToArray();

                    if (this.CouldBeDayPart(firstgroup) &&
                            this.CouldBeMonthPart(secondgroup) &&
                            this.CouldBeYearPart(thirdgroup) &&
                            this.CouldBeHourPart(fourthgroup) &&
                            this.CouldBeSecondOrMinutePart(fifthgroup) &&
                            this.CouldBeSecondOrMinutePart(sixthgroup))
                    {
                        return $"ddMMyy{datetimeseparator}HHmmss";
                    }
                    if (this.CouldBeMonthPart(firstgroup) &&
                        this.CouldBeDayPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup) &&
                        this.CouldBeSecondOrMinutePart(sixthgroup))
                    {
                        return $"MMddyy{datetimeseparator}HHmmss";
                    }
                    if (this.CouldBeYearPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeDayPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup) &&
                        this.CouldBeSecondOrMinutePart(sixthgroup))
                    {
                        return $"yyMMdd{datetimeseparator}HHmmss";
                    }
                }
            }
            //dd-MM-yy HH:mm
            if (width == 14 && payloaddigits == 10)
            {
                var dateseparator = maxset.Select(x => x.Substring(2, 1)).Take(1).First();
                var timeseparator = maxset.Select(x => x.Substring(11, 1)).Take(1).First();
                var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                var secondgroup = maxset.Select(x => x.Substring(3, 2)).ToArray();
                var thirdgroup = maxset.Select(x => x.Substring(6, 2)).ToArray();
                var fourthgroup = maxset.Select(x => x.Substring(9, 2)).ToArray();
                var fifthgroup = maxset.Select(x => x.Substring(12, 2)).ToArray();
                var datetimeseparator = maxset.Select(x => x.Substring(8, 1)).Take(1).First();

                if (this.CouldBeDayPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return $"dd{dateseparator}MM{dateseparator}yy{datetimeseparator}HH{timeseparator}mm";
                }
                if (this.CouldBeMonthPart(firstgroup) &&
                        this.CouldBeDayPart(secondgroup) &&
                        this.CouldBeYearPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return $"MM{dateseparator}dd{dateseparator}yy{datetimeseparator}HH{timeseparator}mm";
                }
                if (this.CouldBeYearPart(firstgroup) &&
                        this.CouldBeMonthPart(secondgroup) &&
                        this.CouldBeDayPart(thirdgroup) &&
                        this.CouldBeHourPart(fourthgroup) &&
                        this.CouldBeSecondOrMinutePart(fifthgroup))
                {
                    return $"yy{dateseparator}MM{dateseparator}dd{datetimeseparator}HH{timeseparator}mm";
                }
            }

            //ddMMyyyyHHmmss
            if(width==14 && payloaddigits == 14)
            {

            }
            //ddMMyyyy HHmmss
            if(width==15 && payloaddigits == 14)
            {

            }
            //dd-MM-yyyy HH:mm
            if(width==16 && payloaddigits == 12)
            {
                var dateseparator = maxset.Select(x => x.Substring(2, 1)).Take(1).First();
                var fourthgroup = maxset.Select(x => x.Substring(11, 2)).ToArray();
                var fifthgroup = maxset.Select(x => x.Substring(14, 2)).ToArray();
                var datetimeseparator = maxset.Select(x => x.Substring(10, 1)).Take(1).First();
                var timeseparator = maxset.Select(x => x.Substring(13, 1)).Take(1).First();

                if (!char.IsDigit(dateseparator[0]))
                {                    
                    var firstgroup = maxset.Select(x => x.Substring(0, 2)).ToArray();
                    var secondgroup = maxset.Select(x => x.Substring(3, 2)).ToArray();
                    var thirdgroup = maxset.Select(x => x.Substring(6, 4)).ToArray();
                    
                    //dd-MM-yyyy HH:mm
                    if (this.CouldBeDayPart(firstgroup) &&
                            this.CouldBeMonthPart(secondgroup) &&
                            this.CouldBeYearPart(thirdgroup) &&
                            this.CouldBeHourPart(fourthgroup) &&
                            this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"dd{dateseparator}MM{dateseparator}yyyy{datetimeseparator}HH{timeseparator}mm";
                    }
                    //MM-dd-yyyy HH:mm
                    if (this.CouldBeMonthPart(firstgroup) &&
                            this.CouldBeDayPart(secondgroup) &&
                            this.CouldBeYearPart(thirdgroup) &&
                            this.CouldBeHourPart(fourthgroup) &&
                            this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"MM{dateseparator}dd{dateseparator}yyyy{datetimeseparator}HH{timeseparator}mm";
                    }
                }
                else
                {
                    dateseparator = maxset.Select(x => x.Substring(4, 1)).Take(1).First();
                    //yyyy-MM-dd HH:mm
                    if (this.CouldBeYearPart(maxset.Select(x => x.Substring(0, 4)).ToArray()) &&
                            this.CouldBeMonthPart(maxset.Select(x => x.Substring(5, 2)).ToArray()) &&
                            this.CouldBeDayPart(maxset.Select(x => x.Substring(8, 2)).ToArray()) &&
                            this.CouldBeHourPart(fourthgroup) &&
                            this.CouldBeSecondOrMinutePart(fifthgroup))
                    {
                        return $"yyyy{dateseparator}MM{dateseparator}dd{datetimeseparator}HH{timeseparator}mm";
                    }                    
                }
            }
            //dd-MM-yy HH:mm:ss
            if (width == 17 && payloaddigits == 12)
            {

            }
            //dd-MM-yyyy HH:mm:ss
            if (width == 19 && payloaddigits == 14)
            {

            }
            throw new Exception($"Unable to determine format. Payloaddigits: {payloaddigits}, Width: {width}, Example: {maxset.ToList().First().ToString()}");
        }
        private bool CouldBeSecondOrMinutePart(string[] values)
        {
            //all integervalues are equal or below 59.
            return values.Select(value => int.Parse(value) >= 0 && int.Parse(value) <= 59).Aggregate((a, b) => a && b);
        }
        private bool CouldBeHourPart(string[] values)
        {
            //all integervalues are equal or below 23.
            return values.Select(value => int.Parse(value) >= 0 && int.Parse(value) <= 23).Aggregate((a, b) => a && b);
        }
        private bool CouldBeDayPart(string[] values)
        {
            //all integervalues are equal or below 31.
            return values.Select(value => int.Parse(value) > 0 && int.Parse(value) <= 31).Aggregate((a, b) => a && b);
        }
        private bool CouldBeMonthPart(string[] values)
        {
            //all integervalues are equal or below 12.
            return values.Select(value => int.Parse(value) > 0 && int.Parse(value) <= 12).Aggregate((a, b) => a && b);
        }
        private bool CouldBeYearPart(string[] values)
        {
            var width = values.Select(x => x.Length).Max();
            if (width == 2)
                return values.Select(value => int.Parse(value) >= 0 && int.Parse(value) <= 99).Aggregate((a, b) => a && b);
            else
                return values.Select(value => int.Parse(value) >= 0 && int.Parse(value) <= 2100).Aggregate((a, b) => a && b);

        }
    }
}
