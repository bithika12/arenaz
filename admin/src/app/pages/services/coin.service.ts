import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class CoinService {
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
        "coinId":editplayer._id,
        "coinNumber":editplayer.coin,
        //"lastName":editplayer.lastname,
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
        return this.http.post(`${environment.BASE_URL}admin/edit-coin`,payloadObj,{headers: this.headers});
    }

  deleteUser(editplayer)
  {
    console.log("pl"+ editplayer.status);
    console.log("pl"+ editplayer.userName);
    let options={coinId:editplayer._id,"userEmail":localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/delete-coin`,options,{headers: this.headers});
  }
  getAllCoins()
  {
     console.log('roles api');
     console.log(this.headers);
    let payloadObj={userEmail:localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/get-user-coins`,payloadObj,{headers: this.headers});
  }

  getAllUserLIsts() {
      console.log('userlist api');
      console.log(this.headers);
       let payloadObj={userEmail:localStorage.getItem('email')}
        return this.http.post(`${environment.BASE_URL}admin/get-users`,payloadObj,{headers: this.headers});
    }
  //add coin
  //addCoin
  addCoin(coinDetails)
  {
    /*let payloadObj=
      {
        "coinNumber":coinDetails.coin,
        "userEmail":localStorage.getItem('email')
      };*/
      let payloadObj=
      {
        "userName":coinDetails.user_name,
        "type":coinDetails.type,
        "coin":coinDetails.coins,
        "reference":coinDetails.reference
      };
      
     return this.http.post(`${environment.BASE_URL}admin/add-user-coins`,payloadObj,{headers: this.headers});
  }
}
