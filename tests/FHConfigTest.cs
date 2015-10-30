using System.Threading.Tasks;
using Xunit;
using FHSDK.Config;

namespace tests
{
    class FHConfigTest
    {
        [Fact]
        public async Task TestReadConfig()
        {
            // given
            var config = FHConfig.GetInstance();
            
            // when
            // default instanciation

            // then
            Assert.Equal("a94a8fe5ccb19ba61c4c0873d391e987982fbbd3", config.GetHost());
        }
    }
}
