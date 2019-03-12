<template>
    <div class="jumbotron" v-resize="onResize">
        <div class="subheading text-center">{{title}}</div>
        <div v-if="isLoading" id="spinner" class="text-center">
            <icon class="display-3" icon="spinner" pulse />
        </div>
        <div class="chartContainer" />
    </div>
</template>

<script>
    var d3 = window.d3;
    var techan = window.techan;

    import Vue from 'vue'

    export default Vue.extend({
        name: 'trade-chart',
        props: ['overlayId'],
        data() {
            return {
                isLoading: true,
                isResizing: true,
                valueText: null,
                title: 'Loading...',
                chartData: null,
                config: null,
                dimension: {
                    width: 360,
                    height: 200,
                    margin: {
                        top: 25,
                        right: 50,
                        bottom: 25,
                        left: 50
                    }
                }
            }

        },
        methods: {
            onResize() {
                if (!this.isResizing) {
                    this.isResizing=true
                    this.setupSize(true)
                    this.renderChart()
                    this.isResizing=false
                }
            },
            renderChart() {
                d3.select(this.$el).select(".chartContainer > *").remove();
                let svg = d3.select(this.$el).select(".chartContainer").append("svg")
                this.config = this.getChartConfig(this.dimension, svg, this.chartData)
                this.draw()
                this.isResizing = false
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
            getChartConfig(dim, svg, chartData) {

                let data = chartData.ohlc

                let margin = { top: dim.margin.top, right: dim.margin.right, bottom: 100, left: dim.margin.left },
                    margin2 = { top: dim.height - 80, right: dim.margin.right, bottom: dim.margin.bottom, left: dim.margin.left },
                    width = dim.width - margin.left - margin.right,
                    height = dim.height - margin.top - margin.bottom,
                    height2 = dim.height - margin2.top - margin2.bottom

                let x = techan.scale.financetime()
                    .range([0, width])

                let x2 = techan.scale.financetime()
                    .range([0, width])

                let y = d3.scaleLinear()
                    .range([height, 0])

                let yVolume = d3.scaleLinear()
                    .range([y(0), y(0.3)])

                let y2 = d3.scaleLinear()
                    .range([height2, 0])

                let yOverlay = d3.scaleLinear()
                    .range([height, 0])

                let brush = d3.brushX()
                    .extent([[0, 0], [width, height2]])
                    .on("end", this.brushed)

                let candlestick = techan.plot.candlestick()
                    .xScale(x)
                    .yScale(y)

                let volume = techan.plot.volume()
                    .accessor(candlestick.accessor())
                    .xScale(x)
                    .yScale(yVolume)

                let close = techan.plot.close()
                    .xScale(x2)
                    .yScale(y2)

                let overlay = techan.plot.atr()
                    .xScale(x)
                    .yScale(yOverlay);

                let xAxis = d3.axisBottom(x)

                let xAxis2 = d3.axisBottom(x2)

                var xTopAxis = d3.axisTop(x);

                let yAxis = d3.axisLeft(y)

                let yAxis2 = d3.axisLeft(y2)
                    .ticks(0)

                var overlayAxis = d3.axisRight(yOverlay);

                let overlayAnnotation = techan.plot.axisannotation()
                    .axis(overlayAxis)
                    .orient('right')
                    .accessor(overlay.accessor())
                    .format(d3.format(',.2f'))
                    .translate([x(1), 0])

                let ohlcAnnotation = techan.plot.axisannotation()
                    .axis(yAxis)
                    .orient('left')
                    .format(d3.format(',.2f'))
                
                let openAnnotation = techan.plot.axisannotation()
                    .axis(yAxis)
                    .orient('left')
                    .accessor(candlestick.accessor())
                    .format(d3.format(',.2f'))

                let closeAnnotation = techan.plot.axisannotation()
                    .axis(yAxis)
                    .orient('right')
                    .accessor(candlestick.accessor())
                    .format(d3.format(',.2f'))
                    .translate([x(1), 0])

                let volumeAxis = d3.axisRight(yVolume)

                let volumeAnnotation = techan.plot.axisannotation()
                    .axis(volumeAxis)
                    .orient("right")
                    .width(35);

                var ohlcVerticalTicks = Math.min(10, Math.round(dim.height / 50)),
                    xTicks = Math.min(10, Math.round(dim.width / 50)),
                    vTicks = Math.min(3, Math.round((height * 0.3) / 50))

                yAxis.ticks(ohlcVerticalTicks)
                volumeAxis.ticks(vTicks)
                overlayAxis.ticks(ohlcVerticalTicks)
                xTopAxis.ticks(xTicks)
                xAxis.ticks(xTicks)
                xAxis2.ticks(xTicks)

                let timeAnnotation = techan.plot.axisannotation()
                    .axis(xAxis)
                    .orient('bottom')
                    .format(d3.timeFormat('%H:%M %Y-%m-%d'))
                    .width(85)
                    .translate([0, height])

                var timeTopAnnotation = techan.plot.axisannotation()
                    .axis(xTopAxis)
                    .orient('top')
                    .format(d3.timeFormat('%H:%M %Y-%m-%d'))
                    .width(85)

                let crosshair = techan.plot.crosshair()
                    .xScale(timeAnnotation.axis().scale())
                    .yScale(ohlcAnnotation.axis().scale())
                    .xAnnotation([timeAnnotation, timeTopAnnotation])
                    .yAnnotation([ohlcAnnotation, overlayAnnotation, volumeAnnotation]) 
                    .verticalWireRange([0, height])

                let trendline = techan.plot.trendline()
                    .xScale(x)
                    .yScale(y)

                let supstance = techan.plot.supstance()
                    .xScale(x).
                    yScale(y)
                    .annotation(ohlcAnnotation)

                let tradearrow = techan.plot.tradearrow().xScale(x).yScale(y).orient(function (p) {
                    return p.type.startsWith("buy") ? "up" : "down"
                }).on("mouseenter", this.enter).on("mouseout", this.out)
                               
                svg
                    .attr("width", width + margin.left + margin.right)
                    .attr("height", height + margin.top + margin.bottom)
                                
                let focus = svg.append("g")
                    .attr("class", "focus")
                    .attr("transform", `translate(${margin.left},${margin.top})`)

                this.valueText = svg.append('text').attr("class", "trade").attr("x", width+margin.right-10).attr("y", 45);
                
                let seriesOpen = [data[0]]
                let seriesClose = [data[data.length - 1]]

                focus.append("clipPath")
                    .attr("id", "clip")
                    .append("rect")
                    .attr("x", 0)
                    .attr("y", y(1))
                    .attr("width", width)
                    .attr("height", y(0) - y(1))

                focus.append("g")
                    .attr("class", "volume")
                    .attr("clip-path", "url(#clip)")

                focus.append("g")
                    .attr("class", "candlestick")
                    .attr("clip-path", "url(#clip)")

                focus.append("g")
                    .attr("class", "x axis top")

                focus.append("g")
                    .attr("class", "x axis bottom")
                    .attr("transform", `translate(0,${height})`)

                focus.append("g")
                    .attr("class", "y axis")

                focus.append("g")
                    .append("text")
                    .attr("x", -6)
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", ".71em")
                    .style("text-anchor", "end")
                    .text(`Price (${chartData.symbol})`)

                focus.append("g")
                    .attr("class", "y axis right")
                    .attr("transform", `translate(${width},0)`)

                focus.append("g")
                    .attr("class", "volume axis")

                if (seriesOpen[0].close > seriesClose[0].close) {
                    focus.append("g").attr("class", "seriesOpen annotation up")
                    focus.append("g").attr("class", "seriesClose annotation down")
                } else {
                    focus.append("g").attr("class", "seriesOpen annotation down")
                    focus.append("g").attr("class", "seriesClose annotation up")
                }

                focus.append('g')
                    .attr("class", "crosshair")
                    .call(crosshair)

                focus.append("g")
                    .attr("class", "trendlines analysis")
                    .attr("clip-path", "url(#clip)")

                focus.append("g")
                    .attr("class", "overlay analysis")
                    .attr("clip-path", "url(#clip)")

                focus.append("g")
                    .attr("class", "supstances analysis")
                    .attr("clip-path", "url(#clip)")

                focus.append("g")
                    .attr("class", "tradearrow")
                    .attr("clip-path", "url(#clip)")

                let context = svg.append("g")
                    .attr("class", "context")
                    .attr("transform", `translate(${margin2.left},${margin2.top})`)

                context.append("g")
                    .attr("class", "close")
                
                context.append("g")
                    .attr("class", "pane")

                context.append("g")
                    .attr("class", "x axis")
                    .attr("transform", `translate(0,${height2})`)

                context.append("g")
                    .attr("class", "y axis")
                    .call(yAxis2)

                // ---------------------------------------
                var accessor = candlestick.accessor()

                x.domain(data.map(accessor.d))
                x2.domain(x.domain())
                y.domain(techan.scale.plot.ohlc(data, accessor).domain())
                y2.domain(y.domain())
                yVolume.domain(techan.scale.plot.volume(data).domain())

                yOverlay.domain(techan.scale.plot.atr(chartData.overlay).domain())

                focus.select("g.x.axis.bottom").call(xAxis)
                focus.select("g.x.axis.top").call(xTopAxis)
                focus.select("g.y.axis").call(yAxis)
                focus.select("g.y.axis.right").call(overlayAxis)
                
                focus.select("g.volume.axis").call(volumeAxis)

                focus.select("g.seriesOpen.annotation").datum(seriesOpen).call(openAnnotation)
                focus.select("g.seriesClose.annotation").datum(seriesClose).call(closeAnnotation)

                focus.select("g.volume").datum(data)
                focus.select("g.candlestick").datum(data)

                focus.select("g.overlay").datum(chartData.overlay).call(overlay)                
                focus.select("g.trendlines").datum(chartData.trendlines).call(trendline).call(trendline.drag)
                focus.select("g.supstances").datum(chartData.supstances).call(supstance).call(supstance.drag)

                focus.select("g.tradearrow").datum(chartData.trades).call(tradearrow)

                context.select("g.close").datum(data).call(close)
                context.select("g.x.axis").call(xAxis2)

                context.select("g.pane").call(brush).selectAll("rect").attr("height", height2)

                x.zoomable().domain(x2.zoomable().domain())

                return {
                    x: x,
                    x2: x2,
                    y: y,
                    yOverlay: yOverlay,
                    xAxis: xAxis,
                    xTopAxis: xTopAxis,
                    yAxis: yAxis,
                    yVolume: yVolume,
                    overlayAxis: overlayAxis,
                    volumeAxis: volumeAxis,
                    svg: svg,
                    focus: focus,
                    candlestick: candlestick,
                    volume: volume,
                    data: data,
                    dataOverlay: chartData.overlay,
                    trendline: trendline,
                    tradearrow: tradearrow,
                    supstance: supstance,
                    overlay: overlay,
                    openAnnotation: openAnnotation,
                    closeAnnotation: closeAnnotation
                }
            },

            brushed() {
                let zoomable = this.config.x.zoomable(),
                    zoomable2 = this.config.x2.zoomable();

                zoomable.domain(zoomable2.domain());
                if (d3.event.selection !== null) zoomable.domain(d3.event.selection.map(zoomable.invert));
                this.draw();
            },

            draw() {
                this.config.y.domain(techan.scale.plot.ohlc(this.config.data.slice.apply(this.config.data, this.config.x.zoomable().domain()), this.config.candlestick.accessor()).domain())
                this.config.yVolume.domain(techan.scale.plot.volume(this.config.data.slice.apply(this.config.data, this.config.x.zoomable().domain()), this.config.volume.accessor().v).domain())
                this.config.yOverlay.domain(techan.scale.plot.atr(this.config.dataOverlay.slice.apply(this.config.dataOverlay, this.config.x.zoomable().domain()), this.config.overlay.accessor()).domain())
                
                this.config.focus.select("g.x.axis.top").call(this.config.xTopAxis)
                this.config.focus.select("g.x.axis.bottom").call(this.config.xAxis)
                this.config.focus.select("g.y.axis").call(this.config.yAxis)
                this.config.focus.select("g.volume.axis").call(this.config.volumeAxis)
                this.config.focus.select("g.y.axis.right").call(this.config.overlayAxis)

                this.config.focus.select("g.seriesOpen.annotation").call(this.config.openAnnotation)
                this.config.focus.select("g.seriesClose.annotation").call(this.config.closeAnnotation)

                // using refresh method is more efficient as it does not perform any data joins
                // Use this if underlying data is not changing
                this.config.focus.select("g.volume").call(this.config.volume)
                this.config.focus.select("g.candlestick").call(this.config.candlestick)                

                this.config.focus.select("g.overlay").call(this.config.overlay.refresh)
                this.config.focus.select("g.trendlines").call(this.config.trendline.refresh)
                this.config.focus.select("g.supstances").call(this.config.supstance.refresh)
                
                this.config.focus.select("g.tradearrow").call(this.config.tradearrow.refresh)
            },
            enter(d) {
                this.valueText.style("display", "inline")
                this.refreshText(d)
            },
            out() {
                this.valueText.style("display", "none")
            },
            refreshText(d) {
                let longDateFormat = d3.timeFormat("%H:%M %Y-%m-%d")
                let valueFormat = d3.format(',.2f')

                let type = d.type,
                    quantity = d.quantity,
                    price = d.price,
                    date = d.date

                this.valueText.text("".concat(type, " ").concat(quantity, " @ ").concat(valueFormat(price), " on ")
                    .concat(longDateFormat(date)))
            },
            setupSize(resize) {
                if (resize) {
                    let d = this.computeDimensions(d3.select(this.$el))

                    if (d.width > 800) {
                        this.dimension.width = 800
                    } else if (d.width < 360) {
                        this.dimension.width = 360
                    } else {
                        this.dimension.width = d.width
                    }

                    this.dimension.height = this.dimension.width * .56 // 16/9
                    if (this.dimension.height < 200) this.dimension.width = 200
                }
                d3.select("#spinner")
                    .style("width", `${this.dimension.width}px`)
                    .style("height", `${this.dimension.height}px`)
            },
            async loadChart(overlayId) {
                try {               
                    this.title = "Loading..."
                    d3.select(this.$el).select(".chartContainer > *").remove();
                    this.isLoading = true
                    this.setupSize(false)
                   
                    let path = `/api/overlay/chartbyoverlay/${overlayId}`

                    let response = await this.$http.get(path)
                    console.log(response.data)
                    this.chartData = response.data
                    this.title = response.data.name                    
                    this.isLoading = false
                    this.renderChart()
                } catch (err) {
                    window.alert(err)
                    console.log(err)
                }
            },
        },
        mounted() {
            this.setupSize(true);
        },
        async beforeRouteUpdate(to, from, next) {
            this.setupSize(true);
            await this.loadChart(to.params.overlayId)
            next()
        },
        async created() {                     
            this.loadChart(this.overlayId)
        }

    })
</script>

<style>
    .jumbotron {
        background-color: #333;
        height: 100%;
        width: 100%;
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
        text-anchor:end;
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

    div.chartContainer .annotation.up path {
        fill: #00AA00;     
    }

    div.chartContainer .annotation.down path {
        fill: #FF0000;
    }

    div.chartContainer .seriesClose.annotation {
        opacity: 1;
    }

    div.chartContainer .seriesClose.annotation text {
        font: 10px sans-serif;
    }

    div.chartContainer .seriesOpen.annotation text {
        font: 10px sans-serif;
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
        stroke: yellow;
    }

    div.chartContainer .mouseover .trendline circle {
        stroke-width: 1;
        fill: yellow;
        display: inline;
    }

    div.chartContainer .mouseover .supstance path {
        stroke: yellow;
        stroke-width: 2;
    }

    div.chartContainer .supstance path {
        stroke-dasharray: 2, 2;
        stroke-width: 1;
    }

    div.chartContainer .supstances .interaction path {
        pointer-events: all;
        cursor: ns-resize;
    }

    div.chartContainer .supstances .axisannotation {
        display: none;
    }

    div.chartContainer .supstances .axisannotation path {
        fill: #1f77b4;
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

    div.chartContainer path.tradearrow.sell {
        stroke-width: 1;
        stroke: #FFFFFF;
        fill: #FF0000;
    }

    div.chartContainer .tradearrow path.highlight {
        fill: none;
        stroke-width: 2;
        stroke: yellow;
    }

    div.chartContainer .overlay path {
        fill: none;
        stroke-width: 1;
        stroke: #aec7e8;
    }

</style>
