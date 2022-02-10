/* global Highcharts */
const vm = {
    stores: [],
    currentStore: null,

    showOffers(storeGuid) {
        const offersList = document.getElementById("offers");
        while (offersList.firstChild) { offersList.removeChild(offersList.firstChild); }
        const charts = document.getElementById("chart");
        while (charts.firstChild) { charts.removeChild(charts.firstChild); }

        const store = this.stores.find(s => s.guid == storeGuid);
        this.currentStore = store;

        if (!store) { return; }
        offersList.appendChild(document.createElement("option"));
        for (const offer of store.offers) {
            const option = document.createElement("option");
            option.value = offer.guid;
            option.text = offer.productName;
            offersList.appendChild(option);
        }
    },

    async drawDiagram(offerGuid) {
        const charts = document.getElementById("chart");
        while (charts.firstChild) { charts.removeChild(charts.firstChild); }
        if (!this.currentStore) { return; }
        const offer = this.currentStore.offers.find(o => o.guid == offerGuid);
        if (!offer) { return; }

        const response = await fetch(`${window.location.href}?offerGuid=${offerGuid}&handler=Trenddata`);
        const data = await response.json();
        Highcharts.chart(this.getHighchartsConfig(data, offer.productName));
    },

    async mounted() {
        try {
            const response = await fetch(`${window.location.href}?handler=Stores`);
            this.stores = await response.json();

            const storeList = document.getElementById("stores");
            storeList.appendChild(document.createElement("option"));
            for (const store of this.stores) {
                const option = document.createElement("option");
                option.value = store.guid;
                option.text = store.name;
                storeList.appendChild(option);
            }
        }
        catch (e) {
            document
                .getElementById("error")
                .innerHTML = "Fehler beim Laden der Stores";
        }

    },

    getHighchartsConfig(data, product) {
        return {
            chart: {
                renderTo: "chart",
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

window.addEventListener("load", () => vm.mounted());
