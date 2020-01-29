import { Injectable } from '@angular/core';
import { User } from '../_models/user';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

/**
 * ANGULAR -- Resolve --
 * Interface that classes can implement to be a data provider.
 * A data provider class can be used with the router to resolve data during navigation.
 * The interface defines a resolve() method that will be invoked when the navigation starts.
 * The router will then wait for the data to be resolved before the route is finally activated.
 *
 * ANGULAR -- Injectable --
 * Decorator that marks a class as available to be provided and injected as a dependency.
 */
@Injectable()
export class ListsResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 5;
  likesParams = 'Likers';

  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    // tslint:disable-next-line: no-string-literal
    return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likesParams).pipe(
      catchError(error => {
        this.alertify.error('Problem  retrieving data');
        this.router.navigate(['/home']);
        return of(null);
      })
    );
  }
}
