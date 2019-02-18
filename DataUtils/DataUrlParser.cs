using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DataUtils
{
    public static class DataUrlParser
    {
        /// <summary></summary>
        /// <exception cref="DataUrlParseException"></exception>
        /// <seealso cref="DataUrl"/>
        /// <param name="dataUrl"></param>
        /// <returns></returns>
        internal static DataUrl Parse(string dataUrl)
        {
            if (dataUrl == null)
            {
                ThrowDueToInvalidArgument(dataUrl, typeof(DataUrlParser) + " can not handle null values");
            }
            if (dataUrl.Substring(0, 5).ToLower() != "data:")
            {
                ThrowDueToInvalidArgument(dataUrl, "DataUrl does not begin with 'data:'");
            }

            // Split up between specification and the data
            dataUrl = dataUrl.Substring(5);
            var firstCommaOffest = dataUrl.IndexOf(',');
            if (firstCommaOffest == -1)
            {
                ThrowDueToInvalidArgument(dataUrl, "Missing comma sign");
            }
            var specification = dataUrl.Substring(0, firstCommaOffest);
            var data = dataUrl.Substring(firstCommaOffest + 1);

            // Split data specification and "dequeue" the content type
            var specificationParts = new List<string>(specification.Split(';'));
            var contentType = specificationParts[0];
            specificationParts.RemoveAt(0);

            // Collect parameters declared in the specification and determin if 
            // data is base64 encoded
            var isBase64Encoded = false;
            var parameters = new List<KeyValuePair<string, string>>();
            foreach(var parameterData in specificationParts)
            {
                var parameterDataParts = parameterData.Split('=');
                var paramName = parameterDataParts[0].Trim();
                var paramValue = parameterDataParts.Length > 1 ? parameterDataParts[1].Trim() : "";
                var decodedParamName = WebUtility.UrlDecode(paramName);
                var decodedValue = WebUtility.UrlDecode(paramValue);
                if (paramName.ToLower() == "base64")
                {
                    isBase64Encoded = true;
                } else
                {
                    parameters.Add(new KeyValuePair<string, string>(paramName, decodedValue));
                }
            }

            return new DataUrl(
                Encoding.ASCII.GetBytes(data), 
                contentType,
                isBase64Encoded,
                parameters
            );
        }

        private static void ThrowDueToInvalidArgument(string dataUrl, string message = "")
        {
            if (message == "")
            {
                throw new DataUrlParseException("DataUrl \"" + dataUrl + "\" is invalid");
            } else
            {
                throw new DataUrlParseException(message + " (dataUrl=\"" + dataUrl + "\")");
            }
        }

        /// <summary>
        /// Class that can be used to parse DataUrl formatted strings, without exception being
        /// thrown in case of a invalid string format 
        /// </summary>
        /// <param name="dataUrl">The DataUrl string (rfc2397)</param>
        /// <param name="outDataUrl"></param>
        public static void TryParse(string dataUrl, out DataUrl outDataUrl)
        {
            try
            {
                outDataUrl = Parse(dataUrl);
                return;
            } catch { }
            outDataUrl = null;
        }
    }
}
