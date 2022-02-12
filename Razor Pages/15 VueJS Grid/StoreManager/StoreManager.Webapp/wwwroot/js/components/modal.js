const modal = {
    // See https://vuejs.org/guide/components/props.html
    props: {
        title: { required: true },
        ok: {},
        yes: {},
        no: {},
        icon: { default: 'info' },
        timeout: { default: 0 }
    },
    data() {
        return {
            showDialog: false,
            message: "",
            buttonClicked: Promise.resolve(),
        }
    },
    methods: {
        handleClick(eventName) {
            this.showDialog = false;
            this.$emit(eventName);
            if (this.buttonResolve) { this.buttonResolve(eventName); }
        },
        show(message) {
            this.message = message;
            if (message === "") { return; }
            this.showDialog = true;
            const self = this;
            this.buttonClicked = new Promise(function (resolve) {
                self.buttonResolve = resolve;
            });
            if (this.timeout && this.ok) {
                setTimeout(() => {
                    if (this.ok) {
                        this.handleClick('ok');
                    }
                }, this.timeout);
            }
        }
    },
    template: `
        <div v-if="showDialog" class="modal d-block" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="d-flex flex-column">
                        <div class="p-1" style="color: white; background-color:var(--bs-primary)">{{ title }}</div>
                        <div class="d-flex align-items-end p-1" style="flex: 1 0 3em">
                            <div style="flex: 0 0 40px">
                                <i v-if="icon == 'info'" class="fas fa-info-circle" style="font-size:30px; color:var(--bs-info)"></i>
                                <i v-if="icon == 'question'" class="fas fa-question-circle" style="font-size:30px; color:var(--bs-info)"></i>
                                <i v-if="icon == 'warning'" class="fas fa-exclamation-triangle" style="font-size:30px; color:var(--bs-warning)"></i>
                                <i v-if="icon == 'error'" class="fas fa-exclamation-circle" style="font-size:30px; color:var(--bs-danger)"></i>
                            </div>
                            <div>
                                {{ message }}
                                <slot></slot>
                            </div>
                        </div>
                        <div class="d-flex p-2 justify-content-end" style="background-color:#f0f0f0; gap: 1em">
                            <button v-if="ok"  v-on:click="handleClick('ok')"  class="btn btn-primary"   style="flex: 0 0 4em">OK</button>
                            <button v-if="yes" v-on:click="handleClick('yes')" class="btn btn-primary"   style="flex: 0 0 4em">Ja</button>
                            <button v-if="no"  v-on:click="handleClick('no')"  class="btn btn-secondary" style="flex: 0 0 4em">Nein</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
`
}

