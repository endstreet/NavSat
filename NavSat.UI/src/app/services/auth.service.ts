import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor()
  {
    localStorage.clear();
  }

  private _updatemenu = new Subject<void>();
  get UpdateMenu() {
    return this._updatemenu;
  }

  Login(data: string) {
    localStorage.setItem("email", data);
    this.UpdateMenu;
  }

  Logout() {
    localStorage.clear();
    this.UpdateMenu.next();
  }

  isLoggedIn() {
    return localStorage.getItem("email") != null;
  }

  isSpy() {

    if (!localStorage.getItem('email')) {
      return false
    } else {
      return (localStorage.getItem('email')!.indexOf("matogen") > 1)
    }
  }
}
