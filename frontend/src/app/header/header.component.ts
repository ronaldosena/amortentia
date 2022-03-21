import { Component, OnInit } from '@angular/core';
import { AuthService } from '../#services/auth.service';
import { AlertifyService } from '../#services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  model: any = {};
  photoUrl: string;

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit() {
    this.authService.currentPhotoUrl.subscribe(
      photo => (this.photoUrl = photo)
    );
  }

  login() {
    this.authService.login(this.model).subscribe(
      next => {
        this.alertify.success('Logged in successful');
      },
      error => {
        this.alertify.error(error);
      },
      () => {
        this.router.navigate(['/members']);
      }
    );
  }

  isLoggedIn(): boolean {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.authService.logout();
    this.router.navigate(['/home']);
    this.alertify.message('Logged Out');
  }
}
