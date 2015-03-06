﻿/*
 Highcharts JS v4.1.3 (2015-02-27)

 (c) 2009-2013 Torstein HÃ¸nsi

 License: www.highcharts.com/license
*/
(function (c) {
    function n(d, a, b) {
        var e, f, g = a.options.chart.options3d, h = !1; b ? (h = a.inverted, b = a.plotWidth / 2, a = a.plotHeight / 2, e = g.depth / 2, f = A(g.depth, 1) * A(g.viewDistance, 0)) : (b = a.plotLeft + a.plotWidth / 2, a = a.plotTop + a.plotHeight / 2, e = g.depth / 2, f = A(g.depth, 1) * A(g.viewDistance, 0)); var j = [], i = b, k = a, v = e, p = f, b = y * (h ? g.beta : -g.beta), g = y * (h ? -g.alpha : g.alpha), o = l(b), s = m(b), t = l(g), u = m(g), w, x, r, n, q, z; c.each(d, function (a) {
            w = (h ? a.y : a.x) - i; x = (h ? a.x : a.y) - k; r = (a.z || 0) - v; n = s * w - o * r; q = -o * t * w - s * t * r + u * x; z = o * u * w + s * u * r + t * x; p > 0 &&
            p < Number.POSITIVE_INFINITY && (n *= p / (z + v + p), q *= p / (z + v + p)); n += i; q += k; z += v; j.push({ x: h ? q : n, y: h ? n : q, z: z })
        }); return j
    } function q(d) { return d !== void 0 && d !== null } function E(d) { var a = 0, b, e; for (b = 0; b < d.length; b++) e = (b + 1) % d.length, a += d[b].x * d[e].y - d[e].x * d[b].y; return a / 2 } function D(d) { var a = 0, b; for (b = 0; b < d.length; b++) a += d[b].z; return d.length ? a / d.length : 0 } function r(d, a, b, e, f, g, c, j) {
        var i = []; return g > f && g - f > o / 2 + 1.0E-4 ? (i = i.concat(r(d, a, b, e, f, f + o / 2, c, j)), i = i.concat(r(d, a, b, e, f + o / 2, g, c, j))) : g < f && f - g > o / 2 + 1.0E-4 ?
        (i = i.concat(r(d, a, b, e, f, f - o / 2, c, j)), i = i.concat(r(d, a, b, e, f - o / 2, g, c, j))) : (i = g - f, ["C", d + b * m(f) - b * B * i * l(f) + c, a + e * l(f) + e * B * i * m(f) + j, d + b * m(g) + b * B * i * l(g) + c, a + e * l(g) - e * B * i * m(g) + j, d + b * m(g) + c, a + e * l(g) + j])
    } function F(d) {
        if (this.chart.is3d()) {
            var a = this.chart.options.plotOptions.column.grouping; a !== void 0 && !a && this.group.zIndex !== void 0 && this.group.attr({ zIndex: this.group.zIndex * 10 }); var b = this.options, e = this.options.states; this.borderWidth = b.borderWidth = b.edgeWidth || 1; c.each(this.data, function (a) {
                if (a.y !==
                null) a = a.pointAttr, this.borderColor = c.pick(b.edgeColor, a[""].fill), a[""].stroke = this.borderColor, a.hover.stroke = c.pick(e.hover.edgeColor, this.borderColor), a.select.stroke = c.pick(e.select.edgeColor, this.borderColor)
            })
        } d.apply(this, [].slice.call(arguments, 1))
    } var o = Math.PI, y = o / 180, l = Math.sin, m = Math.cos, A = c.pick, G = Math.round; c.perspective = n; var B = 4 * (Math.sqrt(2) - 1) / 3 / (o / 2); c.SVGRenderer.prototype.toLinePath = function (d, a) {
        var b = []; c.each(d, function (a) { b.push("L", a.x, a.y) }); d.length && (b[0] = "M", a && b.push("Z"));
        return b
    }; c.SVGRenderer.prototype.cuboid = function (d) {
        var a = this.g(), d = this.cuboidPath(d); a.front = this.path(d[0]).attr({ zIndex: d[3], "stroke-linejoin": "round" }).add(a); a.top = this.path(d[1]).attr({ zIndex: d[4], "stroke-linejoin": "round" }).add(a); a.side = this.path(d[2]).attr({ zIndex: d[5], "stroke-linejoin": "round" }).add(a); a.fillSetter = function (a) {
            var d = c.Color(a).brighten(0.1).get(), f = c.Color(a).brighten(-0.1).get(); this.front.attr({ fill: a }); this.top.attr({ fill: d }); this.side.attr({ fill: f }); this.color = a;
            return this
        }; a.opacitySetter = function (a) { this.front.attr({ opacity: a }); this.top.attr({ opacity: a }); this.side.attr({ opacity: a }); return this }; a.attr = function (a) { a.shapeArgs || q(a.x) ? (a = this.renderer.cuboidPath(a.shapeArgs || a), this.front.attr({ d: a[0], zIndex: a[3] }), this.top.attr({ d: a[1], zIndex: a[4] }), this.side.attr({ d: a[2], zIndex: a[5] })) : c.SVGElement.prototype.attr.call(this, a); return this }; a.animate = function (a, d, f) {
            q(a.x) && q(a.y) ? (a = this.renderer.cuboidPath(a), this.front.attr({ zIndex: a[3] }).animate({ d: a[0] },
            d, f), this.top.attr({ zIndex: a[4] }).animate({ d: a[1] }, d, f), this.side.attr({ zIndex: a[5] }).animate({ d: a[2] }, d, f)) : a.opacity ? (this.front.animate(a, d, f), this.top.animate(a, d, f), this.side.animate(a, d, f)) : c.SVGElement.prototype.animate.call(this, a, d, f); return this
        }; a.destroy = function () { this.front.destroy(); this.top.destroy(); this.side.destroy(); return null }; a.attr({ zIndex: -d[3] }); return a
    }; c.SVGRenderer.prototype.cuboidPath = function (d) {
        var a = d.x, b = d.y, e = d.z, f = d.height, g = d.width, h = d.depth, j = c.map, i = [{
            x: a, y: b,
            z: e
        }, { x: a + g, y: b, z: e }, { x: a + g, y: b + f, z: e }, { x: a, y: b + f, z: e }, { x: a, y: b + f, z: e + h }, { x: a + g, y: b + f, z: e + h }, { x: a + g, y: b, z: e + h }, { x: a, y: b, z: e + h }], i = n(i, c.charts[this.chartIndex], d.insidePlotArea), b = function (a, b) { a = j(a, function (a) { return i[a] }); b = j(b, function (a) { return i[a] }); return E(a) < 0 ? a : E(b) < 0 ? b : [] }, d = b([3, 2, 1, 0], [7, 6, 5, 4]), a = b([1, 6, 7, 0], [4, 5, 2, 3]), b = b([1, 2, 5, 6], [0, 7, 4, 3]); return [this.toLinePath(d, !0), this.toLinePath(a, !0), this.toLinePath(b, !0), D(d), D(a), D(b)]
    }; c.SVGRenderer.prototype.arc3d = function (d) {
        d.alpha *=
        y; d.beta *= y; var a = this.g(), b = this.arc3dPath(d), e = a.renderer, f = b.zTop * 100; a.shapeArgs = d; a.top = e.path(b.top).attr({ zIndex: b.zTop }).add(a); a.side1 = e.path(b.side2).attr({ zIndex: b.zSide1 }); a.side2 = e.path(b.side1).attr({ zIndex: b.zSide2 }); a.inn = e.path(b.inn).attr({ zIndex: b.zInn }); a.out = e.path(b.out).attr({ zIndex: b.zOut }); a.fillSetter = function (a) {
            this.color = a; var b = c.Color(a).brighten(-0.1).get(); this.side1.attr({ fill: b }); this.side2.attr({ fill: b }); this.inn.attr({ fill: b }); this.out.attr({ fill: b }); this.top.attr({ fill: a });
            return this
        }; a.translateXSetter = function (a) { this.out.attr({ translateX: a }); this.inn.attr({ translateX: a }); this.side1.attr({ translateX: a }); this.side2.attr({ translateX: a }); this.top.attr({ translateX: a }) }; a.translateYSetter = function (a) { this.out.attr({ translateY: a }); this.inn.attr({ translateY: a }); this.side1.attr({ translateY: a }); this.side2.attr({ translateY: a }); this.top.attr({ translateY: a }) }; a.animate = function (a, b, d) {
            q(a.end) || q(a.start) ? (this._shapeArgs = this.shapeArgs, c.SVGElement.prototype.animate.call(this,
            { _args: a }, { duration: b, step: function () { var a = arguments[1], b = a.elem, d = b._shapeArgs, e = a.end, a = a.pos, d = c.merge(d, { x: d.x + (e.x - d.x) * a, y: d.y + (e.y - d.y) * a, r: d.r + (e.r - d.r) * a, innerR: d.innerR + (e.innerR - d.innerR) * a, start: d.start + (e.start - d.start) * a, end: d.end + (e.end - d.end) * a }), e = b.renderer.arc3dPath(d); b.shapeArgs = d; b.top.attr({ d: e.top, zIndex: e.zTop }); b.inn.attr({ d: e.inn, zIndex: e.zInn }); b.out.attr({ d: e.out, zIndex: e.zOut }); b.side1.attr({ d: e.side1, zIndex: e.zSide1 }); b.side2.attr({ d: e.side2, zIndex: e.zSide2 }) } }, d)) :
            c.SVGElement.prototype.animate.call(this, a, b, d); return this
        }; a.destroy = function () { this.top.destroy(); this.out.destroy(); this.inn.destroy(); this.side1.destroy(); this.side2.destroy(); c.SVGElement.prototype.destroy.call(this) }; a.hide = function () { this.top.hide(); this.out.hide(); this.inn.hide(); this.side1.hide(); this.side2.hide() }; a.show = function () { this.top.show(); this.out.show(); this.inn.show(); this.side1.show(); this.side2.show() }; a.zIndex = f; a.attr({ zIndex: f }); return a
    }; c.SVGRenderer.prototype.arc3dPath =
    function (d) {
        var a = d.x, b = d.y, e = d.start, c = d.end - 1.0E-5, g = d.r, h = d.innerR, j = d.depth, i = d.alpha, k = d.beta, v = m(e), p = l(e), d = m(c), n = l(c), s = g * m(k), t = g * m(i), u = h * m(k); h *= m(i); var w = j * l(k), x = j * l(i), j = ["M", a + s * v, b + t * p], j = j.concat(r(a, b, s, t, e, c, 0, 0)), j = j.concat(["L", a + u * d, b + h * n]), j = j.concat(r(a, b, u, h, c, e, 0, 0)), j = j.concat(["Z"]), k = k > 0 ? o / 2 : 0, i = i > 0 ? 0 : o / 2, k = e > -k ? e : c > -k ? -k : e, q = c < o - i ? c : e < o - i ? o - i : c, i = ["M", a + s * m(k), b + t * l(k)], i = i.concat(r(a, b, s, t, k, q, 0, 0)), i = i.concat(["L", a + s * m(q) + w, b + t * l(q) + x]), i = i.concat(r(a, b, s, t, q, k,
        w, x)), i = i.concat(["Z"]), k = ["M", a + u * v, b + h * p], k = k.concat(r(a, b, u, h, e, c, 0, 0)), k = k.concat(["L", a + u * m(c) + w, b + h * l(c) + x]), k = k.concat(r(a, b, u, h, c, e, w, x)), k = k.concat(["Z"]), v = ["M", a + s * v, b + t * p, "L", a + s * v + w, b + t * p + x, "L", a + u * v + w, b + h * p + x, "L", a + u * v, b + h * p, "Z"], a = ["M", a + s * d, b + t * n, "L", a + s * d + w, b + t * n + x, "L", a + u * d + w, b + h * n + x, "L", a + u * d, b + h * n, "Z"], b = l((e + c) / 2), e = l(e), c = l(c); return { top: j, zTop: g, out: i, zOut: Math.max(b, e, c) * g, inn: k, zInn: Math.max(b, e, c) * g, side1: v, zSide1: e * g * 0.99, side2: a, zSide2: c * g * 0.99 }
    }; c.Chart.prototype.is3d =
    function () { return this.options.chart.options3d && this.options.chart.options3d.enabled }; c.wrap(c.Chart.prototype, "isInsidePlot", function (d) { return this.is3d() ? !0 : d.apply(this, [].slice.call(arguments, 1)) }); c.getOptions().chart.options3d = { enabled: !1, alpha: 0, beta: 0, depth: 100, viewDistance: 25, frame: { bottom: { size: 1, color: "rgba(255,255,255,0)" }, side: { size: 1, color: "rgba(255,255,255,0)" }, back: { size: 1, color: "rgba(255,255,255,0)" } } }; c.wrap(c.Chart.prototype, "init", function (d) {
        var a = [].slice.call(arguments, 1),
        b; if (a[0].chart.options3d && a[0].chart.options3d.enabled) b = a[0].plotOptions || {}, b = b.pie || {}, b.borderColor = c.pick(b.borderColor, void 0); d.apply(this, a)
    }); c.wrap(c.Chart.prototype, "setChartSize", function (d) { d.apply(this, [].slice.call(arguments, 1)); if (this.is3d()) { var a = this.inverted, b = this.clipBox, c = this.margin; b[a ? "y" : "x"] = -(c[3] || 0); b[a ? "x" : "y"] = -(c[0] || 0); b[a ? "height" : "width"] = this.chartWidth + (c[3] || 0) + (c[1] || 0); b[a ? "width" : "height"] = this.chartHeight + (c[0] || 0) + (c[2] || 0) } }); c.wrap(c.Chart.prototype,
    "redraw", function (d) { if (this.is3d()) this.isDirtyBox = !0; d.apply(this, [].slice.call(arguments, 1)) }); c.Chart.prototype.renderSeries = function () { for (var d, a = this.series.length; a--;) d = this.series[a], d.translate(), d.render() }; c.Chart.prototype.retrieveStacks = function (d) { var a = this.series, b = {}, e, f = 1; c.each(this.series, function (c) { e = d ? c.options.stack || 0 : a.length - 1 - c.index; b[e] ? b[e].series.push(c) : (b[e] = { series: [c], position: f }, f++) }); b.totalStacks = f + 1; return b }; c.wrap(c.Axis.prototype, "init", function (d) {
        var a =
        arguments; if (a[1].is3d()) a[2].tickWidth = c.pick(a[2].tickWidth, 0), a[2].gridLineWidth = c.pick(a[2].gridLineWidth, 1); d.apply(this, [].slice.call(arguments, 1))
    }); c.wrap(c.Axis.prototype, "render", function (d) {
        d.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
            var a = this.chart, b = a.renderer, c = a.options.chart.options3d, f = c.frame, g = f.bottom, h = f.back, f = f.side, j = c.depth, i = this.height, k = this.width, l = this.left, p = this.top; this.horiz ? (this.axisLine && this.axisLine.hide(), h = {
                x: l, y: p + (a.yAxis[0].reversed ?
                -g.size : i), z: 0, width: k, height: g.size, depth: j, insidePlotArea: !1
            }, this.bottomFrame ? this.bottomFrame.animate(h) : this.bottomFrame = b.cuboid(h).attr({ fill: g.color, zIndex: a.yAxis[0].reversed && c.alpha > 0 ? 4 : -1 }).css({ stroke: g.color }).add()) : (c = { x: l, y: p, z: j + 1, width: k, height: i + g.size, depth: h.size, insidePlotArea: !1 }, this.backFrame ? this.backFrame.animate(c) : this.backFrame = b.cuboid(c).attr({ fill: h.color, zIndex: -3 }).css({ stroke: h.color }).add(), this.axisLine && this.axisLine.hide(), a = {
                x: (a.yAxis[0].opposite ? k : 0) + l -
                f.size, y: p, z: 0, width: f.size, height: i + g.size, depth: j + h.size, insidePlotArea: !1
            }, this.sideFrame ? this.sideFrame.animate(a) : this.sideFrame = b.cuboid(a).attr({ fill: f.color, zIndex: -2 }).css({ stroke: f.color }).add())
        }
    }); c.wrap(c.Axis.prototype, "getPlotLinePath", function (c) {
        var a = c.apply(this, [].slice.call(arguments, 1)); if (!this.chart.is3d()) return a; if (a === null) return a; var b = this.chart.options.chart.options3d.depth, a = [{ x: a[1], y: a[2], z: this.horiz || this.opposite ? b : 0 }, { x: a[1], y: a[2], z: b }, { x: a[4], y: a[5], z: b }, {
            x: a[4],
            y: a[5], z: this.horiz || this.opposite ? 0 : b
        }], a = n(a, this.chart, !1); return a = this.chart.renderer.toLinePath(a, !1)
    }); c.wrap(c.Axis.prototype, "getPlotBandPath", function (c) { if (this.chart.is3d()) { var a = arguments, b = a[1], a = this.getPlotLinePath(a[2]); (b = this.getPlotLinePath(b)) && a ? b.push(a[7], a[8], a[4], a[5], a[1], a[2]) : b = null; return b } else return c.apply(this, [].slice.call(arguments, 1)) }); c.wrap(c.Tick.prototype, "getMarkPath", function (c) {
        var a = c.apply(this, [].slice.call(arguments, 1)); if (!this.axis.chart.is3d()) return a;
        a = [{ x: a[1], y: a[2], z: 0 }, { x: a[4], y: a[5], z: 0 }]; a = n(a, this.axis.chart, !1); return a = ["M", a[0].x, a[0].y, "L", a[1].x, a[1].y]
    }); c.wrap(c.Tick.prototype, "getLabelPosition", function (c) { var a = c.apply(this, [].slice.call(arguments, 1)); if (!this.axis.chart.is3d()) return a; a = n([{ x: a.x, y: a.y, z: 0 }], this.axis.chart, !1)[0]; a.x -= !this.axis.horiz && this.axis.opposite ? this.axis.transA : 0; return a }); c.wrap(c.Axis.prototype, "drawCrosshair", function (c) {
        var a = arguments; this.chart.is3d() && a[2] && (a[2] = {
            plotX: a[2].plotXold || a[2].plotX,
            plotY: a[2].plotYold || a[2].plotY
        }); c.apply(this, [].slice.call(a, 1))
    }); c.wrap(c.seriesTypes.column.prototype, "translate", function (d) {
        d.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
            var a = this.chart, b = this.options, e = b.depth || 25, f = (b.stacking ? b.stack || 0 : this._i) * (e + (b.groupZPadding || 1)); b.grouping !== !1 && (f = 0); f += b.groupZPadding || 1; c.each(this.data, function (b) {
                if (b.y !== null) {
                    var c = b.shapeArgs, d = b.tooltipPos; b.shapeType = "cuboid"; c.z = f; c.depth = e; c.insidePlotArea = !0; d = n([{ x: d[0], y: d[1], z: f }],
                    a, !1)[0]; b.tooltipPos = [d.x, d.y]
                }
            })
        }
    }); c.wrap(c.seriesTypes.column.prototype, "animate", function (d) {
        if (this.chart.is3d()) {
            var a = arguments[1], b = this.yAxis, e = this, f = this.yAxis.reversed; if (c.svg) a ? c.each(e.data, function (a) { if (a.y !== null && (a.height = a.shapeArgs.height, a.shapey = a.shapeArgs.y, a.shapeArgs.height = 1, !f)) a.shapeArgs.y = a.stackY ? a.plotY + b.translate(a.stackY) : a.plotY + (a.negative ? -a.height : a.height) }) : (c.each(e.data, function (a) {
                if (a.y !== null) a.shapeArgs.height = a.height, a.shapeArgs.y = a.shapey, a.graphic &&
                a.graphic.animate(a.shapeArgs, e.options.animation)
            }), this.drawDataLabels(), e.animate = null)
        } else d.apply(this, [].slice.call(arguments, 1))
    }); c.wrap(c.seriesTypes.column.prototype, "init", function (c) { c.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) { var a = this.options, b = a.grouping, e = a.stacking, f = 0; if (b === void 0 || b) { b = this.chart.retrieveStacks(e); e = a.stack || 0; for (f = 0; f < b[e].series.length; f++) if (b[e].series[f] === this) break; f = b.totalStacks * 10 - 10 * (b.totalStacks - b[e].position) - f } a.zIndex = f } });
    c.wrap(c.Series.prototype, "alignDataLabel", function (c) { if (this.chart.is3d() && (this.type === "column" || this.type === "columnrange")) { var a = arguments[4], b = { x: a.x, y: a.y, z: 0 }, b = n([b], this.chart, !0)[0]; a.x = b.x; a.y = b.y } c.apply(this, [].slice.call(arguments, 1)) }); c.seriesTypes.columnrange && c.wrap(c.seriesTypes.columnrange.prototype, "drawPoints", F); c.wrap(c.seriesTypes.column.prototype, "drawPoints", F); var C = c.getOptions(); C.plotOptions.cylinder = c.merge(C.plotOptions.column); C = c.extendClass(c.seriesTypes.column,
    { type: "cylinder" }); c.seriesTypes.cylinder = C; c.wrap(c.seriesTypes.cylinder.prototype, "translate", function (d) {
        d.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
            var a = this.chart, b = a.options, e = b.plotOptions.cylinder, b = b.chart.options3d, f = e.depth || 0, g = { x: a.inverted ? a.plotHeight / 2 : a.plotWidth / 2, y: a.inverted ? a.plotWidth / 2 : a.plotHeight / 2, z: b.depth, vd: b.viewDistance }, h = b.alpha, j = e.stacking ? (this.options.stack || 0) * f : this._i * f; j += f / 2; e.grouping !== !1 && (j = 0); c.each(this.data, function (a) {
                var b = a.shapeArgs;
                a.shapeType = "arc3d"; b.x += f / 2; b.z = j; b.start = 0; b.end = 2 * o; b.r = f * 0.95; b.innerR = 0; b.depth = b.height * (1 / l((90 - h) * y)) - j; b.alpha = 90 - h; b.beta = 0; b.origin = g
            })
        }
    }); c.wrap(c.seriesTypes.pie.prototype, "translate", function (d) {
        d.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) {
            var a = this, b = a.chart, e = a.options, f = e.depth || 0, g = b.options.chart.options3d, h = { x: b.plotWidth / 2, y: b.plotHeight / 2, z: g.depth }, j = g.alpha, i = g.beta, k = e.stacking ? (e.stack || 0) * f : a._i * f; k += f / 2; e.grouping !== !1 && (k = 0); c.each(a.data, function (b) {
                b.shapeType =
                "arc3d"; var c = b.shapeArgs; if (b.y) c.z = k, c.depth = f * 0.75, c.origin = h, c.alpha = j, c.beta = i, c = (c.end + c.start) / 2, b.slicedTranslation = { translateX: G(m(c) * a.options.slicedOffset * m(j * y)), translateY: G(l(c) * a.options.slicedOffset * m(j * y)) }
            })
        }
    }); c.wrap(c.seriesTypes.pie.prototype.pointClass.prototype, "haloPath", function (c) { var a = arguments; return this.series.chart.is3d() ? [] : c.call(this, a[1]) }); c.wrap(c.seriesTypes.pie.prototype, "drawPoints", function (d) {
        if (this.chart.is3d()) {
            var a = this.options, b = this.options.states;
            this.borderWidth = a.borderWidth = a.edgeWidth || 1; this.borderColor = a.edgeColor = c.pick(a.edgeColor, a.borderColor, void 0); b.hover.borderColor = c.pick(b.hover.edgeColor, this.borderColor); b.hover.borderWidth = c.pick(b.hover.edgeWidth, this.borderWidth); b.select.borderColor = c.pick(b.select.edgeColor, this.borderColor); b.select.borderWidth = c.pick(b.select.edgeWidth, this.borderWidth); c.each(this.data, function (a) {
                var c = a.pointAttr; c[""].stroke = a.series.borderColor || a.color; c[""]["stroke-width"] = a.series.borderWidth;
                c.hover.stroke = b.hover.borderColor; c.hover["stroke-width"] = b.hover.borderWidth; c.select.stroke = b.select.borderColor; c.select["stroke-width"] = b.select.borderWidth
            })
        } d.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) { var e = this.group; c.each(this.points, function (a) { a.graphic.out.add(e); a.graphic.inn.add(e); a.graphic.side1.add(e); a.graphic.side2.add(e) }) }
    }); c.wrap(c.seriesTypes.pie.prototype, "drawDataLabels", function (d) {
        if (this.chart.is3d()) {
            var a = this; c.each(a.data, function (b) {
                var c = b.shapeArgs,
                d = c.r, g = c.depth, h = (c.alpha || a.chart.options.chart.options3d.alpha) * y, c = (c.start + c.end) / 2, b = b.labelPos; b[1] += -d * (1 - m(h)) * l(c) + (l(c) > 0 ? l(h) * g : 0); b[3] += -d * (1 - m(h)) * l(c) + (l(c) > 0 ? l(h) * g : 0); b[5] += -d * (1 - m(h)) * l(c) + (l(c) > 0 ? l(h) * g : 0)
            })
        } d.apply(this, [].slice.call(arguments, 1))
    }); c.wrap(c.seriesTypes.pie.prototype, "addPoint", function (c) { c.apply(this, [].slice.call(arguments, 1)); this.chart.is3d() && this.update(this.userOptions, !0) }); c.wrap(c.seriesTypes.pie.prototype, "animate", function (d) {
        if (this.chart.is3d()) {
            var a =
            arguments[1], b = this.options.animation, e = this.center, f = this.group, g = this.markerGroup; if (c.svg) if (b === !0 && (b = {}), a) { if (f.oldtranslateX = f.translateX, f.oldtranslateY = f.translateY, a = { translateX: e[0], translateY: e[1], scaleX: 0.001, scaleY: 0.001 }, f.attr(a), g) g.attrSetters = f.attrSetters, g.attr(a) } else a = { translateX: f.oldtranslateX, translateY: f.oldtranslateY, scaleX: 1, scaleY: 1 }, f.animate(a, b), g && g.animate(a, b), this.animate = null
        } else d.apply(this, [].slice.call(arguments, 1))
    }); c.wrap(c.seriesTypes.scatter.prototype,
    "translate", function (c) { c.apply(this, [].slice.call(arguments, 1)); if (this.chart.is3d()) { var a = this.chart, b = a.options.chart.options3d.depth, e = a.options.zAxis || { min: 0, max: b }, f = b / (e.max - e.min), g = [], h; for (h = 0; h < this.data.length; h++) b = this.data[h], g.push({ x: b.plotX, y: b.plotY, z: (b.z - e.min) * f }); a = n(g, a, !0); for (h = 0; h < this.data.length; h++) b = this.data[h], e = a[h], b.plotXold = b.plotX, b.plotYold = b.plotY, b.plotX = e.x, b.plotY = e.y, b.plotZ = e.z } }); c.wrap(c.seriesTypes.scatter.prototype, "init", function (c) {
        var a = c.apply(this,
        [].slice.call(arguments, 1)); if (this.chart.is3d()) this.pointArrayMap = ["x", "y", "z"], this.tooltipOptions.pointFormat = this.userOptions.tooltip ? this.userOptions.tooltip.pointFormat || "x: <b>{point.x}</b><br/>y: <b>{point.y}</b><br/>z: <b>{point.z}</b><br/>" : "x: <b>{point.x}</b><br/>y: <b>{point.y}</b><br/>z: <b>{point.z}</b><br/>"; return a
    }); if (c.VMLRenderer) c.setOptions({ animate: !1 }), c.VMLRenderer.prototype.cuboid = c.SVGRenderer.prototype.cuboid, c.VMLRenderer.prototype.cuboidPath = c.SVGRenderer.prototype.cuboidPath,
    c.VMLRenderer.prototype.toLinePath = c.SVGRenderer.prototype.toLinePath, c.VMLRenderer.prototype.createElement3D = c.SVGRenderer.prototype.createElement3D, c.VMLRenderer.prototype.arc3d = function (d) { d = c.SVGRenderer.prototype.arc3d.call(this, d); d.css({ zIndex: d.zIndex }); return d }, c.VMLRenderer.prototype.arc3dPath = c.SVGRenderer.prototype.arc3dPath, c.wrap(c.Axis.prototype, "render", function (c) {
        c.apply(this, [].slice.call(arguments, 1)); this.sideFrame && (this.sideFrame.css({ zIndex: 0 }), this.sideFrame.front.attr({ fill: this.sideFrame.color }));
        this.bottomFrame && (this.bottomFrame.css({ zIndex: 1 }), this.bottomFrame.front.attr({ fill: this.bottomFrame.color })); this.backFrame && (this.backFrame.css({ zIndex: 0 }), this.backFrame.front.attr({ fill: this.backFrame.color }))
    })
})(Highcharts);