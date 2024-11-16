$(function () {
    baseUrl = getBaseUrl();
    let urlNoc = `${baseUrl}/api/Noc`;
    let initialDateRange = $(".drp").val().split(" - ");
    let initialYear = initialDateRange[0].split("/")[2];
    getChart1(urlNoc, initialDateRange);
    getChart2(urlNoc, initialYear);
    getChart3(urlNoc, initialDateRange);
    getChart4(urlNoc, initialDateRange);
    getChart5(urlNoc, initialDateRange);
    getChart6(urlNoc, initialDateRange);

    $(".drp").on("change", function () {
        dateRange = $(this).val().split(" - ");
        year = dateRange[0].split("/")[2];
        getChart1(urlNoc, dateRange);
        getChart2(urlNoc, year);
        getChart3(urlNoc, dateRange);
        getChart4(urlNoc, dateRange);
        getChart5(urlNoc, dateRange);
        getChart6(urlNoc, dateRange);
    });
});

function getChart1(urlNoc, dateRange) {
    let startDate = dateRange[0];
    let endDate = dateRange[1];

    $.ajax({
        url: urlNoc + "/chart1?startDate=" + startDate + "&endDate=" + endDate,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            if (res.open == 0 && res.closed == 0 && res.overdue == 0) {
                var chart1 = AmCharts.makeChart("chart1", {
                    type: "pie",
                    theme: "light",
                    dataProvider: [
                        {
                            status: "Open",
                            jumlah: 0,
                            color: "#808080",
                        },
                    ],
                    valueField: "jumlah",
                    titleField: "status",
                    colorField: "color",
                    balloon: {
                        fixedPosition: true,
                    },
                    export: {
                        enabled: true,
                    },
                });

                // add label
                chart1.addLabel(0, "50%", "The chart contains no data", "center");
                // set opacity of the chart div
                chart1.chartDiv.style.opacity = 0.5;
            } else {
                AmCharts.makeChart("chart1", {
                    type: "pie",
                    theme: "light",
                    dataProvider: [
                        {
                            status: "Open",
                            jumlah: res.open,
                            color: "#ffff00",
                        },
                        {
                            status: "Closed",
                            jumlah: res.close,
                            color: "#00ff00",
                        },
                        {
                            status: "Overdue",
                            jumlah: res.overdue,
                            color: "#ff0000",
                        },
                    ],
                    valueField: "jumlah",
                    titleField: "status",
                    colorField: "color",
                    balloon: {
                        fixedPosition: true,
                    },
                    export: {
                        enabled: true,
                    },
                });
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart1").empty();
            $("#chart1").text("Data Empty");
        },
    });
}

function getChart2(urlNoc, year) {
    $.ajax({
        url: urlNoc + "/chart2?year=" + year,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            $("#chart2_title").text("Rekap NOC Tahun " + year);
            AmCharts.makeChart("chart2", {
                type: "serial",
                theme: "none",
                legend: {
                    horizontalGap: 10,
                    maxColumns: 1,
                    position: "right",
                    useGraphSettings: true,
                    markerSize: 3,
                },
                dataProvider: [
                    res["january"],
                    res["february"],
                    res["march"],
                    res["april"],
                    res["may"],
                    res["june"],
                    res["july"],
                    res["september"],
                    res["october"],
                    res["november"],
                    res["december"],
                ],
                valueAxes: [
                    {
                        stackType: "regular",
                        axisAlpha: 0.3,
                        gridAlpha: 0,
                    },
                ],
                graphs: [
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Near Miss",
                        type: "column",
                        color: "#000000",
                        valueField: "nearMiss",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Unsafe Action",
                        type: "column",
                        color: "#000000",
                        valueField: "unsafeAction",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Unsafe Condition",
                        type: "column",
                        color: "#000000",
                        valueField: "unsafeCondition",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Safe Action",
                        type: "column",
                        color: "#000000",
                        valueField: "safeAction",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Safe Condition",
                        type: "column",
                        color: "#000000",
                        valueField: "safeCondition",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "First Aid Case",
                        type: "column",
                        color: "#000000",
                        valueField: "firstAidCase",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Medical Treatement Case",
                        type: "column",
                        color: "#000000",
                        valueField: "medicalTreatementCase",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Restricted Work Case",
                        type: "column",
                        color: "#000000",
                        valueField: "restrictedWorkCase",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Lost Time Injury Case",
                        type: "column",
                        color: "#000000",
                        valueField: "lostTimeInjuryCase",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Fatality Case",
                        type: "column",
                        color: "#000000",
                        valueField: "fatalityCase",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Oil Spill",
                        type: "column",
                        color: "#000000",
                        valueField: "oilSpill",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Property Damage",
                        type: "column",
                        color: "#000000",
                        valueField: "propertyDamage",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Fire",
                        type: "column",
                        color: "#000000",
                        valueField: "fire",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "High Potential",
                        type: "column",
                        color: "#000000",
                        valueField: "highPotential",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Security",
                        type: "column",
                        color: "#000000",
                        valueField: "security",
                    },
                    {
                        balloonText:
                            "<b>[[title]]</b><br><span style='font-size:14px'>[[category]]: <b>[[value]]</b></span>",
                        fillAlphas: 0.8,
                        labelText: "[[value]]",
                        lineAlpha: 0.3,
                        title: "Document Validity",
                        type: "column",
                        color: "#000000",
                        valueField: "documentValidity",
                    }
                ],
                categoryField: "monthName",
                categoryAxis: {
                    gridPosition: "start",
                    axisAlpha: 0,
                    gridAlpha: 0,
                    position: "left",
                },
                export: {
                    enabled: true,
                },
            });
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart2").empty();
            $("#chart2").text("Data Empty");
        },
    });
}

