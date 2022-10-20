import { HttpClient } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Feature, FeatureCollection } from '../../services/satellitedata';
import { AgGridAngular } from 'ag-grid-angular';
import { CellClickedEvent, ColDef, GridReadyEvent, ValueGetterParams } from 'ag-grid-community';
import { Observable, of } from 'rxjs';
import { HealthCellRenderer } from './components/healthindicator.component';
import { PopupCellRenderer } from './components/popupbutton.component';
import { ApiBaseUrl } from './../../app.config';

@Component({
  selector: 'app-satellites',
  templateUrl: './satellites.component.html',
  styleUrls: ['./satellites.component.css']
})
export class SatelliteComponent {


  public visible: any;

  // Data that gets displayed in the grid
  //public rowData$: Observable<Feature[]> = of(this.satellites);;
  public rowData$!: Feature[];
  // For accessing the Grid's API
  @ViewChild(AgGridAngular) agGrid!: AgGridAngular;

  constructor(private http: HttpClient, private route: ActivatedRoute,
    private router: Router) { }

  columnDefs: ColDef[] = [
    { valueGetter: this.rowValueGetter,  headerName: 'Name', cellRenderer: PopupCellRenderer },
    { field: 'properties.Constellation', headerName: 'Constellation' },
    { field: 'properties.IsHealthy', headerName: 'Health', cellRenderer: HealthCellRenderer },
    { field: 'geometry.coordinates.0', headerName: 'Latitude' },
    { field: 'geometry.coordinates.1', headerName: 'Longtitude' }

  ];
  rowValueGetter(params: ValueGetterParams) {
    return params.data;
  }
  // DefaultColDef sets props common to all Columns
  public defaultColDef: ColDef = {
    sortable: true,
    filter: true,
  };

  onGridReady(params: GridReadyEvent) {
    this.visible = this.route.snapshot.paramMap.get('filter')!;
    if (this.visible) {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((location) => {
          this.http.get<FeatureCollection>(ApiBaseUrl + '/geovisiblefrom/' + location.coords.longitude.toString() + '/' + location.coords.latitude.toString() + '/' + (location.coords.altitude == null ? '3000' : location.coords.altitude.toString())).subscribe(result => {
            this.rowData$ = result.features;
            });
        });
      } else {
        this.router.navigate(['satellites']);
      }
    }
    else {
      this.http.get<FeatureCollection>(ApiBaseUrl + '/geolocations').subscribe(result => {
        this.rowData$ = result.features;
      });
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

}
