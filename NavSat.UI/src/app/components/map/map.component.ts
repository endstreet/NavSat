import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Loader } from '@googlemaps/js-api-loader';
import { Feature, FeatureCollection } from '../../services/satellitepathdata';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalComponent } from '../../modal/modal.component';
import { styles } from './mapstyles';
import { colors } from './colors';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {


  title = 'google-maps';

  private map!: google.maps.Map
  private position!: GeolocationPosition
  private satellites!: FeatureCollection[]
  private path!: google.maps.LatLng[];

  private flightPath!: google.maps.Polyline;

  constructor(private http: HttpClient, private modalService: NgbModal) {
  }

  ngOnInit(): void {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((location) => {
        this.position = location;
        this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geovisiblefromat/' + this.position.coords.longitude.toString() + '/' + this.position.coords.latitude.toString() + '/' + (this.position.coords.altitude == null ? '3000' : this.position.coords.altitude.toString()) + '?FromDate=10-01-2022%2001%3A00&ToDate=10-15-2022%2001%3A00').subscribe(result => {
          this.satellites = result;
          let loader = new Loader({
            apiKey: 'AIzaSyDAmDOAJBNxAt_sfi_OOBE4c25AUio1GHQ',
          });

          loader.load().then(() => {
            this.map = new google.maps.Map(document.getElementById("gmap")!, {
              center: { lat: this.position.coords.latitude, lng: this.position.coords.longitude },
              zoom: 3,
              styles: styles
            });
            const image = "../../../assets/satellite.png";
            new google.maps.Marker({
              position: { lat: this.position.coords.latitude, lng: this.position.coords.longitude },
              map: this.map,
            });
            var colorIndex: number =0;
            this.satellites.forEach(collection => {
              collection.features.forEach(satelite => {
                this.path = [];

                //console.log(satelite.properties.Name);
                if (satelite.geometry.type == "LineString") {
                  var g = Object.values(satelite.geometry.coordinates)[0];
                  const marker =new google.maps.Marker({
                    position: { lat: g[0], lng: g[1] },
                    map: this.map,
                    title: satelite.properties.Name,
                    icon: image,
                  });
                  // Add a click listener for each marker, and set up the info window.
                  marker.addListener('click', () => {
                    this.openModal(satelite);
                  });
                  Object.values(satelite.geometry.coordinates).forEach(co => this.path.push(new google.maps.LatLng({ lat: co[0], lng: co[1] })));
                  this.flightPath = new google.maps.Polyline({
                    path: this.path,
                    geodesic: true,
                    strokeColor: colors[colorIndex++][1].toString(),
                    strokeOpacity: 1.0,
                    strokeWeight: 2,
                  });
                  this.flightPath.setMap(this.map);
                }
              });
            });
          })
        }, error => console.error(error));
      });
    }
    else {
      //todo: default position
    }
    
  }

  openModal(satellite: Feature) {
    const modalRef = this.modalService.open(ModalComponent,
      {
        scrollable: true,
        windowClass: 'myCustomModalClass',
        // keyboard: false,
        // backdrop: 'static'
      });

    let data = satellite

    modalRef.componentInstance.satellite = data;
    modalRef.result.then((result) => {
      console.log(result);
    }, (reason) => {
    });
  }
}





