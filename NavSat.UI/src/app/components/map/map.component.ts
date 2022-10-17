import { Component, OnInit } from '@angular/core';
import { Loader } from '@googlemaps/js-api-loader';
import { styles } from './mapstyles';


@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements OnInit {


  title = 'google-maps';

  private map!: google.maps.Map
  private position!: GeolocationPosition

  ngOnInit(): void {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition((location) => {
        this.position = location;
      });
    }
    else {
      //todo: default position
    }
    let loader = new Loader({
    apiKey: 'AIzaSyDAmDOAJBNxAt_sfi_OOBE4c25AUio1GHQ',
    });
    loader.load().then(() => {
        this.map = new google.maps.Map(document.getElementById("gmap")!, {
          center: { lat: this.position.coords.latitude, lng: this.position.coords.longitude },
          zoom: 6,
          styles: styles
        })
        const marker = new google.maps.Marker({
          position: { lat: this.position.coords.latitude, lng: this.position.coords.longitude },
          map: this.map,
        });
      })
  }
}




