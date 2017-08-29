using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using Java.Lang;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using GOT.Models;

namespace GOT.API
{
    public class ComputerVisionAPI
    {
        // Replace it with your subscription key here
        string subscriptionKey =  Properties.Resources.ApiKey;

        // This link is location based, you need to change it according to your location
        // You can find all about this in https://azure.microsoft.com/fr-fr/try/cognitive-services/

        const string baseUri = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/recognizeText";
        string operationLocation = null;
        HttpClient client;

        public async Task<Rootobject> Scan(Stream stream)
        {
            // The Scan happens in to phases

            // The first Scans the user's input and send it to the server (Link Above) 
            // And retrieves the location that will be the target of the second request

            await PostImage(stream);

            // The second request uses the operation location that was retrieved and gets the final result
            // that will be converted using NewtonSoft from Json to a C# class

            return await GetResults();
        }
        private async Task PostImage(Stream stream)
        {
            try
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                string uri = baseUri + "?" + "handwriting=true";

                StreamContent content = new StreamContent(stream);

                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                HttpResponseMessage response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                    operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
                else
                {
                    // Display the JSON error data.
                    Debug.WriteLine("\nError:\n");
                    Debug.WriteLine(await response.Content.ReadAsStringAsync());
                    return;
                }
            }
            catch(System.Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private async Task<Rootobject> GetResults()
        {
            string contentString;
            int i = 0;
            do
            {
                Thread.Sleep(1000);
                var response = await client.GetAsync(operationLocation);
                contentString = await response.Content.ReadAsStringAsync();
                ++i;
            }
            while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);

            if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
            {
                Debug.WriteLine("\nTimeout error.\n");
                return null;
            }
            
            return JsonConvert.DeserializeObject<Rootobject>(contentString);
        }
    }
}
