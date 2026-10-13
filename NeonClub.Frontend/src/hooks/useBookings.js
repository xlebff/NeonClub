import { useCallback, useEffect, useState } from 'react';
import { useAuth } from '../context/authContext';
import { apiFetch } from '../api';

export default function useBookings() {
    const { user } = useAuth();
    const [bookings, setBookings] = useState([]);

    useEffect(() => {
        // если не залогинен — ничего не делаем и не трогаем state
        if (!user) return undefined;

        let cancelled = false;

        // async IIFE — все setState будут ПОСЛЕ await,
        // а значит не «синхронно в теле эффекта» → линтер доволен
        (async () => {
            try {
                const data = await apiFetch('/api/bookings/my');
                if (!cancelled) setBookings(data);
            } catch {
                if (!cancelled) setBookings([]);
            }
        })();

        return () => {
            cancelled = true;
        };
    }, [user]);

    const addBooking = useCallback(
        async (zone, hours) => {
            if (!user) return null;
            try {
                const booking = await apiFetch('/api/bookings', {
                    method: 'POST',
                    body: JSON.stringify({ zoneId: zone.id, hours }),
                });
                setBookings((prev) => [booking, ...prev]);
                return booking;
            } catch {
                return null;
            }
        },
        [user]
    );

    const removeBooking = useCallback(
        async (bookingId) => {
            if (!user) return;
            try {
                await apiFetch(`/api/bookings/${bookingId}`, { method: 'DELETE' });
                setBookings((prev) => prev.filter((b) => b.id !== bookingId));
            } catch {
                // молча
            }
        },
        [user]
    );

    return { bookings, addBooking, removeBooking };
}