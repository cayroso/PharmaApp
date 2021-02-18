
<template>
    <my-page :uid="uid"></my-page>
</template>
<script>
    import MyPage from '../../../_Common/Components/Pharmacies/Pharmacy/index.vue';

    export default {
        components: {
            MyPage
        },
        props: {
            uid: String,
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