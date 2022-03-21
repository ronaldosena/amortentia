import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from '../../../#models/user.model';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../../../#services/alertify.service';
import { NgForm } from '@angular/forms';
import { UserService } from 'src/app/#services/user.service';
import { AuthService } from 'src/app/#services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm')
  editForm: NgForm;
  user: User;
  photoUrl: string;

  // Preventing closing browser
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.data.subscribe(data => {
      this.user = data['user'];
    });
    this.authService.currentPhotoUrl.subscribe(photo => this.photoUrl = photo);
  }

  updateUser() {
    this.userService
      .updateUser(this.authService.decodedToken.unique_name, this.user)
      .subscribe(
        next => {
          this.alertify.success('Profile updated!');
          this.editForm.reset(this.user);
        },
        error => {
          this.alertify.error(error);
        }
      );
  }

  updateMainPhoto(photoUrl) {
    this.user.photoUrl = photoUrl;
  }
}
