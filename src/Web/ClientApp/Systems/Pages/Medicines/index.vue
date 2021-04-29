<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-prescription-bottle-alt mr-1"></i>Medicines <span v-if="pharmacyId">- {{pharmacy.name}}</span>
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    
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
                <table-list :header="{key: 'drugId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Name</th>
                        <th v-if="!pharmacyId">Pharmacy</th>
                        <th>Classification</th>
                        <th>Available</th>
                        <th>Price</th>
                        <!--<th></th>-->
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <router-link :to="{name:'medicinesView', params:{ pharmacyId: pharmacyId, id: row.item.drugId}}">
                                {{row.item.name}}
                            </router-link>
                            <div>
                                <small>{{row.item.brand}}</small>
                            </div>
                        </td>
                        <td v-if="!pharmacyId">
                            <router-link :to="{name:'pharmaciesView', params:{ id: row.item.pharmacy.pharmacyId}}">
                                {{row.item.pharmacy.name}}
                            </router-link>
                            
                        </td>
                        <td>
                            {{row.item.classificationText}}
                        </td>
                        <td>
                            <i v-if="row.item.isAvailable" class="fas fa-fw fa-check-circle"></i>
                            <i v-else class="fas fa-fw fa-times-circle"></i>
                        </td>
                        <td>
                            {{row.item.price.price|toCurrency}}
                        </td>
                        <!--<td>
                            <button v-bind:disabled="!row.item.isAvailable" @click="addToCart(row.item)" class="btn btn-sm" v-bind:class="row.item.classification===2? 'btn-danger':'btn-outline-success'">
                                <i class="fas fa-fw fa-cart-plus"></i>
                            </button>
                        </td>-->
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
                            <!--<div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Brand</label>
                                <div class="col align-self-center">
                                    {{row.item.brand}}
                                </div>
                            </div>-->
                            <div v-if="!pharmacyId" class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Pharmacy</label>
                                <div class="col align-self-center">
                                    {{row.item.pharmacy.name}}
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
                                    {{row.item.price.price|toCurrency}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <button v-bind:disabled="!row.item.isAvailable" @click="addToCart(row.item)" class="btn btn-sm" v-bind:class="row.item.classification===2? 'btn-danger':'btn-outline-success'">
                                        <i class="fas fa-fw fa-cart-plus"></i>
                                    </button>
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
            pharmacyId: {
                type: String,
                //    required: true,
            },
        },
        components: {
        },
        data() {
            return {
                baseUrl: `/api/drugs/search`,
                filter: {
                    cacheKey: `filter-${this.uid}/drugs/search`,
                    //query: {
                    //    orderStatus: 0,
                    //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                    //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    //}
                },
                pharmacy: {}
            };
        },

        computed: {

        },
        watch: {
            async $route(to, from) {
                const vm = this
                //this.show = false;
                await vm.search();
            }
        },
        async created() {
            const vm = this;
            const cache = JSON.parse(localStorage.getItem(vm.filter.cacheKey)) || {};

            vm.initializeFilter(cache);

            await vm.search();

        },

        async mounted() {
            const vm = this;
            await vm.getPharmacyInfo();
        },

        methods: {
            getQuery() {

                const vm = this;
                const filter = vm.filter;

                if (vm.busy)
                    return;

                const params = [
                    '?c=', encodeURIComponent(filter.query.criteria),
                    '&p=', filter.query.pageIndex,
                    '&s=', filter.query.pageSize,
                    '&sf=', filter.query.sortField,
                    '&so=', filter.query.sortOrder
                ];

                if (vm.pharmacyId) {
                    params.push('&pharmacyId=');
                    params.push(vm.pharmacyId);
                }

                const query = params.join('');
                return query;
            },

            async getPharmacyInfo() {
                const vm = this;

                if (vm.pharmacyId) {
                    await vm.$util.axios.get(`/api/pharmacy/${vm.pharmacyId}`)
                        .then(resp => vm.pharmacy = resp.data);
                }
            },

            addToCart(item) {
                const vm = this;

                var info = {
                    pharmacyId: item.pharmacy.pharmacyId,
                    pharmacyName: item.pharmacy.name,
                    drugId: item.drugId,
                    drugName: item.name,
                    drugBrand: item.brand,
                    drugClassification: item.classification,
                    drugClassificationText: item.classificationText,
                    drugPrice: item.price.price,
                };

                vm.$bus.$emit('event:add-to-cart', info);

                vm.$bvToast.toast(`"${item.name}" added to cart.`, { title: 'Add Medicine to Shopping Cart', variant: 'success', toaster: 'b-toaster-bottom-right' });
            }
        }
    }
</script>