import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { PaginatedResult } from '../_models/pagination';
import { map } from 'rxjs/operators';

/**
 * ANGULAR Service: frontend communication to backend REST Web API
 */
@Injectable({
  providedIn: 'root'
})
export class UserService {
  // Web API backend URL stored in global variable
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  /**
   * GET: Loads collection of users from the backend, but includes pagination information.
   * @returns users Observable collection from rxjs. Instead of promise.
   */
  public getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
    // TS: Usage of GENERICS for collection
    // TS: Usage of CONST to not modify the collection
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    // TS: Usage of LET for changeable variable
    // ANGULAR: HTTP query string parameters will be appended to HTTP GET method request
    let params = new HttpParams();

    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe( // pipe allows access to rxjs operators
        map(response => { // map is operator from rxjs. Similar to the well known Array.prototype.map function,
          // this operator applies a projection to each value and emits that projection in the output Observable

          // response body contains users collection from the backend
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            // response headers contains pagination info from the backend
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
            return paginatedResult;
          }
        })
      );
  }

  /**
   * GET Single user
   * @param id user identifier
   * @returns user Observable from rxjs. Instead of promise.
   */
  public getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  /**
   * PUT Updates selected user
   * @param id ser identifier
   * @param user model to update
   * @returns void method
   */
  public updateUser(id: number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  /**
   * POST Updates main photo for selected user
   * @param userId user identifier
   * @param id photo identifier
   * @returns void method
   */
  public setMainPhoto(userId: number, id: number) {
    return this.http.post(
      this.baseUrl + 'users/' + userId + '/photos/' + id + '/setMain',
      {}
    );
  }

  /**
   * DELETE Deletes photo for selected user
   * @param userId user identifier
   * @param id photo identifier
   * @returns void method
   */
  public deletePhoto(userId: number, id: number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }
}
