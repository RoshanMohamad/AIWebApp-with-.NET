import { Link } from 'react-router-dom';
import { MessageSquare, TrendingUp, FileText, Image, Sparkles } from 'lucide-react';

function HomePage() {
  return (
    <div className="page-container">
      <h1 className="page-title">Welcome to AI-Powered Web App</h1>
      <p className="page-subtitle">
        Experience the power of artificial intelligence with our suite of AI tools.
        Choose a feature below to get started.
      </p>

      <div className="feature-grid">
        <Link to="/chat" className="feature-card">
          <MessageSquare size={48} className="feature-icon" />
          <h3>AI Chatbot</h3>
          <p>Have a conversation with our intelligent AI assistant</p>
        </Link>

        <Link to="/sentiment" className="feature-card">
          <TrendingUp size={48} className="feature-icon" />
          <h3>Sentiment Analysis</h3>
          <p>Analyze the emotional tone of any text</p>
        </Link>

        <Link to="/document" className="feature-card">
          <FileText size={48} className="feature-icon" />
          <h3>Document Summarization</h3>
          <p>Get concise summaries of long documents</p>
        </Link>

        <Link to="/image" className="feature-card">
          <Image size={48} className="feature-icon" />
          <h3>Image Recognition</h3>
          <p>Analyze and understand images with AI</p>
        </Link>
      </div>

      <div style={{ marginTop: '3rem', textAlign: 'center' }}>
        <Sparkles size={32} style={{ color: '#667eea', marginBottom: '1rem' }} />
        <h2>Powered by OpenAI GPT-4</h2>
        <p style={{ opacity: 0.7, marginTop: '0.5rem' }}>
          Built with .NET 8, ASP.NET Core, React, and OpenAI
        </p>
      </div>
    </div>
  );
}

export default HomePage;
