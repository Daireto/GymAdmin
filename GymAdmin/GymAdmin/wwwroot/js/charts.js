function PieChart(data) {
    Highcharts.setOptions({
        colors: ['#d58900', '#ff1900', '#3b3b3b']
    });

    Highcharts.chart('piecontainer', {
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            type: 'pie',
            backgroundColor: 'transparent',
            width: 420,
            height: 340,
            margin: 0,
            padding: 0,
        },
        title: {
            text: 'Plan más preferido por los usuarios',
            style: {
                color: "#f2c60d"
            },
        },
        tooltip: {
            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>',
        },
        accessibility: {
            point: {
                valueSuffix: '%'
            }
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    color: "#fff"
                }
            }
        },
        series: [{
            name: 'Procentaje de usuarios',
            colorByPoint: true,
            data: data
        }],
        exporting: {
            enabled: false
        }
    });
}

function BarChart(data) {
    Highcharts.setOptions({
        colors: ['#d58900', '#ff1900', '#3b3b3b']
    });

    Highcharts.chart('barcontainer', {
        chart: {
            type: 'column',
            backgroundColor: 'transparent',
            width: 420,
            height: 340,
        },
        title: {
            text: 'Número de contratos de cada servicio',
            style: {
                color: "#f2c60d"
            },
        },
        subtitle: {
            text: null
        },
        xAxis: {
            type: 'category',
            labels: {
                rotation: -45,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif',
                    color: "#fff"
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Cantidad de contratos',
                style: {
                    color: "#fff"
                },
            }
        },
        legend: {
            enabled: false,
        },
        tooltip: {
            pointFormat: 'Cantidad de contratos: <b>{point.y:.0f}</b>'
        },
        series: [{
            name: 'Cantidad de contratos',
            data: data,
            dataLabels: {
                enabled: true,
                rotation: -90,
                color: '#FFFFFF',
                align: 'right',
                format: '{point.y:.0f}',
                y: 10,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif',
                }
            }
        }],
        exporting: {
            enabled: false
        }
    });
}

function EventsBarChart(data) {
    Highcharts.setOptions({
        colors: ['#d58900', '#ff1900', '#3b3b3b']
    });

    Highcharts.chart('eventsbarcontainer', {
        chart: {
            type: 'column',
            backgroundColor: 'transparent',
            width: 420,
            height: 340,
        },
        title: {
            text: 'Eventos más preferidos por los usuarios',
            style: {
                color: "#f2c60d"
            },
        },
        subtitle: {
            text: null
        },
        xAxis: {
            type: 'category',
            labels: {
                rotation: -45,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif',
                    color: "#fff"
                }
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: 'Cantidad de usuarios',
                style: {
                    color: "#fff"
                },
            }
        },
        legend: {
            enabled: false
        },
        tooltip: {
            pointFormat: 'Cantidad de usuarios: <b>{point.y:.0f}</b>'
        },
        series: [{
            name: 'Cantidad de usuarios',
            data: data,
            dataLabels: {
                enabled: true,
                rotation: -90,
                color: '#FFFFFF',
                align: 'right',
                format: '{point.y:.0f}',
                y: 10,
                style: {
                    fontSize: '13px',
                    fontFamily: 'Verdana, sans-serif'
                }
            }
        }],
        exporting: {
            enabled: false
        }
    });
}