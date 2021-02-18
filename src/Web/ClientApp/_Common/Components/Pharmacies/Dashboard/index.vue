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
        <div class="row row-cols-1 row-cols-sm-3 mt-2">

            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <h5 class="card-title">Sales</h5>
                            <i class="fas fa-fw fa-lg fa-money-bill"></i>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.monthSales|toCurrency}}
                            </b>
                            <div>
                                Month
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.weekSales|toCurrency}}
                                <i v-if="item.weekSales>=item.lastWeekSales" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </b>
                            <div>
                                Week
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.todaySales|toCurrency}}
                                <i v-if="item.todaySales>=item.yesterdaySales" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </b>
                            <div>
                                Today
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <h5 class="card-title">Sold</h5>
                            <i class="fas fa-fw fa-lg fa-truck-moving"></i>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>{{item.monthSold}}</b>
                            <div>
                                Month
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.weekSold}}
                                <i v-if="item.weekSold>=item.yesterdaySold" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </b>
                            <div>
                                Week
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.todaySold}}
                                <i v-if="item.todaySold>=item.yesterdaySold" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </b>
                            <div>
                                Today
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <div class="d-flex flex-row justify-content-between align-items-baseline">
                            <h5 class="card-title">Orders</h5>
                            <i class="fas fa-fw fa-lg fa-cubes"></i>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>{{item.numberOfCompleted}}</b>
                            <div>
                                <router-link :to="{name:'orders', query:{ orderStatus: 5}}">
                                    Completed <i class="fas fa-fw fa-chevron-circle-right"></i>
                                </router-link>
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>
                                {{item.numberOfRejected}}
                            </b>
                            <div>
                                <router-link :to="{name:'orders', query:{ orderStatus: 3}}">
                                    Rejected <i class="fas fa-fw fa-chevron-circle-right"></i>
                                </router-link>
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <b>{{item.numberOfCancelled}}</b>
                            <div>
                                <router-link :to="{name:'orders', query:{ orderStatus: 6}}">
                                    Cancelled <i class="fas fa-fw fa-chevron-circle-right"></i>
                                </router-link>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
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
                                    <th class="text-right">Sales</th>
                                    <th class="text-right">Sold</th>
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
                            <b>Top Customers</b>
                            <i class="fas fa-fw fa-lg fa-user-tie"></i>
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

        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../../_Core/Mixins/pageMixin';

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
                    await vm.$util.axios.get(`/api/administrators/default/dashboard`)
                        .then(resp => vm.item = resp.data);
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>