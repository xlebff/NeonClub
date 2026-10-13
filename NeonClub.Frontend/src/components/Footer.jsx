export default function Footer() {
    return (
        <footer className="footer">
            <div className="container footer__inner">
                <p>© {new Date().getFullYear()} Neon Club — компьютерный клуб.</p>
                <p>г. Москва, ул. Пиксельная, 8 · работает 24/7</p>
            </div>
        </footer>
    );
}