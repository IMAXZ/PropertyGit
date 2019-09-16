
// MAP setup
var width = 800,
    height = 550;

// data goes here
var data = [["Gansu", 48], ["Qinghai", 47], ["Guangxi", 45], ["Guizhou", 35], ["Chongqing", 34], ["Beijing", 12], ["Fujian", 35], ["Anhui", 6], ["Guangdong", 40], ["Xizang", 3], ["Xinjiang", 12], ["Hainan", 21], ["Ningxia", 8], ["Shaanxi", 40], ["Shanxi", 11], ["Hubei", 1], ["Hunan", 23], ["Sichuan", 19], ["Yunnan", 19], ["Hebei", 34], ["Henan", 20], ["Liaoning", 14], ["Shandong", 0], ["Tianjin", 12], ["Jiangxi", 20], ["Jiangsu", 37], ["Shanghai", 34], ["Zhejiang", 46], ["Jilin", 38], ["Inner Mongol", 10], ["Heilongjiang", 20], ["Taiwan", 45], ["Xianggang", 35], ["Macau", 10]];
var title = '';
var desc = '';
var credits = '';
var units = 'kWh/㎡/Day';
// DATA 
// parse data properly
var umap = []
data.map(function (d) { umap[d[0]] = Number(d[1]) });

var v = Object.keys(umap).map(function (k) { return umap[k] })

// LOAD DATA
queue()
    .defer(d3.json, "/content/maps/zh-mainland-provinces.topo.json") // mainland
    .defer(d3.json, "/content/maps/zh-chn-twn.topo.json") // taiwan 
    //.defer(d3.json, "maps/zh-hkg-mac.topo.json") // hk and macau
    .await(drawMap); // function that uses files

// DRAW 
// create SVG map
var projection = d3.geo.mercator()
    .center([116, 39])
    .scale(600);

var svg = d3.select("#map").append("svg")
    .attr("preserveAspectRatio", "xMidYMid")
    .attr("viewBox", "0 0 " + width + " " + height);

var path = d3.geo.path()
    .projection(projection);

// COLORS
// define color scale
var colorScale = d3.scale.linear()
           .domain(d3.extent(v))
           .interpolate(d3.interpolateHcl)
           .range(["White", "#1AB394"]);

// add grey color if no values
var color = function (i) {
    if (i == undefined) { return "white" }
    else return colorScale(i)
}

// BACKGROUND
//svg.append("g")
//    .attr("class", "background")
//    .append("rect")
//    .attr("class", "background")
//    .attr("width", width)
//    .attr("height", height)
//    .attr("fill", "#eeeeee")
//    .attr("stroke", "black")
//    .attr("stroke-width", "0.35");

// TITLE AND INFOS
svg.append('g')
    .attr("class", "info")
    .attr("transform", "translate(" + (width - 140) + "," + (height - 180) + ")")
    .append("rect")
    .attr({ fill: "transparent", height: 160, width: 160 })

svg.select('.info')
    .append("g")
    .attr("class", "title")
    .append("text")
    // .attr("dx", function(d){return 35})          
    .attr("transform", "translate(0,-70)")
    .attr("dy", function (d) { return 16 })
    .attr("text-anchor", "middle")
    .attr("font-family", "sans-serif")
    .attr("fill", "#4B4B4B")
    .style("text-decoration", "bold")
    .text(title)
    .attr("font-size", 16)
    .call(wrap, 150);

svg.select('.info')
    .append("g")
    .attr("class", "legend")
    .append("text")
    .attr("dx", function (d) { return 0 })
    .attr("dy", 12)
    .attr("text-anchor", "middle")
    .attr("font-family", "sans-serif")
    .attr("fill", "#aaaaaa")
    .attr("font-size", 12)
    .text(desc)
    .call(wrap, 150);

svg.select('.info')
    .append("g")
    .attr("class", "credits")
    .attr("transform", "translate(0,140)")
    .append("text")
    .attr("dx", function (d) { return 0 })
    .attr("dy", 9)
    .attr("text-anchor", "middle")
    .attr("font-family", "sans-serif")
    .attr("fill", "#aaaaaa")
    .attr("font-size", 11)
    .text(credits)
    .call(wrap, 150);

