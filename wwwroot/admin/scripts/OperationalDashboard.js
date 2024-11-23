$('.drp').daterangepicker({
    format: 'DD/MM/YYYY',
    startDate: '@startDate.ToString("dd/MM/yyyy")',
    endDate: '@endDate.ToString("dd/MM/yyyy")'
});

var chartData1 = @json1;
var chartData2 = @json2;
var chartData3 = @json3;
var chartData4 = @json4;

var chart = AmCharts.makeChart("chartdiv2", {
    "type": "stock",
    "theme": "light",
    "dataSets": [{
        "title": "LINE 350 MK",
        "fieldMappings": [{
            "fromField": "value",
            "toField": "value"
        }],
        "dataProvider": chartData1,
        "categoryField": "date"
    }, {
        "title": "LINE 620 MK",
        "fieldMappings": [{
            "fromField": "value",
            "toField": "value"
        }],
        "dataProvider": chartData2,
        "categoryField": "date"
    }, {
        "title": "LINE 350 TP",
        "fieldMappings": [{
            "fromField": "value",
            "toField": "value"
        }],
        "dataProvider": chartData3,
        "categoryField": "date"
    }, {
        "title": "PERTAGAS",
        "fieldMappings": [{
            "fromField": "value",
            "toField": "value"
        }],
        "dataProvider": chartData4,
        "categoryField": "date"
    }
    ],

    "panels": [{
        "showCategoryAxis": false,
        "title": "Value",
        "percentHeight": 70,
        "stockGraphs": [{
            "id": "g1",
            "valueField": "value",
            "comparable": true,
            "compareField": "value",
            "balloonText": "[[title]]:<b>[[value]]</b>",
            "compareGraphBalloonText": "[[title]]:<b>[[value]]</b>"
        }],
        "stockLegend": {
            "periodValueTextComparing": "[[percents.value.close]]%",
            "periodValueTextRegular": "[[value.close]]"
        }
    }],

    "chartScrollbarSettings": {
        "graph": "g1"
    },

    "chartCursorSettings": {
        "valueBalloonsEnabled": true,
        "fullWidth": true,
        "cursorAlpha": 0.1,
        "valueLineBalloonEnabled": true,
        "valueLineEnabled": true,
        "valueLineAlpha": 0.5
    },

    "periodSelector": {
        "position": "left",
        "periods": [{
            "period": "DD",
            "selected": true,
            "count": 1,
            "label": "1 day"
        }, {
            "period": "MM",
            "selected": true,
            "count": 1,
            "label": "1 month"
        }, {
            "period": "YYYY",
            "count": 1,
            "label": "1 year"
        }, {
            "period": "YTD",
            "label": "YTD"
        }, {
            "period": "MAX",
            "label": "MAX"
        }]
    },

    "dataSetSelector": {
        "position": "left"
    },

    "export": {
        "enabled": true
    }
});

var chart2 = AmCharts.makeChart("chartdiv", {
    "fontSize": "7",
    "theme": "light",
    "type": "gauge",
    "axes": [{
        "topText": @prosentase_pasokan + " %",
        "topTextFontSize": 20,
        "topTextYOffset": 70,
        "axisColor": "#31d6ea",
        "axisThickness": 1,
        "endValue": 100,
        "gridInside": true,
        "inside": true,
        "radius": "50%",
        "valueInterval": 10,
        "tickColor": "#67b7dc",
        "startAngle": -90,
        "endAngle": 90,
        "unit": "%",
        "bandOutlineAlpha": 0,
        "bands": [{
            "color": "#0080ff",
            "endValue": 100,
            "innerRadius": "105%",
            "radius": "170%",
            "gradientRatio": [0.5, 0, -0.5],
            "startValue": 0
        }, {
            "color": "#3cd3a3",
            "endValue": @prosentase_pasokan,
            "innerRadius": "105%",
            "radius": "170%",
            "gradientRatio": [0.5, 0, -0.5],
            "startValue": 0
        }]
    }],
    "arrows": [{
        "alpha": 1,
        "innerRadius": "35%",
        "nailRadius": 0,
        "radius": "170%",
        "value": @prosentase_pasokan
    }]
});

var plotData = [
    {
        label: 'LINE 350 MK',
        data: @realisasi_penjualan1.ToString("N0").Replace(".", ""),
        color: $.urbanApp.danger
    },
    {
        label: 'LINE 620 MK',
        data: @realisasi_penjualan2.ToString("N0").Replace(".", ""),
        color: $.urbanApp.info
    },
    {
        label: 'LINE 350 TP',
        data: @realisasi_penjualan3.ToString("N0").Replace(".", ""),
        color: $.urbanApp.warning
    },
    {
        label: 'PERTAGAS',
        data: @realisasi_penjualan4.ToString("N0").Replace(".", ""),
        color: $.urbanApp.success
    }
];
// Pie chart
$.plot($('.flot-pie'), plotData, {
    series: {
        pie: {
            show: true,
            innerRadius: 0.5,
            stroke: {
                width: 2
            },
            label: {
                show: true,
            }
        }
    },
    legend: {
        show: true
    },
});

var chart3 = AmCharts.makeChart("chartdiv3", {
    "theme": "light",
    "type": "serial",
    "titles": [{
        "text": "Trend ROB LNG Tank Inventory"
    }],
    "startDuration": 2,
    "dataProvider": @jsonROB,
    "valueAxes": [{
        "position": "left",
        "title": "ROB LNG Tank Inventory(M3)"
    }],
    "graphs": [{
        "balloonText": "[[category]]: <b>[[value]]</b>",
        "fillColors": "#FF0F00",
        "fillAlphas": 1,
        "lineAlpha": 0.1,
        "type": "column",
        "valueField": "value"
    }],
    "depth3D": 20,
    "angle": 30,
    "chartCursor": {
        "categoryBalloonEnabled": false,
        "cursorAlpha": 0,
        "zoomable": false
    },
    "categoryField": "date",
    "categoryAxis": {
        "gridPosition": "start",
        "labelRotation": 90
    },
    "export": {
        "enabled": true
    }
});

