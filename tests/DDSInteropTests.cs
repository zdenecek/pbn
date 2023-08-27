using Microsoft.VisualStudio.TestTools.UnitTesting;
using pbn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests
{
    [TestClass()]
    public class AnalysisServiceDdsTests
    {
        private AnalysisService service;

        [TestInitialize()]
        public void Initialize()
        {
            service = new AnalysisServiceDDS();
        }


        [TestMethod()]
        public void CalcAllTablesPbnPbnTest()
        {
            var table = service.AnalyzePbn("N:A873.AJ83.52.Q32 95.74.AQJ84.T754 QT62.KQ62.K93.K8 KJ4.T95.T76.AJ96",
                Vulnerability.NONE);
            


            Assert.AreEqual(7, table.GetDoubleDummyTricks(Suit.NOTRUMP, Position.NORTH));
        }   
    }
}