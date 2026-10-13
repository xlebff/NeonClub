import { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import ZoneCard from '../components/ZoneCard';
import SearchBar from '../components/SearchBar';
import Loader from '../components/Loader';
import Modal from '../components/Modal';
import Button from '../components/Button';
import { useAuth } from '../context/authContext';
import useBookings from '../hooks/useBookings';
import { apiFetch } from '../api';

const categories = [
    { id: 'all', label: 'Все зоны' },
    { id: 'pc', label: 'Игровые ПК' },
    { id: 'vip', label: 'VIP-залы' },
    { id: 'console', label: 'Консоли' },
    { id: 'vr', label: 'VR' },
];

export default function Catalog() {
    const [zones, setZones] = useState([]);
    const [search, setSearch] = useState('');
    const [category, setCategory] = useState('all');
    const [isLoading, setLoading] = useState(true);
    const [selectedZone, setSelectedZone] = useState(null);
    const [hours, setHours] = useState(1);
    const [toast, setToast] = useState('');
    const [error, setError] = useState('');

    const { user } = useAuth();
    const { addBooking } = useBookings();
    const navigate = useNavigate();

    useEffect(() => {
        let cancelled = false;
        apiFetch('/api/zones')
            .then((data) => {
                if (!cancelled) setZones(data);
            })
            .catch((err) => {
                if (!cancelled) setError(err.message);
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });
        return () => {
            cancelled = true;
        };
    }, []);

    useEffect(() => {
        if (!toast) return undefined;
        const timer = setTimeout(() => setToast(''), 3500);
        return () => clearTimeout(timer);
    }, [toast]);

    const filteredZones = useMemo(() => {
        const query = search.trim().toLowerCase();

        return zones.filter((zone) => {
            const matchCategory = category === 'all' || zone.category === category;
            const matchQuery =
                !query ||
                zone.title.toLowerCase().includes(query) ||
                zone.subtitle.toLowerCase().includes(query) ||
                zone.gpu.toLowerCase().includes(query);

            return matchCategory && matchQuery;
        });
    }, [zones, search, category]);

    function handleBookClick(zone) {
        if (!user) {
            navigate('/login', { state: { from: '/catalog' } });
            return;
        }
        setHours(1);
        setSelectedZone(zone);
    }

    async function confirmBooking() {
        const booking = await addBooking(selectedZone, hours);
        if (!booking) {
            setToast('Не удалось создать бронь, попробуйте ещё раз.');
            return;
        }
        setToast(`Зона «${selectedZone.title}» забронирована на ${hours} ч.`);
        setSelectedZone(null);
    }

    return (
        <section className="section page">
            <div className="container">
                <div className="section__head">
                    <h1 className="section__title">Каталог игровых зон</h1>
                    <p className="section__subtitle">Выбери конфигурацию под свою игру</p>
                </div>

                <div className="toolbar">
                    <SearchBar
                        value={search}
                        onChange={setSearch}
                        placeholder="Поиск по названию или видеокарте..."
                    />

                    <div className="filters">
                        {categories.map((item) => (
                            <button
                                key={item.id}
                                type="button"
                                className={
                                    category === item.id ? 'chip chip--active' : 'chip'
                                }
                                onClick={() => setCategory(item.id)}
                            >
                                {item.label}
                            </button>
                        ))}
                    </div>
                </div>

                {isLoading ? (
                    <Loader text="Загружаем игровые зоны..." />
                ) : error ? (
                    <div className="empty">
                        <h3 className="empty__title">Не удалось загрузить зоны</h3>
                        <p className="empty__text">{error}</p>
                    </div>
                ) : (
                    <>
                        <p className="results-count">
                            Найдено зон: <strong>{filteredZones.length}</strong>
                        </p>

                        {filteredZones.length === 0 ? (
                            <div className="empty">
                                <h3 className="empty__title">Ничего не найдено</h3>
                                <p className="empty__text">
                                    Попробуй изменить запрос или выбрать другую категорию.
                                </p>
                                <Button
                                    variant="ghost"
                                    onClick={() => {
                                        setSearch('');
                                        setCategory('all');
                                    }}
                                >
                                    Сбросить фильтры
                                </Button>
                            </div>
                        ) : (
                            <div className="grid">
                                {filteredZones.map((zone, index) => (
                                    <ZoneCard
                                        key={zone.id}
                                        zone={zone}
                                        onBook={handleBookClick}
                                        index={index}
                                    />
                                ))}
                            </div>
                        )}
                    </>
                )}

                <Modal
                    isOpen={Boolean(selectedZone)}
                    title="Бронирование зоны"
                    onClose={() => setSelectedZone(null)}
                    footer={
                        <>
                            <Button variant="ghost" onClick={() => setSelectedZone(null)}>
                                Отмена
                            </Button>
                            <Button onClick={confirmBooking}>Подтвердить</Button>
                        </>
                    }
                >
                    {selectedZone && (
                        <div className="booking-form">
                            <p className="booking-form__zone">
                                {selectedZone.emoji} <strong>{selectedZone.title}</strong>
                            </p>
                            <p className="booking-form__text">{selectedZone.subtitle}</p>

                            <div className="field">
                                <span className="field__label">Количество часов</span>

                                <div className="hours">
                                    <button
                                        type="button"
                                        className="hours__btn"
                                        onClick={() => setHours((prev) => Math.max(1, prev - 1))}
                                        disabled={hours <= 1}
                                        aria-label="Уменьшить"
                                    >
                                        −
                                    </button>

                                    <span className="hours__value">{hours} ч</span>

                                    <button
                                        type="button"
                                        className="hours__btn"
                                        onClick={() => setHours((prev) => Math.min(12, prev + 1))}
                                        disabled={hours >= 12}
                                        aria-label="Увеличить"
                                    >
                                        +
                                    </button>
                                </div>
                            </div>

                            <p className="booking-form__total">
                                Итого: <strong>{selectedZone.price * hours} ₽</strong>
                            </p>
                        </div>
                    )}
                </Modal>

                {toast && <div className="toast">{toast}</div>}
            </div>
        </section>
    );
}