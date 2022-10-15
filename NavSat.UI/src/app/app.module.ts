
import { AppComponent } from './app.component';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
/* Routing */
import { AppRoutingModule } from './app-routing.module';

import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';

import { LogInComponent } from './components/log-in/log-in.component';
import { SatelliteComponent } from './components/satellites/satellites.component';
import { MapComponent } from './components/map/map.component';
import { SpystuffComponent } from './components/spystuff/spystuff.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

/* Angular material */
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AuthGuard } from './services/auth-guard.service';
import { ModalComponent } from './modal/modal.component';


@NgModule({
  declarations: [
    AppComponent,
    LogInComponent,
    SatelliteComponent,
    MapComponent,
    SpystuffComponent,
    ModalComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    NgbModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    FormsModule,
    FlexLayoutModule
  ],
  providers: [AuthGuard],
  bootstrap: [AppComponent],
/*  schemas: [CUSTOM_ELEMENTS_SCHEMA],*/

})
export class AppModule { }
