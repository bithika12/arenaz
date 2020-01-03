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

@Component({
  selector: 'vex-player-create-update',
  templateUrl: './player-create-update.component.html',
  styleUrls: ['./player-create-update.component.scss']
})
export class PlayerCreateUpdateComponent implements OnInit {

  
  form: FormGroup;
  mode: 'create' | 'update' = 'create';

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
              private userService:UserService
              ) {
  }

  ngOnInit() {
    if (this.defaults) {
      console.log(this.defaults);
      this.mode = 'update';
    } else {
      this.defaults = {} as Player;
    }

    this.form = this.fb.group({
      id: this.defaults.id,
      //imageSrc: this.defaults.imageSrc,
      firstname: [this.defaults.firstname || ''],
      lastname: [this.defaults.lastname || ''],
      contact_no: this.defaults.contact_no || ''
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
    if (!player.imageSrc) {
      player.imageSrc = 'assets/img/avatars/1.jpg';
    }
    this.dialogRef.close(player);
  }

  updatePlayer() {
    const editplayer = this.form.value;
    
    editplayer.id = this.defaults.id;

    this.userService.editUser(editplayer).subscribe(User => {
      console.log(User);
      if(User){
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
}
