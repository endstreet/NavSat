import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

@Component({
  selector: 'satellites',
  templateUrl: './satellites.component.html',
  styleUrls: ['./satellites.component.css']
})
export class SatelliteComponent {

  public featurecollections?: FeatureCollection[];
  public email?: string;

  constructor(http: HttpClient) {
    http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geolocations').subscribe(result => {
      this.featurecollections = result;
    }, error => console.error(error));
  }
  ngOnInit() {
    try {
      this.email = localStorage.getItem("email")!;
    } catch (error) { }
  }

  title = 'NavSat.UI';
}
interface Properties {
  Name: string;
  Prn: string;
  Constellation: string;
}

interface Geometry {
  type: string;
  coordinates: number[];
}

interface Feature {
  type: string;
  properties: Properties;
  geometry: Geometry;
}

interface FeatureCollection {
  type: string;
  features: Feature[];
}


