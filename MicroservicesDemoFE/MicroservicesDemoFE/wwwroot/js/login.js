const { createApp } = Vue;

createApp({
    data() {
        return {
            email: '',
            password: '',
            error: null
        };
    },
    methods: {
        async login() {
            try {
                const response = await fetch('http://localhost:5006/api/Auth/login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        email: this.email,
                        password: this.password
                    })
                });

                if (!response.ok) {
                    this.error = "Credenciales inválidas";
                    return;
                }

                const data = await response.json();

                //Guardamos JWT
                localStorage.setItem("jwt", data.token.result);

                //Buscamos returnUrl
                const params = new URLSearchParams(window.location.search);
                const returnUrl = params.get("returnUrl");

                //Redirección inteligente
                if (returnUrl) {
                    window.location.href = returnUrl;
                } else {
                    window.location.href = "/Home/Index";
                }
            }
            catch (err) {
                console.error(err);
                this.error = "Error al conectar con el servidor";
            }
        }
    }
}).mount('#loginApp');