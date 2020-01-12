import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import {
  FormGroup,
  FormControl,
  Validators,
  FormBuilder
} from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // Use in components with the @Output directive to emit custom events synchronously or asynchronously,
  // and register handlers for those events by subscribing to an instance.
  @Output() cancelRegister = new EventEmitter();
  // User data model
  user: User;
  registerForm: FormGroup;

  // Partial will make all the properties of the class optional.
  bsConfig: Partial<BsDatepickerConfig>;

  /**
   * Creates an instance of register component.
   * @param authService Authentication service instance.
   * @param alertify Alertify js service instance.
   * @param fb Angular reactive form builder.
   * @param router Angular router.
   */
  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private fb: FormBuilder,
    private router: Router
  ) {}

  /**
   * A callback method that is invoked immediately after the default change detector has checked
   * the directive's data-bound properties for the first time,
   * and before any of the view or content children have been checked.
   * It is invoked only once when the directive is instantiated.
   */
  ngOnInit() {
    // Ng Bootstrap datepicker control configuration
    this.bsConfig = {
      containerClass: 'theme-red'
    };

    this.createRegisterForm();
  }

  /**
   * Angular reactive form definition with attributes validation.
   */
  createRegisterForm() {
    this.registerForm = this.fb.group(
      {
        gender: ['male'],
        username: ['', Validators.required],
        knownAs: ['', Validators.required],
        dateOfBirth: [null, Validators.required],
        city: ['', Validators.required],
        country: ['', Validators.required],
        password: [
          '',
          [
            Validators.required,
            Validators.minLength(4),
            Validators.maxLength(5)
          ]
        ],
        confirmPassword: ['', Validators.required]
      },
      { validator: this.passwordMatchValidator }
    );
  }

  /**
   * Custom validation rule for the form's inputs
   * Passwords match validator
   * @param g Form group
   * @returns Null if passwords match or validation error object.
   */
  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value
      ? null
      : { mismatch: true };
  }

  /**
   * Register new user method.
   */
  register() {
    // Validates the form
    if (this.registerForm.valid) {
      // Clones forms data to the object and assigns this object to the user
      this.user = Object.assign({}, this.registerForm.value);

      // https://rxjs-dev.firebaseapp.com/guide/observable
      this.authService.register(this.user).subscribe(
        // success(next) callback(notification)
        () => {
          this.alertify.success('registration successful');
        },
        // error callback(notification)
        error => {
          this.alertify.error(error);
        },
        // complete callback(notification)
        // "Error" and "Complete" notifications may happen only once during the Observable Execution,
        // and there can only be either one!!! of them.
        () => {
          this.authService.login(this.user).subscribe(() => {
            this.router.navigate(['/members']);
          });
        }
      );
    }
  }

  cancel() {
    this.cancelRegister.emit(false);
  }
}
