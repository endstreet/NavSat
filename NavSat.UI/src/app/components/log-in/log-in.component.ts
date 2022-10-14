import { Component, OnInit } from '@angular/core';
import { AuthService } from "../../services/auth.service";
import { Router} from '@angular/router';

@Component({
  selector: 'app-log-in',
  templateUrl:'./log-in.component.html',
  styleUrls: ['./log-in.component.css']
})
export class LogInComponent implements OnInit {

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }
  email = ""
  password = ""
  ngOnInit(): void {
    this.authService.Logout();
    this.authService.UpdateMenu.next();
  }

  onSubmit() {
    this.authService.Login(this.email)
    this.authService.UpdateMenu.next();
    this.router.navigate(['satellites']);
  }
}
