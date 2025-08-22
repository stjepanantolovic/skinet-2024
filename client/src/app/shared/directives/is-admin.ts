import { Directive, effect, inject, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../../core/services/account.service';
import { Router } from '@angular/router';

@Directive({
  selector: '[appIsAdmin]' //*appIsAdmin
})
export class IsAdmin {
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef);
  private templateRef = inject(TemplateRef);
private  router = inject(Router);
  constructor() {
    effect(()=>{
      if (this.accountService.isAdmin()) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
      this.router.navigateByUrl('/shop');
    }
    });
   }

 

}
