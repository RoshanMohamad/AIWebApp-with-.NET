import { Link } from 'react-router-dom';
import { MessageSquare, TrendingUp, FileText, Image, Sparkles } from 'lucide-react';

function HomePage() {
  return (
    <div className="page-container">
      <h1 className="page-title" style={{ marginBottom: '0.5rem' }}>
        Your AI-Powered <span style={{ color: '#EF233C' }}>Delights</span>
      </h1>
      <p className="page-subtitle" style={{ marginBottom: '2rem' }}>
        Journey through our AI features, a haven for intelligent solutions,
        text analysis, and document processing - where every feature is pure excellence
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
        <Sparkles size={36} style={{ color: '#EF233C', marginBottom: '1rem' }} />
        <h2 style={{ color: '#4A1C1C', marginBottom: '0.5rem' }}>Powered by Google Gemini AI</h2>
        <p style={{ color: '#6B3030', opacity: 0.8, marginTop: '0.5rem' }}>
          Built with .NET 10, ASP.NET Core, React, and Google Gemini
        </p>
      </div>
    </div>
  );
}

export default HomePage;
