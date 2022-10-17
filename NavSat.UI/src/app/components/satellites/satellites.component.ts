import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalComponent } from '../../modal/modal.component';
import { Feature, FeatureCollection } from '../../services/satellitedata';
import { AgGridAngular } from 'ag-grid-angular';
import { CellClickedEvent, ColDef, GridReadyEvent } from 'ag-grid-community';
import { Observable } from 'rxjs';
import { HealthCellRenderer } from './components/healthindicator.component';
import { PopupCellRenderer } from './components/popupbutton.component';

@Component({
  selector: 'app-satellites',
  templateUrl: './satellites.component.html',
  styleUrls: ['./satellites.component.css']
})
export class SatelliteComponent {

  public satellites?: [];
  public selectedfeature: Feature | undefined;

  public visible: any;

  columnDefs: ColDef[] = [
    { field: 'features.0', headerName: 'Name', cellRenderer: PopupCellRenderer },
    { field: 'features.0.properties.Constellation', headerName: 'Constellation' },
    { field: 'features.0.properties.IsHealthy', headerName: 'Health', cellRenderer: HealthCellRenderer },
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
  //load data from sever
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
