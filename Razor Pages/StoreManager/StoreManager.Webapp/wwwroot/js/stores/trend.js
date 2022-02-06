const dom = {
    stores: document.getElementById("stores"),
    offers: document.getElementById("offers"),
    error: document.getElementById("error"),
    chart: document.getElementById("chart")
}

// *************************************************************************************************

const vm = {
    stores: [],
    currentStore: {},
    trenddata: [],

    async mounted() {
        try {
            const response = await fetch(`${window.location.href}?handler=Stores`);
            if (!response.ok) { throw "Der Server lieferte keine Daten."; }
            const stores = await response.json();
            if (!stores.length) { return; }
            this.stores = stores;
            stores
                .map(s => {
                    const opt = document.createElement("option");
                    opt.value = s.guid;
                    opt.text = s.name;
                    return opt;
                })
                .forEach(opt => {
                    dom.stores.appendChild(opt);
                });
            this.storeChanged(stores[0].guid);
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    storeChanged(storeGuid) {
        try {
            Array.from(dom.offers.childNodes).forEach(child => dom.offers.removeChild(child));
            Array.from(dom.chart.childNodes).forEach(child => dom.chart.removeChild(child));
            this.currentStore = {};
            if (!storeGuid) { return; }
            const store = this.stores.find(s => s.guid == storeGuid);
            if (!store) { return; }
            if (!store.offers.length) { return; }
            store.offers.map(o => {
                const opt = document.createElement("option");
                opt.value = o.guid;
                opt.text = o.productName;
                return opt;
            }).forEach(opt => {
                dom.offers.appendChild(opt);
            });
            this.currentStore = store;
            this.offerChanged(store.offers[0].guid);
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    async offerChanged(offerGuid) {
        try {
            if (!offerGuid) { return; }
            const offer = this.currentStore.offers.find(o => o.guid == offerGuid)
            if (!offer) { return; }
            const response = await fetch(`${window.location.href}?offerGuid=${offer.guid}&handler=Trenddata`);
            if (!response.ok) { throw "Der Server lieferte keine Daten."; }
            this.trenddata = await response.json();
            Highcharts.chart(this.getChartOptions(offer.productName));
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    getChartOptions(product) {
        return {
            title: null,
            chart: {
                renderTo: dom.chart
            },
            title: {
                text: `Preisverlauf von ${product}`
            },
            legend: { enabled: false },
            xAxis: {
                type: 'datetime', tickInterval: 24 * 3600e3
            },
            yAxis: {
                title: { text: "Preis" }
            },
            series: [
                {
                    name: "Preis",
                    data: this.trenddata
                }
            ]
        };
    }
}

window.addEventListener("load", () => vm.mounted());