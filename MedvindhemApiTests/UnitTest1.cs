using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MedvindhemApi.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace MedvindhemApiTests
{
    [TestClass]
    public class UnitTest1
    {
        private IMemoryCache _cache;

        public UnitTest1(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
        [TestMethod]
        public void TestMethod1()            
        {
            var controller = new ApiController(_cache);
            Assert.AreEqual(controller.Get(), new[] { "value1", "value2" });
        }
    }
}
