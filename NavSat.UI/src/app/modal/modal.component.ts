// modal.component.ts
import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Feature } from '../services/satellitedata';

@Component({
  selector: 'app-modal',
  templateUrl: './modal.component.html',
  styleUrls: ['./modal.component.css']
})
export class ModalComponent implements OnInit {

  @Input() satellite!: Feature ;

  constructor(
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit() {

  }

  closeModal() {
    this.activeModal.close();
  }

}
