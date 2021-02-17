<template>
    <div v-cloak>

        {{item}}
        <div class="row row-cols-2 row-cols-sm-3">
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Orders</h5>
                        
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.numberOfCancelled}}</h5>
                            <div>
                                Cancelled
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.numberOfRejected}}</h5>
                            <div>
                                Rejected
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.numberOfCompleted}}</h5>
                            <div>
                                Completed
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Sales</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>
                                {{item.monthSales|toCurrency}}

                            </h5>
                            <div>
                                Month
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>
                                {{item.weekSales|toCurrency}}
                                <i v-if="item.weekSales>=item.lastWeekSales" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </h5>
                            <div>
                                Week
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>
                                {{item.todaySales|toCurrency}}
                                <i v-if="item.todaySales>=item.yesterdaySales" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </h5>
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
                        <h5 class="card-title">Sold</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.monthSold}}</h5>
                            <div>
                                Month
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>
                                {{item.weekSold}}
                                <i v-if="item.weekSold>=item.yesterdaySold" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </h5>
                            <div>
                                Week
                            </div>
                        </div>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>
                                {{item.todaySold}}
                                <i v-if="item.todaySold>=item.yesterdaySold" class="fas fa-fw fa-arrow-up text-success"></i>
                                <i v-else class="fas fa-fw fa-arrow-down text-danger"></i>
                            </h5>
                            <div>
                                Today
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            </div>
        <div class="row">
            <div class="col-sm mb-2">
                <div class="card">
                    <div class="card-header">
                        Top Medicines
                    </div>
                    <div class="table-responsive mb-0">
                        <table class="table">
                            <tbody>
                                <tr v-for="(t,index) in item.topMedicines">
                                    <td>{{t}}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="col-sm mb-2">
                <div class="card">
                    <div class="card-body">
                        <h5 class="card-title">Attachments</h5>
                        <div class="d-flex flex-row justify-content-between">
                            <h5>{{item.attachments}}</h5>
                            <div>
                                <i class="fas fa-fw fa-lg fa-paperclip"></i>
                            </div>
                        </div>
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
                    await vm.$util.axios.get(`/api/administrators/default/dashboard`)
                        .then(resp => vm.item = resp.data);
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>