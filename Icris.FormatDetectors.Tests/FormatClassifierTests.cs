using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Icris.FormatDetectors.Tests
{
    [TestClass]
    public class FormatClassifierTests
    {
        [TestMethod]
        public void TestFormatClassifier()
        {
            var testset = new string[] {
                "true",                 //bool
                "1",                    //int,double
                "2",                    //int,double
                "3.1",                  //double,datetime
                "01/01/2000"            //datetime
            };
            var result = new FormatClassifier().ClassifyFromValues(testset);
            Assert.AreEqual(0.4, result.Probabilities.Where(x => x.Type == typeof(DateTime)).First().Probability);
            Assert.AreEqual(0.2, result.Probabilities.Where(x => x.Type == typeof(bool)).First().Probability);
            Assert.AreEqual(0.4, result.Probabilities.Where(x => x.Type == typeof(int)).First().Probability);
            Assert.AreEqual(0.6, result.Probabilities.Where(x => x.Type == typeof(double)).First().Probability);

        }
    }
}
