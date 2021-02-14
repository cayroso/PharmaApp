<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-shopping-cart mr-1"></i>Orders
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
                <table-list :header="{key: 'orderId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Order Number</th>
                        <th>Total Price</th>
                        <th>Customer</th>
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
                            {{row.item.customer.name}}
                            <div>
                                <div v-if="row.item.customer.phoneNumber">
                                    <div class="small">
                                        <i class="fas fa-fw fa-phone"></i>
                                        {{row.item.customer.phoneNumber}}
                                    </div>
                                </div>                                
                                <div v-if="row.item.customer.email">
                                    <div class="small">
                                        <i class="fas fa-fw fa-at"></i>
                                        {{row.item.customer.email}}
                                    </div>
                                </div>
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
    //import modalAddMember from '../../Modals/Teams/add-member.vue';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            //modalAddTask
            //modalAddMember
        },
        data() {
            return {
                baseUrl: `/api/orders/search-pharmacy-orders`,
                filter: {
                    cacheKey: `filter-${this.uid}/orders/search-pharmacy-orders`,
                    //query: {
                    //    orderStatus: 0,
                    //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                    //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    //}
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

        },

        methods: {
            
        }
    }
</script>