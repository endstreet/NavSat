import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalComponent } from '../../modal/modal.component';
import { Feature, FeatureCollection } from '../../services/satellitedata';
import { AgGridAngular } from 'ag-grid-angular';
import { CellClickedEvent, ColDef, GridReadyEvent } from 'ag-grid-community';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-satellites',
  templateUrl: './satellites.component.html',
  styleUrls: ['./satellites.component.css']
})
export class SatelliteComponent {

  public satellites?: [];
  public selectedfeature: Feature | undefined;

  public visible: any;
//{ field: 'athlete' },
//// Using dot notation to access nested property
//{ field: 'medals.gold', headerName: 'Gold' },
  columnDefs: ColDef[] = [
    { field: 'features.0.properties.Name',headerName: 'Name' },
    { field: 'features.0.properties.Constellation', headerName: 'Constellation' },
    { field: 'features.0.properties.IsHealthy', headerName: 'Health' },
    { field: 'features.0.geometry.coordinates.0', headerName: 'Latitude' },
    { field: 'features.0.geometry.coordinates.1', headerName: 'Longtitude' }
  ];

  // DefaultColDef sets props common to all Columns
  public defaultColDef: ColDef = {
    sortable: true,
    filter: true,
  };

  // Data that gets displayed in the grid
  public rowData$!: Observable<any[]>;
  // For accessing the Grid's API
  @ViewChild(AgGridAngular) agGrid!: AgGridAngular;

  constructor(private http: HttpClient, private route: ActivatedRoute,
    private router: Router, private modalService: NgbModal) {
  }
  // Example load data from sever
  onGridReady(params: GridReadyEvent) {
    this.visible = this.route.snapshot.paramMap.get('filter')!;
    if (this.visible) {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((location) => {
/*          console.info(location);*/
          this.rowData$ = this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geovisible/' + location.coords.longitude.toString() + '/' + location.coords.latitude.toString() + '/' + (location.coords.altitude == null ? '3000' : location.coords.altitude.toString()));
        });
      } else {
        this.router.navigate(['satellites']);
      }
    }
    else {
      this.rowData$ = this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geolocations');//.forEach(element => this.satellites.push(element));;
      //console.info(typeof(this.rowData$.toPromise()));
    }
  }

  // Example of consuming Grid Event
  onCellClicked(e: CellClickedEvent): void {
    console.log('cellClicked', e);
  }

  // Example using Grid's API
  clearSelection(): void {
    this.agGrid.api.deselectAll();
  }

  public ngOnInit(): void {
    //this.visible = this.route.snapshot.paramMap.get('filter')!;
    //if (this.visible) {
    //  if (navigator.geolocation) {
    //    navigator.geolocation.getCurrentPosition((location) => {
    //      console.info(location);
    //      this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geovisible/' + location.coords.longitude.toString() + '/' + location.coords.latitude.toString() + '/' + (location.coords.altitude == null ? '3000' : location.coords.altitude.toString())).subscribe(result => {
    //        this.rowData$ = result;
    //      }, error => console.error(error));
    //    });
    //  } else {
    //    this.router.navigate(['satellites']);
    //  }

    //}
    //else {
    //  this.http.get<FeatureCollection[]>('http://localhost:5091/api/satellitepath/geolocations').subscribe(result => {
    //    this.satellites = result;
    //  }, error => console.error(error));
    //}
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

    flattenObject = (obj:any, prefix = 'satellite') =>
    Object.keys(obj).reduce((acc:any, k) => {
      const pre = prefix.length ? `${prefix}.` : '';
      if (
        typeof obj[k] === 'object' &&
        obj[k] !== null &&
        Object.keys(obj[k]).length > 0
      )
      {
        Object.assign(acc, this.flattenObject(obj[k], pre + k));
      }
      else
      {
        acc [pre + k] = obj[k];
      }
      return acc;
    }, {});
}
