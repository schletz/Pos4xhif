HTMLSelectElement.prototype.setOptions = function (items) {
    Array.from(this.childNodes).forEach(child => this.removeChild(child));
    this.appendChild(document.createElement("option"));
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
    _stores: [],
    _currentStoreGuid: "",
    _offers: [],
    _currentOfferGuid: "",

    get stores() { return this._stores; },
    set stores(value) {
        value = value || [];
        this._stores = value;
        this.currentStoreGuid = "";
        view.stores.setOptions(value.map(s => ({ value: s.guid, text: s.name })));
    },
    get currentStoreGuid() { return this._currentStoreGuid; },
    set currentStoreGuid(value) {
        value = value || "";
        this._currentStoreGuid = value;
        const store = this.stores.find(s => s.guid == value) || {};
        this.offers = store.offers;
    },

    get offers() { return this._offers; },
    set offers(value) {
        value = value || [];
        this._offers = value;
        this.currentOfferGuid = "";
        view.offers.setOptions(value.map(o => ({ value: o.guid, text: o.productName })));
    },
    get currentOfferGuid() { return this._currentOfferGuid; },
    set currentOfferGuid(value) {
        value = value || "";
        this._currentOfferGuid = value;
        Array.from(view.chart.childNodes).forEach(child => view.chart.removeChild(child));
    },

    get currentStore() { return this.stores.find(s => s.guid == this.currentStoreGuid) || {}; },
    get currentOffer() { return this.currentStore.offers.find(o => o.guid == this.currentOfferGuid) || {}; },

    async mounted() {
        try {
            const stores = await getJson(`${window.location.href}?handler=Stores`);
            if (!stores.length) { return; }
            this.stores = stores;
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    async offerChanged(offerGuid) {
        try {
            this.currentOfferGuid = offerGuid;
            if (!offerGuid) { return; }
            const data = await getJson(`${window.location.href}?offerGuid=${offerGuid}&handler=Trenddata`);
            Highcharts.chart(this.getChartOptions(this.currentOffer.productName, data));
        }
        catch (e) {
            error.innerHTML = e;
        }
    },

    getChartOptions(product, data) {
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
                    data: data
                }
            ]
        };
    }
}

// *************************************************************************************************

window.addEventListener("load", () => vm.mounted());