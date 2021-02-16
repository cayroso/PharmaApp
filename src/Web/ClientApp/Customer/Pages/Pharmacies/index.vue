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
                        <th>Contact</th>
                        <th>Address</th>
                        <th>Status</th>
                        <th></th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            {{row.item.name}}
                            <!--<router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                {{row.item.name}}
                            </router-link>-->
                        </td>
                        <td>
                            <div v-if="row.item.phoneNumber" class="small">
                                <i class="fas fa-fw fa-phone"></i>
                                {{row.item.phoneNumber}}
                            </div>

                            <div v-if="row.item.mobileNumber" class="small">
                                <i class="fas fa-fw fa-mobile"></i>
                                {{row.item.mobileNumber}}
                            </div>

                            <div v-if="row.item.email" class="small">
                                <i class="fas fa-fw fa-at"></i>
                                {{row.item.email}}
                            </div>

                        </td>
                        <td>
                            <div v-if="row.item.address" class="small">
                                <i class="fas fa-fw fa-location-arrow"></i>
                                {{row.item.address}}
                            </div>
                            <div v-if="row.item.openingHours"  class="small">
                                <i class="fas fa-fw fa-clock"></i>
                                {{row.item.openingHours}}
                            </div>
                        </td>
                        <td>
                            {{row.item.pharmacyStatusText}}
                        </td>
                        <td>
                            <router-link :to="{name: 'pharmaMedicinesIndex', params:{ pharmacyId: row.item.pharmacyId}}" class="btn btn-sm btn-outline-primary">
                                <i class="fas fa-fw fa-search"></i> Search Medicines
                            </router-link>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    {{row.item.name}}
                                    <!--<router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                        {{row.item.name}}
                                    </router-link>-->
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Contact</label>
                                <div class="col align-self-center">
                                    <div v-if="row.item.phoneNumber" class="small">
                                        <i class="fas fa-fw fa-phone"></i>
                                        {{row.item.phoneNumber}}
                                    </div>

                                    <div v-if="row.item.mobileNumber" class="small">
                                        <i class="fas fa-fw fa-mobile"></i>
                                        {{row.item.mobileNumber}}
                                    </div>

                                    <div v-if="row.item.email" class="small">
                                        <i class="fas fa-fw fa-at"></i>
                                        {{row.item.email}}
                                    </div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Address</label>
                                <div v-if="row.item.address" class="col align-self-center">
                                    <i class="fas fa-fw fa-location-arrow"></i>
                                    {{row.item.address}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Open Hours</label>
                                <div v-if="row.item.openingHours" class="col align-self-center">
                                    <i class="fas fa-fw fa-clock"></i>
                                    {{row.item.openingHours}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Status</label>
                                <div class="col align-self-center">
                                    {{row.item.pharmacyStatusText}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <router-link :to="{name: 'pharmaMedicinesIndex', params:{ pharmacyId: row.item.pharmacyId}}" class="btn btn-sm btn-outline-primary">
                                        <i class="fas fa-fw fa-search"></i> Search Medicines
                                    </router-link>
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