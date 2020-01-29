import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  baseUrl = environment.apiUrl;

  /**
   * Helper to handle JWT security token.
   */
  jwtHelper = new JwtHelperService();

  decodedToken: any;
  currentUser: User;

  // TODO : Read about BehaviorSubject in RxJs
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) {}

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

  login(model: any) {
    return this.http.post(this.baseUrl + 'auth/login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          // LOCAL STORAGE: Store security token and user information in local storage
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));

          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  /**
   * POST method to create new user
   * @param user User model
   * @returns  void method
   */
  register(user: User) {
    return this.http.post(this.baseUrl + 'auth/register', user);
  }

  /**
   * Checks if security token expired
   * @returns  is token expired boolean value
   */
  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
