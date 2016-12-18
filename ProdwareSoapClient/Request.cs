using System;
using System.IO;
using System.Net;
using System.Xml;

namespace ProdwareSoapClient
{
    public class Request
    {
        string url = "http://192.168.93.222:8082/CRMPRODWARE/HttpFront.aspx"; //'Web service URL'
        string action = "/soap/action/query"; //the SOAP method/action name
        public Request()
        {
            
        }

        public int Submit(string xml)
        {

            try
            {

                Console.WriteLine("Submitting...");

                var soapEnvelopeXml = CreateSoapEnvelope(xml);
                var soapRequest = CreateSoapRequest(url, action);
                InsertSoapEnvelopeIntoSoapRequest(soapEnvelopeXml, soapRequest);

                using (var stringWriter = new StringWriter())
                {
                    using (var xmlWriter = XmlWriter.Create(stringWriter))
                    {
                        soapEnvelopeXml.WriteTo(xmlWriter);
                        xmlWriter.Flush();
                    }
                }

                // begin async call to web request.
                var asyncResult = soapRequest.BeginGetResponse(null, null);

                // suspend this thread until call is complete. You might want to
                // do something usefull here like update your UI.
                var success = asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5));

                if (!success)
                {
                    Console.Error.WriteLine("Async Call - Failed");
                    return 400;
                }

                // get the response from the completed web request.
                using (var webResponse = soapRequest.EndGetResponse(asyncResult))
                {
                    string soapResult;
                    var responseStream = webResponse.GetResponseStream();
                    if (responseStream == null)
                    {
                        Console.Error.WriteLine("response - Empty");
                        return 401;
                    }
                    using (var reader = new StreamReader(responseStream))
                    {
                        soapResult = reader.ReadToEnd();
                    }
                    return 200;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Submit - FAILED");
                throw ex;
            }
            
        }

        private static HttpWebRequest CreateSoapRequest(string url, string action)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            return webRequest;
        }

        private static XmlDocument CreateSoapEnvelope(string xml)
        {
            var soapEnvelope = new XmlDocument();
            soapEnvelope.LoadXml(xml); //the SOAP envelope to send
            return soapEnvelope;
        }

        private static void InsertSoapEnvelopeIntoSoapRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }
    }
}
