import { Component, OnInit } from '@angular/core';
import { AuthService } from "./services/auth.service";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  constructor(
    private authService: AuthService
  ) { }

  public email?: string;
  public menu: MenuItem[] = [];

  ngOnInit() {
    this.authService.UpdateMenu.subscribe(res => {
      this.MenuDisplay();
    });
    this.MenuDisplay();
  }

  MenuDisplay() {

      if (this.authService.isLoggedIn()) {
        this.email = localStorage.getItem("email")!;
        this.menu = [{ name: 'Satellite', route: 'satellites', submenu: [{ name: 'All Satellites', route: 'satellites' }, { name: 'Visible Satellites', route: 'satellites/filter/1' }] }];
        this.menu.push({ name: 'Map', route: 'map' })
        if (this.authService.isSpy()) {
          this.menu.push({ name: 'Spy Stuff', route: 'spystuff' })
        }
        this.menu.push({ name: 'Logout - ' + this.email , route: 'login' });
      }
      else {
        this.menu = [{ name: 'Login', route: 'login' }];
      }
  }

  title = 'NavSat.UI';

}

interface MenuItem {
  name: string;
  route: string;
  submenu?: MenuItem[];
}
