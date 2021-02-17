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
                    { to: '/medicines', label: 'Medicines', icon: 'fas fa-fw fa-prescription-bottle-alt' },
                    { to: '/orders', label: 'Orders', icon: 'fas fa-fw fa-cubes' },
                    { to: '/users', label: 'Staffs', icon: 'fas fa-fw fa-users' },
                    //{ to: '/contacts', label: 'Contacts', icon: 'fas fa-fw fa-id-card' },

                ]
            }
        },
        async mounted() {
            const vm = this;

            vm.setupEventReceivers();
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
            setupEventReceivers() {
                const vm = this;


                //  customers
                vm.$bus.$on('event:customer-place-order', async function (resp) {
                    let content = resp.content;
                    content = content.replace(/<b>/g, '');
                    content = content.replace(/<\/b>/g, '');
                    content = content.replace(/<br\/>/g, '');

                    vm.$bvToast.toast(content, {
                        title: resp.title,
                        variant: 'success',
                        solid: true
                    });
                });
                vm.$bus.$on('event:customer-cancelledOrder', async function (resp) {
                    let content = resp.content;
                    content = content.replace(/<b>/g, '');
                    content = content.replace(/<\/b>/g, '');
                    content = content.replace(/<br\/>/g, '');

                    vm.$bvToast.toast(content, {
                        title: resp.title,
                        variant: 'danger',
                        solid: true
                    });
                });
                vm.$bus.$on('event:customer-set-order-to-archived', async function (resp) {
                    let content = resp.content;
                    content = content.replace(/<b>/g, '');
                    content = content.replace(/<\/b>/g, '');
                    content = content.replace(/<br\/>/g, '');

                    vm.$bvToast.toast(content, {
                        title: resp.title,
                        variant: 'info',
                        solid: true
                    });
                });

                //  pharmacy
            },

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
