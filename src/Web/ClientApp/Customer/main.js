﻿'use strict';

import '../_Core/core';
import './main.css';

import Vue from 'vue';
import App from './Pages/_Shared/app.vue';

import VueObserveVisibility from 'vue-observe-visibility'
Vue.use(VueObserveVisibility);

import common from '../_Core/Plugins/common';
Vue.use(common);

import VueRouter from 'vue-router';
Vue.use(VueRouter);

import Router from './router';

import '../_Core/Plugins/bootstrap-vue';

//  global components
import Pagination from '../_Core/Components/pagination.vue';
import SortField from '../_Core/Components/sortfield.vue';
import TableList from '../_Core/Components/table-list.vue';
import GMapLocation from '../_Core/Components/gmap-location.vue';

Vue.component('m-pagination', Pagination);
Vue.component('sort-field', SortField);
Vue.component('table-list', TableList);
Vue.component('gmap-location', GMapLocation);

new Vue({
    el: '#app',
    router: Router,
    components: {
        App
    },
    created() {
        $(document).ready(function () {
            $('.main').addClass('main-loaded');
        });

        //let theme = localStorage.getItem('theme') || '';

        //if (theme) {
        //    //debugger;
        //    let style = document.createElement('link');
        //    style.type = "text/css";
        //    style.rel = "stylesheet";
        //    style.href = theme;// 'https://bootswatch.com/4/yeti/bootstrap.min.css';
        //    document.head.appendChild(style);
        //}
    }
});