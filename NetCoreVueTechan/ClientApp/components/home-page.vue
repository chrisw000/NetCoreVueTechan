<template>
    <chart :overlayId="overlayId"></chart>
</template>

<script>
import Chart from './chart'

export default {
    name: 'home-page',
    data() {
        return {
            overlayId: "55"
        }
    },
    components: {
        "chart": Chart
    }
}
</script>

<style>

</style>
