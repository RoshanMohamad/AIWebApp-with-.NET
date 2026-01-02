import axios from 'axios';

// Use Vite env var when available (set VITE_API_BASE_URL), fallback to localhost
const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string) || 'http://localhost:5000/api';

export interface ChatRequest {
  message: string;
  userId?: string;
  sessionId?: string;
}

export interface ChatResponse {
  response: string;
  sessionId: string;
  timestamp: string;
}

export interface SentimentRequest {
  text: string;
}

export interface SentimentResponse {
  sentiment: string;
  confidence: number;
  explanation: string;
}

export interface DocumentSummary {
  originalText: string;
  summary: string;
  originalLength: number;
  summaryLength: number;
  keyPoints: string[];
}

export interface ImageAnalysisResult {
  description: string;
  tags: string[];
  objects: { name: string; confidence: number }[];
}

export const chatApi = {
  sendMessage: async (request: ChatRequest): Promise<ChatResponse> => {
    const response = await axios.post(`${API_BASE_URL}/chat`, request);
    return response.data;
  },

  getHistory: async (userId: string) => {
    const response = await axios.get(`${API_BASE_URL}/chat/history/${userId}`);
    return response.data;
  },
};

export const sentimentApi = {
  analyze: async (request: SentimentRequest): Promise<SentimentResponse> => {
    const response = await axios.post(`${API_BASE_URL}/sentiment`, request);
    return response.data;
  },
};

export const documentApi = {
  summarize: async (text: string): Promise<DocumentSummary> => {
    const response = await axios.post(`${API_BASE_URL}/document/summarize`, { text });
    return response.data;
  },

  uploadAndSummarize: async (file: File): Promise<DocumentSummary> => {
    const formData = new FormData();
    formData.append('document', file);
    const response = await axios.post(`${API_BASE_URL}/document/upload`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },
};

export const imageApi = {
  analyze: async (file: File): Promise<ImageAnalysisResult> => {
    const formData = new FormData();
    formData.append('image', file);
    const response = await axios.post(`${API_BASE_URL}/image/analyze`, formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
    return response.data;
  },
};
