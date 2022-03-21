import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { AlertifyService } from '../../../#services/alertify.service';
import { AuthService } from '../../../#services/auth.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { ValidationManager } from 'ng2-validation-manager';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from 'src/app/#models/user.model';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output()
  cancelRegister = new EventEmitter();
  model: any = {};
  registerForm: FormGroup;
  form;
  radios = ['Witch', 'Wizard'];
  bsConfig: Partial<BsDatepickerConfig>;
  user: User;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  ngOnInit() {
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.form = new ValidationManager({
      gender: 'required',
      username: 'required|pattern:[A-Za-z0-9]+([_.][A-Za-z0-9]+)*',
      password: 'required|rangeLength:8,50',
      confirmPassword: 'required|equalTo:password',
      dateOfBirth: 'required',
      knownAs: 'required',
      school: 'required|pattern:[A-Za-z0-9]+([_.][A-Za-z0-9]+)*',
      house: 'required|pattern:[A-Za-z0-9]+([_.][A-Za-z0-9]+)*',
      country: 'required|pattern:[A-Za-z0-9]+([_.][A-Za-z0-9]+)*'
    });
    this.form.setValue({
      'gender': 'Witch'
    });
  }

  register() {
    if (this.form.isValid()) {
      this.user = Object.assign({}, this.form.getData());
      this.authService.register(this.user).subscribe(() => {
        this.alertify.success('Registration successful');
      }, error => {
        this.alertify.error(error);
      }, () => { // run on complete
        this.authService.login(this.user).subscribe(() => {
          this.router.navigate(['/members']);
        });
      });
    } else {
      this.alertify.success('Invalid registration form');
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
