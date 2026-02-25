const { createApp } = Vue;

createApp({
    data() {
        return {
            reviews: null,
            showModal: false,
            editMode: false,
            currentReview: { id: '', movieId: '', reviewer: '', rating: '', comment: '' },
            loading: false,          // indica si estamos cargando
            loadingSeconds: 0,       // segundos transcurridos
            loadingInterval: null    // referencia al setInterval
        };
    },
    mounted() {
        const token = localStorage.getItem("jwt");

        if (!token) {
            window.location.href =
                `/auth/login?returnUrl=${encodeURIComponent(window.location.pathname)}`;
            return;
        }

        this.fetchReviews();
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
        async fetchReviews() {
            try {
                const headers = this.getAuthHeaders();
                if (!headers) return;

                this.loading = true;
                this.loadingSeconds = 0;

                // Inicia contador que incrementa cada segundo
                this.loadingInterval = setInterval(() => {
                    this.loadingSeconds++;
                }, 1000);

                const res = await fetch('http://localhost:5018/api/reviews',
                    {
                        method: 'GET',
                        headers: headers
                    });
                this.reviews = await res.json();

            } catch (err) {
                alert("Error al cargar reviews:", err);
                console.error("Error al cargar reviews:", err);
            } finally {
                // Para el contador cuando termina
                clearInterval(this.loadingInterval);
                this.loadingInterval = null;
                this.loading = false;
            }
        },
        openCreateModal() {
            this.editMode = false;
            this.currentReview = { id: '', movieId: '', reviewer: '', rating: '', comment: '' };
            this.showModal = true;
        },
        openEditModal(review) {
            this.editMode = true;
            this.currentReview = { ...review };
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
        },
        async saveReview() {
            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                if (this.editMode) {
                    // PUT
                    const response =await fetch(`http://localhost:5018/api/reviews/${this.currentReview.id}`, {
                        method: 'PUT',
                        headers: headers,
                        body: JSON.stringify(this.currentReview)
                    });

                    if (response.status !== 200) {
                        alert("Error al actualizar review");
                    }

                } else {
                    // POST
                    const response = await fetch('http://localhost:5018/api/reviews', {
                        method: 'POST',
                        headers: headers,
                        body: JSON.stringify(this.currentReview)
                    });

                    if (response.status !== 201) {
                        alert("Error al guardar review");
                    }
                }
                this.closeModal();
                this.fetchReviews();
            } catch (err) {
                alert("Error al guardar review:", err);
                console.error("Error al guardar review:", err);
            }
        },
        async deleteReview(id) {
            if (!confirm('¿Eliminar esta review?')) return;

            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                const response = await fetch(`http://localhost:5018/api/reviews/${id}`,
                    {
                        method: 'DELETE',
                        headers: headers
                    });

                if (response.status !== 200) {
                    alert("Error al eliminar review");
                }

                alert("Error al guardar review");

                this.fetchReviews();
            } catch (err) {
                alert("Error al eliminar review:", err);
                console.error("Error al eliminar review:", err);
            }
        }
    }
}).mount('#app');