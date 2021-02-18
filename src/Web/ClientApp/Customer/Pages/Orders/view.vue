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
                <div v-if="item.status==1 || item.status==2" class="p-1 text-right">
                    <div class="dropdown ">
                        <button class="btn btn-secondary dropdown-toggle" type="button" id="dropdownMenuButton" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            <i class="fas fa-fw fa-cogs mr-1"></i>Update Status
                        </button>
                        <div class="dropdown-menu" aria-labelledby="dropdownMenuButton">
                            <!--placed, accepted-->
                            <a @click.prevent="setOrderStatus(6)" class="dropdown-item" href="#">
                                Cancel
                            </a>
                            <!--rejected, completed, cancelled-->
                            <!--<a v-if="item.status===3||item.status===5||item.status===6" @click.prevent="setOrderStatus(7)" class="dropdown-item" href="#">
                                Archive
                            </a>-->
                        </div>
                    </div>

                    <!--<button v-if="item.status==1 || item.status==2" @click="cancelOrder" class="btn btn-danger">Cancel</button>-->

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
                            <tr v-for="(li,index) in item.lines" v-bind:class="li.classification===2? 'table-danger':''">
                                <td>
                                    {{index+1}}
                                </td>
                                <td>
                                    {{li.drugName}}
                                    <div class="mt-1 ml-2 small">
                                        {{li.classificationText}}
                                    </div>
                                </td>
                                <td class="text-right">
                                    {{li.drugPrice|toCurrency}}
                                </td>
                                <td class="text-right">
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
                    <span class="fas fa-fw fa-money-bill mr-1 d-none"></span>Pharmacy
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
                                {{item.pharmacy.name}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Open Hours</label>
                            <div class="form-control-plaintext">
                                {{item.pharmacy.openingHours}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Address</label>
                            <div class="form-control-plaintext">
                                {{item.pharmacy.address}}
                            </div>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label>Phone Number</label>
                            <div v-if="item.pharmacy.phoneNumber" class="form-control-plaintext">
                                <a :href="`tel:${item.pharmacy.phoneNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-phone"></i>
                                </a>
                                {{item.pharmacy.phoneNumber}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Mobile Number</label>
                            <div v-if="item.pharmacy.mobileNumber" class="form-control-plaintext">
                                <a :href="`sms:${item.pharmacy.mobileNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-sms"></i>
                                </a>
                                {{item.pharmacy.mobileNumber}}
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label>Email</label>
                            <div v-if="item.pharmacy.email" class="form-control-plaintext">
                                <a :href="`mailto:${item.pharmacy.email}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-envelope"></i>
                                </a>
                                {{item.pharmacy.email}}
                            </div>
                        </div>
                    </div>
                </div>
            </b-collapse>
        </div>

        <div v-if="item.fileUploadUrls.length>0" class="card mt-2">
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

        <div class="card mt-2">
            <div @click="toggle('timeline')" class="card-header d-flex flex-row justify-content-between align-items-center">
                <h5 class="mb-0 align-self-start">
                    <span class="fas fa-fw fa-history mr-1 d-none"></span>Timeline
                </h5>
                <div>
                    <span>
                        <span v-if="toggles.timeline" class="fas fa-fw fa-angle-up"></span>
                        <span v-else class="fas fa-fw fa-angle-down"></span>
                    </span>
                </div>
            </div>
            <b-collapse v-model="toggles.timeline">
                
                <div class="table-responsive mb-0">
                    <table class="table">
                        <tbody>
                            <tr v-for="(tl,index) in item.timelines">
                                <td>{{index+1}}</td>
                                <td>
                                    {{tl.statusText}}
                                </td>
                                <td>
                                    {{tl.dateTimeline|moment('calendar')}}
                                </td>
                                <td>
                                    {{tl.user}} - 
                                    <span class="mt-1 small ml-2">
                                        <span v-if="tl.user===item.customer.name">
                                            Customer
                                        </span>
                                        <span v-else>
                                            Staff
                                        </span>
                                    </span>
                                </td>
                                <td>
                                    {{tl.notes}}
                                </td>
                            </tr>
                        </tbody>
                    </table>
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
                togglesKey: `view-my-order/${this.uid}/toggles`,
                toggles: {
                    information: false,
                    pharmacy: false,
                    lineItems: false,
                    prescriptions: false,
                    timelines: false,
                },
                item: {
                    pharmacy: {},
                    lines: [],
                    fileUploadUrls: [],
                    timelines: []
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

                if (vm.id === orderId)
                    await vm.get();
            });

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
                    let api = 'oyeah';
                    let reason = '';

                    switch (status) {
                        case 6:
                            api = `/api/orders/customer/cancel`
                            reason = prompt('Reason for Cancelling Order');
                            if (!reason)
                                return;
                            break;
                        case 7:
                            api = `/api/orders/customer/archive`
                            break;
                    };
                    const payload = {
                        orderId: vm.item.orderId,
                        token: vm.item.token,
                        notes: reason
                    };

                    await vm.$util.axios.put(api, payload)
                        .then(async _ => {
                            vm.$bvToast.toast('Order status updated.', { title: 'Update Order', variant: 'success' });

                            await vm.get();
                        })
                } catch (e) {
                    vm.$util.handleError(e);
                }
            }


        }
    }
</script>