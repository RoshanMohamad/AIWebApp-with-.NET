import { useState } from 'react';
import { Upload } from 'lucide-react';
import { imageApi, ImageAnalysisResult } from '../api/api';

function ImagePage() {
  const [result, setResult] = useState<ImageAnalysisResult | null>(null);
  const [loading, setLoading] = useState(false);
  const [imagePreview, setImagePreview] = useState<string | null>(null);

  const handleImageUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    // Create preview
    const reader = new FileReader();
    reader.onload = (e) => {
      setImagePreview(e.target?.result as string);
    };
    reader.readAsDataURL(file);

    setLoading(true);
    try {
      const response = await imageApi.analyze(file);
      setResult(response);
    } catch (error) {
      console.error('Error analyzing image:', error);
      alert('Error analyzing image. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page-container">
      <h1 className="page-title">Image Recognition</h1>
      <p className="page-subtitle">Upload an image to analyze it with AI</p>

      <div className="form-group">
        <label htmlFor="image">Upload Image</label>
        <input
          id="image"
          type="file"
          accept="image/*"
          onChange={handleImageUpload}
          disabled={loading}
        />
      </div>

      {imagePreview && (
        <div style={{ textAlign: 'center', margin: '2rem 0' }}>
          <img
            src={imagePreview}
            alt="Preview"
            style={{ maxWidth: '100%', maxHeight: '400px', borderRadius: '12px' }}
          />
        </div>
      )}

      {loading && <div className="loading">Analyzing image...</div>}

      {result && (
        <div className="result-card">
          <h3>Description</h3>
          <p>{result.description}</p>

          {result.tags.length > 0 && (
            <>
              <h3 style={{ marginTop: '1.5rem' }}>Tags</h3>
              <div className="tags-container">
                {result.tags.map((tag, index) => (
                  <span key={index} className="tag">
                    {tag}
                  </span>
                ))}
              </div>
            </>
          )}

          {result.objects.length > 0 && (
            <>
              <h3 style={{ marginTop: '1.5rem' }}>Detected Objects</h3>
              <ul className="objects-list">
                {result.objects.map((obj, index) => (
                  <li key={index}>
                    {obj.name} — {(obj.confidence * 100).toFixed(1)}% confidence
                  </li>
                ))}
              </ul>
            </>
          )}
        </div>
      )}
    </div>
  );
}

export default ImagePage;
