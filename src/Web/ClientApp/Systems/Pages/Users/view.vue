<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-user mr-1"></i>View User
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

        <div class="mt-2">
            <div class="card">
                <div class="card-header">
                    Information
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <label>Name</label>
                        <div>
                            {{item.user.firstName}} {{item.user.middleName}} {{item.user.lastName}}
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md">
                            <label for="email">Email</label>
                            <div>
                                <input v-model="item.user.email" type="email" id="email" class="form-control" readonly />
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label for="phoneNumber">Phone</label>
                            <div>
                                <input v-model="item.user.phoneNumber" type="tel" id="phoneNumber" class="form-control" readonly />
                            </div>
                        </div>
                        <div class="form-group col-md">
                            <label for="mobileNumber">Mobile</label>
                            <div>
                                <input v-model="item.user.mobileNumber" type="tel" id="mobileNumber" class="form-control" readonly />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            

            <div v-if="item.pharmacy" class="mt-2">
                <div class="card">
                    <div class="card-header">
                        Pharmacy Information
                    </div>
                    <div class="card-body">
                        <div class="form-group">
                            <label>Name</label>
                            <div>
                                <router-link :to="{name:'pharmaciesView', params:{id:item.pharmacy.pharmacyId}}">
                                    {{item.pharmacy.name}}
                                </router-link>
                                
                            </div>
                        </div>
                        <div class="form-group">
                            <label>Address</label>
                            <div>
                                {{item.pharmacy.address}}
                            </div>
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
            id: { type: String, required: true }
        },

        data() {
            return {
                item: {
                    pharmacy: {},
                    user: {}
                }
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

                if (vm.busy)
                    return;

                try {
                    vm.busy = true;

                    vm.$util.axios.get(`/api/systems/default/users/${vm.id}/`)
                        .then(resp => {
                            vm.item = resp.data;
                        });

                } catch (e) {
                    vm.$util.handleError(e);
                } finally {
                    vm.busy = false;
                }
            }
        }
    }
</script>