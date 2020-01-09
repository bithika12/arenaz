import { Component, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Tournament } from '../interfaces/tournament.model';
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

import { TournamentService } from '../../../../app/pages/services/tournament.service';

@Component({
  selector: 'vex-tournament-create-update',
  templateUrl: './tournament-create-update.component.html',
  styleUrls: ['./tournament-create-update.component.scss']
})
export class TournamentCreateUpdateComponent implements OnInit {

  
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
              private dialogRef: MatDialogRef<TournamentCreateUpdateComponent>,
              private fb: FormBuilder,
              private tournamentService:TournamentService
              ) {
  }

  ngOnInit() {
    if (this.defaults) {
      console.log(this.defaults);
      this.mode = 'update';
    } else {
      this.defaults = {} as Tournament;
    }

    this.form = this.fb.group({
      id: this.defaults.id,
      //imageSrc: this.defaults.imageSrc,
      name: [this.defaults.name || ''],
      round: [this.defaults.round || ''],
      price: [this.defaults.price || ''],
      status: [this.defaults.status || '1']
    });
  }

  save() {
    if (this.mode === 'create') {
      this.createTournament();
    } else if (this.mode === 'update') {
      this.updateTournament();
    }
  }

  createTournament() {
    const tournament = this.form.value;
    if (!tournament.imageSrc) {
      tournament.imageSrc = 'assets/img/avatars/1.jpg';
    }
    this.tournamentService.addTournament(tournament).subscribe(Tournament => {
      console.log(Tournament);
      if(Tournament){
        this.dialogRef.close(tournament);  
      }
    });

    this.dialogRef.close(tournament);
  }

  updateTournament() {
    const edittournament = this.form.value;
    
    edittournament.id = this.defaults.id;

    this.tournamentService.editTournament(edittournament).subscribe(Tournament => {
      console.log(Tournament);
      if(Tournament){
        this.dialogRef.close(edittournament);  
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
