import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SpystuffComponent } from './spystuff.component';

describe('SpystuffComponent', () => {
  let component: SpystuffComponent;
  let fixture: ComponentFixture<SpystuffComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SpystuffComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SpystuffComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
