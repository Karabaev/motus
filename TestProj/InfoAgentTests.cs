namespace TestProj
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using InfoAgent;

    [TestClass]
    public class InfoAgentTests
    {
        [TestMethod]
        public void GetAllMoonwalkFilmInfoListTest()
        {
            InfoAgentService service = new InfoAgentService();
            service.GetAllMoonwalkFilmInfoList();
        }
    }
}
