<template>
    <v-app>
        <navigation :show-navigation="showNavigation"></navigation>
        <v-toolbar app fixed clipped-left class="blue-grey darken-1">
            <v-toolbar-side-icon @click.stop="showNavigation = !showNavigation"></v-toolbar-side-icon>
            <v-toolbar-title class="text-white">.Net Core, Vue, TechanJs</v-toolbar-title>
            <v-spacer></v-spacer>
            <a href="https://github.com/chrisw000/NetCoreVueTechan">
                <v-icon class="btn-white" style="color:white">fa-github</v-icon>
            </a>
        </v-toolbar>
        <main>
            <v-content>
                <v-container>
                    <v-layout align-start justify-space-around fill-height>
                        <v-flex xs12>
                            <router-view></router-view>
                        </v-flex>
                    </v-layout>
                </v-container>
            </v-content>
        </main>
        <bottom></bottom>
    </v-app>
</template>

<script>
    import Navigation from './layout/nav-menu'
    import Bottom from './layout/footer-bar'
    export default {
        name: 'app',
        data: () => ({
            showNavigation: null
        }),
        components: {
            Navigation,
            Bottom
        }
    }
</script>

<style>
    #app {
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
    }
</style>