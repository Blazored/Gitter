using System;

namespace Blazor.Gitter.Library
{
    public class ChatMessageEventArgs : EventArgs
    {
        public IChatMessage ChatMessage;
        public ChatMessageQuoteType QuoteType = ChatMessageQuoteType.Quote;
    }

}