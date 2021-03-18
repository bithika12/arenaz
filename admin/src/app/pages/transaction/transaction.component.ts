import { AfterViewInit, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Observable, of, ReplaySubject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Player } from './interfaces/player.model';
import { MatTableDataSource } from '@angular/material/table';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatDialog } from '@angular/material/dialog';
import { TableColumn } from '../../../@vex/interfaces/table-column.interface';
import { PlayerCreateUpdateComponent } from './player-create-update/player-create-update.component';
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
import { MatSnackBar } from '@angular/material/snack-bar';

import { TransactionService } from '../../../app/pages/services/transaction.service';

@Component({
  selector: 'vex-transaction',
  templateUrl: './transaction.component.html',
  styleUrls: ['./transaction.component.scss'],
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
export class TransactionComponent implements OnInit, AfterViewInit, OnDestroy {

  layoutCtrl = new FormControl('boxed');

  /**
   * Simulating a service with HTTP that returns Observables
   * You probably want to remove this and do all requests in a service with HTTP
   */
  subject$: ReplaySubject<Player[]> = new ReplaySubject<Player[]>(1);
  data$: Observable<Player[]> = this.subject$.asObservable();
  players: Player[];
  roles:[];

  @Input()
  columns: TableColumn<Player>[] = [
    { label: 'Checkbox', property: 'checkbox', type: 'checkbox', visible: true },
    { label: 'Transaction Id', property: '_id', type: 'text', visible: true },
    { label: 'Type', property: 'type', type: 'text', visible: true },
    { label: 'User Name', property: 'user_name', type: 'text', visible: true, cssClasses: ['font-medium'] },
    { label: 'User Email', property: 'user_email', type: 'text', visible: true },
    { label: 'Transaction Key', property: 'transaction_key', type: 'text', visible: true },
    { label: 'Coin Amount', property: 'amount', type: 'text', visible: true },
    { label: 'USD', property: 'amount_usd', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: true },
    { label: 'Created Date', property: 'created_at', type: 'text', visible: true, cssClasses: ['text-secondary', 'font-medium'] },
   
  ];
  pageSize = 10;
  pageSizeOptions: number[] = [10, 20, 50, 100];
  dataSource: MatTableDataSource<Player> | null;
  selection = new SelectionModel<Player>(true, []);
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

  constructor(private dialog: MatDialog, private transactionService:TransactionService,private snackbar: MatSnackBar) {
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  /**
   * Example on how to get data and pass it to the table - usually you would want a dedicated service with a HTTP request for this
   * We are simulating this request here.
   */


  ngOnInit() {

    this.transactionService.getAllTransactions().subscribe(transactions => {
      this.players = transactions["result"];
      this.subject$.next(this.players);
    });

    /*this.transactionService.getAllRoles().subscribe(transactions => {
      this.roles = transactions["result"];
      console.log(this.roles);
      console.log("roles");
      //this.players = transactions["result"]["transactions"];
      this.subject$.next(this.roles);
    });*/

    this.dataSource = new MatTableDataSource();

    this.data$.pipe(
      filter<Player[]>(Boolean)
    ).subscribe(transactions => {
      console.log(transactions);
      this.players = transactions;
      this.dataSource.data = transactions;
    });

    this.searchCtrl.valueChanges.pipe(
      untilDestroyed(this)
    ).subscribe(value => this.onFilterChange(value));
  }


  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
  }
  checkTransaction(){
    this.transactionService.checkTransaction().subscribe(transaction => {
        if(transaction){
            console.log("ok");
            this.snackbar.open("Transaction updated !!",'OK',{
                          verticalPosition: 'top',
                          horizontalPosition:'right'
                        });
        }
      });
    }

  createPlayer() {
    this.dialog.open(PlayerCreateUpdateComponent).afterClosed().subscribe((player: Player) => {
      /**
       * Player is the updated player (if the user pressed Save - otherwise it's null)
       */
      if (player) {
        /**
         * Here we are updating our local array.
         * You would probably make an HTTP request here.
         */
        this.players.unshift(new Player(player));
        this.subject$.next(this.players);
      }
    });
  }

  updatePlayer(player: Player) {
    this.dialog.open(PlayerCreateUpdateComponent, {
      data: player
    }).afterClosed().subscribe(updatedPlayer => {
      /**
       * Player is the updated player (if the user pressed Save - otherwise it's null)
       */
      if (updatedPlayer) {
        /**
         * Here we are updating our local array.
         * You would probably make an HTTP request here.
         */
        const index = this.players.findIndex((existingPlayer) => existingPlayer._id === updatedPlayer._id);

        let userObj = {
          id                : (!updatedPlayer._id)?this.players[index]._id:updatedPlayer._id,
          user_name         : (!updatedPlayer.user_name)?this.players[index].user_name:updatedPlayer.user_name,
          user_email        : (!updatedPlayer.user_email)?this.players[index].user_email:updatedPlayer.user_email,
          amount            : (!updatedPlayer.amount)?this.players[index].amount:updatedPlayer.amount,
          amount_usd        : (!updatedPlayer.amount_usd)?this.players[index].amount_usd:updatedPlayer.amount_usd,
          type              : (!updatedPlayer.type)?this.players[index].type:updatedPlayer.type,
          status            : (!updatedPlayer.status)?this.players[index].status:updatedPlayer.status,
          created_at        : (!updatedPlayer.created_at)?this.players[index].created_at:updatedPlayer.created_at,
          transaction_key   : (!updatedPlayer.transaction_key)?this.players[index].transaction_key:updatedPlayer.transaction_key,
          user_confirmation : (!updatedPlayer.user_confirmation)?this.players[index].user_confirmation:updatedPlayer.user_confirmation,

        };
        this.players[index] = new Player(userObj);
        this.subject$.next(this.players);
      }
    });
  }

  deletePlayer(player: Player) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
     console.log('Hiiiii... Its deleteTransaction');

   this.transactionService.deleteUser(player).subscribe(transaction => {
      if(transaction){
          this.players.splice(this.players.findIndex((existingPlayer) => existingPlayer._id === player._id), 1);
          this.selection.deselect(player);
          this.subject$.next(this.players);
      }
    });
  }

  deletePlayers(players: Player[]) {
    /**
     * Here we are updating our local array.
     * You would probably make an HTTP request here.
     */
    players.forEach(c => this.deletePlayer(c));
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

  ngOnDestroy(){
  }

  
}
