import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class MailService {
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
                //this.headers = new  HttpHeaders().set("email", userEmail);
    }

    getAllUserLIsts() {
      console.log('userlist api');
      console.log(this.headers);
       let payloadObj={userEmail:localStorage.getItem('email')}
        return this.http.post(`${environment.BASE_URL}admin/get-users`,payloadObj,{headers: this.headers});
    }

    editUser(editplayer)
    {
      let payloadObj=
      {
        "notificationId":editplayer._id,
        "receivedUserId":editplayer.received_by_user,
        "message":editplayer.message,
        "userEmail":localStorage.getItem('email'),
       // "roleName":editplayer.roleid,
       // "roleId":editplayer.roleid,
      };
      //payloadObj
      console.log("edit"+editplayer);
      console.log(payloadObj);
      /*var headers_object = new HttpHeaders({
        'Content-Type': 'application/json',
        'access-token': this.access_token
      });

      const httpOptions = {
        headers: headers_object
      };*/
      //editplayer
        return this.http.post(`${environment.BASE_URL}admin/mail-edit`,payloadObj,{headers: this.headers});
    }

  deleteUser(editplayer)
  {
    console.log("pl"+ editplayer.status);
    console.log("pl"+ editplayer.userName);
    let options={coinId:editplayer._id,"userEmail":localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/delete-coin`,options,{headers: this.headers});
  }
  //deleteMail
    deleteMail(editplayer)
  {
    console.log("pl"+ editplayer.status);
    console.log("pl"+ editplayer.userName);
    let options={notificationId:editplayer._id,"userEmail":localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/delete-mail`,options,{headers: this.headers});
  }
  getAllCoins()
  {
     console.log('roles api');
     console.log(this.headers);
    let payloadObj={userEmail:localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/coin-list`,payloadObj,{headers: this.headers});
  }
   getAllNotification()
  {
     //console.log('roles api');
     console.log(this.headers);
    let payloadObj={userEmail:localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/mail-list`,payloadObj,{headers: this.headers});
  }
  //add coin
  //addCoin
  addMail(mailDetails)
  {
  console.log("mailDetails"+mailDetails);
    let payloadObj=
      {
        "receivedUserId":mailDetails.received_by_user,
        "userEmail":localStorage.getItem('email'),
        "message":mailDetails.message
      };
     return this.http.post(`${environment.BASE_URL}admin/mail-add`,payloadObj,{headers: this.headers});
  }
}
