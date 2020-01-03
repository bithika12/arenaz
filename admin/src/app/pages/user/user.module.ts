import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { UserRoutingModule } from './user-routing.module';
import { UserComponent } from './user.component';
import { PageLayoutModule } from '../../../@vex/components/page-layout/page-layout.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { BreadcrumbsModule } from '../../../@vex/components/breadcrumbs/breadcrumbs.module';
import { PlayerCreateUpdateModule } from './player-create-update/player-create-update.module';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatSortModule } from '@angular/material/sort';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { IconModule } from '@visurel/iconify-angular';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ContainerModule } from '../../../@vex/directives/container/container.module';
import { MatSelectModule } from '@angular/material/select';
import { ColorFadeModule } from '../../../@vex/pipes/color/color-fade.module';
import { MatButtonToggleModule } from '@angular/material/button-toggle';


@NgModule({
  declarations: [UserComponent],
  imports: [
    CommonModule,
    UserRoutingModule,
    PageLayoutModule,
    FlexLayoutModule,
    BreadcrumbsModule,
    PlayerCreateUpdateModule,
    MatPaginatorModule,
    MatTableModule,
    MatSortModule,
    MatCheckboxModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    IconModule,
    FormsModule,
    MatTooltipModule,
    ReactiveFormsModule,
    ContainerModule,
    MatSelectModule,
    ColorFadeModule,
    MatButtonToggleModule
  ]
})
export class UserModule {
}
