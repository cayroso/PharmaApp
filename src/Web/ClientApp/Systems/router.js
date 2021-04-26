'use strict';

import VueRouter from 'vue-router';

import index from './Pages/index.vue';

import accountsIndex from './Pages/Accounts/index.vue';

//import contactsIndex from './Pages/Contacts/index.vue';

import medicinesIndex from './Pages/Medicines/index.vue';
import medicinesAdd from './Pages/Medicines/add.vue';
import medicinesView from './Pages/Medicines/view.vue';

import ordersIndex from './Pages/Orders/index.vue';
import ordersAdd from './Pages/Orders/add.vue';
import ordersView from './Pages/Orders/view.vue';

import pharmaciesIndex from './Pages/Pharmacies/index.vue';
import pharmaciesView from './Pages/Pharmacies/view.vue';

import usersIndex from './Pages/Users/index.vue';
//import usersAdd from './Pages/Staffs/add.vue';
import usersView from './Pages/Users/view.vue';


//import tasksIndex from './Pages/Tasks/index.vue';
//import tasksAdd from './Pages/Tasks/add.vue';
//import tasksView from './Pages/Tasks/View/index.vue';

const NotFound = {
    template: '<div>Not found</div>'
};

const routes = [
    { path: '/', name: "index", component: index },

    { path: '/accounts', name: "accounts", component: accountsIndex },

    //{ path: '/contacts', name: "contacts", component: contactsIndex },

    { path: '/medicines', name: "medicines", component: medicinesIndex },
    { path: '/medicines/add', name: "medicinesAdd", component: medicinesAdd },
    { path: '/medicines/view/:id', name: "medicinesView", component: medicinesView, props: true },

    //{ path: '/orders', name: "orders", component: ordersIndex },
    //{ path: '/orders/add', name: "ordersAdd", component: ordersAdd },
    //{ path: '/orders/view/:id', name: "ordersView", component: ordersView, props: true },

    { path: '/pharmacies', name: "pharmaciesIndex", component: pharmaciesIndex },
    { path: '/pharmacies/view/:id', name: "pharmaciesView", component: pharmaciesView, props: true },

    { path: '/users', name: "usersIndex", component: usersIndex },
    //{ path: '/users/add', name: "usersAdd", component: staffsAdd },
    { path: '/users/view/:id', name: "usersView", component: usersView, props: true },

    { path: '*', component: NotFound },
];

const router = new VueRouter({
    base: '/systems',
    mode: "history",
    routes: routes,
});

export default router;
