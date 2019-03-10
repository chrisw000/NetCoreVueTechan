<template>
    <svg v-resize="onResize"></svg>
</template>

<script>
    import Vue from 'vue'

    export default Vue.extend({
        props: ['chartData', 'dimension'],
        data() {
            return {
                _chartData: null,
                isRendering: false,
                valueText: null
            }
        },
        methods: {
            renderChart() {
                throw new Error('Please implement renderChart method in subclass of BaseChart.')
            },
            onResize() {
                throw new Error('Please implement onResize method in subclass of BaseChart.')
            },
            computeDimensions(selection) {
                let dimensions = null
                let node = null

                if (typeof selection.node === "function") {
                    node = selection.node()
                } else {
                    node = selection
                }

                if (node instanceof SVGElement) { // check if node is svg element
                    dimensions = node.getBBox()
                } else { // else is html element
                    dimensions = node.getBoundingClientRect()
                }
                return dimensions
            },
        },
        mounted() {
            this._chartData = this.chartData
            this.renderChart()
        },
        watch: {
            'chartData': function (newVal, oldVal) {
                this._chartData = this.chartData
                this.renderChart()
            }
        }
    })
</script>
<style>
    h1,
    h2 {
        font-weight: normal;
    }

    ul {
        list-style-type: none;
        padding: 0;
    }

    li {
        display: inline-block;
        margin: 0 10px;
    }

    a {
        color: #42b983;
    }

    .jumbotron {
        background-color: #333;
        height: 100%;
        width:100%;
        padding: 20px 0;
    }

    .jumbotron .chartContainer {
        height: 100%;
        width: 100%;
        padding-left: 0px;
        padding-right: 0px;
    }

    div.chartContainer svg {
        font: 10px sans-serif;
        width: 100%;
        max-width: 800px;
    }

    div.chartContainer text.trade {
        fill: white;
        font: 10px sans-serif;
        text-anchor: end;
    }

    div.chartContainer .axis path {
        fill: none;
        stroke: #BBBBBB;
        shape-rendering: crispEdges;
    }

    div.chartContainer .axis line {
        fill: none;
        stroke: #BBBBBB;
        shape-rendering: crispEdges;
    }

    div.chartContainer text {
        fill: #DDDDDD;
    }

    div.chartContainer path {
        fill: none;
        stroke-width: 1;
    }

    div.chartContainer path.line {
        fill: none;
        stroke: #ffffff;
        stroke-width: 1;
    }

    div.chartContainer path.candle {
        stroke: #000000;
        fill: #777777;
        stroke: #777777;
    }

    div.chartContainer path.candle.body {
        stroke-width: 0;
    }

    div.chartContainer path.candle.up {
        fill: #00AA00;
        stroke: #00AA00;
    }

    div.chartContainer path.candle.down {
        fill: #FF0000;
        stroke: #FF0000;
    }

    div.chartContainer .closeValue.annotation.up path {
        fill: #00AA00;
    }

    div.chartContainer path.volume {
        fill: #555555;
    }

    div.chartContainer .indicator-plot path.line {
        fill: none;
        stroke: #BBBBBB;
        stroke-width: 1;
    }

    div.chartContainer .ma-0 path.line {
        stroke: #1f77b4;
    }

    div.chartContainer .ma-1 path.line {
        stroke: #aec7e8;
    }

    div.chartContainer .ma-2 path.line {
        stroke: #ff7f0e;
    }

    div.chartContainer path.macd {
        stroke: #aec7e8;
    }

    div.chartContainer path.signal {
        stroke: #FF9999;
    }

    div.chartContainer path.zero {
        stroke: #BBBBBB;
        stroke-dasharray: 0;
        stroke-opacity: 0.5;
        stroke: #BBBBBB;
        stroke-dasharray: 5, 5;
    }

    div.chartContainer path.difference {
        fill: #777777;
    }

    div.chartContainer path.rsi {
        stroke: #aec7e8;
    }

    div.chartContainer path.overbought {
        stroke: #FF9999;
        stroke-dasharray: 5, 5;
    }

    div.chartContainer path.oversold {
        stroke: #FF9999;
        stroke-dasharray: 5, 5;
    }

    div.chartContainer path.middle {
        stroke: #BBBBBB;
        stroke-dasharray: 5, 5;
    }

    div.chartContainer .analysis path {
        stroke: yellow;
        stroke-width: 0.7;
    }

    div.chartContainer .analysis circle {
        stroke: yellow;
        stroke-width: 0.7;
    }

    div.chartContainer .interaction path {
        pointer-events: all;
    }

    div.chartContainer .interaction circle {
        pointer-events: all;
    }

    div.chartContainer .interaction .body {
        cursor: move;
    }

    div.chartContainer .trendlines .interaction .start {
        cursor: nwse-resize;
    }

    div.chartContainer .trendlines .interaction .end {
        cursor: nwse-resize;
    }

    div.chartContainer .trendline circle {
        stroke-width: 0;
        display: none;
    }

    div.chartContainer .mouseover .trendline path {
        stroke-width: 1;
        stroke: orange;
    }

    div.chartContainer .mouseover .trendline circle {
        stroke-width: 1;
        fill: blue;
        display: inline;
    }

    div.chartContainer .mouseover .supstance path {
        stroke: yellow;
        stroke-width: 0.7;
    }

    div.chartContainer .supstance path {
        stroke-dasharray: 2, 2;
        stroke: cyan;
    }

    div.chartContainer .supstances .interaction path {
        pointer-events: all;
        cursor: ns-resize;
    }

    div.chartContainer .supstances .axisannotation {
        display: none;
    }

    div.chartContainer .supstances .axisannotation path {
        fill: #806517;
        stroke: none;
    }

    div.chartContainer .supstances .mouseover .axisannotation {
        display: inline;
    }

    div.chartContainer .crosshair {
        cursor: crosshair;
    }

    div.chartContainer .crosshair path.wire {
        stroke: #606060;
        stroke-dasharray: 1, 1;
    }

    div.chartContainer .crosshair .axisannotation path {
        fill: #777777;
    }

    div.chartContainer path.tradearrow {
        stroke: none;
    }

    div.chartContainer path.tradearrow.buy {
        stroke-width: 1;
        stroke: #FFFFFF;
        fill: #00AA00;
    }

    div.chartContainer path.tradearrow.buy-pending {
        fill-opacity: 0.2;
        stroke: #0000FF;
        stroke-width: 1.5;
    }

    div.chartContainer path.tradearrow.sell {
        stroke-width: 1;
        stroke: #FFFFFF;
        fill: #FF0000;
    }

    div.chartContainer .tradearrow path.highlight {
        fill: none;
        stroke-width: 2;
    }

    div.chartContainer .tradearrow path.highlight.buy {
        stroke: #0000FF;
    }

    div.chartContainer .tradearrow path.highlight.buy-pending {
        stroke: #0000FF;
        fill: #0000FF;
        fill-opacity: 0.3;
    }

    div.chartContainer .tradearrow path.highlight.sell {
        stroke: #9900FF;
    }

    div.chartContainer .overlay path {
        fill: none;
        stroke-width: 1;
        stroke: #40e0d0;
    }

</style>
