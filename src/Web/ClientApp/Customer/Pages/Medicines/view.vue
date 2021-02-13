<style scoped>
    label {
        font-size: small;
        font-weight: bold;
    }
</style>
<template>
    <div v-cloak>
        <div class="row align-items-center">
            <div class="col">
                <h1 class="h3 mb-sm-0">
                    <i class="fas fa-fw fa-prescription-bottle-alt mr-1"></i>View
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


            <div class="form-group">
                <label for="name">Name</label>
                <div class="form-control-plaintext">
                    {{item.name}}
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md">
                    <label for="price">Price</label>
                    <div class="form-control-plaintext">
                        {{item.price.price|toCurrency}}
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="brandId">Brand</label>
                    <div class="form-control-plaintext">
                        {{item.brand}}
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="classification">Classification</label>
                    <div class="form-control-plaintext">
                        {{item.classificationText}}
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group col-md">
                    <label for="stock">Stock</label>
                    <div class="form-control-plaintext">
                        {{item.stock}}
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="safetyStock">Safety Stock</label>
                    <div class="form-control-plaintext">
                        {{item.safetyStock}}
                    </div>
                </div>
                <div class="form-group col-md">
                    <label for="reorderLevel">Reorder Level</label>
                    <div class="form-control-plaintext">
                        {{item.reorderLevel}}
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
            id: String,
        },

        data() {
            return {
                item: {}
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

                await vm.$util.axios.get(`/api/drugs/${vm.id}`)
                    .then(resp => vm.item = resp.data);
            }
        }
    }
</script>