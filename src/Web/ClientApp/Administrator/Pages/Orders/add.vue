<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-prescription-bottle-alt mr-1"></i>Add
                </h1>
            </div>
            <div class="col-auto">
                <div>
                    <button v-bind:disabled="isDirty && !formIsValid" @click="save" class="btn btn-primary">
                        <span class="fas fa-fw fa-save"></span>
                    </button>
                    <button @click="close" class="btn btn-secondary">
                        <span class="fas fa-fw fa-times-circle"></span>
                    </button>
                </div>
            </div>
        </div>

        <div class="mt-2">


            <div class="form-group">
                <label for="name">Name</label>
                <div>
                    <input v-model="item.name" type="text" id="name" class="form-control" v-bind:class="getValidClass('name')" />
                    <div v-if="validations.has('name')" class="invalid-feedback">
                        {{validations.get('name')}}
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md">
                    <label for="price">Price</label>
                    <div>
                        <input v-model="item.price" type="number" id="price" class="form-control" v-bind:class="getValidClass('price')" />
                        <div v-if="validations.has('price')" class="invalid-feedback">
                            {{validations.get('price')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="brandId">Brand</label>
                    <div>
                        <b-form-select v-model="item.brandId" :options="lookups.brands" id="brandId" value-field="id" text-field="name" v-bind:class="getValidClass('brandId')"></b-form-select>
                        <div v-if="validations.has('brandId')" class="invalid-feedback">
                            {{validations.get('brandId')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="classification">Classification</label>
                    <div>
                        <b-form-select v-model="item.classification" :options="lookups.classifications" id="classification" value-field="id" text-field="name" v-bind:class="getValidClass('classification')"></b-form-select>
                        <div v-if="validations.has('classification')" class="invalid-feedback">
                            {{validations.get('classification')}}
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md">
                    <label for="stock">Stock</label>
                    <div>
                        <input v-model="item.stock" type="number" min="0" id="stock" class="form-control" v-bind:class="getValidClass('stock')" />
                        <div v-if="validations.has('stock')" class="invalid-feedback">
                            {{validations.get('stock')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="safetyStock">Safety Stock</label>
                    <div>
                        <input v-model="item.safetyStock" type="number" min="0" id="safetyStock" class="form-control" v-bind:class="getValidClass('safetyStock')" />
                        <div v-if="validations.has('safetyStock')" class="invalid-feedback">
                            {{validations.get('safetyStock')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="reorderLevel">Reorder Level</label>
                    <div>
                        <input v-model="item.reorderLevel" type="number" min="0" id="reorderLevel" class="form-control" v-bind:class="getValidClass('reorderLevel')" />
                        <div v-if="validations.has('reorderLevel')" class="invalid-feedback">
                            {{validations.get('reorderLevel')}}
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
        },
        components: {
            //uploadPrescription
        },
        data() {
            return {
                isDirty: false,
                validations: new Map(),
                lookups: {
                    brands: [],
                    classifications: [
                        { id: 1, name: 'Over The Counter' },
                        { id: 2, name: 'Prescription' }
                    ]
                },
                item: {
                    brandId: null,
                    classification: null,
                    name: null,
                    price: 0,
                    stock: 0,
                }
            };
        },

        computed: {

            formIsValid() {
                const vm = this;

                //if (!vm.isDirty)
                //    return true;

                const item = vm.item;

                const validations = new Map();

                if (!item.brandId) {
                    validations.set('brandId', 'Brand is required.');
                }
                if (!item.classification) {
                    validations.set('classification', 'Classification is required.');
                }
                if (!item.name) {
                    validations.set('name', 'Name is required.');
                }
                if (isNaN(item.price)) {
                    validations.set('price', 'Price is invalid.');
                }
                if (isNaN(item.stock)) {
                    validations.set('stock', 'Stock is invalid.');
                }
                if (isNaN(item.safetyStock)) {
                    validations.set('safetyStock', 'Safety Stock is invalid.');
                }
                if (isNaN(item.reorderLevel)) {
                    validations.set('reorderLevel', 'Reorder Level is invalid.');
                }

                vm.isDirty = true;
                vm.validations = validations;

                return validations.size == 0;
            },
        },

        async created() {
            const vm = this;

        },

        async mounted() {
            const vm = this;

            await vm.getBrands();
        },

        methods: {
            async getBrands() {
                const vm = this;

                await vm.$util.axios.get(`/api/lookups/brands`)
                    .then(resp => vm.lookups.brands = resp.data);
            },
            getValidClass(field) {
                const vm = this;

                if (!vm.isDirty)
                    return '';

                if (vm.validations.has(field))
                    return 'is-invalid';
                return 'is-valid';
            },

            async save() {
                const vm = this;

                if (vm.busy)
                    return;

                if (!vm.formIsValid)
                    return;

                try {
                    vm.busy = true;

                    const item = vm.item;

                    const payload = vm.$util.clone(item);

                    await vm.$util.axios.post(`/api/drugs/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('New medicine created.', { title: 'Add Medicine', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            vm.close();
                        })
                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false
                }
            }
        }
    }
</script>