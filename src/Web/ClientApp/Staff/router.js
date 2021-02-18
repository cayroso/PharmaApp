'use strict';

import VueRouter from 'vue-router';

import index from './Pages/index.vue';

import accountsIndex from './Pages/Accounts/index.vue';

import medicinesIndex from './Pages/Medicines/index.vue';
import medicinesAdd from './Pages/Medicines/add.vue';
import medicinesView from './Pages/Medicines/view.vue';

import ordersIndex from './Pages/Orders/index.vue';
import ordersAdd from './Pages/Orders/add.vue';
import ordersView from './Pages/Orders/view.vue';

import pharmacyIndex from './Pages/Pharmacy/index.vue';

const NotFound = {
    template: '<div>Not found</div>'
};

const routes = [
    { path: '/', name: "index", component: index },

    { path: '/accounts', name: "accounts", component: accountsIndex },

    { path: '/medicines', name: "medicines", component: medicinesIndex },
    { path: '/medicines/add', name: "medicinesAdd", component: medicinesAdd },
    { path: '/medicines/view/:id', name: "medicinesView", component: medicinesView, props: true },

    { path: '/orders', name: "orders", component: ordersIndex },
    { path: '/orders/add', name: "ordersAdd", component: ordersAdd },
    { path: '/orders/view/:id', name: "ordersView", component: ordersView, props: true },

    { path: '/pharmacy', name: "pharmacy", component: pharmacyIndex },

    { path: '*', component: NotFound },
];

const router = new VueRouter({
    base:'/staff',
    mode: "history",
    routes: routes,
});

export default router;
