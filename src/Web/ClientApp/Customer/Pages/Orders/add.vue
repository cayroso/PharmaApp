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
                    <i class="fas fa-fw fa-shopping-cart mr-1"></i>Order - {{item.pharmacyName}}
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
                        <tr v-for="(drug,index) in item.items" v-bind:class="drug.drugClassification===2 ? 'table-danger':'table-white'">
                            <td>{{index+1}}</td>
                            <td>
                                {{drug.drugName}}
                                <div class="small mt-1 ml-2">
                                    {{drug.drugClassificationText}}
                                </div>
                            </td>
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

        <div v-if="requiresPrescription" class="mt-2">
            <h3 class="mb-0">
                Prescription Screenshots/Attachments
            </h3>

            <div class="mt-2">
                <div class="d-flex flex-row">
                    <b-form-file accept="*/*"
                                 v-model="imageFiles"
                                 @input="onImageFileChange"
                                 :state="imageFilesValid"
                                 placeholder="Choose a file or drop it here..."
                                 :capture="Boolean(0)"
                                 :multiple="Boolean(1)"
                                 drop-placeholder="Drop file here...">
                    </b-form-file>


                    <!--<div v-if="imageFileUrls" class="ml-2">
                        <button @click="clearImageFile(0)" class="btn btn-danger">
                            <span class="fas fa-fw fa-trash"></span>
                        </button>
                    </div>-->
                </div>
                <div v-if="validations.has('imageFiles')" class="text-danger">
                    {{validations.get('imageFiles')}}
                </div>
                <div v-if="imageFileUrls" class="mt-2">
                    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-4">
                        <div v-for="(imageFile,index) in imageFileUrls" class="mb-2 col">
                            <b-img-lazy :src="imageFile" fluid center></b-img-lazy>
                            <button @click="clearImageFile(index)" class="btn btn-outline-danger text-center mt-1">
                                <span class="fas fa-fw fa-trash"></span>
                            </button>
                        </div>
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
            pharmacyId: {
                type: String,
                required: true
            },
        },

        data() {
            return {
                isDirty: false,
                validations: new Map(),
                busy: false,

                imageFiles: null,
                imageFileUrls: null,
                imageFileObjects: null,

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
            },
            requiresPrescription() {
                const item = this.item;

                const line = item.items.find(e => e.drugClassification === 2);

                return line !== undefined;// !line;
            },
            imageFilesValid() {
                const vm = this;

                if (Array.isArray(vm.imageFiles))
                    return vm.imageFiles.length > 0;
                else
                    return vm.imageFiles != null;
            },
            formIsValid() {
                const vm = this;

                const validations = new Map();

                if (vm.requiresPrescription && !vm.imageFilesValid) {
                    validations.set('imageFiles', 'Screenshots of prescription from doctor/clinic is required.');
                }

                vm.isDirty = true;
                vm.validations = validations;

                return validations.size == 0;
            }
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
            onImageFileChange(e) {
                const vm = this;

                var files = [];

                if (Array.isArray(e)) {
                    files = e;
                }
                else {
                    files = [e];
                }

                vm.imageFileUrls = [];
                vm.imageFileObjects = [];

                for (var i = 0; i < files.length; i++) {
                    const file = files[i];
                    if (file) {

                        vm.imageFileUrls.push(URL.createObjectURL(file));
                        vm.imageFileObjects.push(file);
                    }
                }
            },

            clearImageFile(index) {
                const vm = this;

                if (Array.isArray(vm.imageFiles)) {
                    vm.imageFiles.splice(index, 1);
                    vm.imageFileUrls.splice(index, 1);
                }
                else {
                    vm.imageFiles = null;
                    vm.imageFileUrls = null;
                }
            },

            async get() {
                const vm = this;

                var shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];
                var shop = shoppingCart.find(e => e.pharmacyId == vm.pharmacyId);

                //shop.items.forEach(e => {
                //    e.quantity = 0 ;
                //})

                if (shop === undefined) {
                    vm.close();
                }
                else {
                    vm.item = shop;
                }
            },
            async save() {
                const vm = this;
                const item = vm.item;

                if (!vm.formIsValid)
                    return;

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

                const formData = new FormData();

                if (Array.isArray(vm.imageFiles)) {
                    vm.imageFiles.forEach(file => {
                        formData.append('files', file);
                    });
                }
                else {
                    formData.append('files', vm.imageFiles);
                }

                const json = JSON.stringify(payload);
                const blob = new Blob([json], {
                    type: 'application/json'
                });

                formData.append("payload", blob);

                try {
                    await vm.$util.axios.post(`/api/orders/customer/place-order`, formData)
                        .then(resp => {
                            vm.$bvToast.toast('Order placed.', { title: 'Order Medicines', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            vm.removeItem(vm.pharmacyId);

                            setTimeout(_ => {
                                vm.$router.push({ name: 'ordersView', params: { id: resp.data } });
                            }, 1000);

                        })
                } catch (e) {
                    vm.$util.handleError(e);
                }
            },

            removeItem(pharmacyId) {
                const vm = this;

                let shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];

                let shop = shoppingCart.find(e => e.pharmacyId === pharmacyId);

                if (shop) {
                    shoppingCart = shoppingCart.filter(e => e.pharmacyId !== pharmacyId);

                    localStorage.setItem('shopping-cart', JSON.stringify(shoppingCart));

                    vm.$bus.$emit('event:shopping-cart-updated');
                }


            },
        }
    }
</script>