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
                    <i class="fas fa-fw fa-cubes mr-1"></i>View
                </h1>
            </div>
            <div class="col-auto">
                <div>
                    <button @click="get" class="btn btn-primary">
                        <span class="fas fa-fw fa-sync"></span>
                    </button>
                    <button @click="close" class="btn btn-secondary">
                        <span class="fas fa-fw fa-times-circle"></span>
                    </button>
                </div>
            </div>
        </div>

        <div class="card mt-2">
            <div @click="toggle('information')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-money-bill mr-1 d-none"></span>Order Information
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.information" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.information">
                <!--not archived-->
                <div v-if="item.status!==7 && item.status!==3" class="p-1 text-right">
                    <div class="dropdown ">
                        <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i class="fas fa-fw fa-cogs mr-1"></i>Update Status
                        </button>
                        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                            <!--placed-->
                            <a v-if="item.status===1" @click.prevent="setOrderStatus(2)" class="dropdown-item" href="#">
                                <i v-if="item.status===2" class="fas fa-fw fa-check"></i>
                                Accepted
                            </a>
                            <!--placed, accepted-->
                            <a v-if="item.status===1 || item.status===2 || item.status===4" @click.prevent="setOrderStatus(3)" class="dropdown-item" href="#">
                                <i v-if="item.status===3" class="fas fa-fw fa-check"></i>
                                Reject
                            </a>
                            <!--accepted-->
                            <a v-if="item.status===2" @click.prevent="setOrderStatus(4)" class="dropdown-item" href="#">
                                <i v-if="item.status===4" class="fas fa-fw fa-check"></i>
                                Ready for Pickup
                            </a>
                            <!--readyforpickup-->
                            <a v-if="item.status===4" @click.prevent="setOrderStatus(5)" class="dropdown-item" href="#">
                                <i v-if="item.status===5" class="fas fa-fw fa-check"></i>
                                Completed
                            </a>
                            <!--cancelled, completed-->
                            <!--<a v-if="item.status===5 || item.status===6" @click.prevent="setOrderStatus(7)" class="dropdown-item" href="#">
                                <i v-if="item.status===7" class="fas fa-fw fa-check"></i>
                                Archived
                            </a>-->
                        </div>
                    </div>
                </div>
                <div class="p-2">
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label>Number</label>
                            <div class="form-control-plaintext">
                                {{item.number}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Status</label>
                            <div class="form-control-plaintext">
                                {{item.statusText}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Total Price</label>
                            <div class="form-control-plaintext">
                                {{item.grossPrice|toCurrency}}
                            </div>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label>Order Date</label>
                            <div class="form-control-plaintext">
                                {{item.dateOrdered|moment('calendar')}}
                            </div>
                        </div>
                        <template v-if="moment(item.dateStartPickup).isBefore()">
                            <div class="form-group col-md">
                                <label>Pickup Start Date</label>
                                <div class="form-control-plaintext">
                                    {{item.dateStartPickup|moment('calendar')}}
                                </div>
                            </div>
                            <div class="form-group col-md">
                                <label>Pickup End Date</label>
                                <div class="form-control-plaintext">
                                    {{item.dateEndPickup|moment('calendar')}}
                                </div>
                            </div>
                        </template>
                    </div>
                </div>
            </b-collapse>
        </div>

        <div class="card mt-2">
            <div @click="toggle('lineItems')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-money-bill mr-1 d-none"></span>Line Items
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.lineItems" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.lineItems">
                <div class="table-responsive mb-0">
                    <table class="table table-bordered table-sm">
                        <thead>
                            <tr>
                                <th>#</th>
                                <th>Medicine</th>
                                <th>Price</th>
                                <th>Quantity</th>
                                <th>Ext Price</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(li,index) in item.lines">
                                <td>
                                    {{index+1}}
                                </td>
                                <td>
                                    {{li.drugName}}
                                </td>
                                <td>
                                    {{li.drugPrice}}
                                </td>
                                <td>
                                    {{li.quantity}}
                                </td>
                                <td class="text-right">
                                    {{li.extendedPrice|toCurrency}}
                                </td>
                            </tr>
                        </tbody>
                        <tfoot>
                            <tr>
                                <th colspan="4" class="text-right">
                                    Total
                                </th>
                                <th class="text-right">
                                    {{item.grossPrice|toCurrency}}
                                </th>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </b-collapse>
        </div>

        <div class="card mt-2">
            <div @click="toggle('pharmacy')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-money-bill mr-1 d-none"></span>Customer
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.pharmacy" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.pharmacy">
                <div class="p-2">
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label>Name</label>
                            <div class="form-control-plaintext">
                                {{item.customer.name}}
                            </div>
                        </div>

                        <div class="form-group col-md">
                            <label>Phone Number</label>
                            <div v-if="item.customer.phoneNumber" class="form-control-plaintext">
                                <a :href="`tel:${item.customer.phoneNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-phone"></i>
                                </a>
                                {{item.customer.phoneNumber}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Mobile Number</label>
                            <div v-if="item.customer.mobileNumber" class="form-control-plaintext">
                                <a :href="`sms:${item.customer.mobileNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-sms"></i>
                                </a>
                                {{item.customer.mobileNumber}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Email</label>
                            <div v-if="item.customer.email" class="form-control-plaintext">
                                <a :href="`mailto:${item.customer.email}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-envelope"></i>
                                </a>
                                {{item.customer.email}}
                            </div>
                        </div>
                    </div>
                </div>
            </b-collapse>
        </div>

        <div  v-if="item.fileUploadUrls.length>0" class="card mt-2">
            <div @click="toggle('prescriptions')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-money-bill mr-1 d-none"></span>Prescriptions
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.prescriptions" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.prescriptions">
                <div class="p-2">
                    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4">
                        <div v-for="imageUrl in item.fileUploadUrls" class="col">
                            <b-img-lazy :src="imageUrl" fluid center></b-img-lazy>
                        </div>
                    </div>
                </div>
            </b-collapse>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../_Core/Mixins/pageMixin';

    export default {
        mixins: [pageMixin],

        props: {
            uid: String,
            id: {
                type: String,
                required: true
            },
        },

        data() {
            return {
                togglesKey: `view-customer-order/${this.uid}/toggles`,
                toggles: {
                    information: false,
                    pharmacy: false,
                    lineItems: false,
                    prescriptions: false,
                },
                item: {
                    customer: {},
                    lines: [],
                    fileUploadUrls: []
                },
                moment: moment
            };
        },

        computed: {

        },
        watch: {
            async $route(to, from) {
                const vm = this
                //this.show = false;
                await vm.get();
            }
        },
        async created() {
            const vm = this;
            const toggles = JSON.parse(localStorage.getItem(vm.togglesKey)) || null;

            if (toggles)
                vm.toggles = toggles;
        },

        async mounted() {
            const vm = this;

            vm.$bus.$on('event:order-updated', async (orderId) => {
                console.log('fuck', orderId);
                if (vm.id === orderId)
                    await vm.get();
            });
            //vm.$bus.$on('event:customer-place-order', vm.getIfOrder);
            //vm.$bus.$on('event:customer-cancelledOrder', vm.getIfOrder);
            //vm.$bus.$on('event:customer-set-order-to-archived', vm.getIfOrder);

            await vm.get();
        },

        methods: {
            //async getIfOrder(info) {
            //    const vm = this;
            //    debugger;
            //    if (info.orderId === vm.id) {
            //        await vm.get();
            //    }
            //},
            async get() {
                const vm = this;

                await vm.$util.axios.get(`/api/orders/${vm.id}`)
                    .then(resp => vm.item = resp.data);
            },
            async setOrderStatus(status) {
                const vm = this;

                try {
                    let api = '/api/oyeah';
                    let notes = '';

                    switch (status) {
                        case 2:
                            api = `/api/orders/pharmacy/accept`
                            break;
                        case 3:
                            api = `/api/orders/pharmacy/reject`
                            notes = prompt('Reason for Rejecting Order');
                            if (!notes)
                                return;
                            break;
                        case 4:
                            api = `/api/orders/pharmacy/ready-for-pickup`
                            break;
                        case 5:
                            api = `/api/orders/pharmacy/completed`
                            break;
                        case 7:
                            api = `/api/orders/pharmacy/archived`
                            break;
                    };

                    const payload = {
                        orderId: vm.item.orderId,
                        token: vm.item.token,
                        notes: notes
                    };

                    await vm.$util.axios.put(api, payload)
                        .then(async _ => {
                            vm.$bvToast.toast('Order status updated.', { title: 'Update Order', variant: 'success' });

                            await vm.get();
                        });
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }
        }
    }
</script>