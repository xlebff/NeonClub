import { useEffect, useState } from 'react';
import { apiFetch, getToken, setToken } from '../api';
import { AuthContext } from './authContext';

const USER_KEY = 'neon_current_user';

export function AuthProvider({ children }) {
    const [user, setUser] = useState(() => {
        try {
            if (!getToken()) return null;
            const saved = localStorage.getItem(USER_KEY);
            return saved ? JSON.parse(saved) : null;
        } catch {
            return null;
        }
    });

    useEffect(() => {
        if (user) localStorage.setItem(USER_KEY, JSON.stringify(user));
        else localStorage.removeItem(USER_KEY);
    }, [user]);

    async function register({ name, email, password }) {
        try {
            const data = await apiFetch('/api/auth/register', {
                method: 'POST',
                body: JSON.stringify({ name, email, password }),
            });
            setToken(data.token);
            setUser(data.user);
            return { ok: true };
        } catch (err) {
            return { ok: false, error: err.message };
        }
    }

    async function login({ email, password }) {
        try {
            const data = await apiFetch('/api/auth/login', {
                method: 'POST',
                body: JSON.stringify({ email, password }),
            });
            setToken(data.token);
            setUser(data.user);
            return { ok: true };
        } catch (err) {
            return { ok: false, error: err.message };
        }
    }

    function logout() {
        setToken(null);
        setUser(null);
    }

    return (
        <AuthContext.Provider value={{ user, register, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
}