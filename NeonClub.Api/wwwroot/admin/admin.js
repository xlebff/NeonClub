const API = '/api';
const TOKEN_KEY = 'neon_admin_token';

const keyInput = document.getElementById('keyInput');
const keyError = document.getElementById('keyError');
const content = document.getElementById('content');

let token = localStorage.getItem(TOKEN_KEY) || '';

if (token) {
    tryLoadAll().catch(() => {
        token = '';
        localStorage.removeItem(TOKEN_KEY);
    });
}

document.getElementById('loginBtn').addEventListener('click', async () => {
    keyError.textContent = '';

    const email = document.getElementById('emailInput').value.trim();
    const password = document.getElementById('passInput').value;

    if (!email || !password) {
        keyError.textContent = 'Заполните email и пароль';
        return;
    }

    try {
        const data = await fetch(`${API}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password }),
        }).then(async (r) => {
            const body = await r.json().catch(() => ({}));
            if (!r.ok) throw new Error(body.error || `Ошибка ${r.status}`);
            return body;
        });

        if (data.user.role !== 'admin') {
            keyError.textContent = 'Этот аккаунт не администратор';
            return;
        }

        token = data.token;
        localStorage.setItem(TOKEN_KEY, token);
        await tryLoadAll();
    } catch (err) {
        keyError.textContent = err.message;
    }
});

async function tryLoadAll() {
    await Promise.all([loadUsers(), loadBookings()]);
    content.classList.remove('hidden');
}

async function api(path, options = {}) {
    const res = await fetch(`${API}${path}`, {
        ...options,
        headers: {
            'Content-Type': 'application/json',
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...(options.headers || {}),
        },
    });

    if (res.status === 401 || res.status === 403) {
        token = '';
        localStorage.removeItem(TOKEN_KEY);
        content.classList.add('hidden');
        throw new Error('Сессия истекла, войдите снова');
    }

    if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        throw new Error(err.error || `Ошибка ${res.status}`);
    }
    return res.json();
}


async function loadUsers() {
    const users = await api('/admin/users');
    const tbody = document.querySelector('#usersTable tbody');
    tbody.innerHTML = '';
    users.forEach((u) => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${escapeHtml(u.name)}</td>
            <td>${escapeHtml(u.email)}</td>
            <td>${escapeHtml(u.role)}</td>
            <td>${u.bookingsCount}</td>
            <td>${new Date(u.createdAt).toLocaleString('ru-RU')}</td>
            <td><button class="danger" data-id="${u.id}">Удалить</button></td>
        `;
        tbody.appendChild(tr);
    });
}

async function loadBookings() {
    const [bookings, statuses] = await Promise.all([
        api('/admin/bookings'),
        api('/admin/statuses'),
    ]);

    const tbody = document.querySelector('#bookingsTable tbody');
    tbody.innerHTML = '';

    bookings.forEach((b) => {
        const statusOptions = statuses
            .map((s) => `<option value="${escapeHtml(s.name)}" ${s.name === b.status ? 'selected' : ''}>${escapeHtml(s.name)}</option>`)
            .join('');

        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${b.zoneEmoji} ${escapeHtml(b.zoneTitle)}</td>
            <td>${escapeHtml(b.userName)}<br><small>${escapeHtml(b.userEmail)}</small></td>
            <td>${b.hours} ч</td>
            <td>${b.total} ₽</td>
            <td><span class="badge badge--${escapeHtml(b.status)}">${escapeHtml(b.status)}</span></td>
            <td>${new Date(b.createdAt).toLocaleString('ru-RU')}</td>
            <td><select data-id="${b.id}">${statusOptions}</select></td>
        `;
        tbody.appendChild(tr);
    });
}

document.querySelector('#usersTable').addEventListener('click', async (e) => {
    if (e.target.tagName !== 'BUTTON') return;
    const id = e.target.dataset.id;
    if (!confirm('Удалить пользователя?')) return;
    try {
        await api(`/admin/users/${id}`, { method: 'DELETE' });
        await loadUsers();
    } catch (err) { alert(err.message); }
});

document.querySelector('#bookingsTable').addEventListener('change', async (e) => {
    if (e.target.tagName !== 'SELECT') return;
    const id = e.target.dataset.id;
    const status = e.target.value;
    try {
        await api(`/admin/bookings/${id}/status`, {
            method: 'PATCH',
            body: JSON.stringify({ status }),
        });
        await loadBookings();
    } catch (err) { alert(err.message); }
});

function escapeHtml(str) {
    return String(str)
        .replaceAll('&', '&amp;')
        .replaceAll('<', '&lt;')
        .replaceAll('>', '&gt;')
        .replaceAll('"', '&quot;');
}

async function loadZoneSelect() {
    const zones = await api('/admin/zones');
    const sel = document.getElementById('zoneSelect');
    sel.innerHTML = zones
        .map((z) => `<option value="${z.id}">${escapeHtml(z.title)}</option>`)
        .join('');
}

async function loadGallery() {
    const images = await api('/uploads');
    const box = document.getElementById('gallery');
    box.innerHTML = images
        .map(
            (i) => `
            <figure style="display:inline-block;margin:8px;text-align:center;">
                <img src="${i.path}" style="width:160px;height:120px;object-fit:cover;border-radius:8px;" />
                <figcaption style="font-size:12px;color:#999;">${escapeHtml(i.zoneTitle ?? '—')}</figcaption>
                <button class="danger" data-img-id="${i.id}">Удалить</button>
            </figure>
        `
        )
        .join('');
}

document.getElementById('uploadBtn').addEventListener('click', async () => {
    const zoneId = document.getElementById('zoneSelect').value;
    const file = document.getElementById('fileInput').files[0];
    const status = document.getElementById('uploadStatus');

    if (!file) {
        status.textContent = 'Выберите файл';
        return;
    }

    const fd = new FormData();
    fd.append('image', file);

    try {
        const res = await fetch(`/api/uploads/zone/${zoneId}`, {
            method: 'POST',
            headers: { Authorization: `Bearer ${token}` },
            body: fd,
        });
        if (!res.ok) {
            const err = await res.json().catch(() => ({}));
            throw new Error(err.error || `Ошибка ${res.status}`);
        }
        status.textContent = '';
        await loadGallery();
    } catch (err) {
        status.textContent = err.message;
    }
});

document.getElementById('gallery').addEventListener('click', async (e) => {
    if (e.target.tagName !== 'BUTTON') return;
    const id = e.target.dataset.imgId;
    if (!confirm('Удалить изображение?')) return;
    try {
        await api(`/uploads/${id}`, { method: 'DELETE' });
        await loadGallery();
    } catch (err) { alert(err.message); }
});

async function tryLoadAll() {
    await Promise.all([loadUsers(), loadBookings(), loadZoneSelect(), loadGallery()]);
    content.classList.remove('hidden');
}