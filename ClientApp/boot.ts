import './css/site.css';
import './stylus/main.styl';

import Axios from 'axios';
import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuetify from 'vuetify';

Vue.use(VueRouter);
Vue.use(Vuetify, {
    theme: {
        primary: '#3f51b5',
        secondary: '#b0bec5',
        accent: '#8c9eff',
        error: '#b71c1c'
    }
})
Vue.prototype.$http = Axios;

const routes = [
    { path: '/', component: require('./components/dashboard/dashboard.vue.html') },
    { path: '/Application/:id', component: require('./components/application/application.vue.html'), props: true },
    { path: '/Orchestrator', component: require('./components/orchestration/orchestration.vue.html') },
    { path: '/Settings', component: require('./components/settings/settings.vue.html') },
    { path: '/Operations', component: require('./components/operations/operations.vue.html') }
];


let vm: any = new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html')),
});

