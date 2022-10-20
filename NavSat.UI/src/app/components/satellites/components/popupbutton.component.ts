import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';
import { Feature } from '../../../services/satellitedata';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ModalComponent } from '../../../modal/modal.component';

@Component({
  selector: 'popup-cell-renderer',
  template: `
<button class="btn btn-sm btn-primary" (click)="openModal(cellValue)">{{cellValue.properties.Name}}</button>
`,
})
export class PopupCellRenderer implements ICellRendererAngularComp {
  
  public cellValue!: Feature;

  constructor( private modalService: NgbModal) {
  }
  // gets called once before the renderer is used
  agInit(params: ICellRendererParams): void {
    this.cellValue = this.getValueToDisplay(params);
  }

  // gets called whenever the user gets the cell to refresh
  refresh(params: ICellRendererParams) {
    // set value into cell again
    this.cellValue = this.getValueToDisplay(params);
    return true;
  }

  getValueToDisplay(params: ICellRendererParams) {
    return params.valueFormatted ? params.valueFormatted : params.value;
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
    modalRef.componentInstance.coordinates = Object.values(satellite.geometry.coordinates);
    modalRef.result.then((result) => {
      console.log(result);
    }, (reason) => {
    });
  }
}
