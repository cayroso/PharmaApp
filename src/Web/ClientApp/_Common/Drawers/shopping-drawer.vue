<template>
    <b-sidebar id="shoppingDrawer" title="My Shopping Cart" backdrop right shadow>
        <!--{{shoppingCart}}-->
        <div v-if="shoppingCart" class="px-2" v-cloak>
            <ul class="list-unstyled">
                <li v-for="shop in shoppingCart">
                    <div class="d-flex flex-row justify-content-between align-items-center">
                        <span>{{shop.pharmacyName}}</span>
                        <button @click="checkout(shop.pharmacyId)" class="btn btn-sm btn-outline-primary">
                            <span class="fas fa-fw fa-shopping-cart"></span>
                        </button>
                    </div>
                    <ol>
                        <li v-for="(item,index) in shop.items" class="my-1">
                            <div class="d-flex flex-row justify-content-between align-items-center">
                                {{item.drugName}} - {{item.drugPrice|toCurrency}}
                                <button @click="removeItem(shop.pharmacyId, index)" class="btn btn-sm btn-outline-danger mr-5">
                                    <i class="fas fa-fw fa-trash"></i>
                                </button>
                            </div>
                        </li>
                    </ol>
                </li>
            </ul>
        </div>
    </b-sidebar>
</template>
<script>
    export default {
        props: {
            uid: String
        },
        data() {
            return {
                shoppingCart: []
            }
        },
        async mounted() {
            const vm = this;

            await vm.get();

            vm.$bus.$on('event:shopping-cart-updated', function () {
                vm.get();
            })
        },
        methods: {

            async get() {
                const vm = this;

                let shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];

                vm.shoppingCart = shoppingCart;
            },

            removeItem(pharmacyId, index) {
                const vm = this;

                let shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];

                let shop = shoppingCart.find(e => e.pharmacyId === pharmacyId);

                if (shop) {
                    shop.items.splice(index, 1);

                    if (shop.items.length <= 0) {
                        shoppingCart = shoppingCart.filter(e => e.pharmacyId !== pharmacyId);
                    }
                }

                localStorage.setItem('shopping-cart', JSON.stringify(shoppingCart));

                vm.get();
            },
            checkout(pharmacyId) {
                const vm = this;

                vm.$router.push({ name: 'ordersAdd', params: { pharmacyId: pharmacyId } });
            }
        }
    };
</script>