// CAPTION
// Color bar adapted from http://tributary.io/tributary/3650755/
svg.append("g")
    .attr("class", "caption")
    .append("g")
    .attr("class", "color-bar")
    .selectAll("rect")
    .data(d3.range(d3.min(v), d3.max(v), (d3.max(v) - d3.min(v)) / 20.0))
    .enter()
    .append("rect")
    .attr({
        width: 25,
        height: 5,
        y: function (d, i) { return height - 25 - i * 5 },
        x: width - 70,
        fill: function (d, i) { return color(d); }
    })

svg.select(".caption")
    .append("g")
    .attr("transform", "translate(" + (width - 45) + "," + (height - 25 - 5 * 19) + ")")
    .call(d3.svg.axis()
            .scale(d3.scale.linear().domain(d3.extent(v)).range([5 * 20, 0]))
            .ticks(6)
            .orient("right"))
    .attr("font-family", "sans-serif")
    .attr("fill", "#4B4B4B")
    .attr("font-size", 10)

svg.select('.caption')
    .append("g")
    .attr("class", "units")
    .attr("transform", "translate(" + (width - 90) + "," + (height - 28 * 5) + ")")
    .append("text")
    .attr("dx", function (d) { return 0 })
    .attr("dy", 9)
    .attr("text-anchor", "right")
    .attr("font-family", "sans-serif")
    .attr("fill", "#4b4b4b")
    .attr("font-size", 10)
    .text(units)
    .call(wrap, 20);


// DRAW
function drawMap(error, mainland, taiwan, hkmacau) {
    drawShadow(error, taiwan);
    drawProvinces(error, mainland);
    drawTaiwan(error, taiwan);
    window.map_loaded = true;
    //drawHkMacau(error, hkmacau);
}

function translateName(name) {
    var i = name.indexOf('|');
    return name.substr(i + 1);
}

// Mainland provinces
function drawProvinces(error, cn) {

    // var codes=[];
    // for (var i = 0; i < topojson.feature(cn, cn.objects.provinces).features.length; i++) {
    //     codes.push(topojson.feature(cn, cn.objects.provinces).features[i].properties.name)

    // };

    svg.append("g")
        .attr("class", "map")
        .append("g")
        .attr("class", "mainland")
        .selectAll("path")
        .data(topojson.feature(cn, cn.objects.provinces).features)
        .enter()
        .append("path")
        .attr("d", path)
        .attr("id", function (d) { return d.id; })
        .attr("class", "province")
        .attr("fill", "#cccccc")
        .attr("fill", function (d) { return color(umap[d.properties.name]); })
        .attr("stroke", "#AAA")
        .attr("stroke-width", ".35");


    svg.select(".map")
        .append('g')
        .attr("class", "labels")
        .selectAll("text")
        .data(topojson.feature(cn, cn.objects.provinces).features)
        .enter()
        .append("text")
        .text(function (d) {
            var hideIds = [0, 4, 11, 12, 23, 25, 26]
            if (hideIds.indexOf(d.id) >= 0)
                return "";
            else if (d.id == 29)
                return "内蒙古";
            else if (d.id == 2)
                return "广西";
            return translateName(d.properties.name_local);
        })
        .attr({
            width: 200,
            height: 12,
            fill: "white"
        })
        .attr("x", function (d) {
            var ploc = [d.properties.longitude, d.properties.latitude],
                pscr = projection(ploc),
                name = $(this).text();
            return pscr[0] - 6 * name.length;
        })
        .attr("y", function (d) {
            var ploc = [d.properties.longitude, d.properties.latitude],
                pscr = projection(ploc);
            return pscr[1];
        })
        .attr("data-province", function (d) { return d.properties.code_hasc; });;

    svg.select(".map")
        .append("g")
        .attr("class", "markers");
    svg.select(".map")
        .append("g")
        .attr("class", "triggers")
        .selectAll("path")
        .data(topojson.feature(cn, cn.objects.provinces).features)
        .enter()
        .append("path")
        .attr("d", path)
        .attr("id", function (d) { return d.id; })
        .attr("class", "province-trigger")
        .attr("fill", "rgba(255,255,255,0.1)")
        .attr("data-province", function (d) { return d.properties.code_hasc; })
        .attr("data-center", function (d) { return projection([d.properties.longitude, d.properties.latitude]); });
}

