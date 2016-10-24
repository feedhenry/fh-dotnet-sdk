#if __MOBILE__
using Xunit;
using TestMethod = Xunit.FactAttribute;
#else
using System.Net;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif
using System.Collections.Generic;
using System.Net;
using FHSDK;
using Newtonsoft.Json;

namespace tests
{
    [TestClass]
    public class ResponseTest
    {
        [TestMethod]
        public void DeserialseResponseTest()
        {
            //given
            var response = new FHResponse(HttpStatusCode.OK, "[{ \"name\": \"test\" }, { \"name\": \"erik\" }]");

            //when
            var result = response.GetResponseAs<List<Person>>();

            //then
            Assert.IsNotNull(result);
            Assert.AreEqual(result[0].Name, "test");
        }
    }

    internal class Person
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}