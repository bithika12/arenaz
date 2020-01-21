import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class DartService {
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

    getAllGames() {
      console.log('userlist api');
      console.log(this.headers);
       let payloadObj={userEmail:localStorage.getItem('email')}
        return this.http.post(`${environment.BASE_URL}admin/game-list`,payloadObj,{headers: this.headers});
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
    let options={roomName:editplayer.game_name}
    return this.http.post(`${environment.BASE_URL}admin/delete-game`,options,{headers: this.headers});
  }
  getAllRoles()
  {
     console.log('roles api');
     console.log(this.headers);
    //let options={userName:res.userName}
    return this.http.post(`${environment.BASE_URL}admin/get-role`,{headers: this.headers});
  }

}
