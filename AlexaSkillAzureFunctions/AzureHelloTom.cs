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

            Session session = skillRequest.Session;
            
            // get type of request
            var requestType = skillRequest.GetRequestType();

            // handle launchrequest
            if (requestType == typeof(LaunchRequest))
            {
                return ResponseBuilder
                  .Tell("Hallo. Ich bin thomas erster alexa skill.");
            }

            if (requestType == typeof(Alexa.NET.Request.Type.SessionEndedRequest))
            {
                return ResponseBuilder
                  .Tell("Servus. Ich bin dann mal weg.");
            }

            // handle intentrequest
            if (requestType == typeof(IntentRequest))
            {
                var intentRequest = skillRequest.Request as IntentRequest;

                // handle greetingintent
                if (intentRequest.Intent.Name == "GetBirthday")
                {
                    return ProcessBirthdayIntentRequest(intentRequest, session);
                }

                return ResponseBuilder.TellWithCard("Ciao.", "Unbekannter IntentRequest", intentRequest.Intent.Name);
            }

            // default response
            string cardTitle = "Ciao";
            string cardMessage = requestType.FullName;
            return ResponseBuilder.TellWithCard("Oops, da ist was schief gelaufen.", cardTitle, cardMessage);
        }

        public static SkillResponse ProcessBirthdayIntentRequest(IntentRequest intentRequest, Session session)
        {
            var name = intentRequest.Intent.Slots["Name"].Value;
            string geburtstag = string.Empty;
            switch (name)
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

            string title = session.SessionId + ": Geburtstag von " + name;
            string response = string.Empty;
            if (string.IsNullOrEmpty(geburtstag))
            {
                response = "Keeehne Ahnung";
                SkillResponse kaResp = ResponseBuilder.TellWithCard(response, title, response);

                return kaResp;
            }

            response = $"{name} hat am {geburtstag} Geburtstag."; // + Environment.NewLine + session.SessionId;
            var speech = new SsmlOutputSpeech();
            speech.Ssml = "<speak>Das ist meine antwort</speak>";
            var respMessage = new PlainTextOutputSpeech();
            respMessage.Text = response;

            ResponseBody body = new ResponseBody();
            body.OutputSpeech = respMessage;
            //body.ShouldEndSession = false;
            body.Card = new SimpleCard { Title = title, Content = response };

            //SkillResponse resp = ResponseBuilder.TellWithCard(response, title, response);
            SkillResponse resp = new SkillResponse();
            resp.Response = body;
            resp.Version = "1.0";
            return resp;
        }
    }
}
