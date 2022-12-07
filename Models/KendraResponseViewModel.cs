namespace LexV2QnABotApp.Models
{
    public class KendraResponseViewModel
    {
       public List<string> RelatedQnAs { get; set; }

        public KendraResponseViewModel()
        {
            RelatedQnAs = new List<string>();
        }
    }
}
