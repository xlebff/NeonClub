import { Link } from 'react-router-dom';
import useBookings from '../hooks/useBookings';
import Button from '../components/Button';

export default function ProfileBookings() {
    const { bookings, removeBooking } = useBookings();

    if (bookings.length === 0) {
        return (
            <div className="panel">
                <h2 className="panel__title">Мои брони</h2>
                <div className="empty">
                <h3 className="empty__title">Пока пусто</h3>
                <p className="empty__text">
                    Ты ещё не бронировал игровые места. Самое время выбрать зону.
                </p>
                <Link className="btn btn--primary" to="/catalog">
                    Перейти в каталог
                </Link>
                </div>
            </div>
        );
    }

    return (
        <div className="panel">
            <h2 className="panel__title">Мои брони</h2>

            <div className="booking-list">
                {bookings.map((booking) => (
                    <article className="booking" key={booking.id}>
                        <span className="booking__emoji">{booking.emoji}</span>

                        <div className="booking__info">
                            <span className="booking__title">{booking.zoneTitle}</span>
                            <span className="booking__meta">
                                {booking.hours} ч ·{' '}
                                {new Date(booking.createdAt).toLocaleString('ru-RU', {
                                    day: '2-digit',
                                    month: '2-digit',
                                    hour: '2-digit',
                                    minute: '2-digit',
                                })}
                            </span>
                        </div>

                        <span className="booking__price">{booking.total} ₽</span>

                        <Button variant="danger" size="sm" onClick={() => removeBooking(booking.id)}>
                            Отменить
                        </Button>
                    </article>
                ))}
            </div>
        </div>
    );
}