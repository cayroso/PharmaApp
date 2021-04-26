<template>
    <my-page :uid="uid"></my-page>
</template>
<script>
    import MyPage from '../../_Common/Components/Pharmacies/Dashboard/index.vue';

    export default {
        
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
            const vm = this;

        },

        async mounted() {
            const vm = this;

        },

        methods: {
            
        }
    }
</script>