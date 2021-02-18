<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-tachometer-alt mr-1"></i>Dashboard
                </h1>
            </div>
            <div class="col-auto">
                <button @click="get" class="btn btn-outline-primary">
                    <i class="fas fa-fw fa-sync"></i>
                </button>
            </div>
        </div>
        
        <div class="row mt-2">
            <div class="col-sm mb-2">
                <div class="card border-success">
                    <div class="card-header bg-success text-white">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <b>Top Pharmacy</b>
                            <i class="fas fa-fw fa-lg fa-clinic-medical"></i>
                        </div>

                    </div>
                    <div class="table-responsive mb-0">
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Name</th>
                                    <th class="text-right">Sales</th>
                                    <th class="text-right">Orders</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(t,index) in item.topCustomers">
                                    <td>{{index+1}}</td>
                                    <td>{{t.name}}</td>
                                    <td class="text-right">{{t.totalPrice|toCurrency}}</td>
                                    <td class="text-right">{{t.totalOrders}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="col-sm mb-2">
                <div class="card border-success">
                    <div class="card-header bg-success text-white">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <b>Top Medicines</b>
                            <i class="fas fa-fw fa-lg fa-prescription-bottle-alt"></i>
                        </div>
                    </div>
                    <div class="table-responsive mb-0">
                        <table class="table table-sm">
                            <thead>
                                <tr>
                                    <th>#</th>
                                    <th>Name</th>
                                    <th class="text-right">Paid</th>
                                    <th class="text-right">Bought</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(t,index) in item.topMedicines">
                                    <td>{{index+1}}</td>
                                    <td>{{t.name}}</td>
                                    <td class="text-right">{{t.totalPrice|toCurrency}}</td>
                                    <td class="text-right">{{t.totalCount}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>


            <div class="col-sm mb-2">
                <div class="card border-success">
                    <div class="card-header bg-success text-white">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <b>Order Summary</b>
                            <i class="fas fa-fw fa-lg fa-cubes"></i>
                        </div>
                    </div>
                    <div class="table-responsive mb-0">
                        <table class="table table-sm">

                            <tbody>
                                <tr>
                                    <td class="text-center">{{item.numberOfCompleted}}</td>
                                    <td class="text-right">
                                        <router-link :to="{name:'ordersIndex', query:{ orderStatus: 5}}">
                                            Completed <i class="fas fa-fw fa-chevron-circle-right"></i>
                                        </router-link>
                                    </td>

                                </tr>
                                <tr>
                                    <td class="text-center">{{item.numberOfRejected}}</td>
                                    <td class="text-right">
                                        <router-link :to="{name:'ordersIndex', query:{ orderStatus: 3}}">
                                            Rejected <i class="fas fa-fw fa-chevron-circle-right"></i>
                                        </router-link>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="text-center">{{item.numberOfCancelled}}</td>
                                    <td class="text-right">
                                        <router-link :to="{name:'ordersIndex', query:{ orderStatus: 6}}">
                                            Cancelled <i class="fas fa-fw fa-chevron-circle-right"></i>
                                        </router-link>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>





        </div>
    </div>
</template>
<script>
    import pageMixin from '../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
        },

        data() {
            return {
                item: {}
            };
        },

        computed: {

        },

        async created() {
            const vm = this;

        },

        async mounted() {
            const vm = this;

            await vm.get();
        },

        methods: {
            async get() {
                const vm = this;

                try {
                    await vm.$util.axios.get(`/api/customers/default/dashboard`)
                        .then(resp => vm.item = resp.data);
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>