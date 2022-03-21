import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../#models/user.model';
import { PaginatedResult } from '../#models/pagination.model';
import { map } from 'rxjs/operators';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + localStorage.getItem('token')
  })
};

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.base_url;

  constructor(private http: HttpClient) {
    this.http.head
  }

  getUsers(page?, itemsPerPage?, userParams?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

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

    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') != null) {
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }
          return paginatedResult;
        })
      );
  }

  getUser(username): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + username, httpOptions);
  }

  updateUser(username: string, user: User) {
    return this.http.put(this.baseUrl + 'users/' + username, user);
  }

  setMainPhoto(username: string, photoId: string) {
    return this.http.post(this.baseUrl + 'users/' + username + '/photos/' + photoId + '/setMain', {});
  }

  deletePhoto(username: string, photoId: string) {
    return this.http.delete(this.baseUrl + 'users/' + username + '/photos/' + photoId);
  }
}
