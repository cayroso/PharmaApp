<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-clinic-medical mr-1"></i>Pharmacies
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <div class="mr-1">
                        <router-link to="/medicines/add" class="btn btn-primary">
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
                <table-list :header="{key: 'pharmacyId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Address</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                {{row.item.name}}
                            </router-link>
                        </td>
                        <td>
                            {{row.item.pharmacyStatusText}}
                        </td>
                        <td>
                            {{row.item.address}}
                        </td>
                        <td>
                            <router-link :to="{name: 'pharmaMedicinesIndex', params:{ pharmacyId: row.item.pharmacyId}}">
                                Search Medicines
                            </router-link>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    <router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                        {{row.item.name}}
                                    </router-link>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Brand</label>
                                <div class="col align-self-center">
                                    {{row.item.brand}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Classification</label>
                                <div class="col align-self-center">
                                    {{row.item.classificationText}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Available</label>
                                <div class="col align-self-center">
                                    <span v-if="row.item.isAvailable" class="fas fa-fw fa-check"></span>
                                    <span v-else class="fas fa-fw fa-times"></span>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Price</label>
                                <div class="col align-self-center">

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
            urlAdd: String,
            urlView: String,
        },
        components: {
        },
        data() {
            return {
                baseUrl: `/api/pharmacy/search`,
                filter: {
                    cacheKey: `filter-${this.uid}/drugs`,
                    //query: {
                    //    orderStatus: 0,
                    //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                    //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    //}
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