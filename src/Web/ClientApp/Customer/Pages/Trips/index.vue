﻿<template>
    <div v-cloak>

        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-map-marked mr-1"></i>Trips
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <div class="mr-1">
                        <router-link to="/trips/add" class="btn btn-primary">
                            <i class="fas fa-plus"></i>
                        </router-link>
                    </div>

                    <div class="flex-grow-1">
                        <div class="input-group">
                            <input v-model="filter.query.criteria" @keyup.enter="search(1)" type="text" class="form-control" placeholder="Enter criteria..." aria-label="Enter criteria..." aria-describedby="button-addon2">
                            <div class="input-group-append">
                                <button @click="search(1)" class="btn btn-primary" type="button" id="button-addon2">
                                    <span class="fa fas fa-fw fa-search"></span>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <b-collapse v-model="filter.visible">

        </b-collapse>

        <b-overlay :show="busy">
            <div class="mt-2 table-responsive shadow-sm">
                <table-list :header="{key: 'tripId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Driver</th>
                        <th>Status</th>
                        <th>Location</th>
                        <th>Date</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <span>
                                <b-avatar size="sm" :src="row.item.driver.urlProfilePicture" :inline="true"></b-avatar>
                                <router-link :to="{name: 'tripsView', params:{id: row.item.tripId}}">
                                    <div v-if="row.item.driver">
                                        {{row.item.driver.firstName}} {{row.item.driver.middleName}} {{row.item.driver.lastName}}
                                    </div>
                                    <div v-else>
                                        View
                                    </div>
                                </router-link>
                            </span>
                        </td>
                        <td>
                            {{row.item.statusText}}
                        </td>
                        <td>
                            <div class="small">Pickup: {{row.item.startAddress}}</div>
                            <div class="small">Destination: {{row.item.endAddress}}</div>
                        </td>
                        <td>
                            {{row.item.dateCreated|moment('calendar')}}
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Driver</label>
                                <div class="col align-self-center">
                                    <b-avatar :src="row.item.driver.urlProfilePicture"></b-avatar>
                                    <router-link :to="{name: 'tripsView', params:{id: row.item.tripId}}">
                                        <span v-if="row.item.driver">
                                            {{row.item.driver.firstName}} {{row.item.driver.middleName}} {{row.item.driver.lastName}}
                                        </span>
                                        <span v-else>
                                            View
                                        </span>
                                    </router-link>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Status</label>
                                <div class="col align-self-center">
                                    {{row.item.statusText}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Location</label>
                                <div class="col align-self-center">
                                    <div class="small">Pickup: {{row.item.startAddress}}</div>
                                    <div class="small">Destination: {{row.item.endAddress}}</div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Date</label>
                                <div class="col align-self-center">
                                    {{row.item.dateCreated|moment('calendar')}}
                                </div>
                            </div>
                        </div>
                    </template>
                </table-list>





            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>

    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
        },

        data() {
            return {
                baseUrl: `/api/riders/trips/search`,
                filter: {
                    cacheKey: `filter-${this.uid}/search-trips`,
                },
            };
        },

        computed: {

        },

        async created() {
            const vm = this;
            const cache = JSON.parse(localStorage.getItem(vm.filter.cacheKey)) || {};

            vm.initializeFilter(cache);

            await vm.search();
        },

        async mounted() {
            const vm = this;

        },

        methods: {

        }
    }
</script>