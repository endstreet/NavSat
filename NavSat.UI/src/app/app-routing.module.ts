import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './services/auth-guard.service';
import { LogInComponent } from './components/log-in/log-in.component';
import { SatelliteComponent } from './components/satellites/satellites.component';
import { MapComponent } from './components/map/map.component';
import { SpystuffComponent } from './components/spystuff/spystuff.component';

const routes: Routes = [
/*  { path: '', pathMatch: 'full', redirectTo: 'login' },*/
  { path: 'login', component: LogInComponent },
  { path: 'satellites', component: SatelliteComponent, canActivate: [AuthGuard]  },
  { path: 'map', component: MapComponent, canActivate: [AuthGuard]  },
  { path: 'spystuff', component: SpystuffComponent, canActivate: [AuthGuard]  },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  providers: [AuthGuard]
})
export class AppRoutingModule { }

