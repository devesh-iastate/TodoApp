import axios from 'axios';
import { auth } from '@/services/firebase';

const api = axios.create({
    baseURL: '/api',
});

api.interceptors.request.use(async (config) => {
    const user = auth.currentUser;
    if (user) {
        const token = await user.getIdToken(false);
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

api.interceptors.response.use(
    res => res,
    async (err) => {
        const { config } = err;
        if (err.response?.status === 401 && !config._retry) {
            config._retry = true;
            const user = auth.currentUser;
            if (user) {
                try {
                    const newToken = await user.getIdToken(true);
                    config.headers.Authorization = `Bearer ${newToken}`;
                    return api.request(config);
                } catch (e) {
                    console.error(e);
                }
            }
        }
        if (err.response?.status === 401) {
            await auth.signOut();
            alert('Session expired or invalid. Please sign in again.');
        }
        return Promise.reject(err);
    }
);

export default api;