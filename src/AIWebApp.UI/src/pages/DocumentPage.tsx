import { useState } from 'react';
import { documentApi, DocumentSummary } from '../api/api';

function DocumentPage() {
  const [text, setText] = useState('');
  const [result, setResult] = useState<DocumentSummary | null>(null);
  const [loading, setLoading] = useState(false);

  const summarizeText = async () => {
    if (!text.trim()) return;

    setLoading(true);
    try {
      const response = await documentApi.summarize(text);
      setResult(response);
    } catch (error) {
      console.error('Error summarizing document:', error);
      alert('Error summarizing document. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setLoading(true);
    try {
      const response = await documentApi.uploadAndSummarize(file);
      setResult(response);
      setText(response.originalText);
    } catch (error) {
      console.error('Error uploading document:', error);
      alert('Error uploading document. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-container">
      <h1 className="page-title">Document Summarization</h1>
      <p className="page-subtitle">Get concise summaries of your documents</p>

      <div className="form-group">
        <label htmlFor="file">Upload Document (.txt)</label>
        <input
          id="file"
          type="file"
          accept=".txt"
          onChange={handleFileUpload}
          disabled={loading}
        />
      </div>

      <div style={{ textAlign: 'center', margin: '1rem 0', opacity: 0.5 }}>
        <p>— OR —</p>
      </div>

      <div className="form-group">
        <label htmlFor="text">Paste Text Directly</label>
        <textarea
          id="text"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Paste your document text here..."
        />
      </div>

      <button onClick={summarizeText} disabled={loading || !text.trim()}>
        {loading ? 'Summarizing...' : 'Summarize Document'}
      </button>

      {result && (
        <div className="result-card">
          <h3>Summary</h3>
          <p>{result.summary}</p>

          <h3 style={{ marginTop: '1.5rem' }}>Key Points</h3>
          <ul className="key-points">
            {result.keyPoints.map((point, index) => (
              <li key={index}>{point}</li>
            ))}
          </ul>

          <div style={{ marginTop: '1.5rem', opacity: 0.7, fontSize: '0.9rem' }}>
            <p>Original length: {result.originalLength} characters</p>
            <p>Summary length: {result.summaryLength} characters</p>
            <p>Compression: {((1 - result.summaryLength / result.originalLength) * 100).toFixed(1)}%</p>
          </div>
        </div>
      )}
    </div>
  );
}

export default DocumentPage;
