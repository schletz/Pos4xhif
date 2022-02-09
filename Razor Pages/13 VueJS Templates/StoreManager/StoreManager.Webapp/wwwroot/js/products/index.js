const app = {
    data() {
        return {
            categories: [],
            categoryGuid: "",
            products: [],
            searchProduct: "",
        }
    },
    async mounted() {
        try {
            this.categories = await Vue.$get("Categories");
        }
        catch (e) {
            alert(e.message);
        }
    },
    methods: {
        async loadProducts() {
            try {
                this.products = await Vue.$get("Products", { categoryGuid: this.categoryGuid });
                for (p of this.products) {
                    p.availableFromDate = p.availableFrom
                        ? new Date(p.availableFrom).toLocaleDateString()
                        : "";
                }
            }
            catch (e) {
                alert(e.message);
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