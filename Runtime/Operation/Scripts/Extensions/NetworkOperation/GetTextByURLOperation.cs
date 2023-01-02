using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public class GetTextByURLOperation : GetByURLOperation<string>
    {
        public GetTextByURLOperation(string url) : base(url)
        {
        }

        protected override string CreateResultFromDownloadHandler(DownloadHandler handler)
        {
            return handler.text;
        }
    }
}