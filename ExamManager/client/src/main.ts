import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router';
import ScheduleComponent from './components/ScheduleComponent.vue'
import IndexComponent from './components/IndexComponent.vue'

import App from './App.vue'

const routes = [
    { path: '/', component: IndexComponent },    
    { path: '/schedule', component: ScheduleComponent },
]

const router = createRouter({
    history: createWebHistory(),
    routes: routes
});

const app = createApp(App)
app.use(router)
app.mount('#app')


