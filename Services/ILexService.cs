using Amazon.LexModelsV2;
using Amazon.LexRuntimeV2.Model;

namespace LexV2QnABotApp.Services
{
    public interface ILexService
    {
        string PostContentToLex(string messageToSend);

        Task<RecognizeTextResponse> SendTextMsgToLex(string messageToSend, Dictionary<string, string> lexSessionAttributes, string sessionId);

        Task<RecognizeTextResponse> SendTextMsgToLex(string messageToSend, string sessionID);

        //Task<Stream> SendAudioMsgToLex(Stream audioToSend);
    }
}
