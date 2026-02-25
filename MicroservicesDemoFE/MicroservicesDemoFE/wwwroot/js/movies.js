const { createApp } = Vue;

createApp({
    data() {
        return {
            movies: null,
            showModal: false,
            editMode: false,
            currentMovie: { id: '', title: '', director: '', year: '' },
            loading: false,
            loadingSeconds: 0,
            loadingInterval: null
        };
    },
    mounted() {
        const token = localStorage.getItem("jwt");

        if (!token) {
            window.location.href =
                `/auth/login?returnUrl=${encodeURIComponent(window.location.pathname)}`;
            return;
        }

        this.fetchMovies();
    },
    methods: {

        getAuthHeaders() {
            const token = localStorage.getItem("jwt");

            if (!token) {
                alert("No estás autenticado");
                return null;
            }

            return {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            };
        },

        async fetchMovies() {
            try {
                const headers = this.getAuthHeaders();
                if (!headers) return;

                this.loading = true;
                this.loadingSeconds = 0;

                this.loadingInterval = setInterval(() => {
                    this.loadingSeconds++;
                }, 1000);

                const res = await fetch(
                    'http://localhost:5018/api/movies',
                    {
                        method: 'GET',
                        headers: headers
                    }
                );

                this.movies = await res.json();

            } catch (err) {
                alert("Error al cargar películas");
                console.error(err);
            } finally {
                clearInterval(this.loadingInterval);
                this.loadingInterval = null;
                this.loading = false;
            }
        },

        openCreateModal() {
            this.editMode = false;
            this.currentMovie = { id: '', title: '', director: '', year: '' };
            this.showModal = true;
        },

        openEditModal(movie) {
            this.editMode = true;
            this.currentMovie = { ...movie };
            this.showModal = true;
        },

        closeModal() {
            this.showModal = false;
        },

        async saveMovie() {
            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                if (this.editMode) {

                    const response = await fetch(
                        `http://localhost:5018/api/movies/${this.currentMovie.id}`,
                        {
                            method: 'PUT',
                            headers: headers,
                            body: JSON.stringify(this.currentMovie)
                        });

                    if (response.status !== 200) {
                        alert("Error al actualizar película");
                        return;
                    }

                } else {

                    const response = await fetch(
                        'http://localhost:5018/api/movies',
                        {
                            method: 'POST',
                            headers: headers,
                            body: JSON.stringify(this.currentMovie)
                        });

                    if (response.status !== 201) {
                        alert("Error al guardar película");
                        return;
                    }
                }

                this.closeModal();
                this.fetchMovies();

            } catch (err) {
                alert("Error al guardar película");
                console.error(err);
            }
        },

        async deleteMovie(id) {
            if (!confirm('¿Eliminar esta película?')) return;

            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                const response = await fetch(
                    `http://localhost:5018/api/movies/${id}`,
                    {
                        method: 'DELETE',
                        headers: headers
                    });

                if (response.status !== 200) {
                    alert("Error al eliminar película");
                    return;
                }

                this.fetchMovies();

            } catch (err) {
                alert("Error al eliminar película");
                console.error(err);
            }
        }
    }
}).mount('#app');