(function ($) {
    'use strict';

    $(document).ready(function () {
        // Rickshaw charts
        var seriesData = [[], [], []];
        var random = new Rickshaw.Fixtures.RandomData(150);

        for (var i = 0; i < 150; i++) {
            random.addData(seriesData);
        }

        var graph1Element = document.getElementById('dashboard-rickshaw');
        if (graph1Element) {
            var graph1 = new Rickshaw.Graph({
                element: graph1Element,
                width: $('#dashboard-rickshaw').parent().width(),
                height: 250,
                padding: {
                    top: 2
                },
                interpolation: 'cardinal',
                renderer: 'area',
                series: [{
                    color: $.urbanApp.primary,
                    data: seriesData[0],
                    name: 'Download'
                }, {
                    color: $.urbanApp.bodyBg,
                    data: seriesData[1],
                    name: 'Upload'
                }]
            });
            graph1.render();
        } else {
            console.error('Element with ID "dashboard-rickshaw" not found');
        }

        var seriesData2 = [[], [], []];
        var random2 = new Rickshaw.Fixtures.RandomData(130);

        for (var v = 0; v < 130; v++) {
            random2.addData(seriesData2);
        }

        var graph2Element = document.getElementById('dashboard-rickshaw2');
        if (graph2Element) {
            var graph2 = new Rickshaw.Graph({
                element: graph2Element,
                width: $('#dashboard-rickshaw2').parent().width(),
                renderer: 'area',
                height: 133,
                padding: {
                    top: 2
                },
                interpolation: 'cardinal',
                stroke: false,
                preserve: true,
                hover: {
                    xFormatter: function (x) {
                        return new Date(x * 1000).toString();
                    },
                    yFormatter: function (y) {
                        return Math.round(y);
                    }
                },
                series: [{
                    color: $.urbanApp.success,
                    name: 'Earnings',
                    data: seriesData2[0]
                }]
            });
            graph2.render();
        } else {
            console.error('Element with ID "dashboard-rickshaw2" not found');
        }

        setInterval(function () {
            random.removeData(seriesData);
            random.addData(seriesData);
            if (graph1) {
                graph1.update();
            }
        }, 1000);

        $(window).resize(function () {
            if (graph1) {
                graph1.configure({
                    width: $('#dashboard-rickshaw').parent().width()
                });
                graph1.render();
            }

            if (graph2) {
                graph2.configure({
                    width: $('#dashboard-rickshaw2').parent().width()
                });
                graph2.render();
            }
        });

        var browserData = [
            {
                label: 'IE',
                data: 15,
                color: $.urbanApp.danger
            },
            {
                label: 'Safari',
                data: 14,
                color: $.urbanApp.info
            },
            {
                label: 'Chrome',
                data: 34,
                color: $.urbanApp.warning
            },
            {
                label: 'Opera',
                data: 13,
                color: $.urbanApp.success
            },
            {
                label: 'Firefox',
                data: 24,
                color: $.urbanApp.dark
            }
        ];

        // Pie chart
        $.plot($('.flot-pie'), browserData, {
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

        var visits = [
            [0, 8],
            [1, 1],
            [2, 1],
            [3, 6],
            [4, 4],
            [5, 3],
            [6, 9],
            [7, 7],
            [8, 1]
        ];

        var plotdata = [{
            data: visits,
            color: $.urbanApp.success
        }];

        // Line chart
        $.plot($('.flot-line'), plotdata, {
            series: {
                lines: {
                    show: true,
                    lineWidth: 1,
                    fill: true
                },
                shadowSize: 0
            },
            grid: {
                color: $.urbanApp.border,
                borderWidth: 0,
                hoverable: true,
            },
            xaxis: {
                min: 0,
                max: 8
            },
            yaxis: {
                min: 0,
                max: 10
            }
        });

        var barData = [
            {
                data: [['M', 80], ['T', 40], ['W', 20], ['Th', 20], ['F', 50]],
                bars: {
                    show: true,
                    barWidth: 0.6,
                    align: 'center',
                    fill: true,
                    lineWidth: 0,
                    fillColor: $.urbanApp.default
                }
            }
        ];

        $.plot($('.flot-bars'), barData, {
            grid: {
                hoverable: false,
                clickable: false,
                color: 'white',
                borderWidth: 0,
            },
            yaxis: {
                show: false
            },
            xaxis: {
                mode: 'categories',
                tickLength: 0,
                axisLabelUseCanvas: true,
                axisLabelFontSizePixels: 12,
                axisLabelFontFamily: 'Roboto',
                axisLabelPadding: 5
            }
        });
    });
})(jQuery);