function getChart3(urlNoc, dateRange) {
    let startDate = dateRange[0];
    let endDate = dateRange[1];

    $.ajax({
        url: urlNoc + "/chart3?startDate=" + startDate + "&endDate=" + endDate,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            AmCharts.makeChart("chart3", {
                type: "serial",
                theme: "light",
                categoryField: "departemen",
                rotate: true,
                startDuration: 1,
                categoryAxis: {
                    gridPosition: "start",
                    position: "left",
                },
                trendLines: [],
                graphs: [
                    {
                        balloonText: "Open:[[value]]",
                        fillAlphas: 0.8,
                        id: "AmGraph-1",
                        lineAlpha: 0.2,
                        title: "Open",
                        type: "column",
                        valueField: "open",
                        fillColors: "red",
                    },
                    {
                        balloonText: "Closed:[[value]]",
                        fillAlphas: 0.8,
                        id: "AmGraph-2",
                        lineAlpha: 0.2,
                        title: "Closed",
                        type: "column",
                        valueField: "closed",
                        fillColors: "green",
                    },
                ],
                valueAxes: [
                    {
                        id: "ValueAxis-1",
                        position: "top",
                        axisAlpha: 0,
                    },
                ],
                dataProvider: res,
                export: {
                    enabled: true,
                },
            });
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart3").empty();
            $("#chart3").text("Data Empty");
        },
    });
}

function getChart4(urlNoc, dateRange) {
    let startDate = dateRange[0];
    let endDate = dateRange[1];

    $.ajax({
        url: urlNoc + "/chart4?startDate=" + startDate + "&endDate=" + endDate,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            if (res.length > 0) {
                AmCharts.makeChart("chart4", {
                    type: "serial",
                    theme: "light",
                    categoryField: "name",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "Open",
                            type: "column",
                            valueField: "open",
                            fillColors: "red",
                        },
                        {
                            balloonText: "Closed:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-2",
                            lineAlpha: 0.2,
                            title: "Closed",
                            type: "column",
                            valueField: "closed",
                            fillColors: "green",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });
            } else {
                var chart4 = AmCharts.makeChart("chart4", {
                    type: "serial",
                    theme: "light",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "empty",
                            type: "column",
                            valueField: 100,
                            fillColors: "gray",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });

                // add label
                chart4.addLabel(0, "50%", "The chart contains no data", "center");
                // set opacity of the chart div
                chart4.chartDiv.style.opacity = 0.5;
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart4").empty();
            $("#chart4").text("Data Empty");
        },
    });
}

function getChart5(urlNoc, dateRange) {
    let startDate = dateRange[0];
    let endDate = dateRange[1];

    $.ajax({
        url: urlNoc + "/chart5?startDate=" + startDate + "&endDate=" + endDate,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            if (res.length > 0) {
                AmCharts.makeChart("chart5", {
                    type: "serial",
                    theme: "light",
                    categoryField: "unsafeAction",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "Open",
                            type: "column",
                            valueField: "open",
                            fillColors: "red",
                        },
                        {
                            balloonText: "Closed:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-2",
                            lineAlpha: 0.2,
                            title: "Closed",
                            type: "column",
                            valueField: "closed",
                            fillColors: "green",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });
            } else {
                var chart5 = AmCharts.makeChart("chart5", {
                    type: "serial",
                    theme: "light",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "empty",
                            type: "column",
                            valueField: 100,
                            fillColors: "gray",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });

                // add label
                chart5.addLabel(0, "50%", "The chart contains no data", "center");
                // set opacity of the chart div
                chart5.chartDiv.style.opacity = 0.5;
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart5").empty();
            $("#chart5").text("Data Empty");
        },
    });
}

function getChart6(urlNoc, dateRange) {
    let startDate = dateRange[0];
    let endDate = dateRange[1];

    $.ajax({
        url: urlNoc + "/chart6?startDate=" + startDate + "&endDate=" + endDate,
        contentType: "application/json",
        type: "GET",
        dataType: "json",
        success: function (res) {
            if (res.length > 0) {
                AmCharts.makeChart("chart6", {
                    type: "serial",
                    theme: "light",
                    categoryField: "unsafeCondition",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "Open",
                            type: "column",
                            valueField: "open",
                            fillColors: "red",
                        },
                        {
                            balloonText: "Closed:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-2",
                            lineAlpha: 0.2,
                            title: "Closed",
                            type: "column",
                            valueField: "closed",
                            fillColors: "green",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });
            } else {
                var chart6 = AmCharts.makeChart("chart6", {
                    type: "serial",
                    theme: "light",
                    rotate: true,
                    startDuration: 1,
                    categoryAxis: {
                        gridPosition: "start",
                        position: "left",
                    },
                    trendLines: [],
                    graphs: [
                        {
                            balloonText: "Open:[[value]]",
                            fillAlphas: 0.8,
                            id: "AmGraph-1",
                            lineAlpha: 0.2,
                            title: "empty",
                            type: "column",
                            valueField: 100,
                            fillColors: "gray",
                        },
                    ],
                    valueAxes: [
                        {
                            id: "ValueAxis-1",
                            position: "top",
                            axisAlpha: 0,
                        },
                    ],
                    dataProvider: res,
                    export: {
                        enabled: true,
                    },
                });

                // add label
                chart6.addLabel(0, "50%", "The chart contains no data", "center");
                // set opacity of the chart div
                chart6.chartDiv.style.opacity = 0.5;
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            $("#chart6").empty();
            $("#chart6").text("Data Empty");
        },
    });
}