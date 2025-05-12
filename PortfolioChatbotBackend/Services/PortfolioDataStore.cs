

namespace PortfolioChatbotBackend.Services
{
    public class PortfolioDataStore
    {
        private readonly Dictionary<string, string> _documentContent = new();

        public void StoreDocument(string documentId, string content)
        {
            _documentContent[documentId] = content;
        }

        public string GetDocumentContent(string documentId)
        {
            return _documentContent.TryGetValue(documentId, out var content) ? content : string.Empty;
        }

        public IEnumerable<string> GetAllDocumentIds()
        {
            return _documentContent.Keys;
        }
    }

}