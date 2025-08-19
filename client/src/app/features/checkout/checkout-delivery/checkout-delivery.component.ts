import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DelibveryMethod } from '../../../shared/models/deliveryMethod';
import { delay, firstValueFrom } from 'rxjs';

@Component({
  selector: 'app-checkout-delivery',
  imports: [
    MatRadioModule,
    CurrencyPipe
  ],
  templateUrl: './checkout-delivery.component.html',
  styleUrl: './checkout-delivery.component.scss'
})
export class CheckoutDeliveryComponent implements OnInit {
  checkoutService = inject(CheckoutService);
  cartService = inject(CartService);
  deliveryComplete = output<boolean>();

  ngOnInit(): void {
    this.checkoutService.getDeliveryMethods().subscribe({
      next: methods => {
        if (this.cartService.cart()?.deliveryMethodId) {
          const method = methods.find(x => x.id == this.cartService.cart()?.deliveryMethodId);
          if (method) {
            this.cartService.selectedDelivery.set(method);
            this.deliveryComplete.emit(true);
          }
        }
      }
    });
  }

  async updateDeliveryMethod(method: DelibveryMethod) {
    console.log("updateDeliveryMethod", method);
    this.cartService.selectedDelivery.set(method);
    console.log("this.cartService.cart: ", this.cartService.cart());    
    const cart = this.cartService.cart();
    if (cart) {
      console.log("setCart=> cart before adding method.id: ",cart);    
      cart.deliveryMethodId = method.id;
      console.log("etCart=> cart after adding method.id: ",cart);  
      await firstValueFrom(this.cartService.setCart(cart));
      console.log("etCart=> cartServicecart after adding method.id: ", this.cartService.cart()); 
      this.deliveryComplete.emit(true);
    }
  }

}