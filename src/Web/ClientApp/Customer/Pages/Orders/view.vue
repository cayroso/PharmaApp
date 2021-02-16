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
                    <i class="fas fa-fw fa-prescription-bottle-alt mr-1"></i>View Order
                </h1>
            </div>
            <div class="col-auto">
                <div>
                    <button @click="get" class="btn btn-danger">
                        <span class="fas fa-fw fa-redo"></span> Cancel
                    </button>

                    <button @click="get" class="btn btn-primary">
                        <span class="fas fa-fw fa-sync"></span>
                    </button>
                    <button @click="close" class="btn btn-secondary">
                        <span class="fas fa-fw fa-times-circle"></span>
                    </button>
                </div>
            </div>
        </div>

        <div class="mt-2">
            <hr />
            <h4>
                Order Information
            </h4>

            <div class="form-row">
                <div class="form-group col">
                    <label>Number</label>
                    <div class="form-control-plaintext">
                        {{item.number}}
                    </div>
                </div>
                <div class="form-group col">
                    <label>Status</label>
                    <div class="form-control-plaintext">
                        {{item.statusText}}
                    </div>
                </div>
                <div class="form-group col">
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

            <hr />
            <h4>
                Pharmacy
            </h4>
            <div class="form-row">
                <div class="form-group col-md">
                    <label>Name</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.name}}
                    </div>
                </div>
                <div class="form-group col">
                    <label>Open Hours</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.openingHours}}
                    </div>
                </div>
                <div class="form-group col">
                    <label>Addres</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.address}}
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col">
                    <label>Phone Number</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.phoneNumber}}
                    </div>
                </div>
                <div class="form-group col">
                    <label>Mobile Number</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.mobileNumber}}
                    </div>
                </div>
                <div class="form-group col">
                    <label>Email</label>
                    <div class="form-control-plaintext">
                        {{item.pharmacy.email}}
                    </div>
                </div>
            </div>
        </div>

        <div class="mt-2">
            <hr />
            <h4>
                Items
            </h4>
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
                        <tr v-for="(drug,index) in item.lines">
                            <td>{{index+1}}</td>
                            <td>
                                {{drug.drugName}}
                                <div class="mt-1 ml-2 small">
                                    {{drug.classificationText}}
                                </div>
                            </td>
                            <td class="text-right">
                                {{drug.drugPrice|toCurrency}}
                            </td>
                            <td class="text-right">
                                {{drug.quantity}}
                            </td>
                            <td class="text-right">
                                {{drug.extendedPrice|toCurrency}}
                            </td>
                        </tr>

                    </tbody>
                    <tfoot>
                        <tr>
                            <th colspan="4" class="text-right">Total</th>
                            <th class="text-right">{{item.grossPrice|toCurrency}}</th>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>

        <div v-if="item.fileUploadUrls && item.fileUploadUrls.length>0" class="mt-2">
            <hr />
            <h4>
                Prescription Screenshots/Attachments
            </h4>
            <div class="mt-2">
                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4">
                    <div v-for="imageUrl in item.fileUploadUrls" class="col">
                        <b-img-lazy :src="imageUrl" fluid center></b-img-lazy>
                    </div>
                </div>
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
            id: {
                type: String,
                required: true
            },
        },

        data() {
            return {
                item: {
                    pharmacy: {},
                    lines: [],
                    fileUploadUrls: []
                },
                moment: moment
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

                await vm.$util.axios.get(`/api/orders/${vm.id}`)
                    .then(resp => vm.item = resp.data);
            },

            
        }
    }
</script>