<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col-sm">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-shopping-cart mr-1"></i>Orders
                </h1>
            </div>
            <div class="col-sm-auto">
                <div class="d-flex flex-row">
                    <div class="mr-1">
                        <router-link to="/medicines/add" class="btn btn-primary">
                            <i class="fas fa-plus"></i>
                        </router-link>
                    </div>

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
                <table-list :header="{key: 'brandId', columns:[]}" :items="filter.items" :getRowNumber="getRowNumber" :setSelected="setSelected" :isSelected="isSelected" table-css="">
                    <template #header>
                        <th class="text-center">#</th>
                        <th>Name</th>
                        <th>Classification</th>
                        <th>Available</th>
                        <th>Price</th>
                    </template>
                    <template slot="table" slot-scope="row">
                        <td v-text="getRowNumber(row.index)" class="text-center"></td>
                        <td>
                            <router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                {{row.item.name}}
                            </router-link>                            
                            <div>
                                <small>{{row.item.brand}}</small>
                            </div>
                        </td>
                        <td>
                            {{row.item.classificationText}}
                        </td>
                        <td>
                            {{row.item.isAvailable}}
                        </td>
                        <td>
                            {{row.item.price.price|toCurrency}}
                        </td>
                    </template>

                    <template slot="list" slot-scope="row">
                        <div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Name</label>
                                <div class="col align-self-center">
                                    <router-link :to="{name:'medicinesView', params:{id:row.item.drugId}}">
                                        {{row.item.name}}
                                    </router-link>                                    
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Brand</label>
                                <div class="col align-self-center">
                                    {{row.item.brand}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Classification</label>
                                <div class="col align-self-center">
                                    {{row.item.classificationText}}
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Available</label>
                                <div class="col align-self-center">
                                    <span v-if="row.item.isAvailable" class="fas fa-fw fa-check"></span>
                                    <span v-else class="fas fa-fw fa-times"></span>
                                </div>
                            </div>
                            <div class="form-group mb-0 row no-gutters">
                                <label class="col-3 col-form-label">Price</label>
                                <div class="col align-self-center">
                                    {{row.item.price.price|toCurrency}}
                                </div>
                            </div>

                        </div>
                    </template>
                </table-list>





            </div>
        </b-overlay>


        <m-pagination :filter="filter" :search="search" :showPerPage="true" class="mt-2"></m-pagination>

        <modal-add-member ref="modalAddMember" @saved="search"></modal-add-member>
    </div>
</template>
<script>
    import paginatedMixin from '../../../_Core/Mixins/paginatedMixin';
    import modalAddMember from '../../Modals/Teams/add-member.vue';

    export default {
        mixins: [paginatedMixin],

        props: {
            uid: String,
            urlAdd: String,
            urlView: String,
        },
        components: {
            //modalAddTask
            modalAddMember
        },
        data() {
            return {
                baseUrl: `/api/drugs`,
                filter: {
                    cacheKey: `filter-${this.uid}/drugs`,
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
            addTeamMember(item) {
                const vm = this;

                //const payload = {
                //    contactId: item.contactId,
                //    firstName: item.firstName,
                //    middleName: item.middleName,
                //    lastName: item.lastName,

                //    statusText: item.statusText,
                //    rating: item.rating,
                //};

                vm.$refs.modalAddMember.open(item.teamId, item.name);
            },

            async removeTeamMember(team, user) {
                const vm = this;

                try {
                    this.$bvModal.msgBoxConfirm(`Are you sure you want to remove "${user.name}" from "${team.name}"?`)
                        .then(async value => {
                            if (value) {

                                await vm.$util.axios.delete(`/api/teams/${team.teamId}/remove-member/${user.userId}/`)
                                    .then(resp => {
                                        vm.$bvToast.toast('User removed from group.', { title: 'Remove User from Group', variant: 'success', toaster: 'b-toaster-bottom-right' });
                                    })

                                vm.search();
                            }
                        })
                        .catch(err => {
                            vm.$util.handleError(err);
                        });
                } catch (e) {
                    vm.$util.handleError(e)
                }
            },

            async removeTeam(team) {
                const vm = this;

                try {
                    this.$bvModal.msgBoxConfirm(`Are you sure you want to delete "${team.name}"?`)
                        .then(async value => {
                            if (value) {

                                await vm.$util.axios.delete(`/api/teams/${team.teamId}`)
                                    .then(resp => {
                                        vm.$bvToast.toast('Team deleted.', { title: 'Delete Team', variant: 'warning', toaster: 'b-toaster-bottom-right' });
                                    })

                                vm.search();
                            }
                        })
                        .catch(err => {
                            vm.$util.handleError(err);
                        });
                } catch (e) {
                    vm.$util.handleError(e);
                }
            },
        }
    }
</script>