function addMarker(name) {
    //#FFA53D
    var center = $(".province-trigger[data-province='" + name + "']").data("center");
    if (!center) {
        console.log(name + " is not recognized as a province code!");
        return;
    }
    var ps = center.split(","),
        p = [+ps[0], +ps[1]];
    $("text[data-province='" + name + "']").hide();
    svg.select(".map")
        .select(".markers")
        .append("ellipse")
            .attr("cx", p[0])
            .attr("cy", p[1])
            .attr("rx", 4)
            .attr("ry", 4)
            .attr("fill", "#FFA53D");
}

// Taiwan
function drawTaiwan(error, cn) {
    // Mainland
    svg.select(".map")
        .append('g')
        .selectAll("path")
        .data(topojson.feature(cn, cn.objects.layer1).features.filter(function (d) { return d.properties.GU_A3 === 'TWN'; }))
        .enter()
        .append("path")
        .attr("d", path)
        .attr("id", function (d) { return d.id; })
        .attr("class", "province")
        .attr("fill", "#cccccc")
        .attr("fill", function (d) { return color(umap[d.properties.NAME]); })
        .attr("data-province", function (d) { '台湾' })
        .attr("data-center", function (d) {
            return projection([d.properties.longitude, d.properties.latitude]);
        });
    svg.select(".map")
        .append('g')
        .attr("class", "china")
        .selectAll("path")
        .data(topojson.feature(cn, cn.objects.layer1).features)
        .enter()
        .append("path")
        .attr("d", path)
        .attr("class", "china")
        .attr("fill", "none")
        .attr("stroke", "#dddddd")
        .attr("stroke-width", "1");
}

function drawShadow(error, cn) {
    var shadowDepth = 1;
    for (var i = 1; i <= shadowDepth; i++) {
        svg.append('g')
            .attr("class", "shadow")
            .attr("transform", "translate(" + i + "," + i + ")")
            .selectAll("path")
            .data(topojson.feature(cn, cn.objects.layer1).features)
            .enter()
            .append("path")
            .attr("d", path)
            .attr("class", "china")
            .attr("fill", "none")
            .attr("stroke", "#dddddd")
            .attr("stroke-width", "2");

    }

}


// HK and Macau
function drawHkMacau(error, cn) {


    var projection2 = d3.geo.mercator()
    .center([126, 17])
    .scale(2000);

    var path2 = d3.geo.path()
        .projection(projection2);

    svg.select('.map')
        .append("g")
        .attr("class", "hk")
        .attr("transform", "translate(50," + (height - 120) + ")")
        .selectAll("path")
        .data(topojson.feature(cn, cn.objects.layer1).features)
        .enter()
        .append("path")
        .attr("d", path2)
        .attr("id", function (d) { return d.id; })
        .attr("class", "province")
        .attr("fill", function (d) { return color(umap["Xianggang"]); })
        .attr("stroke", "black")
        .attr("stroke-width", "0.35");

    svg.select(".hk")
        .append("text") //add some text
        .attr("dx", function (d) { return 20 })
        .attr("dy", function (d) { return 35 })
        .attr("font-family", "sans-serif")
        .attr("fill", "#aaaaaa")
        .attr("font-size", 10)
        .text("Hong Kong & Macau")

    // add demarcation
    svg.select(".hk")
       .append("svg:line")
         .attr("x1", 30)
         .attr("y1", -10)
         .attr("x2", 150)
         .attr("y2", 20)
         .style("stroke", "#cccccc")
         .style("stroke-width", 3);

    svg.select(".hk")
        .append("svg:line")
         .attr("x1", 150)
         .attr("y1", 20)
         .attr("x2", 150)
         .attr("y2", 60)
         .style("stroke", "#cccccc")
         .style("stroke-width", 3);

}

// TODO : Haiwai

function wrap(text, width) {
    text.each(function () {
        var text = d3.select(this),
            words = text.text().split(/\s+/).reverse(),
            word,
            line = [],
            lineNumber = 0,
            lineHeight = 0.7, // ems
            y = text.attr("y"),
            dy = parseFloat(text.attr("dy")),
            tspan = text.text(null).append("tspan").attr("x", 0).attr("y", y).attr("dy", dy);
        while (word = words.pop()) {
            line.push(word);
            tspan.text(line.join(" "));
            if (tspan.node().getComputedTextLength() > width) {
                line.pop();
                tspan.text(line.join(" "));
                line = [word];
                tspan = text.append("tspan").attr("x", 0).attr("y", y).attr("dy", ++lineNumber * lineHeight + dy).text(word);
            }
        }
    });
}
