<template>
        <v-navigation-drawer v-model="show"
                             fixed
                             clipped
                             class="grey lighten-4"
                             app>
            <v-list dense
                    class="grey lighten-4">
                <template v-for="(item, i) in items">
                    <v-layout v-if="item.heading"
                              :key="i"
                              row
                              align-center>
                        <v-flex xs6>
                            <v-subheader v-if="item.heading">
                                {{ item.heading }}
                            </v-subheader>
                        </v-flex>
                    </v-layout>
                    <v-divider v-else-if="item.divider"
                               :key="i"
                               dark
                               class="my-3"></v-divider>
                    <v-list-tile v-else
                                 :key="i"
                                 :to="item.path">
                        <v-list-tile-action>
                            <v-icon>{{ item.icon }}</v-icon>
                        </v-list-tile-action>
                        <v-list-tile-content>
                            <v-list-tile-title class="grey--text">
                                {{ item.text }}
                            </v-list-tile-title>
                        </v-list-tile-content>
                    </v-list-tile>
                </template>
            </v-list>
        </v-navigation-drawer>
</template>

<script>
    export default {
        data: () => ({
            items: [
                { icon: 'home', text: 'Home', path: '/' },
                { divider: true },
                { heading: 'Markets' },
                { icon: 'fa-bitcoin', text: 'BTC-USD', path: '/chart/52' },
                { icon: 'fa-bitcoin', text: 'BTC-EUR', path: '/chart/32'  },
                { icon: 'fa-bitcoin', text: 'BTC-GBP', path: '/chart/76'  },
                { divider: true },
                ],
            show: null
        }),
        watch: {
            showNavigation: function () {
                this.show = this.showNavigation;
            }
        },
        props: {
            showNavigation: {
                type: Boolean,
                default: null
            }
        }
    }
</script>