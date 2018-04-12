import './css/site.css';
import './stylus/main.styl';

import Vue from 'vue';
import VueRouter from 'vue-router';
import Vuetify from 'vuetify';

Vue.use(VueRouter);
Vue.use(Vuetify);

const routes = [
    { path: '/', component: require('./components/dashboard/dashboard.vue.html') },
    { path: '/application/:id', component: require('./components/application/application.vue.html'), props: true },
    { path: '/fetchdata', component: require('./components/fetchdata/fetchdata.vue.html') }
];

new Vue({
    el: '#app-root',
    router: new VueRouter({ mode: 'history', routes: routes }),
    render: h => h(require('./components/app/app.vue.html'))
});
