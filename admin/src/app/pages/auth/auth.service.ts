import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor,HttpClient } from '@angular/common/http';
import { environment } from './../../../environments/environment';

import { Observable, of } from 'rxjs';
import { tap, delay,map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  constructor(private http: HttpClient) {}
  isLoggedIn = (localStorage.getItem('access_token'))?true:false; 
  
  // store the URL so we can redirect after logging in
 
  redirectUrl: string;

  login(email: string, password: string){
     return this.http.post<any>(`${environment.BASE_URL}admin/login`, { email, password })
      .pipe(map(fetchresult => {
      //console.log(" user",fetchresult)
         
          if (fetchresult && fetchresult.result.access_token) {
              localStorage.setItem('access_token', fetchresult.result.access_token);
              localStorage.setItem('name', fetchresult.result.name);
              localStorage.setItem('email', fetchresult.result.email);
              this.isLoggedIn = true
              return fetchresult.result;
          }
          return fetchresult;
     }));
  }


  logout(): Observable<boolean> {
    return of(true).pipe(
      delay(1000),
      tap(val =>{
         localStorage.removeItem('access_token');
         localStorage.removeItem('name');
         localStorage.removeItem('email');
         this.isLoggedIn = false;
         })
    );
  }

  getToken() {

        return localStorage.getItem('access_token')
    }
}