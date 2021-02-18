<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-cubes mr-1"></i>My Orders
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <div class="mr-1">
                        <b-form-select v-model="filter.query.orderStatus" :options="lookups.orderStatuses" @change="search(1)"></b-form-select>
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
                <table-list :header="{key: 'orderId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Order Number</th>
                        <th>Total Price</th>
                        <th>Pharmacy</th>
                        <th>Date</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <router-link :to="{name:'ordersView', params:{ id: row.item.orderId}}">
                                {{row.item.number}}
                            </router-link>
                            <div class="small">
                                {{row.item.statusText}}
                            </div>
                        </td>
                        <td>
                            {{row.item.grossPrice|toCurrency}}
                        </td>
                        <td>
                            {{row.item.pharmacy.name}}
                            <div class="row">
                                <div v-if="row.item.pharmacy.phoneNumber" class="col-md-auto">
                                    <div class="small">
                                        <i class="fas fa-fw fa-phone"></i>
                                        {{row.item.pharmacy.phoneNumber}}
                                    </div>
                                </div>
                                <div v-if="row.item.pharmacy.mobileNumber" class="col-md-auto">
                                    <div class="small">
                                        <i class="fas fa-fw fa-mobile"></i>
                                        {{row.item.pharmacy.mobileNumber}}
                                    </div>
                                </div>
                                <div v-if="row.item.pharmacy.email" class="col-md-auto">
                                    <div class="small">
                                        <i class="fas fa-fw fa-at"></i>
                                        {{row.item.pharmacy.email}}
                                    </div>
                                </div>
                            </div>
                            <div v-if="row.item.pharmacy.openingHours" class="small">
                                <i class="fas fa-fw fa-clock"></i>
                                {{row.item.pharmacy.openingHours}}
                            </div>
                            <div v-if="row.item.pharmacy.address" class="small">
                                <i class="fas fa-fw fa-location-arrow"></i>
                                {{row.item.pharmacy.address}}
                            </div>
                        </td>
                        <td>
                            <ul class="list-unstyled">
                                <li>
                                    Ordered: {{row.item.dateOrdered|moment('calendar')}}
                                </li>
                                <template v-if="moment(row.item.dateStartPickup).isBefore()">
                                    <li>
                                        Start Pickup: {{row.item.dateStartPickup|moment('calendar')}}
                                    </li>
                                    <li>
                                        End Pickup: {{row.item.dateEndPickup|moment('calendar')}}
                                    </li>
                                </template>
                            </ul>
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Number</label>
                                <div class="col align-self-center">
                                    <router-link :to="{name:'ordersView', params:{ id: row.item.orderId}}">
                                        {{row.item.number}}
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
                                <label class="col-3 col-form-label">Total Price</label>
                                <div class="col align-self-center">
                                    {{row.item.grossPrice|toCurrency}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Pharmacy</label>
                                <div class="col align-self-center">
                                    {{row.item.pharmacy.name}}
                                    <div class="row">
                                        <div class="col-md-auto">
                                            <div class="small">
                                                <i class="fas fa-fw fa-phone"></i>
                                                {{row.item.pharmacy.phoneNumber}}
                                            </div>
                                        </div>
                                        <div class="col-md-auto">
                                            <div class="small">
                                                <i class="fas fa-fw fa-mobile"></i>
                                                {{row.item.pharmacy.mobileNumber}}
                                            </div>
                                        </div>
                                        <div class="col-md-auto">
                                            <div class="small">
                                                <i class="fas fa-fw fa-at"></i>
                                                {{row.item.pharmacy.email}}
                                            </div>
                                        </div>
                                    </div>
                                    <div class="small">
                                        <i class="fas fa-fw fa-clock"></i>
                                        {{row.item.pharmacy.openingHours}}
                                    </div>
                                    <div class="small">
                                        <i class="fas fa-fw fa-location-arrow"></i>
                                        {{row.item.pharmacy.address}}
                                    </div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Date</label>
                                <div class="col align-self-center">
                                    <ul class="list-unstyled">
                                        <li>
                                            Ordered: {{row.item.dateOrdered|moment('calendar')}}
                                        </li>
                                        <template v-if="moment(row.item.dateStartPickup).isBefore()">
                                            <li>
                                                Start Pickup: {{row.item.dateStartPickup|moment('calendar')}}
                                            </li>
                                            <li>
                                                End Pickup: {{row.item.dateEndPickup|moment('calendar')}}
                                            </li>
                                        </template>
                                    </ul>
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
            //pharmacyId: {
            //    type: String,
            //    required: true,
            //},
        },
        components: {
        },
        data() {
            return {
                baseUrl: `/api/orders/search-my-orders`,
                filter: {
                    cacheKey: `filter-${this.uid}/orders/search-my-orders`,
                    query: {
                        orderStatus: 0,
                        //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                        //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    }
                },
                lookups: {
                    orderStatuses: [
                        { value: '0', text: 'All' },
                        { value: '1', text: 'Placed' },
                        { value: '2', text: 'Accepted' },
                        { value: '3', text: 'Rejected' },
                        { value: '4', text: 'Ready for Pickup' },
                        { value: '5', text: 'Completed' },
                        { value: '6', text: 'Cancelled' },
                    ]
                },
                moment: moment
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
            //await vm.getPharmacyInfo();
        },

        methods: {
            initializeFilter(cache) {
                const filter = this.filter;
                const urlParams = new URLSearchParams(window.location.search);

                filter.query.criteria = urlParams.get('c') || cache.criteria || filter.query.criteria;
                filter.query.pageIndex = parseInt(urlParams.get('p'), 10) || cache.pageIndex || filter.query.pageIndex;
                filter.query.pageSize = parseInt(urlParams.get('s'), 10) || cache.pageSize || filter.query.pageSize;
                filter.query.sortField = urlParams.get('sf') || cache.sortField || filter.query.sortField;
                filter.query.sortOrder = parseInt(urlParams.get('so'), 10) || cache.sortOrder || filter.query.sortOrder;
                filter.visible = cache.visible || filter.visible;

                filter.query.orderStatus = urlParams.get('orderStatus') || cache.orderStatus || filter.query.orderStatus;

            },

            getQuery() {

                const vm = this;
                const filter = vm.filter;

                if (vm.busy)
                    return;

                const query = [
                    '?c=', encodeURIComponent(filter.query.criteria),
                    '&p=', filter.query.pageIndex,
                    '&s=', filter.query.pageSize,
                    '&sf=', filter.query.sortField,
                    '&so=', filter.query.sortOrder,
                    '&orderStatus=', filter.query.orderStatus
                ].join('');

                return query;
            },

            saveQuery() {
                const vm = this;
                const filter = vm.filter;

                localStorage.setItem(filter.cacheKey, JSON.stringify({
                    criteria: filter.query.criteria,
                    pageIndex: filter.query.pageIndex,
                    pageSize: filter.query.pageSize,
                    sortField: filter.query.sortField,
                    sortOrder: filter.query.sortOrder,
                    orderStatus: filter.query.orderStatus,
                    visible: filter.visible
                }));
            },
        }
    }
</script>