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

import { TransactionService } from '../../../../app/pages/services/transaction.service';
import {MatSelectChange} from "@angular/material/select";
//import {Tournament} from "../../tournament/interfaces/tournament.model";
import {Observable, ReplaySubject} from "rxjs";
import {filter} from "rxjs/operators";
import {MatTableDataSource} from "@angular/material/table";
import { Router } from '@angular/router';
import { Location } from '@angular/common';

import { MatSnackBar } from '@angular/material/snack-bar';


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

   transactiontypeArr: any[] = [
    { name: 'Withdraw',value:'Withdraw' },
    { name: 'Deposit',value:'Deposit' },
  ];

  transactionStatus:any[]= [
    { name:'New', value: 'New'},
    { name:'Completed', value: 'Completed'},
    { name:'Expired', value: 'Expired'},
    { name:'Cancelled', value: 'Cancelled'},
    { name:'Errror', value: 'Errror'},
  ];

  brands: Brand[] = [
    { value: 'Louis Vuitton', viewValue: 'Louis Vuitton' },
    { value: 'Gucci', viewValue: 'Gucci' },
    { value: 'Prada', viewValue: 'Prada' },
    { value: 'Chanel', viewValue: 'Chanel' },
  ];
  selected=false;
  roleid=0;
  

  subject$: ReplaySubject<Player[]> = new ReplaySubject<Player[]>(1);
  data$: Observable<Player[]> = this.subject$.asObservable();
  userlists= [];
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
  selectUsername
  selectUseremail
  selectStatus

  constructor(@Inject(MAT_DIALOG_DATA) public defaults: any,
              private dialogRef: MatDialogRef<PlayerCreateUpdateComponent>,
              private fb: FormBuilder,
              private userService:TransactionService,
              private router: Router,
              private location: Location,
              private snackbar: MatSnackBar

              ) {
  }

  ngOnInit() {


    this.userService.getAllUserLIsts().subscribe(Users => {
      console.log(Users);
      this.userlists = Users["result"];
      console.log("this.rolelists"+this.userlists);
    })

    ////////////////////////////
    if (this.defaults) {
      console.log(this.defaults);
      this.mode = 'update';
      this.buttonDisabled=true;
      this.selectUsername = (this.defaults.user_name) ? this.defaults.user_name : '';
      this.selectUseremail = (this.defaults.user_email) ? this.defaults.user_email : '';
      this.selectStatus = (this.defaults.status) ? this.defaults.status : '';
    } else {
      this.defaults = {} as Player;

    } 

    this.form = this.fb.group({
      id: this.defaults._id,
      user_name: [this.defaults.user_name || ''],
      user_email: [this.defaults.user_email || ''],
      amount: this.defaults.amount || '',
      amount_usd: this.defaults.amount_usd || '',
      type:[this.defaults.type || ''],
      transaction_key:[this.defaults.transaction_key || ''],
      status:[this.defaults.status || '']
    });
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
   console.log(player);

    this.userService.addTransaction(player).subscribe(User => {
      if(User['status']==422){
         console.log("not");
         this.snackbar.open(User['message'],'OK',{
                          verticalPosition: 'top',
                          horizontalPosition:'right'
                        });
      }

      if(User['status']==1){
          
        console.log("ok");
        location.reload();
        this.dialogRef.close(player);

      }else{
         console.log("not");
         this.snackbar.open(User['message'],'OK',{
                          verticalPosition: 'top',
                          horizontalPosition:'right'
                        });
      }
    });
  }

  updatePlayer() {
    const editplayer = this.form.value;

    editplayer.id = this.defaults._id;

    this.userService.editTransaction(editplayer).subscribe(User => {
      console.log(User);
      if(User){
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
  onLabelChange() {
    /*const index = this.tournaments.findIndex(c => c === row);
    this.tournaments[index].labels = change.value;
    this.subject$.next(this.tournaments);*/
  }

  onUserChange(event){
    const index = this.userlists.findIndex(x => x.userName === event.value);
    console.log(index);
    this.selectUseremail = this.userlists[index].email
  }

  onEmailChange(event){
    const index = this.userlists.findIndex(x => x.email === event.value);
    console.log(index);
    this.selectUsername = this.userlists[index].userName
  }
  //this.rolelists

}
