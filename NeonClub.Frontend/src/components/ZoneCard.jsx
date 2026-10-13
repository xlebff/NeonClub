import { Link } from 'react-router-dom';
import { imageUrl } from '../api';

export default function ZoneCard({ zone, onBook, index = 0 }) {
    const animationDelay = `${index * 45}ms`;
    const cover = zone.images?.[0];

    return (
        <article className="zone-card" style={{ animationDelay }}>
            <div className="zone-card__head">
                {cover ? (
                    <img
                        className="zone-card__image"
                        src={imageUrl(cover)}
                        alt={zone.title}
                        loading="lazy"
                    />
                ) : (
                    <span className="zone-card__emoji">{zone.emoji}</span>
                )}

                {zone.tag && <span className="zone-card__tag">{zone.tag}</span>}
            </div>

            <h3 className="zone-card__title">{zone.title}</h3>
            <p className="zone-card__subtitle">{zone.subtitle}</p>

            <ul className="zone-card__specs">
                <li>{zone.gpu}</li>
                <li>{zone.cpu}</li>
                <li>{zone.monitor}</li>
            </ul>

            <div className="zone-card__footer">
                <span className="zone-card__price">
                    {zone.price} ₽<small>/час</small>
                </span>

                <div className="zone-card__actions">
                    <Link className="btn btn--ghost btn--sm" to={`/catalog/${zone.id}`}>
                        Подробнее
                    </Link>
                    {onBook && (
                        <button
                            className="btn btn--primary btn--sm"
                            type="button"
                            onClick={() => onBook(zone)}
                        >
                            Бронь
                        </button>
                    )}
                </div>
            </div>
        </article>
    );
}