<style scoped>
    label {
        font-size: small;
        font-weight: bold;
    }
</style>
<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-shopping-cart mr-1"></i>Order
                </h1>
            </div>
            <div class="col-auto">
                <div>
                    <button @click="get" class="btn btn-primary">
                        <span class="fas fa-fw fa-sync"></span>
                    </button>
                    <button @click="save" class="btn btn-primary">
                        <span class="fas fa-fw fa-save"></span>
                    </button>

                    <button @click="close" class="btn btn-secondary">
                        <span class="fas fa-fw fa-times-circle"></span>
                    </button>
                </div>
            </div>
        </div>

        <div class="mt-2">

            {{item.pharmacyName}}
            <div class="table-responsive mb-0">
                <table class="table table-bordered">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Name</th>
                            <th>Price</th>
                            <th>Quantity</th>
                            <th>Ext Price</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="(drug,index) in item.items">
                            <td>{{index+1}}</td>
                            <td>{{drug.drugName}}</td>
                            <td>{{drug.drugPrice}}</td>
                            <td>
                                <input type="number" v-model="drug.drugQuantity" min="0" class="form-control" />
                            </td>
                            <td class="text-right">{{(drug.drugQuantity*drug.drugPrice)|toCurrency}}</td>
                        </tr>

                    </tbody>
                    <tfoot>
                        <tr>
                            <th colspan="4" class="text-right">Total</th>
                            <th class="text-right">{{totalPrice|toCurrency}}</th>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
            pharmacyId: {
                type: String,
                required: true
            },
        },

        data() {
            return {
                item: {
                    items: []
                }
            };
        },

        computed: {
            totalPrice() {
                const vm = this;
                const item = vm.item;

                let total = 0;

                item.items.forEach(e => {
                    total += e.drugPrice * e.drugQuantity;
                });

                return total;
            }
        },

        async created() {
            const vm = this;

        },

        async mounted() {
            const vm = this;

            await vm.get();


        },
        watch: {
            async $route(to, from) {
                const vm = this
                //this.show = false;
                await vm.get();
            }
        },
        methods: {
            async get() {
                const vm = this;

                var shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];
                var shop = shoppingCart.find(e => e.pharmacyId == vm.pharmacyId);

                //shop.items.forEach(e => {
                //    e.quantity = 0 ;
                //})

                vm.item = shop;
            },
            async save() {
                const vm = this;
                const item = vm.item;

                const items = item.items.filter(e => e.drugQuantity > 0).map(e => {
                    return {
                        drugId: e.drugId,
                        drugQuantity: e.drugQuantity
                    };
                });
                const payload = {
                    pharmacyId: item.pharmacyId,
                    items: items
                };

                try {
                    await vm.$util.axios.post(`/api/orders/customer/add-order`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('Reservation placed.', { title: 'Reserve Medicines', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            setTimeout(_ => {
                                vm.$router.push({ name: 'ordersView', params: { id: resp.data } });
                            }, 2000);

                        })
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>