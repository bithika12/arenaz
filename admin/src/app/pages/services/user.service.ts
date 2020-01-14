import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class UserService {
    access_token;
    headers;
  constructor(private http: HttpClient, public authService: AuthService) {
            this.access_token = this.authService.getToken();
           console.log(" org :",this.access_token)
           console.log(" located in auth.gaurd access_token :",localStorage.getItem('access_token'))
            this.headers = new  HttpHeaders().set("access_token", this.access_token);
  }
   /* constructor(private http: HttpClient) {
               this.access_token = this.authService.getToken();
                this.access_token = 'dflkgnd46598';
                this.headers = new  HttpHeaders().set("access_token", this.access_token);
    }*/

    getAllUsers() {
        return this.http.post(`${environment.BASE_URL}admin/users-list`,{headers: this.headers});
    }

    editUser(editplayer)
    {
        return this.http.post(`${environment.BASE_URL}admin/edit-user`,editplayer,{headers: this.headers});
    }

  deleteUser(editplayer)
  {
    console.log(this.access_token);
    return this.http.post(`${environment.BASE_URL}admin/delete-user`,editplayer,{headers: this.headers});
  }

}
