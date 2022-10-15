import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { Position } from 'google-map-react';
import { ModalComponent } from '../../modal/modal.component';
import { Feature, FeatureCollection } from '../../services/satellitedata';


@Component({
  selector: 'satellites',
  templateUrl: './satellites.component.html',
  styleUrls: ['./satellites.component.css']
})
export class SatelliteComponent {

  public featurecollections?: FeatureCollection[];
  public selectedfeature: Feature | undefined;

  visible: any;

  closeResult: string = '';
  lng!: string;
  lat!: string;
  alt!: string | undefined;

  constructor(private http: HttpClient, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) {


  }

  public ngOnInit(): void {
    this.visible = this.route.snapshot.paramMap.get('filter')!;
    if (this.visible) {
      this.getUserLocation();
      this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/GeoVisible/' + this.lng + '/' + this.lat + '/' + this.alt).subscribe(result => {
        this.featurecollections = result;
      }, error => console.error(error));
    }
    else {
      this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geolocations').subscribe(result => {
        this.featurecollections = result;
      }, error => console.error(error));
    }
  }

  getUserLocation = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(position => {
        this.lat = position.coords.latitude.toString();
        this.lng = position.coords.longitude.toString();
        this.alt = position.coords.altitude == null ? '3000': position.coords.altitude.toString();  
      });
                                             
    } else {
      // code for legacy browsers
    }
  };

  openModal(satellite:Feature) {
    const modalRef = this.modalService.open(ModalComponent,
      {
        scrollable: true,
        windowClass: 'myCustomModalClass',
        // keyboard: false,
        // backdrop: 'static'
      });

    let data =satellite

    modalRef.componentInstance.satellite = data;
    modalRef.result.then((result) => {
      console.log(result);
    }, (reason) => {
    });
  }
}

