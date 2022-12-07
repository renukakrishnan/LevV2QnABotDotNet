using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.LexModelsV2;
using Amazon.CognitoIdentity;
using Amazon.CognitoIdentity.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Amazon.LexRuntimeV2;
using Amazon.LexRuntimeV2.Model;
using System.IO;
using Amazon.Runtime;
using Microsoft.Extensions.Options;

namespace LexV2QnABotApp.Services
{
    public class LexService : ILexService
    {

        private AmazonLexRuntimeV2Client awsLexClient;
        private CognitoAWSCredentials credentials;
        private Dictionary<string, string> _lexSessionAttribs;
        private AWSSettings awssettings;
        //private string SessionId;

        public LexService(IOptions<AWSSettings> settings)
        { 
            //Get credentials from Cognito
            
            awssettings = settings.Value;
            credentials = new CognitoAWSCredentials(awssettings.CognitoPoolID, Amazon.RegionEndpoint.USEast1);
            //Instantiate Lex Client with Region
            
            awsLexClient = new AmazonLexRuntimeV2Client(credentials, Amazon.RegionEndpoint.USEast1);
            
        }
        public string PostContentToLex(string messageToSend)
        {
            return "test";
        }

        public async Task<RecognizeTextResponse> SendTextMsgToLex(string messageToSend, Dictionary<string, string> lexSessionAttributes, string sessionID)
        {
            RecognizeTextResponse lexTextResponse = await SendTextMsgToLex(messageToSend,sessionID);
            return lexTextResponse;
        }

        public async Task<RecognizeTextResponse> SendTextMsgToLex(string messageToSend,string sessionID)
        {
            
            RecognizeTextResponse lexTextResponse = new RecognizeTextResponse();
            RecognizeTextRequest lexTextRequest = new RecognizeTextRequest()
            {
                
                BotAliasId = awssettings.BotAliasID,
                BotId = awssettings.BotID,
                Text = messageToSend,
                LocaleId = "en_US",
                SessionId = sessionID

            };
            
            try
            {
                lexTextResponse = await awsLexClient.RecognizeTextAsync(lexTextRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lexTextResponse;
        }

        /*public async Task<Stream> SendAudioMsgToLex(Stream audioToSend)
        {
            throw new NotImplementedException();
        }*/
    }
}
