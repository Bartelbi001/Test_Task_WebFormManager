import axios from 'axios';

// API base URL (changed in the .env file)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api';

// Axios Configuration
const api = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// ✅ Sending the form
export const submitForm = async (formData) => {
    try {
        const response = await api.post('/submissions', formData);
        return response.data;
    } catch (error) {
        console.error('Error when submitting the form:', error);
        throw error;
    }
};

// ✅ Receiving all applications
export const getSubmissions = async () => {
    try {
        const response = await api.get('/submissions');
        return response.data;
    } catch (error) {
        console.error('Error when receiving applications:', error);
        throw error;
    }
};

// ✅ Search for applications by row
export const searchSubmissions = async (query) => {
    try {
        const response = await api.get(`/submissions/search?query=${encodeURIComponent(query)}`);
        return response.data;
    } catch (error) {
        console.error('Error when searching for applications:', error);
        throw error;
    }
};
