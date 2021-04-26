<template>
    <div v-cloak>

        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-clinic-medical mr-1"></i>Pharmacies
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <!--<div class="mr-1">
                        <router-link to="/staffs/add" class="btn btn-primary">
                            <i class="fas fa-plus"></i>
                        </router-link>
                    </div>-->

                    <div class="flex-grow-1">
                        <div class="input-group">
                            <input v-model="filter.query.criteria" @keyup.enter="search(1)" type="text" class="form-control" placeholder="Enter criteria..." aria-label="Enter criteria..." aria-describedby="button-addon2">
                            <div class="input-group-append">
                                <button @click="search(1)" class="btn btn-primary" type="button" id="button-addon2">
                                    <span class="fa fas fa-fw fa-search"></span>
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <b-collapse v-model="filter.visible">

        </b-collapse>

        <b-overlay :show="busy">
            <div class="mt-2 table-responsive shadow-sm">
                <table-list :header="{key: 'pharmacyId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Name</th>
                        <th>Email</th>
                        <th>Phone</th>
                        <th>Mobile</th>
                        <th>Address</th>                        
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <router-link :to="{name:'pharmaciesView', params:{id:row.item.pharmacyId}}">
                                {{row.item.name}}
                            </router-link> 
                        </td>
                        <td>
                            <div v-if="row.item.email">
                                <a :href="`mailto:${row.item.email}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-envelope"></i>
                                </a>
                                {{row.item.email}}
                            </div>
                        </td>
                        <td>
                            <div v-if="row.item.phoneNumber">
                                <a :href="`tel:${row.item.phoneNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-phone"></i>
                                </a>
                                {{row.item.phoneNumber}}
                            </div>
                        </td>
                        <td>
                            <div v-if="row.item.mobileNumber">
                                <a :href="`tel:${row.item.mobileNumber}`" class="btn btn-outline-primary">
                                    <i class="fas fa-fw fa-phone"></i>
                                </a>
                                {{row.item.mobileNumber}}
                            </div>
                        </td>
                        <td>
                            {{row.item.address}}
                        </td>                        
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <b-avatar :src="row.item.urlProfilePicture"></b-avatar>

                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    {{row.item.firstName}} {{row.item.middleName}} {{row.item.lastName}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Email</label>
                                <div class="col align-self-center">
                                    <div v-if="row.item.email">
                                        <a :href="`mailto:${row.item.email}`" class="btn btn-outline-primary">
                                            <i class="fas fa-fw fa-envelope"></i>
                                        </a>
                                        {{row.item.email}}
                                    </div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Phone Number</label>
                                <div class="col align-self-center">
                                    <div v-if="row.item.phoneNumber">
                                        <a :href="`tel:${row.item.phoneNumber}`" class="btn btn-outline-primary">
                                            <i class="fas fa-fw fa-phone"></i>
                                        </a>
                                        {{row.item.phoneNumber}}
                                    </div>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Roles</label>
                                <div class="col align-self-center">
                                    <ul class="list-unstyled">
                                        <li v-for="r in row.item.roles">
                                            <i v-if="r=='Administrator'" class="fas fa-fw fa-user-shield"></i>
                                            <i v-else class="fas fa-fw fa-user"></i>
                                            {{r}}
                                        </li>
                                    </ul>
                                </div>
                            </div>

                            <div class="form-group mb-0 row no-gutters">
                                <div class="offset-3 col align-self-center">
                                    <!--<button v-if="row.item.userId !== uid" @click="$bus.$emit('event:send-message',row.item.staffId)" class="btn btn-sm btn-outline-primary">
                                        <i class="fas fa-fw fa-comment"></i>
                                    </button>-->
                                    <button @click="openManageUserRole(row.item)" class="btn btn-sm btn-outline-warning">
                                        <i class="fas fa-fw fa-key"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </template>
                </table-list>

            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>

        <modal-manage-user-role ref="modalManageUserRole" @saved="search"></modal-manage-user-role>
    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';

    import ModalManageUserRole from '../../Modals/Users/manage-user-roles.vue';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            ModalManageUserRole
        },
        data() {
            return {
                baseUrl: `/api/systems/default/pharmacies`,
                filter: {
                    cacheKey: `filter-${this.uid}/staffs`,
                    //query: {
                    //    orderStatus: 0,
                    //    dateStart: moment().startOf('week').format('YYYY-MM-DD'),
                    //    dateEnd: moment().endOf('week').format('YYYY-MM-DD')
                    //}
                },
            };
        },

        computed: {

        },

        async created() {
            const vm = this;
            const cache = JSON.parse(localStorage.getItem(vm.filter.cacheKey)) || {};

            vm.initializeFilter(cache);

            await vm.search();

        },

        async mounted() {
            const vm = this;

        },

        methods: {
            openManageUserRole(item) {
                const vm = this;

                vm.$refs.modalManageUserRole.open(item.staffId);
            },
        }
    }
</script>