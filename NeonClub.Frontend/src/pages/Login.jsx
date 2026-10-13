import { useState } from 'react';
import { Link, Navigate, useLocation, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import Button from '../components/Button';

export default function Login() {
    const [form, setForm] = useState({ email: '', password: '' });
    const [error, setError] = useState('');

    const { user, login } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();

    if (user) {
        return <Navigate to="/profile" replace />;
    }

    function handleChange(event) {
        const { name, value } = event.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        setError('');
    }

    async function handleSubmit(event) {
        event.preventDefault();

        if (!form.email.trim() || !form.password) {
            setError('Заполните все поля');
            return;
        }

        const result = await login(form);

        if (!result.ok) {
            setError(result.error);
            return;
        }

        const redirectTo = location.state?.from || '/profile';
        navigate(redirectTo, { replace: true });
    }

    return (
        <section className="section page">
            <div className="container">
                <form className="form" onSubmit={handleSubmit}>
                    <h1 className="form__title">Вход</h1>
                    <p className="form__subtitle">С возвращением в NEON CLUB</p>

                    <label className="field">
                        <span className="field__label">Email</span>
                        <input
                            className="field__input"
                            type="text"
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            placeholder="player@neon.club"
                        />
                    </label>

                    <label className="field">
                        <span className="field__label">Пароль</span>
                        <input
                            className="field__input"
                            type="password"
                            name="password"
                            value={form.password}
                            onChange={handleChange}
                            placeholder="••••••"
                        />
                    </label>

                    {error && <p className="form__error">{error}</p>}

                    <Button type="submit" full>
                        Войти
                    </Button>

                    <p className="form__footer">
                            Нет аккаунта?{' '}
                        <Link className="form__link" to="/register">
                            Зарегистрироваться
                        </Link>
                    </p>
                </form>
            </div>
        </section>
    );
}