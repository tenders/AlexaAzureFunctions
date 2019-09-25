using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AlexaSkillAzureFunctions
{
    public static class AzureHelloWorld
    {
        [FunctionName("AzureHelloWorld")]
        public static object Run(
  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alexa/helloworld")]
  HttpRequest req)
        {
            return new
            {
                version = "1.0",
                response = new
                {
                    outputSpeech = new
                    {
                        type = "PlainText",
                        text = "Hello World from an Azure Function!"
                    },
                    shouldEndSession = true
                }
            };
        }
    }
}
