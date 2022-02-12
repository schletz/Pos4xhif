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
            loading: false,
            activeProduct: null,
        }
    },
    async mounted() {
        this.sendToServer(async () => {
            this.categories = await Vue.$get("Categories");
        });
    },
    methods: {
        async sendToServer(action) {
            this.loading = true;
            try {
                await action();
            }
            catch (e) {
                this.$refs.errorDialog.show(e.message);
            }
            finally {
                this.loading = false;
            }
        },
        async loadProducts() {
            if (this.hasChangedProducts) {
                this.$refs.discardDialog.show("Es gibt ungespeicherte Änderungen. Wollen Sie diese verwerfen?")
                if ((await this.$refs.discardDialog.buttonClicked) != "yes") {
                    return;
                }
            }
            await this.sendToServer(async () => {
                this.loading = true;
                const products = await Vue.$get("Products", { categoryGuid: this.categoryGuid });
                for (const p of products) {
                    p.availableFrom = p.availableFrom
                        ? p.availableFrom.substring(0, 10)
                        : "";
                }
                this.products = products;
            });
        },
        async upsertProducts() {
            await this.sendToServer(async () => {
                for (const p of this.products.filter(p => p.changed)) {
                    delete p.validation;
                    try {
                        if (p.guid) {
                            await Vue.$put("Product", p)
                        }
                        else {
                            const newProduct = await Vue.$post("Product", p)
                            this.products[this.products.indexOf(p)] = newProduct;
                        }
                        delete p.changed;
                    }
                    catch (e) {
                        p.validation = e.validation;
                        this.$refs.errorDialog.show(e.message);
                        await this.$refs.errorDialog.buttonClicked;
                    }
                }
            });
        },

        async deleteActiveProduct() {
            if (!this.activeProduct) { return; }
            this.$refs.deleteConfirmDialog.show(`Möchten Sie das Produkt ${this.activeProduct.name} löschen?`);
            if ((await this.$refs.deleteConfirmDialog.buttonClicked) != "yes") {
                return;
            }
            await this.sendToServer(async () => {
                await Vue.$delete("Product", { productGuid: this.activeProduct.guid });
                this.products = this.products.filter(p => p != this.activeProduct);
                this.activeProduct = null;
            });
        },

        addProduct() {
            this.products.push({ productCategoryGuid: this.categoryGuid });
            this.$nextTick(() => {
                const app = document.getElementById("app");
                app.scrollIntoView(false);
            });

        }
    },
    computed: {
        filteredProducts() {
            return this.products.filter(p => !p.name || p.changed || p.name.toLowerCase().indexOf(this.searchProduct.toLowerCase()) != -1)
        },

        hasChangedProducts() {
            return this.products.find(p => p.changed) != null;
        }
    }
}

Vue.$mount(app, "app");