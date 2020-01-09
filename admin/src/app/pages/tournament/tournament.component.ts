import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Observable, of, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Tournament } from './interfaces/tournament.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../@vex/interfaces/table-column.interface';
import { TournamentCreateUpdateComponent } from './tournament-create-update/tournament-create-update.component';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/twotone-search';
import icAdd from '@iconify/icons-ic/twotone-add';
import icFilterList from '@iconify/icons-ic/twotone-filter-list';
import { SelectionModel } from '@angular/cdk/collections';
import icMoreHoriz from '@iconify/icons-ic/twotone-more-horiz';
import icFolder from '@iconify/icons-ic/twotone-folder';
import { fadeInUp400ms } from '../../../@vex/animations/fade-in-up.animation';
import { MAT_FORM_FIELD_DEFAULT_OPTIONS, MatFormFieldDefaultOptions } from '@angular/material/form-field';
import { stagger40ms } from '../../../@vex/animations/stagger.animation';
import { FormControl } from '@angular/forms';
import { untilDestroyed } from 'ngx-take-until-destroy';
import { MatSelectChange } from '@angular/material/select';
import theme from '../../../@vex/utils/tailwindcss';

import { TournamentService } from '../../../app/pages/services/tournament.service';

@Component({
  selector: 'vex-tournament',
  templateUrl: './tournament.component.html',
  styleUrls: ['./tournament.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ],
  providers: [
    {
      provide: MAT_FORM_FIELD_DEFAULT_OPTIONS,
      useValue: {
        appearance: 'standard'
      } as MatFormFieldDefaultOptions
    }
  ]
})
export class TournamentComponent implements OnInit, AfterViewInit, OnDestroy {

  layoutCtrl = new FormControl('boxed');

  /**
   * Simulating a service with HTTP that returns Observables
   * You probably want to remove this and do all requests in a service with HTTP
   */
  subject$: ReplaySubject<Tournament[]> = new ReplaySubject<Tournament[]>(1);
  data$: Observable<Tournament[]> = this.subject$.asObservable();
  tournaments: Tournament[];

  @Input()
  columns: TableColumn<Tournament>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Image', property: 'image', type: 'image', visible: false },
    { label: 'Name', property: 'name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'Round', property: 'round', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Price', property: 'price', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Status', property: 'status', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
    { label: 'Actions', property: 'actions', type: 'button', visible: true }
  ];
  pageSize = 10;
  pageSizeOptions: number[] = [5, 10, 20, 50];
  dataSource: MatTableDataSource<Tournament> | null;
  selection = new SelectionModel<Tournament>(true, []);
  searchCtrl = new FormControl();

  icEdit = icEdit;
  icSearch = icSearch;
  icDelete = icDelete;
  icAdd = icAdd;
  icFilterList = icFilterList;
  icMoreHoriz = icMoreHoriz;
  icFolder = icFolder;

  theme = theme;

  @ViewChild(MatPaginator, { static: true }) paginator: MatPaginator;
  @ViewChild(MatSort, { static: true }) sort: MatSort;

  constructor(private dialog: MatDialog, private tournamentService:TournamentService) {
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  /**
   * Example on how to get data and pass it to the table - usually you would want a dedicated service with a HTTP request for this
   * We are simulating this request here.
   */
  

  ngOnInit() {
    
    this.tournamentService.getAllTournaments().subscribe(tournaments => {
      this.tournaments = tournaments["result"]["tournaments"];
      this.subject$.next(this.tournaments);   
    });

    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<Tournament[]>(Boolean)
    ).subscribe(tournaments => {
      this.tournaments = tournaments;
      this.dataSource.data = tournaments;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }
 

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }

  createTournament() {
    this.dialog.open(TournamentCreateUpdateComponent).afterClosed().subscribe((tournament: Tournament) => {
      /**
       * Tournament is the updated tournament (if the user pressed Save - otherwise it's null)
       */
      if (tournament) {
        /**
         * Here we are updating our local array.
         * You would probably make an HTTP request here.
         */
        this.tournaments.unshift(new Tournament(tournament));
        this.subject$.next(this.tournaments);
      }
    });
  }

  updateTournament(tournament: Tournament) {
    this.dialog.open(TournamentCreateUpdateComponent, {
      data: tournament
    }).afterClosed().subscribe(updatedTournament => {
      /**
       * Tournament is the updated tournament (if the user pressed Save - otherwise it's null)
       */
      if (updatedTournament) {
        /**
         * Here we are updating our local array.
         * You would probably make an HTTP request here.
         */
        const index = this.tournaments.findIndex((existingTournament) => existingTournament.id === updatedTournament.id);
        
        let tournamentObj = {
          id: (!updatedTournament.id)?this.tournaments[index].id:updatedTournament.id,
          name: (!updatedTournament.name)?this.tournaments[index].name:updatedTournament.name,
          round: (!updatedTournament.round)?this.tournaments[index].round:updatedTournament.round,
          price: (!updatedTournament.price)?this.tournaments[index].price:updatedTournament.price,
          image: (!updatedTournament.image)?this.tournaments[index].image:updatedTournament.image,
          status: (!updatedTournament.status)?this.tournaments[index].status:updatedTournament.status          
        };
        this.tournaments[index] = new Tournament(tournamentObj);
        this.subject$.next(this.tournaments);
      }
    });
  }

  deleteTournament(tournament: Tournament) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */     
    tournament.status = '0';    
    console.log(tournament);
    this.tournamentService.editTournament(tournament).subscribe(tm => {
      if(tm){
          this.tournaments.splice(this.tournaments.findIndex((existingTournament) => existingTournament.id === tournament.id), 1);
          this.selection.deselect(tournament);
          this.subject$.next(this.tournaments);
      }
    });    
  }

  deleteTournaments(tournaments: Tournament[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    tournaments.forEach(c => this.deleteTournament(c));
  }

  onFilterChange(value: string) {
    if (!this.dataSource) {
      return;
    }
    value = value.trim();
    value = value.toLowerCase();
    this.dataSource.filter = value;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  /** Whether the number of selected elements matches the total number of rows. */
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /** Selects all rows if they are not all selected; otherwise clear selection. */
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  trackByProperty<T>(index: number, column: TableColumn<T>) {
    return column.property;
  }

  onLabelChange(change: MatSelectChange, row: Tournament) {
    const index = this.tournaments.findIndex(c => c === row);
    this.tournaments[index].labels = change.value;
    this.subject$.next(this.tournaments);
  }

  ngOnDestroy() {
  }
}
