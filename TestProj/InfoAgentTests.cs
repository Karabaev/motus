namespace TestProj
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using InfoAgent.Moonwalk;

    [TestClass]
    public class InfoAgentTests
    {
        [TestMethod]
        public void GetAllMoonwalkFilmInfoListTest()
        {
            Service service = new Service();
            var result = service.GetAllFilmInfoList();
            Assert.IsTrue(result.Any());
        }
    }
}
