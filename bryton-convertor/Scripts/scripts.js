var map;
var elevationchart;
var pointMarker = null;
var points = [];
var pointDistances = [];
var markers = [];

$(document).ready(function () {
    var routeId = $("input[name='RouteId']").val();
    var maxLong, minLong, maxLat, minLat, distance;

    $.getJSON("/Route/TrackPoints?routeId=" + routeId, function (data) {
        $.each(data.Points, function (key, val) {
            points.push(val);
            pointDistances.push(val.Dist);
        });

        initialize_map(data.MaxLat, data.MinLat, data.MaxLong, data.MinLong);
        initialize_elevationchart(data.Distance);
    });
});

function initialize_map(maxLat, minLat, maxLong, minLong) {
    var myOptions = {
        center: new google.maps.LatLng((maxLat + minLat) / 2, (maxLong + minLong) / 2),
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"),
            myOptions);

    map.fitBounds(new google.maps.LatLngBounds(new google.maps.LatLng(minLat, minLong), new google.maps.LatLng(maxLat, maxLong)));

    var trackCoordinates = [];

    $.each(points, function (key, val) {
        trackCoordinates.push(new google.maps.LatLng(val.Lat, val.Long));
    });

    var trackPath = new google.maps.Polyline({
        path: trackCoordinates,
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 2
    });

    trackPath.setMap(map);
}

function initialize_elevationchart(distance) {
    var data = [];

    $.each(points, function (key, val) {
        data.push([val.Dist, val.Ele]);
    });

    elevationchart = new Highcharts.Chart({
        chart: {
            renderTo: 'chartcontainer',
            zoomType: 'x',
            spacingRight: 20
        },
        title: {
            text: 'Elevation'
        },
        subtitle: {
            text: document.ontouchstart === undefined ?
				'Click and drag in the plot area to zoom in' :
				'Drag your finger over the plot to zoom in'
        },
        xAxis: {
            type: 'linear',
            maxZoom: 100,
            title: {
                text: null
            }
        },
        yAxis: {
            title: {
                text: 'Elevation'
            },
            showFirstLabel: false
        },
        toolTip: {
            shared: true
        },
        legend: {
            enabled: false
        },
        plotOptions: {
            series: {
                point: {
                    events: {
                        mouseOver: function () {
                            setPointMarker(this.x);
                        },
                        click: function () {
                            addMarker(this.x);
                        }
                    }
                },
                events: {
                    mouseOut: function () {
                        setPointMarker(null);
                    }
                }
            },
            area: {
                fillColor: {
                    linearGradient: [0, 0, 0, 300],
                    stops: [
						[0, Highcharts.getOptions().colors[0]],
						[1, 'rgba(2,0,0,0)']
					]
                },
                lineWidth: 1,
                marker: {
                    enabled: false,
                    states: {
                        hover: {
                            enabled: true,
                            radius: 5
                        }
                    }
                },
                shadow: false,
                states: {
                    hover: {
                        lineWidth: 1
                    }
                }
            }
        },
        series: [{
            type: 'area',
            name: 'Ele',
            data: data
        }]
    });
}

function setPointMarker(dist) {
    if (dist === null && pointMarker != null) {
        pointMarker.setVisible(false);
    }
    else {
        var pos = getPointPosition(dist);
        if (pointMarker === null) {
            pointMarker = new google.maps.Marker({
                position: pos,
                map: map
            });
        }
        pointMarker.setPosition(pos);
        pointMarker.setVisible(true);
    }
}

function addMarker(dist) {
    var pos = getPointPosition(dist);

    pointMarker = new google.maps.Marker({
        position: pos,
        map: map
    });

    markers.push(pointMarker);

    elevationchart.xAxis[0].addPlotLine({
        value: dist,
        color: 'red',
        width: 1,
        id: 'plot-line' + dist
    });

}

function getPointPosition(dist) {
    var x = $.inArray(dist, pointDistances);
    var point = points[x];
    if (point === undefined)
        return;
    return new google.maps.LatLng(point.Lat, point.Long);
}