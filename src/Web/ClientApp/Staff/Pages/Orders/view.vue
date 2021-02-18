
<template>
    <my-page :uid="uid" :id="id"></my-page>
</template>
<script>
    import MyPage from '../../../_Common/Components/Pharmacies/Orders/view.vue';

    export default {

        props: {
            uid: String,
            id: {
                type: String,
                required: true
            },
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