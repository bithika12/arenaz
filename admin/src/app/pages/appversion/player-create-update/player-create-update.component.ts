import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Player } from '../interfaces/player.model';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icClose from '@iconify/icons-ic/twotone-close';
import icPrint from '@iconify/icons-ic/twotone-print';
import icDownload from '@iconify/icons-ic/twotone-cloud-download';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icPhone from '@iconify/icons-ic/twotone-phone';
import icPerson from '@iconify/icons-ic/twotone-person';
import icMyLocation from '@iconify/icons-ic/twotone-my-location';
import icLocationCity from '@iconify/icons-ic/twotone-location-city';
import icEditLocation from '@iconify/icons-ic/twotone-edit-location';

import { UserService } from '../../../../app/pages/services/user.service';
import {MatSelectChange} from "@angular/material/select";
//import {Tournament} from "../../tournament/interfaces/tournament.model";
import {Observable, ReplaySubject} from "rxjs";
import {filter} from "rxjs/operators";
import {MatTableDataSource} from "@angular/material/table";
import { Router } from '@angular/router';
import { Location } from '@angular/common';
import { CoinService } from '../../../../app/pages/services/coin.service';

export interface Brand {
  value: string;
  viewValue: string;
}

export interface Coin {
  value: number;
}

@Component({
  selector: 'vex-player-create-update',
  templateUrl: './player-create-update.component.html',
  styleUrls: ['./player-create-update.component.scss']
})
export class PlayerCreateUpdateComponent implements OnInit {
  user_name:any;
  selected=false;
  selectItemsSchedule:any;
  //gameDeactivationArr:any;
  //emailVerificationArr:any;
  //ipChangeVerificationArr:any;

  options: string[] = ['One', 'Two', 'Three'];

  brands: Brand[] = [
    { value: 'Louis Vuitton', viewValue: 'Louis Vuitton' },
    { value: 'Gucci', viewValue: 'Gucci' },
    { value: 'Prada', viewValue: 'Prada' },
    { value: 'Chanel', viewValue: 'Chanel' },
  ];
usertypeArr: any[] = [
    { name: 'Yes' },
    { name: 'No' }
    
    
];

gameDeactivationArr: any[] = [
    { name: 'Yes' },
    { name: 'No' }
    
    
];
emailVerificationArr: any[] = [
    { name: 'Yes' },
    { name: 'No' }
    
    
];

ipChangeVerificationArr: any[] = [
    { name: 'Yes' },
    { name: 'No' }
    
    
];
freeCoinArr: any[] = [
    { value: 0 },
    { value: 5 },
    { value: 10 }

    
    
];
  coins: Coin[] = [
    { value: 50},
    { value: 100},
    { value: 200 },
    { value: 500 },
  ];
  subject$: ReplaySubject<Player[]> = new ReplaySubject<Player[]>(1);
  data$: Observable<Player[]> = this.subject$.asObservable();
  rolelists= [];
  Coin=[];
  coinList=[];
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<Player> | null;
  icMoreVert = icMoreVert;
  icClose = icClose;

  icPrint = icPrint;
  icDownload = icDownload;
  icDelete = icDelete;

