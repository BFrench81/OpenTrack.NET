using System;
using System.IO;

namespace OpenTrack.Utilities
{
    public interface IMessageLogger
    {
        void MessageSending(string sendContent);

        void MessageReceived(string replyContent);
    }

    public class FileMessageLogger : IMessageLogger
    {
        public string Path { get; set; }


        public void MessageSending(string sendContent)
        {
            var fileName = string.Format("Request.{0}.xml", Guid.NewGuid());
            var filePath = string.IsNullOrWhiteSpace(this.Path) ? fileName : System.IO.Path.Combine(this.Path, fileName);

            using (var w = new StreamWriter(filePath))
            {
                w.Write(sendContent);
            }
        }

        public void MessageReceived(string replyContent)
        {
            var fileName = string.Format("Response.{0}.xml", Guid.NewGuid());
            var filePath = string.IsNullOrWhiteSpace(this.Path) ? fileName : System.IO.Path.Combine(this.Path, fileName);

            using (var w = new StreamWriter(filePath))
            {
                w.Write(replyContent);
            }
        }
    }

    internal class DelegateMessageLogger : IMessageLogger
    {
        private readonly Action<string> _messageSending;
        private readonly Action<string> _messageReceived;

        public DelegateMessageLogger(Action<string> messageSending, Action<string> messageReceived)
        {
            _messageSending = messageSending;
            _messageReceived = messageReceived;
        }

        public void MessageSending(string sendContent)
        {
            if (this._messageSending == null)
            {
                return;
            }
            this._messageSending(sendContent);
        }

        public void MessageReceived(string replyContent)
        {
            if (this._messageReceived == null)
            {
                return;
            }
            this._messageReceived(replyContent);
        }
    }
}