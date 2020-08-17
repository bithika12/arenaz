import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class UserService {
    access_token;
    headers;
  /*constructor(private http: HttpClient, public authService: AuthService) {
            this.access_token = this.authService.getToken();
           console.log(" org :",this.access_token)
           console.log(" located in auth.gaurd access_token :",localStorage.getItem('access_token'))
            this.headers = new  HttpHeaders().set("access_token", this.access_token);
  }*/
    constructor(private http: HttpClient, public authService: AuthService) {
               this.access_token = this.authService.getToken();
                let userEmail= this.authService.getEmail();
                //this.access_token = 'dflkgnd46598';
                //this.access_token='a4525543-00e4-40b4-8fd0-d31d1d400621';
                this.headers = new  HttpHeaders().set("access_token", this.access_token);
                //this.headers = new  HttpHeaders().set("access_token", this.access_token);
                //this.headers = new  HttpHeaders().set("access_token", this.access_token);
    }

    getAllUsers() {
      console.log('userlist api');
      console.log(this.headers);
        return this.http.post(`${environment.BASE_URL}admin/users-list`,{headers: this.headers});
    }

    editUser(editplayer)
    {
      let payloadObj=
      {
        "coinNumber":editplayer.coin,
        "firstName":editplayer.firstname,
        "lastName":editplayer.lastname,
        "userEmail":editplayer.useremail,
        "roleName":editplayer.roleid,
        "roleId":editplayer.roleid,
        "status":editplayer.status
      };
      //payloadObj
      console.log(editplayer);
      console.log(payloadObj);
      /*var headers_object = new HttpHeaders({
        'Content-Type': 'application/json',
        'access-token': this.access_token
      });

      const httpOptions = {
        headers: headers_object
      };*/
      //editplayer
        return this.http.post(`${environment.BASE_URL}admin/edit-user`,payloadObj,{headers: this.headers});
    }

  deleteUser(editplayer)
  {
    console.log("pl"+ editplayer.status);
    console.log("pl"+ editplayer.userName);
    let options={userName:editplayer.userName}
    return this.http.post(`${environment.BASE_URL}admin/delete-user`,options,{headers: this.headers});
  }
  getAllRoles()
  {
     console.log('roles api');
     console.log(this.headers);
    //let options={userName:res.userName}
    return this.http.post(`${environment.BASE_URL}admin/get-role`,{headers: this.headers});
  }
  addUser(addPlayer){
    let payloadObj=
      {
        "userName":addPlayer.username,
        "firstName":addPlayer.firstname,
        "lastName":addPlayer.lastname,
        "email":addPlayer.useremail,
        //"roleName":addPlayer.roleid,
        "roleId":addPlayer.roleid,
        "coinNumber":addPlayer.coin,
        "password":addPlayer.password
      };
    //payloadObj
    console.log(addPlayer);
    console.log(payloadObj);
    //addplayer
    return this.http.post(`${environment.BASE_URL}admin/add-user`,payloadObj,{headers: this.headers});
  }

}
