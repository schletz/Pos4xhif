HTMLSelectElement.prototype.setOptions = function (items) {
    Array.from(this.childNodes).forEach(child => this.removeChild(child));
    items.map(o => {
        const opt = document.createElement("option");
        opt.value = o.value;
        opt.text = o.text;
        return opt;
    }).forEach(opt => {
        this.appendChild(opt);
    });
}

HTMLSelectElement.prototype.clear = function (items) {
    Array.from(this.childNodes).forEach(child => this.removeChild(child));
}

async function getJson(url) {
    if (!url) { return {}; }
    const response = await fetch(url);
    if (!response.ok) { throw "Der Server lieferte keine Daten."; }
    return await response.json();
}

// *************************************************************************************************

const view = {
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
            const stores = await getJson(`${window.location.href}?handler=Stores`);
            if (!stores.length) { return; }
            this.stores = stores;
            view.stores.setOptions(stores.map(s => ({ value: s.guid, text: s.name })));
            this.storeChanged(stores[0].guid);
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    storeChanged(storeGuid) {
        try {
            view.offers.clear();
            Array.from(view.chart.childNodes).forEach(child => view.chart.removeChild(child));
            this.currentStore = {};
            if (!storeGuid) { return; }
            const store = this.stores.find(s => s.guid == storeGuid);
            if (!store || !store.offers || !store.offers.length) { return; }
            view.offers.setOptions(store.offers.map(o => ({ value: o.guid, text: o.productName })));
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
            this.trenddata = await getJson(`${window.location.href}?offerGuid=${offer.guid}&handler=Trenddata`);
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
                renderTo: view.chart
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

// *************************************************************************************************

window.addEventListener("load", () => vm.mounted());