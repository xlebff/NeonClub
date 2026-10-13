import { useState } from 'react';
import { Link, Navigate, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/authContext';
import Button from '../components/Button';

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function Register() {
    const [form, setForm] = useState({ name: '', email: '', password: '', confirm: '' });
    const [errors, setErrors] = useState({});

    const { user, register } = useAuth();
    const navigate = useNavigate();

    if (user) {
        return <Navigate to="/profile" replace />;
    }

    function handleChange(event) {
        const { name, value } = event.target;
        setForm((prev) => ({ ...prev, [name]: value }));
        setErrors((prev) => ({ ...prev, [name]: '' }));
    }

    function validate() {
        const next = {}

        if (!form.name.trim()) next.name = 'Введите имя'
            if (!form.email.trim()) {
                next.email = 'Введите email';
        } else if (!EMAIL_PATTERN.test(form.email.trim())) {
            next.email = 'Некорректный email';
        }

        if (!form.password) {
            next.password = 'Введите пароль';
        } else if (form.password.length < 6) {
            next.password = 'Минимум 6 символов';
        }

        if (form.confirm !== form.password) {
            next.confirm = 'Пароли не совпадают';
        }

        return next;
    }

    async function handleSubmit(event) {
        event.preventDefault();

        const nextErrors = validate();
        setErrors(nextErrors);
        if (Object.keys(nextErrors).length > 0) return;

        const result = await register(form);

        if (!result.ok) {
            setErrors({ email: result.error });
            return;
        }

        navigate('/profile');
    }

    return (
        <section className="section page">
            <div className="container">
                <form className="form" onSubmit={handleSubmit}>
                    <h1 className="form__title">Регистрация</h1>
                    <p className="form__subtitle">Создай аккаунт и бронируй зоны онлайн</p>

                    <label className="field">
                        <span className="field__label">Имя</span>
                        <input
                            className={
                                errors.name ? 'field__input field__input--error' : 'field__input'
                            }
                            type="text"
                            name="name"
                            value={form.name}
                            onChange={handleChange}
                            placeholder="Как тебя зовут?"
                        />
                        {errors.name && <span className="field__error">{errors.name}</span>}
                    </label>

                    <label className="field">
                        <span className="field__label">Email</span>
                        <input
                            className={
                                errors.email ? 'field__input field__input--error' : 'field__input'
                            }
                            type="text"
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            placeholder="player@neon.club"
                        />
                        {errors.email && <span className="field__error">{errors.email}</span>}
                    </label>

                    <label className="field">
                        <span className="field__label">Пароль</span>
                        <input
                            className={
                                errors.password ? 'field__input field__input--error' : 'field__input'
                            }
                            type="password"
                            name="password"
                            value={form.password}
                            onChange={handleChange}
                            placeholder="Минимум 6 символов"
                        />
                        {errors.password && <span className="field__error">{errors.password}</span>}
                    </label>

                    <label className="field">
                        <span className="field__label">Подтверждение пароля</span>
                        <input
                            className={
                                errors.confirm ? 'field__input field__input--error' : 'field__input'
                            }
                            type="password"
                            name="confirm"
                            value={form.confirm}
                            onChange={handleChange}
                            placeholder="Повторите пароль"
                        />
                        {errors.confirm && <span className="field__error">{errors.confirm}</span>}
                    </label>

                    <Button type="submit" full>
                        Зарегистрироваться
                    </Button>

                    <p className="form__footer">
                        Уже есть аккаунт?{' '}
                        <Link className="form__link" to="/login">
                            Войти
                        </Link>
                    </p>
                </form>
            </div>
        </section>
    );
}