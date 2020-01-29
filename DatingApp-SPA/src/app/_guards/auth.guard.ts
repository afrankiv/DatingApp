import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

/**
 * ANGULAR: NAVIGATION [CanActivate]
 *
 * Interface that a class can implement to be a guard deciding if a route can be activated.
 * If all guards return true, navigation will continue. If any guard returns false,
 * navigation will be cancelled. If any guard returns a UrlTree, current navigation will
 * be cancelled and a new navigation will be kicked off to the UrlTree returned from the guard.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  canActivate(): boolean {
    if (this.authService.loggedIn()) {
      return true;
    }
    this.alertify.error('You shall not pass!');
    this.router.navigate(['/home']);

    return false;
  }
}
