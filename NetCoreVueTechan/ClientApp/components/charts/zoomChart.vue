<script>
    import BaseChart from './baseChart'

    var d3 = window.d3;
    var techan = window.techan;

    export default BaseChart.extend({
        name: 'zoom-chart',
        methods: {
            renderChart() {
                this.isRendering = true
                d3.select(this.$el).select("svg > *").remove();
                this.config = this.getChartConfig(this.dimension, this.$el, this._chartData)
                this.draw()
                this.isRendering = false
            },
            onResize() {
                console.log("onResize" + this.isRendering);
                //TODO
            },
            getChartConfig(dim, el, chartData) {

                let data = chartData.ohlc
                let d = this.computeDimensions(d3.select(el.parentNode))

                dim.width = d.width
                if(dim.width>800) dim.width = 800
                dim.height = dim.width * .56 // 16/9
                if(dim.height<200) dim.height = 200

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

                let brush = d3.brushX()
                    .extent([[0, 0], [width, height2]])
                    .on("end", this.brushed)

                let candlestick = techan.plot.candlestick()
                    .xScale(x)
                    .yScale(y)

                let volume = techan.plot.volume()
                    .xScale(x)
                    .yScale(yVolume)

                let close = techan.plot.close()
                    .xScale(x2)
                    .yScale(y2)

                let xAxis = d3.axisBottom(x)

                let xAxis2 = d3.axisBottom(x2)

                let yAxis = d3.axisLeft(y)

                let yAxis2 = d3.axisLeft(y2)
                    .ticks(0)

                let ohlcAnnotation = techan.plot.axisannotation()
                    .axis(yAxis)
                    .orient('left')
                    .format(d3.format(',.2f'))

                let timeAnnotation = techan.plot.axisannotation()
                    .axis(xAxis)
                    .orient('bottom')
                    .format(d3.timeFormat('%Y-%m-%d %H:%M'))
                    .width(85)
                    .translate([0, height])

                let crosshair = techan.plot.crosshair()
                    .xScale(x)
                    .yScale(y)
                    .xAnnotation(timeAnnotation)
                    .yAnnotation(ohlcAnnotation)

                let svg = d3.select(el)
                    .attr("width", width + margin.left + margin.right)
                    .attr("height", height + margin.top + margin.bottom)

                let focus = svg.append("g")
                    .attr("class", "focus")
                    .attr("transform", `translate(${margin.left},${margin.top})`)

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
                    .attr("class", "x axis")
                    .attr("transform", `translate(0,${height})`)

                focus.append("g")
                    .attr("class", "y axis")
                    .append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("y", 6)
                    .attr("dy", ".71em")
                    .style("text-anchor", "end")
                    .text(`Price (${chartData.symbol})`)

                focus.append('g')
                    .attr("class", "crosshair")
                    .call(crosshair)

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

                focus.select("g.candlestick").datum(data)
                focus.select("g.volume").datum(data)

                context.select("g.close").datum(data).call(close)
                context.select("g.x.axis").call(xAxis2)

                // Associate the brush with the scale and render the brush only AFTER a domain has been applied
                context.select("g.pane").call(brush).selectAll("rect").attr("height", height2)

                x.zoomable().domain(x2.zoomable().domain())

                return {
                    x: x,
                    x2: x2,
                    y: y,
                    xAxis: xAxis,
                    yAxis: yAxis,
                    svg: svg,
                    focus: focus,
                    candlestick: candlestick,
                    volume: volume,
                    data: data
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
                let candlestickSelection = this.config.svg.select("g.candlestick");
                this.config.y.domain(techan.scale.plot.ohlc(this.config.data.slice.apply(this.config.data, this.config.x.zoomable().domain()), this.config.candlestick.accessor()).domain());
                candlestickSelection.call(this.config.candlestick);
                this.config.focus.select("g.volume").call(this.config.volume);
                // using refresh method is more efficient as it does not perform any data joins
                // Use this if underlying data is not changing
                this.config.svg.select("g.candlestick").call(this.config.candlestick.refresh);
                this.config.focus.select("g.x.axis").call(this.config.xAxis);
                this.config.focus.select("g.y.axis").call(this.config.yAxis);
            }
        }
    })
</script>
