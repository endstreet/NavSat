import { Component } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'health-cell-renderer',
  template: `
<svg viewBox="0 0 2 2" xmlns="http://www.w3.org/2000/svg" style="max-height:20px;">
    <circle cx="1" cy="1" r="0.5" style="fill:{{this.cellValue}} " />
</svg>
`,
})
export class HealthCellRenderer implements ICellRendererAngularComp {
  
  public cellValue!: string;

  // gets called once before the renderer is used
  agInit(params: ICellRendererParams): void {
    this.cellValue = this.getValueToDisplay(params) ? "green" : "red";

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
}
