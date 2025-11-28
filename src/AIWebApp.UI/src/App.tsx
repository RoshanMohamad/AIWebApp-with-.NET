import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { Bot, Home, MessageSquare, FileText, Image, TrendingUp } from 'lucide-react';
import HomePage from './pages/HomePage';
import ChatPage from './pages/ChatPage';
import SentimentPage from './pages/SentimentPage';
import DocumentPage from './pages/DocumentPage';
import ImagePage from './pages/ImagePage';
import './App.css';

function App() {
  return (
    <Router>
      <div className="app">
        <nav className="navbar">
          <div className="nav-brand">
            <Bot size={36} />
            <h1>AI Delights</h1>
          </div>
          <div className="nav-links">
            <Link to="/" className="nav-link">
              <Home size={20} />
              <span>Home</span>
            </Link>
            <Link to="/chat" className="nav-link">
              <MessageSquare size={20} />
              <span>Chat</span>
            </Link>
            <Link to="/sentiment" className="nav-link">
              <TrendingUp size={20} />
              <span>Sentiment</span>
            </Link>
            <Link to="/document" className="nav-link">
              <FileText size={20} />
              <span>Document</span>
            </Link>
            <Link to="/image" className="nav-link">
              <Image size={20} />
              <span>Image</span>
            </Link>
          </div>
        </nav>

        <main className="main-content">
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/chat" element={<ChatPage />} />
            <Route path="/sentiment" element={<SentimentPage />} />
            <Route path="/document" element={<DocumentPage />} />
            <Route path="/image" element={<ImagePage />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;