var chart4 = AmCharts.makeChart("chartdiv4", {
    "theme": "light",
    "titles": [{
        "text": "Trend Boil of Gas (BOG)"
    }],

    "type": "serial",
    "startDuration": 2,
    "dataProvider": @jsonBOG,
    "valueAxes": [{
        "position": "left",
        "title": "Nilai BOG (M3)"
    }],
    "graphs": [{
        "balloonText": "[[category]]: <b>[[value]]</b>",
        "fillColors": "#04D215",
        "fillAlphas": 1,
        "lineAlpha": 0.1,
        "type": "column",
        "valueField": "value"
    }],
    "depth3D": 20,
    "angle": 30,
    "chartCursor": {
        "categoryBalloonEnabled": false,
        "cursorAlpha": 0,
        "zoomable": false
    },
    "categoryField": "date",
    "categoryAxis": {
        "gridPosition": "start",
        "labelRotation": 90
    },
    "export": {
        "enabled": true
    }
});

var chart5 = AmCharts.makeChart("chartdiv5", {
    "theme": "light",
    "type": "serial",
    "titles": [{
        "text": "Trend Regas Sent Out Rate"
    }],

    "startDuration": 2,
    "dataProvider": @jsonRegasRate,
    "valueAxes": [{
        "position": "left",
        "title": "Regas Sent Out Rate (MMSCFD)"
    }],
    "graphs": [{
        "balloonText": "[[category]]: <b>[[value]]</b>",
        "fillColors": "#0D52D1",
        "fillAlphas": 1,
        "lineAlpha": 0.1,
        "type": "column",
        "valueField": "value",
    }],
    "depth3D": 20,
    "angle": 30,
    "chartCursor": {
        "categoryBalloonEnabled": false,
        "cursorAlpha": 0,
        "zoomable": false
    },
    "categoryField": "date",
    "categoryAxis": {
        "gridPosition": "start",
        "labelRotation": 90
    },
    "export": {
        "enabled": true
    }
});

function updateDashboards(tahun) {
    getDashboardData(tahun, function (result) {
        console.log(result);
        chart2.arrows[0].setValue(result.prosentase_pasokan);
        chart2.axes[0].setTopText(result.prosentase_pasokan + " %");
        chart2.axes[0].bands[1].setEndValue(result.prosentase_pasokan);

        $("#target_pasokan").html(result.target_pasokan);
        $("#realisasi_pasokan").html(result.realisasi_pasokan);
        $("#realisasi_penjualan").html(result.realisasi_penjualan);

        var plotData = [
            {
                label: 'LINE 350 MK',
                data: result.realisasi_penjualan1,
                color: $.urbanApp.danger
            },
            {
                label: 'LINE 620 MK',
                data: result.realisasi_penjualan2,
                color: $.urbanApp.info
            },
            {
                label: 'LINE 350 TP',
                data: result.realisasi_penjualan3,
                color: $.urbanApp.warning
            },
            {
                label: 'PERTAGAS',
                data: result.realisasi_penjualan4,
                color: $.urbanApp.success
            }
        ];
        // Pie chart
        $.plot($('.flot-pie'), plotData, {
            series: {
                pie: {
                    show: true,
                    innerRadius: 0.5,
                    stroke: {
                        width: 2
                    },
                    label: {
                        show: true,
                    }
                }
            },
            legend: {
                show: true
            },
        });
    });
}

function updateGraphs(rd) {
    dates = rd.split("-");
    start = dates[0].trim();
    end = dates[1].trim();
    sd = start.split("/");
    ed = end.split("/");
    startDate = sd[2] + "-" + sd[1] + "-" + sd[0];
    endDate = ed[2] + "-" + ed[1] + "-" + ed[0];
    getROBByPeriod(startDate, endDate, function (result) {
        chart3.dataProvider = result;
        chart3.validateData();
    });

    getBOGByPeriod(startDate, endDate, function (result) {
        chart4.dataProvider = result;
        chart4.validateData();
    });

    getRegasRateByPeriod(startDate, endDate, function (result) {
        chart5.dataProvider = result;
        chart5.validateData();
    });
}

function getROBByPeriod(startDate, endDate, handleData) {
    $.ajax({
        url: "getROBByPeriod",
        method: "GET",
        data: {
            "startDate": startDate,
            "endDate": endDate
        },
        dataType: "json",
        success: function (result) {
            handleData(result);
        }
    });
}

function getBOGByPeriod(startDate, endDate, handleData) {
    $.ajax({
        url: "getBOGByPeriod",
        method: "GET",
        data: {
            "startDate": startDate,
            "endDate": endDate
        },
        dataType: "json",
        success: function (result) {
            handleData(result);
        }
    });
}

function getRegasRateByPeriod(startDate, endDate, handleData) {
    $.ajax({
        url: "getRegasRateByPeriod",
        method: "GET",
        data: {
            "startDate": startDate,
            "endDate": endDate
        },
        dataType: "json",
        success: function (result) {
            handleData(result);
        }
    });
}

function getDashboardData(tahun, handleData) {
    $.ajax({
        url: "getDashboardData",
        method: "GET",
        data: {
            "tahun": tahun,
        },
        dataType: "json",
        success: function (result) {
            handleData(result);
        }
    });
}