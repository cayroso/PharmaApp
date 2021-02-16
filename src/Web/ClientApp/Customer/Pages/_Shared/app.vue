<template>
    <div v-cloak>
        <app-bar :uid="uid" :appName="appName" :urlProfilePicture="urlProfilePicture" :menus="menus"></app-bar>
        <main class="container-lg main mb-5 mb-md-0 pb-5 pb-sm-0">
            <router-view :uid="uid"></router-view>
        </main>
        <bottom-nav :menus="menus"></bottom-nav>

        <modal-view-chat ref="modalViewChat" :uid="uid"></modal-view-chat>
    </div>
</template>
<script>
    'use strict';
    import appMixin from '../../../_Core/Mixins/appMixin';

    import modalViewChat from '../../../_Core/Modals/Chats/view.vue';

    //import SystemBar from './system-bar.vue';
    import AppBar from './app-bar.vue';
    import NavDrawer from './nav-drawer.vue';
    //import AppFooter from './footer.vue';
    import BottomNav from './bottom-nav.vue';

    export default {
        mixins: [appMixin],
        components: {

            modalViewChat,

            //SystemBar,
            AppBar, NavDrawer,
            //AppFooter,
            BottomNav,

        },
        props: {
            uid: String,
            appName: {
                type: String,
                //required: true,
                default: 'Pharma App'
            },
            urlProfilePicture: String,
        },
        data() {
            return {
                menus: [
                    { to: '/', label: 'Home', icon: 'fas fa-fw fa-home' },
                    { to: '/pharmacies', label: 'Pharmacies', icon: 'fas fa-fw fa-clinic-medical' },
                    { to: '/medicines', label: 'Medicines', icon: 'fas fa-fw fa-tasks' },
                    { to: '/orders', label: 'Reservations', icon: 'fas fa-fw fa-cubes' },
                    //{ to: '/trips', label: 'Trips', icon: 'fas fa-fw fa-map-marked' },
                ]
            }
        },
        async mounted() {
            const vm = this;

            vm.$bus.$on('event:add-to-cart', async function (info) {

                let shoppingCart = JSON.parse(localStorage.getItem('shopping-cart')) || [];

                var shop = shoppingCart.find(e => e.pharmacyId == info.pharmacyId);
                var drug = {
                    drugId: info.drugId,
                    drugName: info.drugName,
                    drugBrand: info.drugBrand,
                    drugClassification: info.drugClassification,
                    drugClassificationText: info.drugClassificationText,
                    drugPrice: info.drugPrice,
                    drugQuantity: null
                };
                
                if (shop) {
                    var item = shop.items.find(e => e.drugId === info.drugId)

                    if (!item) {
                        shop.items.push(drug);
                    }
                }
                else {
                    var newItem = {
                        pharmacyId: info.pharmacyId,
                        pharmacyName: info.pharmacyName,
                        items: [drug]
                    }

                    shoppingCart.push(newItem);
                }



                localStorage.setItem('shopping-cart', JSON.stringify(shoppingCart));

                vm.$bus.$emit('event:shopping-cart-updated');

                //vm.$bvToast.toast(`The system has assigned a driver to your trip request.`, {
                //    title: `Driver Assigned`,
                //    variant: 'info',
                //    solid: true
                //});
            });

            vm.$bus.$on('event:driver-accepted', async function (resp) {
                vm.$bvToast.toast(`Driver accepted the trip request. Wait for the driver's fare offer.`, {
                    title: `Driver Accepted Trip Request`,
                    variant: 'info',
                    solid: true
                });
            });

            vm.$bus.$on('event:driver-rejected', async function (resp) {
                vm.$bvToast.toast(`The assigned driver rejected the trip request. System will look for another available driver.`, {
                    title: `Driver Rejected Trip Request`,
                    variant: 'info',
                    solid: true
                });
            });

            vm.$bus.$on('event:driver-fare-offered', async function (resp) {
                vm.$bvToast.toast(`The driver offered fare is ${resp.fare}.`, {
                    title: `Driver Offered Fare`,
                    variant: 'info',
                    solid: true
                });
            });

            vm.$bus.$on('event:driver-trip-inprogress', async function (resp) {
                vm.$bvToast.toast(`The driver set the trip request to in-progress.`, {
                    title: `Trip In-Progress`,
                    variant: 'info',
                    solid: true
                });
            });

            vm.$bus.$on('event:driver-trip-completed', async function (resp) {
                vm.$bvToast.toast(`The driver set the trip request to completed.`, {
                    title: `Trip Completed`,
                    variant: 'info',
                    solid: true
                });
            });

            //let theme = localStorage.getItem('theme') || '';
            //if (theme) {
            //    //debugger;
            //    let style = document.createElement('link');
            //    style.type = "text/css";
            //    style.rel = "stylesheet";
            //    style.href = theme;// 'https://bootswatch.com/4/yeti/bootstrap.min.css';
            //    document.head.appendChild(style);
            //}
        },
        async created() {
            //const vm = this;
            //let theme = localStorage.getItem('theme') || '';
            //if (theme) {
            //    //debugger;
            //    let style = document.createElement('link');
            //    style.type = "text/css";
            //    style.rel = "stylesheet";
            //    style.href = theme;// 'https://bootswatch.com/4/yeti/bootstrap.min.css';
            //    document.head.appendChild(style);
            //}
        },
        methods: {
            //async getMembershipInfo() {
            //    const vm = this;
            //    try {
            //        await vm.$util.axios.get(`api/organizations/${vm.organizationId}/membership-info/${vm.uid}`).
            //            then(resp => {
            //                const data = resp.data;
            //                //vm.membership = data;
            //                if (data.status === 2) {
            //                    var isAdmin = data.roles.find(e => e.roleId === 'organizationadministrator') !== undefined;
            //                    data.isAdmin = isAdmin;
            //                    data.isMember = !isAdmin;
            //                    data.isAdminOrMember = data.isAdmin || data.isMember;
            //                }
            //                vm.$bus.$emit('event:membership', data);
            //            });
            //    } catch (e) {
            //    }
            //},
        }
    }
</script>
