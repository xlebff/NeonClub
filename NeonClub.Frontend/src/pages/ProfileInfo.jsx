import { useNavigate } from 'react-router-dom';
import Button from '../components/Button';
import { useAuth } from '../context/authContext';
import useBookings from '../hooks/useBookings';

export default function ProfileInfo() {
    const { user, logout } = useAuth();
    const { bookings } = useBookings();
    const navigate = useNavigate();

    const totalSpent = bookings.reduce((sum, item) => sum + item.total, 0);

    function handleLogout() {
        logout();
        navigate('/');
    }

    return (
        <div className="panel">
            <h2 className="panel__title">Данные игрока</h2>

            <div className="info-list">
                <div className="info-list__row">
                    <span>Имя</span>
                    <strong>{user.name}</strong>
                </div>
                <div className="info-list__row">
                    <span>Email</span>
                    <strong>{user.email}</strong>
                </div>
                <div className="info-list__row">
                    <span>Броней оформлено</span>
                    <strong>{bookings.length}</strong>
                </div>
                <div className="info-list__row">
                    <span>Сумма броней</span>
                    <strong>{totalSpent} ₽</strong>
                </div>
            </div>

            <Button variant="danger" onClick={handleLogout}>
                Выйти из аккаунта
            </Button>
        </div>
    );
}