var map;
var elevationchart;
var pointMarker = null;
var viewmodel = {};
var pointDistances = [];

$(document).ready(function () {
    var routeId = $("input[name='RouteId']").val();

    $("#btnSave").click(function () {
        var data = { routeId: viewmodel.Id(), name: viewmodel.Name(), markers: [] };
        
        $.each(viewmodel.markers(), function (key, val) { 
            data.markers.push( { pointId: val.id, pointType: val.type() });
        });

        $("#downloadmodelinput").val(JSON.stringify(data));        
    });

    $.getJSON("/Route/TrackPoints?routeId=" + routeId, function (data) {
        viewmodel = ko.mapping.fromJS(data);
        viewmodel.bounds = ko.computed(function () {
            return new google.maps.LatLngBounds(
                                    new google.maps.LatLng(this.MinLat(), this.MinLong()),
                                    new google.maps.LatLng(this.MaxLat(), this.MaxLong())
                                );
        }, viewmodel);

        viewmodel.center = ko.computed(function () {
            return new google.maps.LatLng((this.MaxLat() + this.MinLat()) / 2, (this.MaxLong() + this.MinLong()) / 2);
        }, viewmodel);

        viewmodel.markers = ko.observableArray();

        viewmodel.optionValues = ["Valley", "Peak"];

        viewmodel.addMarker = function (marker) {
            marker.type.subscribe(function (newType) {
                marker.mapMarker.setIcon(newType == "Peak" ? "/Content/images/markers/bike_downhill.png" : "/Content/images/markers/bike_rising.png")
            });
            viewmodel.markers.push(marker);
            viewmodel.markers.sort(function (left, right) { return left.x - right.x; });
        };

        viewmodel.removeMarker = function () {
            this.mapMarker.setMap(null);
            elevationchart.xAxis[0].removePlotLine('plot-line' + this.x);
            viewmodel.markers.remove(this);
        };

        ko.applyBindings(viewmodel);

        initialize_map();
        initialize_elevationchart();

    });
});

function initialize_map() {
    var myOptions = {
        center: viewmodel.center(),
        zoom: 8,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    map = new google.maps.Map(document.getElementById("map_canvas"),
            myOptions);

    map.fitBounds(viewmodel.bounds());

    var trackCoordinates = [];

    $.each(viewmodel.Points(), function (key, val) {
        trackCoordinates.push(new google.maps.LatLng(val.Lat(), val.Long()));
    });

    var trackPath = new google.maps.Polyline({
        path: trackCoordinates,
        strokeColor: "#FF0000",
        strokeOpacity: 1.0,
        strokeWeight: 2
    });

    trackPath.setMap(map);
}

function initialize_elevationchart() {
    var heightdata = [];

    $.each(viewmodel.Points(), function (key, val) {
        heightdata.push([val.Dist(), val.Ele()]);
        pointDistances.push(val.Dist());
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
        tooltip: {
            formatter: function () {
                return '<b>Distance:</b> ' + (this.points[0].x / 1000).toFixed(2) +
                    'km<br/><b>Elevation:</b> ' + this.points[0].y + 'm';
            },
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
            data: heightdata
        }]
    });
}

function setPointMarker(dist) {
    if (dist === null && pointMarker != null) {
        pointMarker.setVisible(false);
    }
    else {
        var point = getPoint(dist);
        var pos = new google.maps.LatLng(point.Lat(), point.Long());
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
    var point = getPoint(dist);

    tmpMarker = new google.maps.Marker({
        position: new google.maps.LatLng(point.Lat(), point.Long()),
        map: map,
        animation: google.maps.Animation.DROP,
        icon: '/Content/images/markers/bike_downhill.png'
    });

    elevationchart.xAxis[0].addPlotLine({
        value: dist,
        color: 'red',
        width: 1,
        id: 'plot-line' + dist
    });

    var marker = {
        mapMarker: tmpMarker,
        id: point.Id(),
        x: dist,
        distance: (dist / 1000).toFixed(2),
        elevation: point.Ele(),
        type: ko.observable("Peak")
    };

    viewmodel.addMarker(marker);
}

function getPoint(dist){
    var x = $.inArray(dist, pointDistances);
    return viewmodel.Points()[x];
}