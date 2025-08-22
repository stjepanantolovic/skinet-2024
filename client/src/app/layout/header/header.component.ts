import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { MatIcon } from '@angular/material/icon';
import { MatButton } from '@angular/material/button';
import { MatBadge } from '@angular/material/badge';
import { MatProgressBar } from '@angular/material/progress-bar';
import { MatMenu,  MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { MatToolbar } from '@angular/material/toolbar';
import { MatSidenavContainer, MatSidenav, MatSidenavContent } from '@angular/material/sidenav';
import { MatDivider, MatList, MatListItem, MatNavList } from '@angular/material/list';
import { BusyService } from '../../core/services/busy.service';
import { CartService } from '../../core/services/cart.service';
import { AccountService } from '../../core/services/account.service';
import { IsAdmin } from "../../shared/directives/is-admin";



@Component({
  selector: 'app-header',
  imports: [
    RouterLink, RouterLinkActive,
    MatIcon, MatButton, MatBadge, MatProgressBar,
    MatMenu, MatMenuTrigger, MatMenuItem,
    MatToolbar, MatSidenavContainer, MatSidenav, MatSidenavContent,
    MatListItem,
    MatNavList,
    MatDivider,
    IsAdmin,
    IsAdmin
],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  busyService = inject(BusyService);
  cartService = inject(CartService);
  accountService = inject(AccountService);
  private router = inject(Router);

   mobileOpen = false;

  logout() {
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        // this.router.navigateByUrl('/');
      }
    })
  }

}
