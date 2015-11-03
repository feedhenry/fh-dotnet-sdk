using FHSDK;
using FHSDK.Config;
using Newtonsoft.Json.Linq;
using tests.Mocks;
using Xunit;

namespace tests.Config
{
    public class CloudPropsTest
    {
        [Fact]
        public void TestReadCloudPropsWithJsonContainingValues()
        {
            // given
            var json = JObject.Parse(@"{
                'url': 'URL',
                'hosts': {'host': 'HOST',  'environment': 'ENV'}
             }");
            var config = new FHConfig(new MockDeviceService());
            var props = new CloudProps(json, config);

            // when
            var host = props.GetCloudHost();
            var env = props.GetEnv();

            // then
            Assert.Equal("URL", host);
            Assert.Equal("ENV", env);
        }

        [Fact]
        public void TestURLFormatting()
        {
            // given
            var json = JObject.Parse(@"{
                'url': 'http://someserver.com/',
                'hosts': {'host': 'HOST',  'environment': 'ENV'}
             }");
            var config = new FHConfig(new MockDeviceService());
            var props = new CloudProps(json, config);

            // when
            var host = props.GetCloudHost();

            // then
            Assert.Equal("http://someserver.com", host);
        }

        [Fact]
        public void TestReadCloudPropsWithJsonEmptyValuesForUrlAndHostWithUrl()
        {
            // given
            var json = JObject.Parse(@"{
                'hosts': {'host': 'HOST', 'environment': 'ENV', 'url': 'URL_HOST'}
             }");
            var config = new FHConfig(new MockDeviceService());
            var props = new CloudProps(json, config);

            // when
            var host = props.GetCloudHost();
            var env = props.GetEnv();

            // then
            Assert.Equal("URL_HOST", host);
            Assert.Equal("ENV", env);
        }

        [Fact]
        public void TestReadCloudPropsWithJsonEmptyValuesForUrlAndHostWithoutUrl()
        {
            // given
            var json = JObject.Parse(@"{
                'hosts': {'host': 'HOST',  'environment': 'ENV', 'releaseCloudUrl': 'RELEASE_CLOUD_URL'}
             }");
            var config = new FHConfig(new MockDeviceService());
            var props = new CloudProps(json, config);

            // when
            var host = props.GetCloudHost();
            var env = props.GetEnv();

            // then
            Assert.Equal("RELEASE_CLOUD_URL", host);
            Assert.Equal("ENV", env);
        }
    }
}