import { Injectable } from '@angular/core';
import { HttpRequest, HttpHeaders, HttpHandler, HttpEvent, HttpInterceptor,HttpClient ,HttpParams } from '@angular/common/http';

import { environment } from './../../../environments/environment';
import { AuthService } from '../auth/auth.service';
import { Observable, of } from 'rxjs';
import { tap, delay,map } from 'rxjs/operators';
import { retry, catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class TransactionService {
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

    getAllTransactions() {
      console.log('userlist api');
      console.log(this.headers);
        return this.http.post(`${environment.BASE_URL}admin/transaction-list`,{headers: this.headers});
    }

    editTransaction(editplayer)
    {
      let payloadObj=
      { "transaction_id" : editplayer.id,
        "user_name":editplayer.user_name,
        "user_email":editplayer.user_email,
        "type":editplayer.type,
        "amount":editplayer.amount,
        "amount_usd":editplayer.amount_usd,
        "transaction_key":editplayer.transaction_key,
        "status":editplayer.status
      };
      //payloadObj
      console.log(editplayer);
      console.log(payloadObj);

        return this.http.post(`${environment.BASE_URL}admin/edit-transaction`,payloadObj,{headers: this.headers});
    }

  deleteUser(editplayer)
  {

    let options={transactionId:editplayer._id,"userEmail":localStorage.getItem('email')}
    return this.http.post(`${environment.BASE_URL}admin/delete-transaction`,options,{headers: this.headers});
  }
 
  addTransaction(addPlayer){
    let payloadObj=
      {
        "user_name":addPlayer.user_name,
        "user_email":addPlayer.user_email,
        "type":addPlayer.type,
        "amount":addPlayer.amount,
        "amount_usd":addPlayer.amount_usd,
        "transaction_key":addPlayer.transaction_key,
        "status":addPlayer.status
      };
    //payloadObj
    console.log(addPlayer);
    console.log(payloadObj);
    //addplayer
    return this.http.post(`${environment.BASE_URL}admin/transaction-add`,payloadObj,{headers: this.headers}) .pipe(map(fetchresult => {
          console.log("fetchresult"+JSON.stringify(fetchresult));
          return fetchresult;
     }));
  }

  getAllUserLIsts() {
      console.log('userlist api');
      console.log(this.headers);
       let payloadObj={userEmail:localStorage.getItem('email')}
        return this.http.post(`${environment.BASE_URL}admin/get-users`,payloadObj,{headers: this.headers});
    }

}


