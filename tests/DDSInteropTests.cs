using pbn.model;
using pbn.service;

namespace tests
{
    [TestClass()]
    public class AnalysisServiceDdsTests
    {
        private IAnalysisService service = null!;

        [TestInitialize()]
        public void Initialize()
        {
            service = new DdsAnalysisService();
        }


        [TestMethod()]
        public void CalcAllTablesPbnPbnTest()
        {
            var table = service.AnalyzePbn("N:A873.AJ83.52.Q32 95.74.AQJ84.T754 QT62.KQ62.K93.K8 KJ4.T95.T76.AJ96",
                Vulnerability.None);
            


            Assert.AreEqual(7, table.GetDoubleDummyTricks(Suit.Notrump, Position.North));
        }   
    }
}