import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import Button from '../components/Button';
import Modal from '../components/Modal';
import Loader from '../components/Loader';
import { useAuth } from '../context/authContext';
import useBookings from '../hooks/useBookings';
import { apiFetch, imageUrl } from '../api';

export default function ZonePage() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { user } = useAuth();
    const { addBooking } = useBookings();

    const [zone, setZone] = useState(null);
    const [isLoading, setLoading] = useState(true);
    const [hours, setHours] = useState(1);
    const [isModalOpen, setModalOpen] = useState(false);

    useEffect(() => {
        let cancelled = false;
        apiFetch(`/api/zones/${id}`)
            .then((data) => {
                if (!cancelled) setZone(data);
            })
            .catch(() => {
                if (!cancelled) setZone(null);
            })
            .finally(() => {
                if (!cancelled) setLoading(false);
            });
        return () => {
            cancelled = true;
        };
    }, [id]);

    if (isLoading) {
        return (
            <section className="section page">
                <div className="container">
                    <Loader text="Загружаем зону..." />
                </div>
            </section>
        );
    }

    if (!zone) {
        return (
            <section className="section page">
                <div className="container empty">
                    <h1 className="empty__title">Зона не найдена</h1>
                    <p className="empty__text">
                        Возможно, она была удалена или ссылка устарела.
                    </p>
                    <Button onClick={() => navigate('/catalog')}>Вернуться в каталог</Button>
                </div>
            </section>
        );
    }

    const cover = zone.images?.[0];

    async function handleBook() {
        if (!user) {
            navigate('/login', { state: { from: `/catalog/${zone.id}` } });
            return;
        }
        const booking = await addBooking(zone, hours);
        if (booking) setModalOpen(true);
    }

    return (
        <section className="section page">
            <div className="container">
                <button className="back-link" type="button" onClick={() => navigate(-1)}>
                    ← Назад
                </button>

                <div className="zone-detail">
                    <div className="zone-detail__visual">
                        {cover ? (
                            <img
                                className="zone-detail__image"
                                src={imageUrl(cover)}
                                alt={zone.title}
                            />
                        ) : (
                            <span className="zone-detail__emoji">{zone.emoji}</span>
                        )}
                        {zone.tag && <span className="zone-card__tag">{zone.tag}</span>}
                    </div>

                    <div className="zone-detail__info">
                        <h1 className="zone-detail__title">{zone.title}</h1>
                        <p className="zone-detail__subtitle">{zone.subtitle}</p>
                        <p className="zone-detail__description">{zone.description}</p>

                        <h2 className="zone-detail__heading">Характеристики</h2>
                        <ul className="spec-list">
                            <li>
                                <span>Процессор</span>
                                <strong>{zone.cpu}</strong>
                            </li>
                            <li>
                                <span>Видеокарта</span>
                                <strong>{zone.gpu}</strong>
                            </li>
                            <li>
                                <span>Оперативная память</span>
                                <strong>{zone.ram}</strong>
                            </li>
                            <li>
                                <span>Монитор</span>
                                <strong>{zone.monitor}</strong>
                            </li>
                        </ul>

                        <h2 className="zone-detail__heading">В стоимость входит</h2>
                        <ul className="tag-list">
                            {zone.features.map((feature) => (
                                <li className="tag-list__item" key={feature}>
                                    {feature}
                                </li>
                            ))}
                        </ul>

                        <div className="zone-detail__booking">
                            <div className="hours">
                                <button
                                    type="button"
                                    className="hours__btn"
                                    onClick={() => setHours((prev) => Math.max(1, prev - 1))}
                                >
                                    −
                                </button>
                                <span className="hours__value">{hours} ч</span>
                                <button
                                    type="button"
                                    className="hours__btn"
                                    onClick={() => setHours((prev) => Math.min(12, prev + 1))}
                                >
                                    +
                                </button>
                            </div>

                            <div className="zone-detail__price">
                                {zone.price * hours} ₽
                                <small>{zone.price} ₽/час</small>
                            </div>

                            <Button onClick={handleBook}>Забронировать</Button>
                        </div>

                        {!user && (
                            <p className="zone-detail__hint">
                                Чтобы забронировать место, <Link to="/login">войдите</Link> или{' '}
                                <Link to="/register">зарегистрируйтесь</Link>.
                            </p>
                        )}
                    </div>
                </div>

                <Modal
                    isOpen={isModalOpen}
                    title="Бронь подтверждена"
                    onClose={() => setModalOpen(false)}
                    footer={<Button onClick={() => navigate('/profile/bookings')}>Мои брони</Button>}
                >
                    <p className="booking-form__text">
                        Зона <strong>{zone.title}</strong> забронирована на {hours} ч. Сумма к оплате на
                        месте: <strong>{zone.price * hours} ₽</strong>.
                    </p>
                </Modal>
            </div>
        </section>
    );
}