<template>
    <my-page :uid="uid" role-id="systems"></my-page>
</template>
<script>
    import MyPage from '../../../_Common/Components/Pharmacies/Orders/index.vue';

    export default {
        //mixins: [paginatedMixin],

        props: {
            uid: String
        },
        components: {
            MyPage
        },
        data() {
            return {
                
            };
        },

        computed: {

        },

        async created() {
            const vm = this;            
        },

        async mounted() {
            const vm = this;

        },

        methods: {
            
        }
    }
</script>