<style></style>
<template>
    <div :id="mapName" style="height:100%;width:100%;" v-cloak></div>
</template>
<script>
    export default {
        props: {
            mapName: String,
            draggable: Boolean,
            fixed: Boolean,
            showLocation: Boolean,
            cx: Number,
            cy: Number,

            markerClickAction: Function,
            //items: Array,
        },
        watch: {
            'cx': function (newValue, oldValue) {
                const vm = this;
                //debugger
                if (vm.map.setCenter) {
                    //var lng = vm.centerPosition.lng();
                    //debugger
                    //vm.centerPosition.lat = newValue;
                    //vm.centerPosition = new google.maps.LatLng(newValue,lng);
                    //vm.map.setCenter(vm.centerPosition);
                    //vm.setMarker();
                }
            },
            'cy': function (newValue, oldValue) {
                const vm = this;
                //debugger
                if (vm.map.setCenter) {
                    //vm.centerPosition.lng = newValue;
                    //vm.map.setCenter(vm.centerPosition);
                    //vm.setMarker();
                }
            }
        },
        data() {
            return {
                navigator: {},
                centerPosition: { lat: 0, lng: 0 },// { lat: 13.942504351499613, lng: 120.72873957918004 },// { lat: 13.8954684059025, lng: 120.906667412659 }, //13.942504351499613, 120.72873957918004
                map: {},
                marker: {},
                infoWindow: {},
                geocode: {},
                directionsService: {},
                directionsRenderer: {},

                items: [],
                markers: [],
            };
        },
        created() {
            const vm = this;
        },

        async mounted() {
            const vm = this;

            vm.navigator = navigator;
            vm.navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia || navigator.msGetUserMedia || navigator.oGetUserMedia;

            var timerId = setInterval(_ => {
                if (google && google.maps) {

                    clearInterval(timerId);

                    //vm.initMap();

                    vm.$emit('onMapReady');
                }
            }, 250);
        },

        methods: {
            async initMap(items) {
                const vm = this;

                vm.items = items;
                //vm.centerPosition = {
                //    lat: cx, lng: cy
                //};

                /*
                Build list of map types.
                You can also use var mapTypeIds = ["roadmap", "satellite", "hybrid", "terrain", "OSM"]
                but static lists suck when Google updates the default list of map types.
                */
                var mapTypeIds = [];
                for (var type in google.maps.MapTypeId) {
                    mapTypeIds.push(google.maps.MapTypeId[type]);
                }
                //mapTypeIds.push("OSM");

                const lastZoom = Number.parseInt(localStorage.getItem('zoom')) || 15;

                vm.map = new google.maps.Map(document.getElementById(vm.mapName), {
                    center: vm.centerPosition,//{ lat: 13.948779, lng: 120.733035 }, //13.948779,120.733035
                    zoom: lastZoom,
                    //mapTypeId: "OSM",
                    mapTypeControl: true,
                    streetViewControl: false,
                    mapTypeControlOptions: {
                        mapTypeIds: mapTypeIds
                    }
                });

                vm.map.mapTypes.set("OSM", new google.maps.ImageMapType({
                    getTileUrl: function (coord, zoom) {
                        // "Wrap" x (longitude) at 180th meridian properly
                        // NB: Don't touch coord.x: because coord param is by reference, and changing its x property breaks something in Google's lib
                        var tilesPerGlobe = 1 << zoom;
                        var x = coord.x % tilesPerGlobe;
                        if (x < 0) {
                            x = tilesPerGlobe + x;
                        }
                        // Wrap y (latitude) in a like manner if you want to enable vertical infinite scrolling

                        return "https://tile.openstreetmap.org/" + zoom + "/" + x + "/" + coord.y + ".png";
                    },
                    tileSize: new google.maps.Size(256, 256),
                    name: "Open Street Map",
                    maxZoom: 18
                }));

                vm.infoWindow = new google.maps.InfoWindow;
                vm.geocoder = new google.maps.Geocoder;
                vm.directionsService = new google.maps.DirectionsService();
                vm.directionsRenderer = new google.maps.DirectionsRenderer({
                    suppressMarkers: true,
                    //draggable: true
                });

                vm.directionsRenderer.setMap(vm.map);

                if (vm.navigator.geolocation) {

                    if (vm.cx === 0 && vm.cy === 0) {
                        await vm.getCurrentLocation();
                    }
                    else {

                        vm.map.setCenter(vm.centerPosition);
                        vm.setMarker();
                    }
                    google.maps.event.addListener(vm.map, 'zoom_changed', function (arg) {
                        localStorage.setItem('zoom', this.zoom);
                    });

                } else {
                    // Browser doesn't support Geolocation
                    vm.handleLocationError(false, vm.infoWindow, vm.map.getCenter());
                    debugger;
                }

                vm.placeMarkers(vm.items, true);
            },

            handleLocationError(browserHasGeolocation, infoWindow, pos) {
                const vm = this;

                vm.infoWindow.setPosition(pos);
                vm.infoWindow.setContent(browserHasGeolocation ?
                    'Error: The Geolocation service failed.' :
                    'Error: Your browser doesn\'t support geolocation.');
                vm.infoWindow.open(map);
            },

            geocodeLatLng(event, marker) {
                const vm = this;

                vm.geocoder.geocode({ 'location': marker.position }, function (results, status) {
                    if (status === 'OK') {
                        vm.$emit(event, results[0], { lat: vm.centerPosition.lat, lng: vm.centerPosition.lng });
                    } else {
                        window.alert('Geocoder failed due to: ' + status);
                    }
                });
            },

            calculateAndDisplayRoute(position1, position2) {
                const vm = this;

                vm.directionsService.route(
                    {
                        origin: position1,
                        destination: position2,
                        travelMode: google.maps.TravelMode.DRIVING,
                    },
                    (response, status) => {

                        if (status === "OK") {
                            //var tripInfo = response.routes[0].legs[0];
                            //vm.$emit('onCalculatedTrip', tripInfo);

                            vm.directionsRenderer.setDirections(response);
                        } else {
                            window.alert("Directions request failed due to " + status);
                        }
                    }
                );
            },

            async getCurrentLocation() {
                const vm = this;

                await vm.navigator.geolocation.getCurrentPosition(function (position) {

                    //if (!vm.fixed) {
                    vm.centerPosition = {
                        lat: position.coords.latitude,
                        lng: position.coords.longitude
                    };

                    //}

                    vm.map.setCenter(vm.centerPosition);

                    if (vm.showLocation) {

                        vm.setMarker();
                        //vm.marker = new google.maps.Marker({
                        //    draggable: vm.draggable,
                        //    //animation: google.maps.Animation.BOUNCE,

                        //    position: vm.centerPosition,
                        //    map: vm.map,
                        //    //title: "Your Current Location",
                        //    //label: {
                        //    //    text: 'You',
                        //    //    //fontFamily: 'Fontawesome',
                        //    //},
                        //});

                        ////var latlng = new google.maps.LatLng(40.748774, -73.985763);
                        //vm.marker.setPosition(vm.centerPosition);

                        //google.maps.event.addListener(vm.marker, 'dragend', function (event) {
                        //    vm.centerPosition = this.getPosition();
                        //    vm.geocodeLatLng();
                        //});
                    }

                });

            },

            setMarker() {
                const vm = this;

                if (vm.marker && vm.marker.setMap)
                    vm.marker.setMap(null);

                var iconBase = 'http://maps.google.com/mapfiles/kml/pal3/';
                vm.marker = new google.maps.Marker({
                    draggable: vm.draggable,
                    //animation: google.maps.Animation.BOUNCE,

                    position: vm.centerPosition,
                    map: vm.map,
                    title: "Your Current Location",
                    icon: iconBase + '/icon56.png'
                    //label: {
                    //    text: 'You',
                    //    //fontFamily: 'Fontawesome',
                    //},
                });

                ////var latlng = new google.maps.LatLng(40.748774, -73.985763);
                //vm.marker.setPosition(vm.centerPosition);

                google.maps.event.addListener(vm.marker, 'dragend', function (event) {
                    vm.centerPosition = this.getPosition();
                    vm.geocodeLatLng('onAddress', vm.marker);
                });
            },

            placeMarkers(items, recenter) {
                //debugger;
                const vm = this;
                let markers = vm.markers;

                if (markers && markers.length > 0) {
                    markers.forEach(marker => marker.setMap(null));
                }

                if (items && items.length) {

                    markers = [];

                    var iconBase = 'http://maps.google.com/mapfiles/kml/pal3/';
                    items.forEach(item => {

                        var marker = new google.maps.Marker({
                            draggable: false,
                            //animation: google.maps.Animation.BOUNCE,

                            position: {
                                lat: item.geoX,
                                lng: item.geoY,
                            },
                            map: vm.map,
                            item: item,
                            title: `${item.name}`,
                            icon: iconBase + 'icon38.png'
                            //label: {
                            //    text: 'You',
                            //    //fontFamily: 'Fontawesome',
                            //},
                        });

                        google.maps.event.addListener(marker, 'click', function (event) {

                            if (vm.markerClickAction) {
                                vm.infoWindow = new google.maps.InfoWindow({
                                    //content: `${item.name}`
                                    content: `
<div class="container-fluid">
    <h5>
        <a href="customer/medicines/${item.pharmacyId}/">
            ${item.name}
        </a>
    </h5>
    <div class="form-group row">
        <label class="col-sm-auto col-form-label">Open Hours</label>
        <div class="col-sm">
            <div class="form-control-plaintext">
                ${item.openingHours}
            </div>
        </div>
    </div>
    <div class="form-row">
        <div class="form-group col">
            <label>Phone Number</label>
            <div class="form-control-plaintext">
                ${item.phoneNumber || ''}
            </div>
        </div>
        <div class="form-group col">
            <label>Mobile Number</label>
            <div class="form-control-plaintext">
                ${item.mobileNumber || ''}
            </div>
        </div>
        <div class="form-group col">
            <label>Email</label>
            <div class="form-control-plaintext">
                ${item.email || ''}
            </div>
        </div>
    </div>
    <div class="form-group">
        <label>Address</label>
        <div class="form-control-plaintext">
            ${item.address}
        </div>
    </div>
</div>
                                            `
                                });
                                vm.infoWindow.open(vm.initMap, marker);
                                //vm.markerClickAction(item);
                            }

                            vm.calculateAndDisplayRoute(vm.centerPosition, { lat: item.geoX, lng: item.geoY });
                        });

                        markers.push(marker);
                    });

                    vm.markers = markers;
                }


                //if (recenter) {
                vm.map.setCenter(vm.centerPosition);
                //}
            }
        }
    }
</script>