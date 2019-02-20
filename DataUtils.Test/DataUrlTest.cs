using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace DataUtils.Test
{
    public class DataUrlTest
    {
        public const string Content = "This is Sparta";
        public const string Base64Content = "VGhpcyBpcyBTcGFydGE="; // This is Sparta

        [Fact]
        public void test_that_we_can_parse_data_url()
        {
            var theory = new Dictionary<string, DataUrlSpec>()
            {
                {
                    "data:text/plain;base64," + Base64Content,
                    new DataUrlSpec() {ContentType="text/plain", ContentRaw = Content}
                },
                {
                    "data:text/plain," + Content,
                    new DataUrlSpec() {ContentType="text/plain", ContentRaw = Content}
                },
                {
                    "data:text/plain;base64;paramX=monkey;paramY=frodo+baggins," + Base64Content,
                    new DataUrlSpec() {
                        ContentType ="text/plain",
                        ContentRaw = Content,
                        Parameters = new KeyValuePair<string, string> [] {
                            new KeyValuePair<string, string>("paramX", "monkey"),
                            new KeyValuePair<string, string>("paramY", "frodo baggins")
                        }
                    }
                }
            };

            foreach (var data in theory)
            {
                var input = data.Key;
                var expected = data.Value;
                var result = new DataUrl(input);

                Assert.Equal(expected.ContentType, result.ContentType);
                Assert.Equal(expected.ContentRaw, result.ReadAsString());
                Assert.Equal(Base64Content, result.ReadAsBase64EncodedString());
                Assert.Equal(expected.Parameters.Count(), result.Parameters.Count());
                Assert.Equal(input, result.ToString());

                foreach (var param in expected.Parameters)
                {
                    var found = result.Parameters.Where(x => x.Key == param.Key && x.Value == param.Value);
                    Assert.True(found.Count() == 1, "Expected to find a parameter with name " + param.Key);
                }
            }
        }


        [Fact]
        public void test_that_we_can_create_data_urls()
        {
            var noneAsciiContent = "ÄÖÅ entré";
            var nonAsciiContentBase64 = "w4TDlsOFIGVudHLDqQ==";
            var theory = new Dictionary<string, DataUrlSpec>()
            {
               /* {
                    "data:text/plain;base64," + Base64Content,
                    new DataUrlSpec() {ContentType="text/plain", ContentBase64 = Base64Content, NoneBase64Content = Content}
                }, */
                {
                    "data:text/plain;base64;paramX=monkey;paramY="+WebUtility.UrlEncode("frodo baggins äöå")+"," + nonAsciiContentBase64,
                    new DataUrlSpec() {
                        ContentType ="text/plain",
                        ContentBase64 = nonAsciiContentBase64,
                        ContentRaw = noneAsciiContent,
                        Parameters = new KeyValuePair<string, string> [] {
                            new KeyValuePair<string, string>("paramX", "monkey"),
                            new KeyValuePair<string, string>("paramY", "frodo baggins äöå")
                        }
                    }
                }
            };

            foreach (var data in theory)
            {
                var expected = data.Key;
                var input = data.Value;
                DataUrl result;
                result = DataUrl.NewDataUrl(input.ContentRaw, input.ContentType, input.Parameters);

                Assert.Equal(input.ContentType, result.ContentType);
                Assert.Equal(input.ContentRaw, result.ReadAsString());
                Assert.Equal(input.ContentBase64, result.ReadAsBase64EncodedString());
                Assert.Equal(input.Parameters.Count(), result.Parameters.Count());
                Assert.Equal(expected, result.ToString());

                foreach (var param in input.Parameters)
                {
                    var found = result.Parameters.Where(x => x.Key == param.Key && x.Value == param.Value);
                    Assert.True(found.Count() == 1, "Expected to find a parameter with name " + param.Key);
                }

            }
        }

        [Fact]
        public void test_that_we_can_create_data_url_from_already_base64_encoded_string()
        {
            var theory = new Dictionary<string, DataUrlSpec>()
            {
                {
                    "data:text/plain;base64," + Base64Content,
                    new DataUrlSpec() {ContentType="text/plain", ContentRaw = Content}
                },
                {
                    "data:text/plain;base64;paramX=monkey;paramY=frodo+baggins," + Base64Content,
                    new DataUrlSpec() {
                        ContentType ="text/plain",
                        ContentRaw = Content,
                        Parameters = new KeyValuePair<string, string> [] {
                            new KeyValuePair<string, string>("paramX", "monkey"),
                            new KeyValuePair<string, string>("paramY", "frodo baggins")
                        }
                    }
                }
            };

            foreach (var data in theory)
            {
                var expected = data.Key;
                var input = data.Value;
                var result = DataUrl.NewFromBase64EncodedString(Base64Content, input.ContentType, input.Parameters);

                Assert.Equal(input.ContentType, result.ContentType);
                Assert.Equal(input.ContentRaw, result.ReadAsString());
                Assert.Equal(Base64Content, result.ReadAsBase64EncodedString());
                Assert.Equal(input.Parameters.Count(), result.Parameters.Count());
                Assert.Equal(expected, result.ToString());

                foreach (var param in input.Parameters)
                {
                    var found = result.Parameters.Where(x => {
                        return x.Key == param.Key && x.Value == param.Value;
                    });
                    Assert.True(found.Count() == 1, "Expected to find a parameter with name " + param.Key);
                }
            }
        }

        [Fact]
        public void test_that_we_can_try_to_parse_jibberisch_without_crashing()
        {
            var parsedJibberisch = DataUrlParser.TryParse("jibberisch", out var dataUrl);
            Assert.False(parsedJibberisch);
            Assert.Null(dataUrl);

            var parsedNull = DataUrlParser.TryParse(null, out dataUrl);
            Assert.False(parsedNull);
            Assert.Null(dataUrl);
        }

        [Fact]
        public void test_that_we_can_try_to_parse_valid_data_url()
        {
            var dataUrlString = "data:text/plain;base64," + Base64Content;
            var parsed = DataUrlParser.TryParse(dataUrlString, out var dataUrl);
            Assert.True(parsed);
            Assert.NotNull(dataUrl);
            Assert.Equal("text/plain", dataUrl.ContentType);
            Assert.Equal(Content, dataUrl.ReadAsString());
            Assert.Equal(Base64Content, dataUrl.ReadAsBase64EncodedString());
            Assert.Equal(dataUrlString, dataUrl.ToString());
        }
    }

    class DataUrlSpec
    {
        public string ContentType;
        public string ContentBase64;
        public string ContentRaw;
        public IEnumerable<KeyValuePair<string, string>> Parameters = new KeyValuePair<string, string>[] {};
    }
}
