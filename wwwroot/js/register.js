const apiBase = '/api/conta';

document.getElementById('formCadastro').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const titular = document.getElementById('titular').value;
    const email = document.getElementById('email').value;
    const cpf = document.getElementById('cpf').value;
    const senha = document.getElementById('senha').value;
    const dataNascValue = document.getElementById('dataNascimento').value;

    if (!titular) {
        alert('Erro: O titular é obrigatório.');
        return;
    }
    if (titular.length > 100) {
        alert('Erro: O titular não pode ter mais de 100 caracteres.');
        return;
    }
    if (!email) {
        alert('Erro: O e-mail é obrigatório.');
        return;
    }
    if (email.length > 100) {
        alert('Erro: O e-mail não pode ter mais de 100 caracteres.');
        return;
    }
    if (!/\S+@\S+\.\S+/.test(email)) {
        alert('Erro: Formato de e-mail inválido.');
        return;
    }
    if (!cpf) {
        alert('Erro: O CPF é obrigatório.');
        return;
    }
    if (cpf.length !== 11 || !/^\d+$/.test(cpf)) { 
        alert('Erro: O CPF inválido.');
        return;
    }
    if (!senha || senha.length < 6 || senha.length > 50) {
        alert('Erro: A senha deve ter entre de 6 e 50 caracteres.');
        return;
    }
    if (!dataNascValue) {
        alert('Erro: A data de nascimento é obrigatória.');
        return;
    }

    const dados = {
        tipo: 'poupanca', 
        titular: titular,
        email: email,
        cpf: cpf,
        senha: senha,
        dataNascimento: dataNascValue || null,
        numero: 0,   
        agencia: 0, 
        saldo: 0,    
        taxaRendimento: 0.005,
        aniversario: new Date().toISOString().split('T')[0]
    };

    const response = await fetch(apiBase, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dados)
    });

    if (response.ok) {
        alert('Conta criada com sucesso! Agora você pode realizar o login.');
        window.location.href = 'login.html';
    } else {
        const responseClone = response.clone();
        try {
            const errorData = await response.json();
            if (response.status === 400 && errorData.errors) {
                let errorMessages = [];
                for (const key in errorData.errors) {
                    if (errorData.errors.hasOwnProperty(key)) {
                        errorMessages = errorMessages.concat(errorData.errors[key]);
                    }
                }
                alert('Erro de validação:\n' + errorMessages.join('\n'));
            } else {
                alert('Erro: ' + (errorData.message || JSON.stringify(errorData)));
            }
        } catch {
            const msg = await responseClone.text();
            alert('Erro ao cadastrar: ' + msg);
        }
    }
});