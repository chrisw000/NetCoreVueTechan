<template>
    <v-card>
        <v-container fluid
                     grid-list-lg>
            <v-layout row wrap>
                <v-flex xs12 v-if="loading">
                    <v-card color="blue-grey darken-2" class="white--text">
                        <v-card-title primary-title>
                            <div class="jumbotron">  
                                <h1><icon icon="spinner" pulse /></h1>
                                <div class="headline">Loading...</div>
                            </div>
                        </v-card-title>
                    </v-card>
                </v-flex>

                <v-flex xs12 v-if="!loading">
                    <v-card color="blue-grey darken-2" class="white--text">
                        <v-card-title primary-title>
                            <div class="jumbotron">
                                <div class="headline">{{title}}</div>
                                <div class="chartContainer">
                                    <tradeChart :chart-data="chartData" :dimension="dimension"></tradeChart>
                                </div>
                            </div>
                        </v-card-title>
                        <v-card-actions>
                            <v-btn flat dark>Buy</v-btn>
                            <v-btn flat dark>Sell</v-btn>
                            <v-spacer></v-spacer>
                            <v-btn flat dark>Alert</v-btn>
                        </v-card-actions>
                    </v-card>
                </v-flex>

            </v-layout>
        </v-container>
    </v-card>
</template>

<script>
    import TradeChart from './charts/tradeChart'

    export default {
        name: 'chart',
        props: {
            overlayId: String
        },
        components: {
            TradeChart
        },
        data() {
            return {
                loading: true,
                title: '',
                chartData: null,
                dimension: {
                    width: 300,
                    height: 200,
                    margin: {
                        top: 25,
                        right: 50,
                        bottom: 25,
                        left: 50
                    },
                    plot: {
                        width: null,
                        height: null
                    },
                    ohlc: {
                        height: null
                    },
                    indicator: {
                        height: null,
                        padding: null,
                        top: null,
                        bottom: null
                    }
                }
            }
        },
        methods: {
            async loadChart(overlayId) {
                try {
                    this.loading = true;
                    let response = await this.$http.get(`/api/overlay/chartbyoverlay/${overlayId}`)
                    console.log(response.data)
                    this.chartData = response.data
                    this.title = response.data.name
                    this.loading = false;
                } catch (err) {
                    window.alert(err)
                    console.log(err)
                }
            }
        },

        async beforeRouteUpdate(to, from, next) {
            await this.loadChart(to.params.overlayId)
            next()
        },

        async created() {
            this.loadChart(this.overlayId)
        }
    }
</script>