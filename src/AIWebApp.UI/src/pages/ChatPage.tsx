import { useState } from 'react';
import { Send } from 'lucide-react';
import { chatApi, ChatResponse } from '../api/api';

interface Message {
  id: string;
  text: string;
  isUser: boolean;
  timestamp: Date;
}

function ChatPage() {
  const [messages, setMessages] = useState<Message[]>([]);
  const [input, setInput] = useState('');
  const [sessionId, setSessionId] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const sendMessage = async () => {
    if (!input.trim()) return;

    const userMessage: Message = {
      id: Date.now().toString(),
      text: input,
      isUser: true,
      timestamp: new Date(),
    };

    setMessages((prev) => [...prev, userMessage]);
    setInput('');
    setLoading(true);

    try {
      const response: ChatResponse = await chatApi.sendMessage({
        message: input,
        sessionId: sessionId || undefined,
      });

      if (!sessionId) {
        setSessionId(response.sessionId);
      }

      const aiMessage: Message = {
        id: (Date.now() + 1).toString(),
        text: response.response,
        isUser: false,
        timestamp: new Date(response.timestamp),
      };

      setMessages((prev) => [...prev, aiMessage]);
    } catch (error) {
      console.error('Error sending message:', error);
      const errorMessage: Message = {
        id: (Date.now() + 1).toString(),
        text: 'Sorry, there was an error processing your request.',
        isUser: false,
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, errorMessage]);
    } finally {
      setLoading(false);
    }
  };

  const handleKeyPress = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      sendMessage();
    }
  };

  return (
    <div className="page-container">
      <h1 className="page-title">AI Chatbot</h1>
      <p className="page-subtitle">Chat with our AI assistant powered by GPT-4</p>

      <div className="chat-container">
        <div className="messages-area">
          {messages.length === 0 && (
            <div style={{ textAlign: 'center', opacity: 0.5, marginTop: '2rem' }}>
              <p>Start a conversation by typing a message below.</p>
            </div>
          )}
          {messages.map((message) => (
            <div key={message.id} className={`message ${message.isUser ? 'user' : 'ai'}`}>
              <div className="message-content">
                <p>{message.text}</p>
              </div>
            </div>
          ))}
          {loading && (
            <div className="message ai">
              <div className="message-content">
                <p>Thinking...</p>
              </div>
            </div>
          )}
        </div>

        <div className="input-area">
          <input
            type="text"
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyPress={handleKeyPress}
            placeholder="Type your message..."
            disabled={loading}
          />
          <button onClick={sendMessage} disabled={loading || !input.trim()}>
            <Send size={20} />
          </button>
        </div>
      </div>
    </div>
  );
}

export default ChatPage;
