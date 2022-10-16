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

  public visible: any;

  closeResult: string = '';


  constructor(private http: HttpClient, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) {


  }

  public ngOnInit(): void {
    this.visible = this.route.snapshot.paramMap.get('filter')!;
    if (this.visible) {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((location) => {
          console.info(location);
            this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geovisible/' + location.coords.longitude.toString() + '/' + location.coords.latitude.toString() + '/' + (location.coords.altitude == null ? '3000' : location.coords.altitude.toString())).subscribe(result => {
            this.featurecollections = result;
          }, error => console.error(error));
        });
      } else {
        this.router.navigate(['satellites']);
      }

    }
    else {
      this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geolocations').subscribe(result => {
        this.featurecollections = result;
      }, error => console.error(error));
    }
  }


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

