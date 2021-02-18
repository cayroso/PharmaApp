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
                    <i class="fas fa-fw fa-clinic-medical mr-1"></i>Pharmacy Information
                </h1>
            </div>
            <div class="col-auto">
                <div>
                     <button  @click="get" class="btn btn-primary">
                        <span class="fas fa-fw fa-sync"></span>
                    </button>
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
                    <label for="phoneNumber">Phone</label>
                    <div>
                        <input v-model="item.phoneNumber" type="tel" id="phoneNumber" class="form-control" v-bind:class="getValidClass('phoneNumber')" />
                        <div v-if="validations.has('phoneNumber')" class="invalid-feedback">
                            {{validations.get('phoneNumber')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="mobileNumber">Mobile</label>
                    <div>
                        <input v-model="item.mobileNumber" type="tel" id="mobileNumber" class="form-control" v-bind:class="getValidClass('mobileNumber')" />
                        <div v-if="validations.has('mobileNumber')" class="invalid-feedback">
                            {{validations.get('mobileNumber')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="email">Email</label>
                    <div>
                        <input v-model="item.email" type="email" id="email" class="form-control" v-bind:class="getValidClass('email')" />
                        <div v-if="validations.has('email')" class="invalid-feedback">
                            {{validations.get('email')}}
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-row">
                <div class="form-group col-md-4">
                    <label for="pharmacyStatus">Status</label>
                    <div>
                        <b-form-select v-model="item.pharmacyStatus" :options="lookups.statuses" value-field="id" text-field="name" id="pharmacyStatus" class="form-control" v-bind:class="getValidClass('pharmacyStatus')" />
                        <div v-if="validations.has('pharmacyStatus')" class="invalid-feedback">
                            {{validations.get('pharmacyStatus')}}
                        </div>
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="openingHours">Opening Hours</label>
                    <div>
                        <input v-model="item.openingHours" type="text" id="openingHours" class="form-control" v-bind:class="getValidClass('openingHours')" />
                        <div v-if="validations.has('openingHours')" class="invalid-feedback">
                            {{validations.get('openingHours')}}
                        </div>
                    </div>
                </div>
            </div>

            <div class="form-group">
                <label for="address">Address</label>
                <div>
                    <textarea v-model="item.address" type="text" id="address" class="form-control" v-bind:class="getValidClass('address')"></textarea>
                    <div v-if="validations.has('address')" class="invalid-feedback">
                        {{validations.get('address')}}
                    </div>
                </div>
            </div>
        </div>

        <div style="height:500px;">
            <g-map ref="gmap" map-name="local-map"
                   :fixed="false"
                   :show-location="true"
                   :cx="item.geoX"
                   :cy="item.geoY"
                   :draggable="true"
                   @onMapReady="onMapReady"
                   @onAddress="onAddress">
            </g-map>
        </div>
    </div>
</template>
<script>
    import pageMixin from '../../../../_Core/Mixins/paginatedMixin';
    import gMap from './_map.vue';

    export default {
        mixins: [pageMixin],
        components: {
            gMap
        },
        props: {
            uid: String,
        },

        data() {
            return {
                isDirty: false,
                validations: new Map(),
                lookups: {                    
                    statuses: [
                        { id: 1, name: 'Open' },
                        { id: 2, name: 'Closed' },
                        { id: 3, name: 'Holiday Closed' }
                    ]
                },
                item: {
                    geoX: 0,
                    geoY: 0
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

                if (!item.pharmacyStatus) {
                    validations.set('pharmacyStatus', 'Status is required.');
                }
                if (!item.name) {
                    validations.set('name', 'Name is required.');
                }
                if (!item.openingHours) {
                    validations.set('openingHours', 'Opening Hours is required.');
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
            //await vm.get();
        },

        methods: {
            async onMapReady() {
                const vm = this;

                await vm.get();

                const gmap = vm.$refs.gmap;
                
                gmap.initMap(vm.item.geoX, vm.item.geoY);
            },
            onAddress(address, location) {
                const vm = this;
                vm.item.address = address.formatted_address;
                vm.item.geoX = location.lat();
                vm.item.geoY = location.lng();
            },
            getValidClass(field) {
                const vm = this;

                if (!vm.isDirty)
                    return '';

                if (vm.validations.has(field))
                    return 'is-invalid';
                return 'is-valid';
            },
            async get() {
                const vm = this;

                await vm.$util.axios.get(`/api/pharmacy`)
                    .then(resp => vm.item = resp.data);
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
                    
                    await vm.$util.axios.put(`/api/pharmacy/`, payload)
                        .then(resp => {
                            vm.$bvToast.toast('Pharmacy information created.', { title: 'Updated Pharmacy Information', variant: 'success', toaster: 'b-toaster-bottom-right' });

                            vm.get();
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