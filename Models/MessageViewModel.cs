namespace LexV2QnABotApp.Models
{
    public class MessageViewModel
    {
        public string ChatMessage { get; set; }
       
        public string MessageType { get; set; }
        public string BotMessageContentType { get; set; }
        public ImageResponseCardViewModel ResponseCardViewModel{get; set;}

        public KendraResponseViewModel KendraResponseViewModel { get; set; }

    }
}
