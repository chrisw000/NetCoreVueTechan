<template>
    <v-card>
        <v-container fluid
                     grid-list-lg>
            <v-layout row wrap>

                <v-flex xs12>
                    <v-card color="blue-grey darken-2" class="white--text">

                        <v-card-title primary-title>
                            <router-view />                                                        
                        </v-card-title>

                    </v-card>
                </v-flex>

            </v-layout>
        </v-container>
    </v-card>
</template>
