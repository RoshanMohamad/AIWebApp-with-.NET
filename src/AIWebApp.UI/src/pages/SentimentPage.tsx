import { useState } from 'react';
import { sentimentApi, SentimentResponse } from '../api/api';

function SentimentPage() {
  const [text, setText] = useState('');
  const [result, setResult] = useState<SentimentResponse | null>(null);
  const [loading, setLoading] = useState(false);

  const analyzeSentiment = async () => {
    if (!text.trim()) return;

    setLoading(true);
    try {
      const response = await sentimentApi.analyze({ text });
      setResult(response);
    } catch (error) {
      console.error('Error analyzing sentiment:', error);
      alert('Error analyzing sentiment. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const getSentimentClass = (sentiment: string) => {
    const s = sentiment.toLowerCase();
    if (s === 'positive') return 'sentiment-positive';
    if (s === 'negative') return 'sentiment-negative';
    return 'sentiment-neutral';
  };

  return (
    <div className="page-container">
      <h1 className="page-title">Sentiment Analysis</h1>
      <p className="page-subtitle">Analyze the emotional tone of your text</p>

      <div className="form-group">
        <label htmlFor="text">Enter Text to Analyze</label>
        <textarea
          id="text"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Type or paste your text here..."
        />
      </div>

      <button onClick={analyzeSentiment} disabled={loading || !text.trim()}>
        {loading ? 'Analyzing...' : 'Analyze Sentiment'}
      </button>

      {result && (
        <div className="result-card">
          <h3>Analysis Result</h3>
          <div className={`sentiment-badge ${getSentimentClass(result.sentiment)}`}>
            {result.sentiment}
          </div>
          <p>
            <strong>Confidence:</strong> {(result.confidence * 100).toFixed(1)}%
          </p>
          <p style={{ marginTop: '1rem' }}>
            <strong>Explanation:</strong> {result.explanation}
          </p>
        </div>
      )}
    </div>
  );
}

export default SentimentPage;
