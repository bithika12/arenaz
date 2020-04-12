import { Component, Inject, LOCALE_ID, Renderer2 } from '@angular/core';
import { ConfigName, ConfigService } from '../@vex/services/config.service';
import { MatIconRegistry } from '@angular/material/icon';
import { Settings } from 'luxon';
import { DOCUMENT } from '@angular/common';
import { Platform } from '@angular/cdk/platform';
import { NavigationService } from '../@vex/services/navigation.service';
import icLayers from '@iconify/icons-ic/twotone-layers';
import icAssigment from '@iconify/icons-ic/twotone-assignment';
import { LayoutService } from '../@vex/services/layout.service';
import { ActivatedRoute } from '@angular/router';
import { filter } from 'rxjs/operators';
import { coerceBooleanProperty } from '@angular/cdk/coercion';
import { SplashScreenService } from '../@vex/services/splash-screen.service';
import { Style, StyleService } from '../@vex/services/style.service';

@Component({
  selector: 'vex-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  title = 'vex';

  constructor(private configService: ConfigService,
              private styleService: StyleService,
              private iconRegistry: MatIconRegistry,
              private renderer: Renderer2,
              private platform: Platform,
              @Inject(DOCUMENT) private document: Document,
              @Inject(LOCALE_ID) private localeId: string,
              private layoutService: LayoutService,
              private route: ActivatedRoute,
              private navigationService: NavigationService,
              private splashScreenService: SplashScreenService) {
    this.iconRegistry.setDefaultFontSetClass('iconify');
    Settings.defaultLocale = this.localeId;

    if (this.platform.BLINK) {
      this.renderer.addClass(this.document.body, 'is-blink');
    }

    this.route.queryParamMap.pipe(
      filter(queryParamMap => queryParamMap.has('rtl') && coerceBooleanProperty(queryParamMap.get('rtl')))
    ).subscribe(queryParamMap => {
      this.document.body.dir = 'rtl';
      this.configService.updateConfig({
        rtl: true
      });
    });

    this.route.queryParamMap.pipe(
      filter(queryParamMap => queryParamMap.has('layout'))
    ).subscribe(queryParamMap => this.configService.setConfig(queryParamMap.get('layout') as ConfigName));

    this.route.queryParamMap.pipe(
      filter(queryParamMap => queryParamMap.has('style'))
    ).subscribe(queryParamMap => this.styleService.setStyle(queryParamMap.get('style') as Style));

    this.navigationService.items = [
      {
        type: 'link',
        label: 'Dashboard',
        route: '/',
        icon: icLayers
      },
      {
        type: 'link',
        label: 'User Module',
        route: '/user',
        icon: icAssigment
      },
      {
        type: 'link',
        label: 'Match Player List',
        route: '/dart',
        icon: icAssigment
      },
      {
        type: 'link',
        label: 'Coin Module',
        route: '/coin',
        icon: icAssigment
      },
      {
        type: 'link',
        label: 'Game Module',
        route: '/game',
        icon: icAssigment
      },
      {
        type: 'link',
        label: 'Online User Module',
        route: '/online',
        icon: icAssigment
      },
      {
        type: 'link',
        label: 'Mailbox Module',
        route: '/mail',
        icon: icAssigment
      }
    ];
  }
}
