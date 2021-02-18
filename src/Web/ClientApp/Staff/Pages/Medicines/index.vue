<template>
    <my-page :uid="uid"></my-page>
</template>
<script>
    import MyPage from '../../../_Common/Components/Pharmacies/Medicines/index.vue';

    export default {
        //mixins: [paginatedMixin],

        props: {
            uid: String,
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

        },

        async mounted() {

        },

        methods: {

        }
    }
</script>