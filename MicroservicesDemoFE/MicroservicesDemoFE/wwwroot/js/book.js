const { createApp } = Vue;

createApp({
    data() {
        return {
            books: null,
            showModal: false,
            editMode: false,
            currentBook: { id: '', title: '', author: '' },
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

        this.fetchBooks();
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
        async fetchBooks() {
            try {
                const headers = this.getAuthHeaders();
                if (!headers) return;

                this.loading = true;
                this.loadingSeconds = 0;

                // Inicia contador que incrementa cada segundo
                this.loadingInterval = setInterval(() => {
                    this.loadingSeconds++;
                }, 1000);

                const res = await fetch('http://localhost:5018/api/books',
                {
                    method: 'GET',
                    headers: headers
                });
                this.books = await res.json();

            } catch (err) {
                alert("Error al cargar libros:", err);
                console.error("Error al cargar libros:", err);
            } finally {
                // Para el contador cuando termina
                clearInterval(this.loadingInterval);
                this.loadingInterval = null;
                this.loading = false;
            }
        },
        openCreateModal() {
            this.editMode = false;
            this.currentBook = { id: '', title: '', author: ''};
            this.showModal = true;
        },
        openEditModal(book) {
            this.editMode = true;
            this.currentBook = { ...book };
            this.showModal = true;
        },
        closeModal() {
            this.showModal = false;
        },
        async saveBook() {
            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                if (this.editMode) {
                    // PUT
                    const response = await fetch(`http://localhost:5018/api/books/${this.currentBook.id}`, {
                        method: 'PUT',
                        headers: headers,
                        body: JSON.stringify(this.currentBook)
                    });

                    if (response.status !== 200) {
                        alert("Error al actualizar libro");
                    }

                } else {
                    // POST
                    const response = await fetch('http://localhost:5018/api/books', {
                        method: 'POST',
                        headers: headers,
                        body: JSON.stringify(this.currentBook)
                    });

                    if (response.status !== 201) {
                        alert("Error al guardar libro");
                    }
                }
                this.closeModal();
                this.fetchBooks();
            } catch (err) {
                alert("Error al guardar libro:", err);
                console.error("Error al guardar libro:", err);
            }
        },
        async deleteBook(id) {
            if (!confirm('¿Eliminar este libro?')) return;

            const headers = this.getAuthHeaders();
            if (!headers) return;

            try {
                const response = await fetch(`http://localhost:5018/api/books/${id}`,
                    {
                        method: 'DELETE',
                        headers: headers
                    });

                if (response.status !== 200) {
                    alert("Error al eliminar libro");
                }

                this.fetchBooks();
            } catch (err) {
                alert("Error al eliminar libro", err);
                console.error("Error al eliminar libro:", err);
            }
        }
    }
}).mount('#app');