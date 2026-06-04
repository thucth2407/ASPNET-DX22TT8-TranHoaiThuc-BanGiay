/* ── CONFETTI ── */
(function () {
    const canvas = document.getElementById('confetti-canvas');
    const ctx = canvas.getContext('2d');
    canvas.width = window.innerWidth;
    canvas.height = window.innerHeight;
    const colors = ['#b89c6e', '#e8d9c0', '#c8c4bc', '#2a2926', '#f2ede6', '#111110'];
    const pieces = Array.from({ length: 120 }, () => ({
        x: Math.random() * canvas.width, y: Math.random() * canvas.height - canvas.height,
        w: Math.random() * 10 + 4, h: Math.random() * 6 + 3,
        color: colors[Math.floor(Math.random() * colors.length)],
        rot: Math.random() * Math.PI * 2, rotV: (Math.random() - 0.5) * 0.1,
        vx: (Math.random() - 0.5) * 2, vy: Math.random() * 3 + 1.5, opacity: 1
    }));
    let frame = 0;
    function draw() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        pieces.forEach(p => {
            p.x += p.vx; p.y += p.vy; p.rot += p.rotV;
            if (frame > 180) p.opacity = Math.max(0, p.opacity - 0.008);
            ctx.save(); ctx.globalAlpha = p.opacity;
            ctx.translate(p.x, p.y); ctx.rotate(p.rot);
            ctx.fillStyle = p.color;
            ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
            ctx.restore();
            if (p.y > canvas.height + 20) p.y = -20;
        });
        frame++;
        if (frame < 300) requestAnimationFrame(draw);
        else ctx.clearRect(0, 0, canvas.width, canvas.height);
    }
    draw();
})();

/* ── COPY ORDER ID ── */
function copyOrderId() {
    navigator.clipboard.writeText('#MURA-2025-00847').then(() => {
        const btn = document.querySelector('.order-id-copy');
        btn.innerHTML = '<svg width="16" height="16" fill="none" stroke="var(--gold)" stroke-width="1.5" viewBox="0 0 24 24"><polyline points="20 6 9 17 4 12"/></svg>';
        setTimeout(() => { btn.innerHTML = '<svg width="16" height="16" fill="none" stroke="currentColor" stroke-width="1.5" viewBox="0 0 24 24"><rect x="9" y="9" width="13" height="13" rx="2"/><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/></svg>' }, 2000);
    });
}
window.addEventListener('resize', () => { const c = document.getElementById('confetti-canvas'); c.width = window.innerWidth; c.height = window.innerHeight });