# DataUtils.DataUrl

Class that can be used as a [value object](https://en.wikipedia.org/wiki/Value_object) representing a DataUrl ([rfc2397](https://tools.ietf.org/html/rfc2397))

- Parse DataUrl strings
- Construct DataUrl objects from byte array or string
- Cross platform, targets .NET Standard 2.0 
- Published as [Nuget package](	https://www.nuget.org/packages/DataUtils.DataUrl)

## Reading data URL's from string

```C#

// From data url string
var dataUrl = new DataUrl("data:image/gif...."); 

// Get content as string
dataUrl.ReadAsString();

// Get content as string with different encoding
dataUrl.ReadAsString(Encoding.UTF7);

// Get content as base64 encoded string
dataUrl.ReadAsStringBase64Encoded();

// Get raw content as byte array
dataUrl.Content

// Get content type
dataUrl.ContentType;

// Get possibly attached parameters as IEnumerable<KeyValuePair<string, string>>
dataUrl.Parameters; 

``` 

The constructor will throw `DataUrlParseException` if it's not possible to construct a DataUrl instance from the given string. You can also use `DataUrlParser.TryParse` if you don't want to deal with exceptions.

```C#
if (DataUrlParser.TryParse(dataUrlString, out var parsedDataUrl)) {
    // We could parse a data url
}
```

## Creating data URL's

```C#

// Create from string
var dataUrl = DataUrl.NewDataUrl(jsonString, "application/json");

// Create from image
var bytes = File.ReadAllBytes("C:\\somepath\\picture.png");
var dataUrl = DataUrl.NewDataUrl(bytes, "image/png");

// Create from already base64 encoded string
var dataUrl = DataUrl.NewFromBase64EncodedString(base64EncodedString, "image/png");

// Create a data URL with attached parameters
var parameters = new KeyValuePair<string, string> [] { 
    new KeyValuePair<string, string>("type", "token"), 
    new KeyValuePair<string, string>("authority", "https://...")
};
var dataUrl = DataUrl.NewDataUrl(
    jwtString, // Can also use a byte array
    "text/plain", 
    parameters
);

// When having another encoding other than default
DataUrl.NewDataUrl(myString, "text/plain", Encoding.UTF7);
DataUrl.NewDataUrl(myString, "text/plain", parameters, Encoding.UTF7);
```
