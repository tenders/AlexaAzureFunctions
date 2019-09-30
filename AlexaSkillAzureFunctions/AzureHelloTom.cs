using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Alexa.NET.Request.Type;
using Alexa.NET;
using Alexa.NET.Response;
using Alexa.NET.Request;

namespace AlexaSkillAzureFunctions
{
    public static class AzureHelloTom
    {
        [FunctionName("AzureHelloTom")]
        public static async Task<SkillResponse> Run(
  [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "alexa/hellotom")]
  HttpRequest req)
        {
            // read content as skill request
            var payload = await req.ReadAsStringAsync();
            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(payload);

            // get type of request
            var requestType = skillRequest.GetRequestType();

            // handle launchrequest
            if (requestType == typeof(LaunchRequest))
            {
                return ResponseBuilder
                  .Tell("Hallo. Ich bin thomas erster alexa skill.");
            }

            // handle intentrequest
            if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                // handle greetingintent
                if (intentRequest.Intent.Name == "GetBirthday")
                {
                    var name = intentRequest.Intent.Slots["Name"].Value;
                    string geburtstag = string.Empty;
                    switch(name)
                    {
                        case "thomas":
                        case "papa":
                            {
                                geburtstag = "17.08.";
                                break;
                            }
                        case "bine":
                        case "mama":
                            {
                                geburtstag = "31.03.";
                                break;
                            }
                        case "zoe":
                            {
                                geburtstag = "29.03.";
                                break;
                            }
                        case "kaja":
                            {
                                geburtstag = "10.04.";
                                break;
                            }
                        default:
                            {
                                geburtstag = string.Empty;
                                break;
                            }
                    }

                    string title = "Geburtstag von " + name;
                    string response = string.Empty;
                    if(string.IsNullOrEmpty(geburtstag))
                    {
                        response = "Keeehne Ahnung";

                        return ResponseBuilder.TellWithCard(response, title, response);

                    }

                    response = $"{name} hat am {geburtstag} Geburtstag.";
                    return ResponseBuilder.TellWithCard(response, title, response);
                }
            }

            // default response
            return ResponseBuilder.Tell("Oops, da ist was schief gelaufen.");
        }
    }
}
