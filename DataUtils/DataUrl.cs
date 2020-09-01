using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DataUtils
{
    /// <summary>
    /// Class that can be used as value object representing a DataUrl (rfc2397).
    /// </summary>
    /// <seealso cref="NewDataUrl(string, string, Encoding)"/>
    public struct DataUrl
    {
        /// <summary>
        /// Create new DataUrl from string
        /// </summary>
        /// <param name="content">The content that should be embedded in the DataUrl</param>
        /// <param name="contentType">Conten type (mime) of the embedded content</param>
        /// <param name="enc">Encoding.Default will be used by default of no other Encoding is provided</param>
        /// <returns>The constructed DataUrl instance</returns>
        public static DataUrl NewDataUrl(string content, string contentType, Encoding enc = null)
        {
            return NewDataUrl(content, contentType, null, enc);
        }

        /// <summary>
        /// Create new DataUrl from string
        /// </summary>
        /// <param name="content">The content that should be embedded in the DataUrl</param>
        /// <param name="contentType">Conten type (mime) of the embedded content</param>
        /// <param name="parameters">List of key-value pair that should be attached in the DataUrl</param>
        /// <param name="enc">Encoding.Default will be used by default of no other Encoding is provided</param>
        /// <returns>The constructed DataUrl instance</returns>
        public static DataUrl NewDataUrl(string content, string contentType, IEnumerable<KeyValuePair<string, string>> parameters = null, Encoding enc = null)
        {
            if (enc == null)
            {
                enc = Encoding.Default;
            }
            var bytes = enc.GetBytes(content);
            return NewDataUrl(bytes, contentType, parameters);
        }

        /// <summary>
        /// Create new DataUrl from byte array
        /// </summary>
        /// <param name="content">The content that should be embedded in the DataUrl</param>
        /// <param name="contentType">Conten type (mime) of the embedded content</param>
        /// <param name="parameters">List of key-value pair that should be attached in the DataUrl</param>
        /// <returns>The constructed DataUrl instance</returns>
        public static DataUrl NewDataUrl(byte[] content, string contentType, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            var base64Bytes = Encoding.ASCII.GetBytes(Convert.ToBase64String(content));
            return new DataUrl(
                base64Bytes,
                contentType,
                isBase64Encoded: true,
                parameters: parameters
            );
        }

        /// <summary>
        /// Create new DataUrl from already base64 encoded content
        /// </summary>
        /// <param name="base64EncodedContent">The content that should be embedded in the DataUrl</param>
        /// <param name="contentType">Conten type (mime) of the embedded content</param>
        /// <param name="parameters">List of key-value pair that should be attached in the DataUrl</param>
        /// <returns>The constructed DataUrl instance</returns>
        public static DataUrl NewFromBase64EncodedString(string base64EncodedContent, string contentType, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            var base64Bytes = Encoding.ASCII.GetBytes(base64EncodedContent);
            return new DataUrl(
                base64Bytes,
                contentType,
                isBase64Encoded: true,
                parameters: parameters
            );
        }

        /// <summary>
        /// Tells whether or not the content is base64 encoded. All DataUrl instances created with the
        /// static factory methods above will generate base64 encoded content, this library provides no other 
        /// options. There's however possible to construct DataUrl instances from string, which content is not
        /// base64 encoded, hence we need this flag to know how to handle the content when returning it 
        /// as a string.
        /// </summary>
        private bool _isBase64Encoded;


        /// <summary>
        /// Create new DataUrl instance from a DataUrl formatted string (rfc2397)
        /// </summary>
        /// <exception cref="DataUrlParseException"></exception>
        /// <param name="dataUrl">DataUrl formatted string (rfc2397)</param>
        /// <returns></returns>
        public DataUrl(string dataUrl)
        {
            var parsed = DataUrlParser.Parse(dataUrl);
            Content = parsed.Content;
            ContentType = parsed.ContentType;
            Parameters = parsed.Parameters;
            _isBase64Encoded = parsed._isBase64Encoded;
        }

        internal DataUrl(byte[] content, string contentType, bool isBase64Encoded, IEnumerable<KeyValuePair<string, string>> parameters = null)
        {
            Content = content;
            ContentType = contentType;
            _isBase64Encoded = isBase64Encoded;
            Parameters = parameters ?? new KeyValuePair<string, string> [] { };
        }

        /// <summary>
        /// The content type of the embedded content 
        /// </summary>
        public string ContentType { get; internal set; }

        /// <summary>
        /// Embedded content as a byte array
        /// </summary>
        /// <seealso cref="ReadAsString"/>
        /// <seealso cref="ReadAsString(Encoding)"/>
        public byte[] Content { get; internal set; }

        /// <summary>
        /// List of paramateras attached to the DataUrl
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Parameters { get; internal set; }

        /// <summary>
        /// Get the content of the DataUrl as string (not base64 encoded)
        /// </summary>
        /// <seealso cref="ReadAsBase64EncodedString"/>
        /// <seealso cref="Content"/>
        /// <returns>Content of the DataUrl</returns>
        public string ReadAsString()
        {
            return ReadAsString(Encoding.Default);
        }

        /// <summary>
        /// Get the content of the DataUrl as string, knowing that the content has an encoding which 
        /// is not Encoding.Default
        /// </smmary>
        /// <param name="enc">The expected coding </param>
        /// <returns>Content of the DataUrl</returns>
        public string ReadAsString(Encoding enc)
        {
            if (_isBase64Encoded)
            {
                string ascii = Encoding.ASCII.GetString(Content);
                byte[] data = Convert.FromBase64String(ascii);
                return enc.GetString(data);
            } else
            {
                return enc.GetString(Content);
            }
        }

        /// <summary>
        /// Get the content of the DataUrl as a base64 encoded string
        /// </summary>
        /// <seealso cref="Content"/>
        /// <returns>Content of the DataUrl</returns>
        public string ReadAsBase64EncodedString()
        {
            if (_isBase64Encoded)
            {
                return Encoding.ASCII.GetString(Content);
            } else
            {
                return Convert.ToBase64String(Content);
            }
        }

        /// <summary>
        /// Returns the string representation of this DataUrl instance (rfc2397) 
        /// </summary>
        /// <returns>rfc2397</returns>
        public new string ToString()
        {
            var str = new StringBuilder("data:" + ContentType);
            if (_isBase64Encoded)
            {
                str.Append(";base64");
            }

            foreach (var param in Parameters)
            {
                str.Append(";" + WebUtility.UrlEncode(param.Key) + "=" + WebUtility.UrlEncode(param.Value));
            }

            str.Append(",");
            str.Append(Encoding.ASCII.GetString(Content));

            return str.ToString();
        }

    }
}
