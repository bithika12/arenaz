import { Injectable } from '@angular/core';
import { HttpClient , HttpHeaders } from '@angular/common/http';

import { environment } from './../../../environments/environment';
//import { AuthService } from '../auth/auth.service';

@Injectable({ providedIn: 'root' })
export class TournamentService {
    access_token;
    headers;
  /*constructor(private http: HttpClient, public authService: AuthService) {
            this.access_token = this.authService.getToken();
            this.headers = new  HttpHeaders().set("access_token", this.access_token);
  }*/
    constructor(private http: HttpClient) {
                //this.access_token = this.authService.getToken();
                this.access_token = 'dflkgnd46598';
                this.headers = new  HttpHeaders().set("access_token", this.access_token);
    }

    getAllTournaments() {
        let payloadObj={userEmail:localStorage.getItem('email')}
        return this.http.post(`${environment.BASE_URL}admin/game-list`,payloadObj,{headers: this.headers});
    }

    editTournament(edittournament)
    {
        return this.http.post(`${environment.BASE_URL}admin/edit-tournament`,edittournament,{headers: this.headers});
    }

    addTournament(addtournament)
    {
        return this.http.post(`${environment.BASE_URL}admin/add-tournament`,addtournament,{headers: this.headers});
    }


}
