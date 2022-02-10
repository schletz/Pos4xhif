/* global modal */

const app = {
    components: {
        'modal': modal
    },
    data() {
        return {
            categories: [],
            categoryGuid: "",
            products: [],
            searchProduct: "",
            errorMessage: ""
        }
    },
    async mounted() {
        try {
            this.categories = await Vue.$get("Categories");
        }
        catch (e) {
            this.errorMessage = e.message;
        }
    },
    methods: {
        async loadProducts() {
            try {
                this.products = await Vue.$get("Products", { categoryGuid: this.categoryGuid });
                for (const p of this.products) {
                    p.availableFromDate = p.availableFrom
                        ? new Date(p.availableFrom).toLocaleDateString()
                        : "";
                }
            }
            catch (e) {
                this.errorMessage = e.message;
            }
        }
    },
    computed: {
        filteredProducts() {
            return this.products.filter(p => p.name.toLowerCase().indexOf(this.searchProduct.toLowerCase()) != -1)
        }
    }
}

Vue.$mount(app, "app");