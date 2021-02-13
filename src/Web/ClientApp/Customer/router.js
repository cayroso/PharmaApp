'use strict';

import VueRouter from 'vue-router';

import index from './Pages/index.vue';

import accountsIndex from './Pages/Accounts/index.vue';

import contactsIndex from './Pages/Contacts/index.vue';
import contactsAdd from './Pages/Contacts/add.vue';
import contactsView from './Pages/Contacts/View/index.vue';

import documentsIndex from './Pages/Documents/index.vue';
import documentsAdd from './Pages/Documents/add.vue';
import documentsView from './Pages/Documents/view.vue';


import medicinesIndex from './Pages/Medicines/index.vue';
import medicinesView from './Pages/Medicines/view.vue';

import ordersAdd from './Pages/Orders/add.vue';
import ordersIndex from './Pages/Orders/index.vue';
import ordersView from './Pages/Orders/view.vue';

import pharmaciesIndex from './Pages/Pharmacies/index.vue';
//import tasksAdd from './Pages/Tasks/add.vue';
//import tasksView from './Pages/Tasks/View/index.vue';

import tripsIndex from './Pages/Trips/index.vue'; 
import tripsAdd from './Pages/Trips/Add/index.vue';
import tripsView from './Pages/Trips/View/index.vue';

const NotFound = {
    template: '<div>Not found</div>'
};

const routes = [
    { path: '/', name: "index", component: index },

    { path: '/accounts', name: "accounts", component: accountsIndex },

    { path: '/contacts', name: "contacts", component: contactsIndex },
    { path: '/contacts/add', name: "contactsAdd", component: contactsAdd },
    { path: '/contacts/view/:id', name: "contactsView", component: contactsView, props: true },

    { path: '/documents', name: "documents", component: documentsIndex },
    { path: '/documents/add', name: "documentsAdd", component: documentsAdd },
    { path: '/documents/view/:id', name: "documentsView", component: documentsView, props: true },

    { path: '/orders/', name: "ordersIndex", component: ordersIndex, props: true },
    { path: '/orders/:pharmacyId/add', name: "ordersAdd", component: ordersAdd, props: true },
    { path: '/orders/view/:id', name: "ordersView", component: ordersView, props: true },

    { path: '/medicines', name: "medicinesIndex", component: medicinesIndex},
    { path: '/medicines/:pharmacyId', name: "pharmaMedicinesIndex", component: medicinesIndex, props: true },
    { path: '/medicines/view/:id', name: "medicinesView", component: medicinesView, props: true },

    { path: '/pharmacies', name: "pharmacies", component: pharmaciesIndex },

    { path: '/trips', name: "trips", component: tripsIndex },
    { path: '/trips/add', name: "tripsAdd", component: tripsAdd },
    { path: '/trips/view/:id', name: "tripsView", component: tripsView, props: true },

    { path: '*', component: NotFound },
];

const router = new VueRouter({
    base:'/customer',
    mode: "history",
    routes: routes,
});

export default router;
