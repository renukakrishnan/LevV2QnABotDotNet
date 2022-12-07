using Amazon.LexModelsV2;
using Amazon.LexModelsV2.Model;
using Amazon.Runtime.Internal.Transform;
using LexV2QnABotApp.Extensions;
using LexV2QnABotApp.Models;
using LexV2QnABotApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LexV2QnABotApp.Controllers
{
    public class HelpDeskBotController : Controller
    {
        private ILexService awsLexSvc;
        private ISession userHttpSession;
        private string lexSessionID;
        private Dictionary<string, string> lexSessionData;
        private List<MessageViewModel> chatMessages = new List<MessageViewModel>();
        private string botMsgKey = "ChatBotMessages",
                       botAtrribsKey = "LexSessionData",
                       userSessionID = String.Empty;
        public HelpDeskBotController(ILexService awsLexService)
        {
            awsLexSvc = awsLexService;
        }

        public IActionResult Index(List<MessageViewModel> messages)
        {
            this.chatMessages.Clear();
            this.HttpContext.Session.Clear();
            chatMessages.Add(new MessageViewModel()
            {
                ChatMessage = "Hi there, Welcome! Type Hi anytime to start a conversation.",
                MessageType = "BotMessage"
            });
            return View(chatMessages);
        }


        public IActionResult Chat(List<MessageViewModel> messages)
        {
/*            for (int i = 0; i < 10; i++)
            {
                chatMessages.Add(new MessageViewModel(){
                    ChatMessage = "Hello",
                    MessageType = "BotMessage" }
                );
                chatMessages.Add(new MessageViewModel(){
                    ChatMessage = "Hi",
                    MessageType = "UserMessage"
                }
                );
            }*/
			return View(chatMessages);
        }


        [HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetChatMessage(string userMessage)
        {
           
            //Get user session and chat info
            userHttpSession = HttpContext.Session;
            userSessionID = userHttpSession.Id;
            chatMessages = userHttpSession.Get<List<MessageViewModel>>(botMsgKey) ?? new List<MessageViewModel>();
            lexSessionData = userHttpSession.Get<Dictionary<string, string>>(botAtrribsKey) ?? new Dictionary<string, string>();
            lexSessionID = userHttpSession.Get<string>("LexSessionID") ?? Guid.NewGuid().ToString();
                
            //No message was provided, return to current view
            if (String.IsNullOrEmpty(userMessage)) return View("Index", chatMessages);

            //A Valid Message exists, Add to page and allow Lex to process
            chatMessages.Add(new MessageViewModel()
            { ChatMessage = userMessage,
                MessageType = "UserMessage",
            });

            //await postUserData(chatMessages);

            //Call Amazon Lex with Text, capture response
            var lexResponse = await awsLexSvc.SendTextMsgToLex(userMessage, lexSessionData, lexSessionID);

            lexSessionData = lexResponse.SessionStateValue.SessionAttributes;
            if (lexResponse.SessionStateValue.DialogAction.Type == Amazon.LexRuntimeV2.DialogActionType.ElicitSlot ||
                lexResponse.SessionStateValue.DialogAction.Type == Amazon.LexRuntimeV2.DialogActionType.ConfirmIntent)
            {

                foreach (var message in lexResponse.Messages)
                {
                    if (message.ContentType == Amazon.LexRuntimeV2.MessageContentType.PlainText)
                    {
                        chatMessages.Add(
                            new MessageViewModel()
                            {
                                MessageType = "BotMessage",
                                ChatMessage = message.Content,
                                BotMessageContentType = message.ContentType.Value
                            });
                    }
                    else if (message.ContentType == Amazon.LexRuntimeV2.MessageContentType.ImageResponseCard)
                    {
                        var btntexts = new List<string>();
                        foreach (var v in message.ImageResponseCard.Buttons)
                        {
                            btntexts.Add(v.Text);
                        }
                        ImageResponseCardViewModel vm = new ImageResponseCardViewModel() {
                            Title = message.ImageResponseCard.Title,
                            reponseCardMessages = btntexts
                   
                        };

                        chatMessages.Add(
                            new MessageViewModel()
                            {
                                MessageType = "BotMessage",
                                ChatMessage = message.Content,
                                BotMessageContentType = message.ContentType.Value,
                                ResponseCardViewModel = vm
                            });
                    }
                }
            }
            else if (lexResponse.SessionStateValue.DialogAction.Type == Amazon.LexRuntimeV2.DialogActionType.Close)
            {
                foreach (var message in lexResponse.Messages)
                {
                    if (lexResponse.Interpretations[0].Intent.Name.Contains("Kendra"))
                    {
                        KendraResponseViewModel kvm = new KendraResponseViewModel();
                        if (lexResponse.RequestAttributes.ContainsKey("x-amz-lex:kendra-search-response-question_answer-question-2"))
                        {
                            kvm.RelatedQnAs.Add(lexResponse.RequestAttributes["x-amz-lex:kendra-search-response-question_answer-question-2"]);
                        }
                        if (lexResponse.RequestAttributes.ContainsKey("x-amz-lex:kendra-search-response-question_answer-question-3"))
                        {
                            kvm.RelatedQnAs.Add(lexResponse.RequestAttributes["x-amz-lex:kendra-search-response-question_answer-question-3"]);
                        }
                        if (lexResponse.RequestAttributes.ContainsKey("x-amz-lex:kendra-search-response-question_answer-question-4"))
                        {
                            kvm.RelatedQnAs.Add(lexResponse.RequestAttributes["x-amz-lex:kendra-search-response-question_answer-question-4"]);
                        }

                        chatMessages.Add(
                        new MessageViewModel()
                        {
                            MessageType = "BotMessage",
                            ChatMessage = message.Content ?? "For additional info, please reach out to us at @mpsa_team. Thank you!",
                            KendraResponseViewModel =kvm
                        });
                    }
                    else
                    {
                        chatMessages.Add(
                        new MessageViewModel()
                        {
                            MessageType = "BotMessage",
                            ChatMessage = message.Content ?? "For additional info, please reach out to us at @mpsa_team. Thank you!"
                        });
                    }

                    

                }
            }

            //Add updated botMessages and lexSessionData object to Session
            userHttpSession.Set<List<MessageViewModel>>(botMsgKey, chatMessages);
            userHttpSession.Set<Dictionary<string, string>>(botAtrribsKey, lexSessionData);
            userHttpSession.Set<string>("LexSessionID", lexSessionID);

            return View("Index", chatMessages);
        }

        public async Task<IActionResult> postUserData(List<MessageViewModel> messages)
        {
            //testing
            return await Task.Run(() => Index(messages));
        }

        public async Task<IActionResult> Clear()
        {
            this.chatMessages.Clear();
            chatMessages.Add(new MessageViewModel()
            {
                ChatMessage = "Hi there, Welcome! Type Hi anytime to start a coversation.",
                MessageType = "BotMessage"
            });
            
            this.HttpContext.Session.Clear();
            //lexSessionID
            return View("Index", chatMessages);
        }

    }
}
