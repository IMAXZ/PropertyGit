PIXI.utils._saidHello = true;
var Drawer = Drawer || function () {
    var d = {};
    //内部属性方法
    d._ = {
        needRedraw: false,
        width: 0,
        height: 0,
        objects: {
            meters: {},
            inverters: {},
            dcbus: {},
            convs: {},
            dev: {}
        },
        //主循环
        update: function () {
            var renderer = d._.renderer,
                target = d._.target,
                container = d._.container;
            d._.draw();
            renderer.resize(target.offsetWidth, target.offsetHeight);
            renderer.render(container);
            requestAnimationFrame(d._.update);
        },
        //绘制接线图
        draw: function () {
            this.temp = {};
            if (this.needRedraw) {
                var conns = this.conn,
                    x = 0,
                    g = this.g;
                g.removeChildren();
                if (conns && conns.length) {
                    for (var i = 0; i < conns.length; i++) {
                        x += this.drawPage(conns[i], x);
                    }
                }
                this.needRedraw = false;
                this.width = x;
                g.beginFill(0, 0);
                g.lineStyle(0, this.options.stroke, 0);
                var t = this.target;
                g.drawRect(-t.offsetWidth, -t.offsetHeight, this.width + t.offsetWidth * 2, this.height + t.offsetHeight * 2);
                if (this.centerAfterDraw) {
                    d.center();
                    this.centerAfterDraw = false;
                }
            }
        },
        //画接线图页
        drawPage: function (conn, x) {
            if (!conn || !conn.invGroups)
                return 0;
            var sumMode = 0,
                lastRect = -1,
                lastRectMode = 0,
                lastMeter = -1,
                lastMeterMode = 0,
                firstInvMode = 0,
                lastInvMode = 0;
            for (var i = 0; i < conn.invGroups.length; i++) {
                var inv = conn.invGroups[i];
                if (inv.mode)
                    inv.mode = inv.mode;
                else if (inv.convs && inv.convs.length)
                    inv.mode = inv.convs.length / 5;
                else
                    inv.mode = 1;
                this.drawInvGroup(inv, x + sumMode * 320);
                //多个直流柜的情况
                if (lastRect != inv.dcbus) {
                    if (lastRectMode > 0) {
                        this.drawDCBus(x + (sumMode - lastRectMode) / 320, lastRectMode, lastRect);
                    }
                    lastRect = inv.dcbus;
                    lastRectMode = inv.mode;
                } else {
                    lastRectMode += inv.mode;
                }
                //多个电表的情况
                if (lastMeter != inv.meter) {
                    if (lastMeterMode > 0) {
                        this.drawMeter(x + (sumMode - lastMeterMode) / 320, lastMeterMode, lastMeter, firstInvMode, lastInvMode);
                    }
                    lastMeter = inv.meter;
                    lastMeterMode = inv.mode;
                    firstInvMode = inv.mode;
                } else {
                    lastMeterMode += inv.mode;
                }
                lastRect = inv.dcbus;
                lastMeter = inv.meter;
                lastInvMode = inv.mode;
                sumMode += inv.mode;
            }
            this.drawEnd(x, sumMode, conn);
            if (lastRectMode > 0) {
                this.drawDCBus(x + (sumMode - lastRectMode) / 320, lastRectMode, lastRect);
            }
            if (lastMeterMode > 0) {
                this.drawMeter(x + (sumMode - lastMeterMode) / 320, lastMeterMode, lastMeter, firstInvMode, lastInvMode);
            }
            return sumMode * 320;
        },
        //标题
        drawEnd: function (x, mode, conn) {
            var g = this.g,
                c = this.container,
                s = this.options,
                anchor = new PIXI.Point(0.5, 1),
                name = conn.name ? conn.name : this.objects.name;
            g.lineStyle(4, s.stroke);
            g.moveTo(x + 50, 30);
            g.lineTo(x + mode * 320 - 50, 30);
            if (name) {
                g.addChild(this.getText(name, x + mode * 320 / 2, 28, s.titleColor, anchor, 14));
            }
        },
        //画电表
        drawMeter: function (x, mode, id, firstMode, lastMode) {
            var gAll = this.g,
                g = new PIXI.Graphics(),
                s = this.options,
                o = this.objects,
                rectWidth = mode * 320 - 50,
                m = o.meters[id] || {},
                textColor = m.isAllError ? s.textColorError : s.textColor,
                borderColor = m.isAllError ? s.strokeError : s.stroke;
            g.lineStyle(s.strokeWidth, borderColor);
            g.interactive = true;
            g.on("click", function () {
                if (s.click) {
                    s.click(id);
                }
            });
            //逆变器连接线
            if (firstMode + lastMode <= mode) {
                g.moveTo(x + firstMode * 160, 110);
                g.lineTo(x + mode * 320 - lastMode * 160, 110);
            }
            var mid = mode * 320 / 2 + x;
            g.moveTo(mid, 30);
            g.lineTo(mid, 110);
            //电表
            g.beginFill(0, 0);
            g.drawCircle(mid - 50, 60, 15);
            g.drawCircle(mid, 60, 15);
            g.drawCircle(mid + 50, 60, 15);
            g.moveTo(mid - 50, 40);
            g.lineTo(mid - 50, 80);
            g.moveTo(mid + 50, 40);
            g.lineTo(mid + 50, 80);
            g.moveTo(mid - 35, 60);
            g.lineTo(mid - 15, 60);
            g.moveTo(mid + 15, 60);
            g.lineTo(mid + 35, 60);
            g.moveTo(mid + 65, 60);
            g.lineTo(mid + 85, 60);
            g.moveTo(mid - 26, 50);
            g.lineTo(mid - 30, 70);
            g.moveTo(mid - 20, 50);
            g.lineTo(mid - 24, 70);
            g.moveTo(mid + 24, 50);
            g.lineTo(mid + 20, 70);
            g.moveTo(mid + 30, 50);
            g.lineTo(mid + 26, 70);
            g.moveTo(mid + 74, 50);
            g.lineTo(mid + 70, 70);
            g.moveTo(mid + 80, 50);
            g.lineTo(mid + 76, 70);
            g.endFill();
            g.beginFill(borderColor);
            g.drawCircle(mid - 50, 45, 2);
            g.drawCircle(mid, 45, 2);
            g.drawCircle(mid + 50, 45, 2);
            g.endFill();
            g.addChild(this.getText(m.name || s.mask, mid - 10, 85, textColor, "right"));
            g.addChild(this.getText(m.output || s.mask, mid + 10, 85, textColor));
            gAll.addChild(g);
        },
        drawDCBus: function (x, mode, id) {
            var gAll = this.g,
                g = new PIXI.Graphics(),
                s = this.options,
                o = this.objects,
                rectWidth = mode * 320 - 50,
                bus = o.dcbus[id] || {},
                textColor = bus.isAllError ? s.textColorError : s.textColor,
                borderColor = bus.isAllError ? s.strokeError : s.stroke;
            g.lineStyle(s.strokeWidth, borderColor);
            g.interactive = true;
            g.on("click", function () {
                if (s.click) {
                    s.click(id);
                }
            });
            g.beginFill(0, 0);
            g.drawRect(x + 25, 260, rectWidth, 85);
            g.endFill();
            g.addChild(this.getText(bus.name || s.mask, x + 30, 265, textColor));
            gAll.addChild(g);
        },
        //画逆变器组
        drawInvGroup: function (inv, x) {
            var gAll = this.g,
                s = this.options,
                o = this.objects,
                i = o.inverters[inv.id] || { needSeperator: true },
                err = i && i.isError,
                stroke = err ? s.strokeError : s.stroke,
                textColor = err ? s.textColorError : s.textColor,
                g = new PIXI.Graphics(),
                width = inv.mode * 320,
                mid = width / 2;
            g.interactive = true;
            g.position.x = x;
            g.position.y = 110;
            g.lineStyle(s.strokeWidth, s.stroke);
            //隔离变
            g.moveTo(mid, 0);
            if (i.needSeperator) {
                g.lineTo(mid, 10);
                g.drawEllipse(mid, 20, 10, 10);
                g.drawEllipse(mid, 35, 10, 10);
                g.moveTo(mid, 45);
                g.lineTo(mid, 55);
            } else {
                g.lineTo(mid, 55);
            }
            g.moveTo(mid, 115);
            g.lineTo(mid, 185);
            var convsWidth = (inv.convs || []).length * 56,
                    convLinksHalfWidth = (convsWidth > 0 ? convsWidth - 56 : 0) / 2;
            if (convLinksHalfWidth > 0) {
                g.moveTo(mid - convLinksHalfWidth, 185);
                g.lineTo(mid + convLinksHalfWidth, 185);
            }
            g.beginFill(0, 0);
            g.drawRect(mid - 30, 55, 60, 60);
            g.endFill();
            g.moveTo(mid + 30, 55);
            g.lineTo(mid - 30, 115);
            g.moveTo(mid - 24, 69);
            g.arc(mid - 14, 73, 10, Math.PI * 3 / 2 - .9, 3 * Math.PI / 2 + .8);
            g.moveTo(mid + 11, 63);
            g.arc(mid + 1, 59, 10, Math.PI / 2 - .9, Math.PI / 2 + .8);
            g.moveTo(mid - 4, 100);
            g.lineTo(mid + 22, 100);
            g.moveTo(mid - 4, 107);
            g.lineTo(mid + 22, 107);
            g.on("click", function () {
                if (s.click)
                    s.click(inv.id);
            })
            g.addChild(this.getText(i.name || s.mask, mid - 40, 26, textColor, 'right'));
            g.addChild(this.getText(i.ia || s.mask, mid - 40, 44, textColor, 'right'));
            g.addChild(this.getText(i.ib || s.mask, mid - 40, 62, textColor, 'right'));
            g.addChild(this.getText(i.ic || s.mask, mid - 40, 80, textColor, 'right'));
            g.addChild(this.getText(i.pdc || s.mask, mid - 40, 98, textColor, 'right'));
            g.addChild(this.getText(i.daygen || '', mid - 40, 116, textColor, 'right'));
            g.addChild(this.getText(i.model || s.mask, mid + 40, 26, textColor));
            g.addChild(this.getText(i.ua || s.mask, mid + 40, 44, textColor));
            g.addChild(this.getText(i.ub || s.mask, mid + 40, 62, textColor));
            g.addChild(this.getText(i.uc || s.mask, mid + 40, 80, textColor));
            g.addChild(this.getText(i.power || s.mask, mid + 40, 98, textColor));
            g.addChild(this.getText(i.temperature || s.mask, mid + 40, 116, textColor));
            this.drawGround(x + mid, 276);
            if (inv.convs && inv.convs.length) {
                for (var i = 0; i < inv.convs.length; i++) {
                    var convHeight = this.drawConv(inv.convs[i], x + mid + 56 * i - convLinksHalfWidth, inv.dcbus);
                    if (this.height < convHeight)
                        this.height = convHeight;
                }
            }
            gAll.addChild(g);
        },
        //画汇流箱
        drawConv: function (conv, x, dcbus) {
            var s = this.options,
                gAll = this.g,
                o = this.objects,
                t = this.temp,
                g = new PIXI.Graphics(),
                d = o.dcbus[dcbus] || {},
                c = o.convs[conv] || {},
                sprite = PIXI.Sprite.fromImage(s.convImg),
                height = 170 + (c.roads ? c.roads.length : 0) * 27,
                textColor = c && c.isAllError ? s.textColorError : s.textColor;
            g.interactive = true;
            g.lineStyle(s.strokeWidth, s.stroke);
            g.position.x = x;
            g.position.y = 295;
            g.moveTo(0, 0);
            g.lineTo(0, 20);
            t.dcbus = t.dcbus || {};
            var r = t.dcbus[dcbus] = t.dcbus[dcbus] || 0;
            t.dcbus[dcbus]++;
            this.drawBreaker(x, 316, d.roads && d.roads.length > r && d.roads[r]);
            var dcRoadTextColor;
            if (d.roads && d.roads.length > r) {
                var v = d.roads[r];
                if (v < d.lowMin) {
                    dcRoadTextColor = s.textColorError;
                } else if (v < d.lowMax) {
                    dcRoadTextColor = s.textColorWarning;
                } else {
                    dcRoadTextColor = s.textColor;
                }
            }
            g.addChild(this.getText((d.roads && d.roads.length > r) ? (d.roads[r] + ' A') : s.mask, -2, 0, dcRoadTextColor, 'right'));
            g.moveTo(0, 40);
            g.lineTo(0, 70);
            g.moveTo(9, 80);
            g.lineTo(20, 80);
            g.lineTo(20, height);
            //img
            sprite.interactive = true;
            sprite.x = -35;
            sprite.y = 69;
            sprite.width = 56;
            sprite.height = 73;
            sprite.on("click", function () {
                if (s.click) {
                    s.click(conv);
                }
            })
            g.addChild(sprite);
            //文字
            if (!t.convCount)
                t.convCount = 0;
            var titleY = 138;
            if (t.convCount % 2) {
                titleY = 153;
            }
            t.convCount++;
            //各支路
            for (var i = 0; c.roads && i < c.roads.length; i++) {
                var v = c.roads[i],
                    baseTop = 170 + i * 27,
                    tColor,
                    sColor;
                if (v < c.lowMin) {
                    tColor = s.textColorError;
                    sColor = s.strokeError;
                } else if (v < c.lowMax) {
                    tColor = s.textColorWarning;
                    sColor = s.strokeWarning;
                } else {
                    tColor = s.textColor;
                    sColor = s.stroke;
                }
                g.lineStyle(s.strokeWidth, sColor);
                var text = this.getText(c.roads[i] + ' A', 20, baseTop, tColor, 'right');
                g.addChild(text);
                g.moveTo(-12, baseTop + 21);
                g.lineTo(19, baseTop + 21);
                g.moveTo(14, baseTop + 15);
                g.lineTo(14, baseTop + 27);
                g.moveTo(-27, baseTop + 21);
                g.lineTo(-19, baseTop + 21);
                g.moveTo(-18, baseTop + 18);
                g.lineTo(-18, baseTop + 24);
                g.moveTo(-13, baseTop + 16);
                g.lineTo(-13, baseTop + 26);
                g.moveTo(-28, baseTop + 13);
                g.lineTo(-19, baseTop + 20);
                g.moveTo(-23, baseTop + 13);
                g.lineTo(-14, baseTop + 20);
                g.drawRect(-25, baseTop + 15, 20, 12);
                g.beginFill(sColor);
                g.lineStyle(0, sColor);
                g.moveTo(14, baseTop + 21);
                g.lineTo(0, baseTop + 15);
                g.lineTo(0, baseTop + 27);
                g.endFill();
            }
            g.addChild(this.getText(c.name || s.mask, 20, titleY, textColor, "right"));
            gAll.addChild(g);
            return height + 320;
        },
        //画开关
        drawBreaker: function (x, y, status) {
            var g = this.g,
                s = this.options;
            g.lineStyle(s.strokeWidth, s.stroke);
            g.moveTo(x - 3, y - 3);
            g.lineTo(x + 3, y + 3);
            g.moveTo(x - 3, y + 3);
            g.lineTo(x + 3, y - 3);
            g.beginFill(s.stroke);
            g.drawEllipse(x, y + 19, 2, 2);
            g.endFill();
            g.moveTo(x, y + 19);
            if (!status) {
                g.lineTo(x + 9.5, y + 2.5);
            } else {
                g.lineTo(x, y);
            }
        },
        //画接地线
        drawGround: function (x, y) {
            var g = this.g,
                    s = this.options;
            g.lineStyle(s.strokeWidth, s.stroke);
            g.drawRect(x + 25, y - 7, 30, 14);
            g.moveTo(x, y);
            g.lineTo(x + 30, y);
            g.beginFill(s.stroke);
            g.drawPolygon(x + 30, y - 4, x + 30, y + 4, x + 50, y);
            g.endFill();
            g.moveTo(x + 55, y)
            g.lineTo(x + 65, y);
            g.moveTo(x + 65, y - 10);
            g.lineTo(x + 65, y + 10);
            g.moveTo(x + 70, y - 7);
            g.lineTo(x + 70, y + 7);
            g.moveTo(x + 75, y - 4);
            g.lineTo(x + 75, y + 4);
        },
        //获取文字
        getText: function (text, x, y, color, anchor, fontsize) {
            fontsize = fontsize ? fontsize : 12;
            var text = new PIXI.Text(text, { font: fontsize + 'px 微软雅黑', fill: color });
            text.x = x;
            text.y = y;
            if (typeof (anchor) == "object")
                text.anchor = anchor;
            else if (anchor == "right")
                text.anchor = new PIXI.Point(1, 0);
            else if (anchor == "center")
                text.anchor = new PIXI.Point(0.5, 0);
            return text;
        }
    };
    //拖拽开始
    d._.onDragStart = function (e) {
        this.data = e.data;
        this.beginPosition = e.data.getLocalPosition(this.parent);
        this.beginPosition.x -= this.position.x;
        this.beginPosition.y -= this.position.y;
        this.dragging = true;
    }
    //拖拽结束
    d._.onDragEnd = function (e) {
        delete (this.dragging);
        delete (this.data);
        delete (this.beginPosition);
    }
    //拖拽
    d._.onDragMove = function (e) {
        if (this.dragging) {
            var newPosition = this.data.getLocalPosition(this.parent),
                x = newPosition.x - this.beginPosition.x,
                y = newPosition.y - this.beginPosition.y,
                _ = d._,
                w = _.width,
                h = _.height,
                t = d._.target,
                tw = t.offsetWidth,
                th = t.offsetHeight,
                scale = _.container.scale,
                padding = 100;
            if (x > padding / scale.x - w && x < (tw - padding) / scale.x)
                this.position.x = Math.round(x);
            if (y > padding / scale.y - h && y < (th - padding) / scale.y)
                this.position.y = Math.round(y);
        }
    }
    //初始化
    d.init = function (o) {
        var t = d._;
        var options = t.options = $.extend({}, Drawer.defaults, o);
        var target = t.target = document.getElementById(options.targetId);
        t.renderer = PIXI.autoDetectRenderer(target.offsetWidth, target.offsetHeight, {
            view: target, backgroundColor: options.background,
            antialias: true
        });
        t.container = new PIXI.Container();
        t.container.interactive = true;
        t.g = new PIXI.Graphics();
        t.g.interactive = true;
        d._.draw();
        t.container.addChild(t.g);
        t.g//背景可移动
            .on('mousedown', t.onDragStart)
            .on('touchstart', t.onDragStart)
            .on('mouseup', t.onDragEnd)
            .on('mouseupoutside', t.onDragEnd)
            .on('touchend', t.onDragEnd)
            .on('touchendoutside', t.onDragEnd)
            .on('mousemove', t.onDragMove)
            .on('touchmove', t.onDragMove);
        requestAnimationFrame(d._.update);
    }
    //设置连接图
    d.setConn = function (conn) {
        d._.conn = conn;
        d._.needRedraw = true;
        d._.centerAfterDraw = true;
    }
    //设置数据
    d.setData = function (data) {
        var o = this._.objects,
                s = this._.options;
        if (!data) {
            return;
        }
        o.name = data.PowerName;
        for (var i = 0; i < data.Devices.length; i++) {
            var dev = data.Devices[i];
            switch (dev.DeviceType) {
                case 1:
                    if (!dev.Model)
                        dev.Model = 500;
                    var models = +dev.Model,
                            needSeperator = false;
                    for (var j = 0; j < models.length; j++) {
                        if ((+models[j]) > 200) {
                            needSeperator = true;
                            break;
                        }
                    }
                    o.inverters[dev.DeviceId] = { name: dev.DeviceName, model: dev.Model, needSeperator: needSeperator };
                    break;
                case 2:
                    o.convs[dev.DeviceId] = { name: dev.DeviceName, road: dev.InRoad };
                    if (dev.InRoad > 16) {
                        o.convs[dev.DeviceId].roadInvalid = true;
                    }
                    break;
                case 3:
                    o.dcbus[dev.DeviceId] = { name: dev.DeviceName, road: dev.InRoad };
                    break;
                case 6:
                    o.meters[dev.DeviceId] = { name: dev.DeviceName };
                    break;
                default: break;
            }
        }
        //汇流箱
        var totalRoads = 0,
                totalCurrents = 0;
        for (var i = 0; i < data.ConvData.length; i++) {
            var d = data.ConvData[i],
                    c = o.convs[d.DeviceId];
            if (!c)
                continue;
            if (c.roadInvalid) {
                for (var k = d.Currents.length - 1; k >= 0; k--) {
                    if (d.Currents[k] != 0) {
                        break;
                    }
                    c.road = k;
                }
                if (c.road == 0) {
                    c.road = 16;
                }
            }
            c.roads = [];
            totalRoads += c.road;
            for (var j = 0; j < c.road; j++) {
                c.roads[j] = d.Currents[j];
                c.isAllError = d.IsError;
                totalCurrents += d.Currents[j];
            }
        }
        var avgCurrent = totalCurrents / totalRoads;
        if (avgCurrent > 1) {
            var lowMax = avgCurrent / 3,
                    lowMin = avgCurrent / 7;
            for (var id in o.convs) {
                o.convs[id].lowMax = lowMax;
                o.convs[id].lowMin = lowMin;
            }
        }
        //直流柜
        totalRoads = totalCurrents = 0;
        for (var i = 0; i < data.DCBusData.length; i++) {
            var d = data.DCBusData[i],
                    b = o.dcbus[d.DeviceId];
            if (!b)
                continue;
            b.roads = [];
            totalRoads += b.road;
            for (var j = 0; j < b.road; j++) {
                b.roads[j] = d.Currents[j];
                b.isAllError = d.IsError;
                totalCurrents += d.Currents[j];
            }
        }
        avgCurrent = totalCurrents / totalRoads;
        if (avgCurrent > 1) {
            var lowMax = avgCurrent / 3,
                    lowMin = avgCurrent / 7;
            for (var id in o.dcbus) {
                o.dcbus[id].lowMax = lowMax;
                o.dcbus[id].lowMin = lowMin;
            }
        }
        //逆变器
        for (var i = 0; i < data.InverterData.length; i++) {
            var d = data.InverterData[i],
                    inv = o.inverters[d.DeviceId];
            inv.isError = d.IsError;
            inv.ua = 'Ua : ' + d.AVoltage + ' V';
            inv.ub = 'Ub : ' + d.BVoltage + ' V';
            inv.uc = 'Uc : ' + d.CVoltage + ' V';
            inv.ia = 'Ia : ' + d.ACurrent + ' A';
            inv.ib = 'Ib : ' + d.BCurrent + ' A';
            inv.ic = 'Ic : ' + d.CCurrent + ' A';
            inv.pdc = 'P[DC] : ' + d.DCPower + ' kW';
            inv.power = 'P : ' + d.TActivePower + ' kW';
            inv.temperature = 'T : ' + d.Temperature + ' ℃';
            if (d.DayGen) {
                inv.daygen = '日发电量 : ' + d.DayGen;
            }
        }
        for (var i = 0; i < data.MeterData.length; i++) {
            var d = data.MeterData[i],
                    m = o.meters[d.DeviceId];
            m.output = '发电量 : ' + d.Power + 'kWh';
        }
        this._.needRedraw = true;
    };
    //居中
    d.center = function () {
        var t = d._.target,
            g = d._.g,
            _ = d._;
        g.x = Math.round((t.offsetWidth - _.width) / 2);
        g.y = Math.round((t.offsetHeight - _.height) / 2);
    };
    return d;
};
//默认配置项
Drawer.defaults = {
    convImg: '/Images/home/conv.png',//汇流箱图标地址
    targetId: 'target-canvas',//目标
    strokeWidth: 1, //线宽
    stroke: 0x1AB394, //线条颜色
    strokeWarning: 0xFFf0ad4e,//电流低颜色
    strokeError: 0xFFd9534f,//告警线条颜色
    background: 0xFFFFFF, //背景颜色
    textColor: 0x555555,//字体颜色
    textColorWarning: 0xFFf0ad4e,//电流低颜色
    textColorError: 0xFFd9534f,//告警字体颜色
    titleColor: 0xF8AC59,
    mask: '-',//无数据填充
    click: function (id, type) {
        console.log(id);
    },//点击设备事件
}