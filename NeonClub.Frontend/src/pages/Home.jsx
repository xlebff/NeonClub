import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import ZoneCard from '../components/ZoneCard';
import Loader from '../components/Loader';
import { apiFetch } from '../api';

const stats = [
    { value: '48', label: 'игровых мест' },
    { value: '24/7', label: 'режим работы' },
    { value: '1 Гбит/с', label: 'скорость интернета' },
];

const features = [
    { icon: '(¬‿¬ )', title: 'Топовое железо', text: 'RTX 40-й серии, мониторы от 144 до 240 Гц.' },
    { icon: '( づ￣ ³￣ )づ', title: 'Кухня и бар', text: 'Пицца, снеки и энергетики прямо к твоему месту.' },
    { icon: '(ง\'-\'́)ง', title: 'Турниры', text: 'Каждую пятницу — внутренние турниры с призовым фондом.' },
    { icon: '( ‾́ ◡ ‾́ )', title: 'Зоны отдыха', text: 'Диваны, настолки и консольные зоны для компании.' },
];

export default function Home() {
    const [popularZones, setPopularZones] = useState([]);
    const [isLoading, setLoading] = useState(true);

    useEffect(() => {
        apiFetch('/api/zones')
            .then((zones) => setPopularZones(zones.filter((z) => z.popular)))
            .catch(() => setPopularZones([]))
            .finally(() => setLoading(false));
    }, []);

    return (
        <div className="page">
            <section className="hero">
                <div className="container hero__inner">
                    <div className="hero__content">
                        <span className="hero__badge">● Открыто прямо сейчас</span>
                        <h1 className="hero__title">
                            Компьютерный клуб <span>NEON CLUB</span>
                        </h1>
                        <p className="hero__text">
                            Восемь игровых зон, консоли, VR-арена и турниры каждую неделю.
                            Забронируй место онлайн — приходи и играй.
                        </p>

                        <div className="hero__actions">
                            <Link className="btn btn--primary" to="/catalog">
                                Выбрать зону
                            </Link>
                            <Link className="btn btn--ghost" to="/register">
                                Стать участником
                            </Link>
                        </div>

                        <div className="hero__stats">
                            {stats.map((item) => (
                                <div className="stat" key={item.label}>
                                    <div className="stat__value">{item.value}</div>
                                    <div className="stat__label">{item.label}</div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            </section>

            <section className="section">
                <div className="container">
                    <div className="section__head">
                        <h2 className="section__title">Популярные зоны</h2>
                        <Link className="section__link" to="/catalog">
                            Смотреть все →
                        </Link>
                    </div>

                    {isLoading ? (
                        <Loader text="Загружаем зоны..." />
                    ) : (
                        <div className="grid">
                            {popularZones.map((zone) => (
                                <ZoneCard key={zone.id} zone={zone} />
                            ))}
                        </div>
                    )}
                </div>
            </section>

            <section className="section">
                <div className="container">
                    <h2 className="section__title">Почему у нас</h2>
                    <div className="features">
                        {features.map((item) => (
                            <div className="feature" key={item.title}>
                                <span className="feature__icon">{item.icon}</span>
                                <h3 className="feature__title">{item.title}</h3>
                                <p className="feature__text">{item.text}</p>
                            </div>
                        ))}
                    </div>
                </div>
            </section>

            <section className="section">
                <div className="container">
                    <div className="cta">
                        <h2 className="cta__title">Готов занять своё место?</h2>
                        <p className="cta__text">
                            Зарегистрируйся — и бронируй зоны онлайн в один клик.
                        </p>
                        <Link className="btn btn--primary" to="/register">
                            Создать аккаунт
                        </Link>
                    </div>
                </div>
            </section>
        </div>
    );
}