﻿var chart;
function drawChart(e, name) {
    var options = {
        chart: {
            type: 'candlestick',
            height: 350
        },
        series: [],
        title: {
            text: name,
            align: 'center'
        },
        xaxis: {
            type: 'datetime'
        },
        yaxis: {
            tooltip: {
                enabled: true
            }
        },
        noData: {
            text: 'Loading...'
        },
        legend: {
            show: false
        }
    };

    chart = new ApexCharts(document.querySelector("#myChart"), options);
    chart.width = 700;
    chart.render();
};

function updateChart(data) {
    chart.appendSeries({
        data: [
            {
                x: data.time,
                y: [data.open, data.high, data.low, data.close]
            }
        ]
    }, false);
}

function updateOptions(data) {
    chart.updateOptions({
        noData: {
            text: data
        }
    });
}