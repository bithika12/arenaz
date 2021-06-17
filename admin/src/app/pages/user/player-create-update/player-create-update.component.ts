import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup,Validators } from '@angular/forms';
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

import { MatSnackBar } from '@angular/material/snack-bar';

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
   buttonDisabled=false;

   usertypeArr: any[] = [
    { name: 'Active',value:'active' },
    { name: 'Inactive',value:'inactive' },
    { name: 'Deleted',value:'deleted' },
    { name: 'Blocked',value:'blocked' },
    { name: 'Duplicate',value:'Duplicate' }
  ];

  brands: Brand[] = [
    { value: 'Louis Vuitton', viewValue: 'Louis Vuitton' },
    { value: 'Gucci', viewValue: 'Gucci' },
    { value: 'Prada', viewValue: 'Prada' },
    { value: 'Chanel', viewValue: 'Chanel' },
  ];
  selected=false;
  roleid=0;
  coins: Coin[] = [
    { value: 50},
    { value: 100},
    { value: 200 },
    { value: 500 },
  ];
  subject$: ReplaySubject<Player[]> = new ReplaySubject<Player[]>(1);
  data$: Observable<Player[]> = this.subject$.asObservable();
  rolelists= [];
  countrylists= [];
  Coin=[];
  coinList=[];
  form: FormGroup;
  mode: 'create' | 'update' = 'create';
  dataSource: MatTableDataSource<Player> | null;
  icMoreVert = icMoreVert;
  icClose = icClose;
  selectItemsSchedule:any;

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
              private snackbar: MatSnackBar,
              private coinService :CoinService

              ) {
  }

  ngOnInit() {
    this.loadCountry()
    this.userService.getAllRoles().subscribe(Roles => {
      console.log(Roles);      
      this.rolelists = Roles["result"];
      
    })
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
      this.buttonDisabled=true;
    } else {
      this.defaults = {} as Player;

    }
    this.selectItemsSchedule=this.defaults.countryName;
    this.form = this.fb.group({
      id: this.defaults.id,
      //imageSrc: this.defaults.imageSrc,
      firstname: [this.defaults.firstName || ''],
      lastname: [this.defaults.lastName || ''],
      contact_no: this.defaults.contact_no || '',
      rolename:[this.defaults.roleName || ''],
     // rolename:[this.defaults.roleName || ''],
      roleid:[this.defaults.roleId || ''],
      coin:[this.defaults.startCoin || ''],    
     
      username:[this.defaults.userName || ''],
      useremail:[this.defaults.email || ''],
      password:[this.defaults.password || ''],
      status:[this.defaults.status || ''],
      countryName:[this.defaults.countryName || '']
    });
  }
  loadCountry(){
    this.coinService.getAllCountryLists().subscribe(Country => {      
      this.countrylists = Country["result"];
      
    }) 
  }

  onKeyUser(value:string){
    if(value != ''){
    console.log("val search")
   
    let filter = value.toLowerCase();
    for ( let i = 0 ; i < this.countrylists.length; i ++ ) {
        
        let option = this.countrylists[i];
        
          this.countrylists.push( option );
       
      }
    }
    else
    {
      this.countrylists=[];
    }
  }
  save() {
    if (this.mode === 'create') {
      this.createPlayer();
    } else if (this.mode === 'update') {
      this.updatePlayer();
    }
  }

  createPlayer() {
    const player = this.form.value;
    if (!player.imageSrc) {
      player.imageSrc = 'assets/img/avatars/1.jpg';
    }
    this.userService.addUser(player).subscribe(User => {
      //console.log("result"+JSON.stringify(User.message));
      if(User['status']==422){
         console.log("not");
         this.snackbar.open(User['message'],'OK',{
                          verticalPosition: 'top',
                          horizontalPosition:'right'
                        });
      }
      //if(User){
      if(User['status']==1){
          console.log("ok");
        //this.router.navigate(['/user']);
        //this.ngOnInit();
        location.reload();
        this.dialogRef.close(player);
      }

      else{
         console.log("not");
         this.snackbar.open(User['message'],'OK',{
                          verticalPosition: 'top',
                          horizontalPosition:'right'
                        });
      }
    });
   // this.dialogRef.close(player);
  }

  updatePlayer() {
    const editplayer = this.form.value;

    editplayer.id = this.defaults.id;

    this.userService.editUser(editplayer).subscribe(User => {
      console.log(User);
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