  icPerson = icPerson;
  icMyLocation = icMyLocation;
  icLocationCity = icLocationCity;
  icEditLocation = icEditLocation;
  icPhone = icPhone;

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
              private dialogRef: MatDialogRef<PlayerCreateUpdateComponent>,
              private fb: FormBuilder,
              private userService:UserService,
              private router: Router,
              private location: Location,
              private coinService:CoinService,

              ) {
  }

  ngOnInit() {

   this.coinService.getAllCountryLists().subscribe(Roles => {
      console.log(Roles);
      //roleList: RoleList[] =Roles;
      this.rolelists = Roles["result"];
      console.log("this.rolelists"+this.rolelists);

      //this.subject$.next(this.rolelists);
    })
    /*this.userService.getAllRoles().subscribe(Roles => {
      console.log(Roles);
      //roleList: RoleList[] =Roles;
      this.rolelists = Roles["result"];

      //this.subject$.next(this.rolelists);
    })*/
    this.coinList = [
      { value: 10},
      { value: 50},
      { value: 100},
      { value: 250 },
      { value: 500 },
    ];
   ////////////////////////////////

    ////////////////////////////
    if (this.defaults) {
      console.log(this.defaults);
      this.mode = 'update';
    } else {
      this.defaults = {} as Player;
    }
   this.selectItemsSchedule=JSON.parse(this.defaults.banned_country);

   //this.selectItemsSchedule=this.defaults.banned_country
   console.log("this selectItemsSchedule***"+this.selectItemsSchedule)
    this.form = this.fb.group({
      id: this.defaults.id,
      //imageSrc: this.defaults.imageSrc,
      app_version: [this.defaults.app_version || ''],
      download_link: [this.defaults.download_link || ''],
      coin_price_usd: [this.defaults.coin_price_usd || ''],
      //wallet_api_link
      wallet_api_link: [this.defaults.wallet_api_link || ''],
      wallet_key: [this.defaults.wallet_key || ''],
      api_expiration_time: [this.defaults.api_expiration_time || ''],
      e_currency_price_api: [this.defaults.e_currency_price_api || ''],
      transaction_fee_withdrawl: [this.defaults.transaction_fee_withdrawl || ''],

      transaction_fee_deposit: [this.defaults.transaction_fee_deposit || ''],

      minimum_deposit: [this.defaults.minimum_deposit || ''],
      minimum_withdrawl: [this.defaults.minimum_withdrawl || ''],
      new_account_gift_coins: [this.defaults.new_account_gift_coins || ''],

      master_message: [this.defaults.master_message || ''],

      allow_mini_account_withdrawal: [this.defaults.allow_mini_account_withdrawal || ''],

      support_email: [this.defaults.support_email || ''],

      market_volatility:[this.defaults.market_volatility || ''],

      
      banned_country:[this.defaults.banned_country || ''],
      
      email_verify:[this.defaults.email_verify || ''],

      game_deactivation:[this.defaults.game_deactivation || ''],

      ip_verify:[this.defaults.ip_verify || ''],

      auto_refill_coins:[this.defaults.auto_refill_coins || ''],

      free_coin_incentive:[this.defaults.free_coin_incentive || ''],

     _id:[this.defaults._id || ''],
    });
  }

  save() {
    if (this.mode === 'create') {
      this.createPlayer();
    } else if (this.mode === 'update') {
      this.updatePlayer();
    }
  }
  onKeyUser(value:string){
    if(value != ''){
    console.log("val search")
   // this.employee_pop=[];
    let filter = value.toLowerCase();
    for ( let i = 0 ; i < this.rolelists.length; i ++ ) {
        console.log("ppp"+ this.rolelists[i])
        let option = this.rolelists[i];
        /*if ( (option.country_name.toLowerCase().indexOf(filter) >= 0) || (option.country_name.toLowerCase().indexOf(filter) >= 0)) {*/
          this.rolelists.push( option );
       // }
      }
    }
    else
    {
      this.rolelists=[];
    }
  }
  createPlayer() {
    const coins = this.form.value;
    if (!coins.imageSrc) {
      coins.imageSrc = 'assets/img/avatars/1.jpg';
    }
    this.coinService.addCoin(coins).subscribe(User => {
      location.reload();
      this.dialogRef.close(coins);
    });
  }

  updatePlayer() {
    const editplayer = this.form.value;

    editplayer.id = this.defaults.id;

    console.log("editplayer"+JSON.stringify(editplayer))

    this.coinService.editVersion(editplayer).subscribe(User => {
      //console.log(User);
      if(User){
        //this.router.navigate(['/user']);
        //this.ngOnInit();
        location.reload();
        this.dialogRef.close(editplayer);
      }
    });
  }

  isCreateMode() {
    return this.mode === 'create';
  }

  isUpdateMode() {
    return this.mode === 'update';
  }
  changeValue(value) {
    console.log(value);
    //console.log(this.peopleForm.get("roleid").value);
    //console.log()
    //const index = this.rolelists.findIndex(c => c === row);
    //console.log(index);
    //console.log(this.rolelists);
    this.defaults.roleName = value;
    //this.subject$.next(this.rolelists);
  }
  /*onLabelChange(change: MatSelectChange, row: Tournament) {
    const index = this.tournaments.findIndex(c => c === row);
    this.tournaments[index].labels = change.value;
    this.subject$.next(this.tournaments);
  }*/
  //this.rolelists

}
