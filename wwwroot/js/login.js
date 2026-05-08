sessionStorage.clear();

const currentTheme = localStorage.getItem('theme') || 'light';
document.body.classList.toggle('dark-mode', currentTheme === 'dark');

document.getElementById('themeToggle')?.addEventListener('click', () => {
    const isDark = document.body.classList.toggle('dark-mode');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
});

document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const cpf = document.getElementById('cpf').value.trim();
    const senha = document.getElementById('password').value;
    const notificacao = document.getElementById('notificacao');

    const response = await fetch('/api/conta/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ cpf, senha })
    });

    if (response.ok) {
        notificacao.className = 'hidden';
        const data = await response.json();
        alert(`Bem-vindo, ${data.titular}!`);

        sessionStorage.setItem('usuarioLogado', data.titular);
        sessionStorage.setItem('cpfLogado', cpf);
        
        const instanceResp = await fetch('/api/instance');
        const instanceId = await instanceResp.json();
        sessionStorage.setItem('serverInstance', instanceId);

        if (data.isAdmin) {
            sessionStorage.setItem('isAdmin', 'true');
            window.location.href = 'admin.html';
        } else {
            window.location.href = 'transactions.html'; 
        }
    } else {
        const erro = await response.text();
        notificacao.textContent = erro;
        notificacao.className = ''; 
    }
});

document.getElementById('forgotPassword').addEventListener('click', (e) => {
    e.preventDefault();
    alert('Funcionalidade "Esqueci minha senha" em desenvolvimento. Por favor, entre em contato com o suporte.');
});

document.getElementById('openAccount').addEventListener('click', (e) => {
    e.preventDefault();
    window.location.href = 'register.html'; 
});

document.getElementById('adminLoginBtn').addEventListener('click', () => {
    document.getElementById('cpf').value = 'admin';
    document.getElementById('password').value = 'admin123';
    document.getElementById('loginForm').requestSubmit();
});