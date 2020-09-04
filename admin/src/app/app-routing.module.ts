import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LayoutComponent } from '../@vex/layout/layout.component';
import { VexRoutes } from '../@vex/interfaces/vex-route.interface';
import { AuthGuard } from '../app/pages/auth/auth.guard';

const childrenRoutes: VexRoutes = [
  {
    path: '',
    canActivate: [AuthGuard],
    redirectTo: 'dashboards/analytics',
    pathMatch: 'full',
  },
  {
    path: 'dashboards/analytics',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/dashboards/dashboard-analytics/dashboard-analytics.module').then(m => m.DashboardAnalyticsModule),
  },
  {
    path: 'user',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/user/user.module').then(m => m.UserModule),
  },
  {
    path: 'dart',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/dart/dart.module').then(m => m.DartModule),
  },
  /*{
    path: 'tournament',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/tournament/tournament.module').then(m => m.TournamentModule),
  },*/
  //coin

  {
    path: 'coin',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/coin/coin.module').then(m => m.DartModule),
  },
  {
    path: 'game',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/game/game.module').then(m => m.DartModule),
  },
  {
    path: 'online',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/online/online.module').then(m => m.OnlineModule),
  },
  {
    path: 'mail',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/mail/mail.module').then(m => m.MailModule),
  },
  
   {
    path: 'appversion',
    canActivate: [AuthGuard],
    loadChildren: () => import('./pages/appversion/appversion.module').then(m => m.DartModule),
  }
];
const routes: Routes = [
  {
    path: 'login',
    loadChildren: () => import('./pages/login/login.module').then(m => m.LoginModule),
  },
  {
    path: '',
    component: LayoutComponent,
    children: childrenRoutes
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    // preloadingStrategy: PreloadAllModules,
    scrollPositionRestoration: 'enabled',
    relativeLinkResolution: 'corrected',
    anchorScrolling: 'enabled'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
