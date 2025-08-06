import { Component, inject, OnInit, output } from '@angular/core';
import { CheckoutService } from '../../../core/services/checkout.service';
import { MatRadioModule } from '@angular/material/radio';
import { CurrencyPipe } from '@angular/common';
import { CartService } from '../../../core/services/cart.service';
import { DelibveryMethod } from '../../../shared/models/deliveryMethod';
import { delay } from 'rxjs';

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
    // console.log(' onOninit cdComp cart deliveryMethodId ', this.cartService.cart()?.deliveryMethodID);
    // console.log(' onOninit cdComp cart', this.cartService.cart())
    this.checkoutService.getDeliveryMethods().subscribe({
      next: methods => {
        // console.log(' onOninit cdComp delivery methods: ', methods);
        // console.log(' onOninit cdComp after get delivery methods subscribe cart deliveryMethodId: ', this.cartService.cart()?.deliveryMethodID);
        // let copy = Object.assign({}, this.cartService.cart());
        // console.log(' onOninit cdComp after get delivery methods subscribe cart COPY: ', copy);
        // console.log(' onOninit cdComp after get delivery methods subscribe cart: ', this.cartService.cart());        
        if (this.cartService.cart()?.deliveryMethodID) {
          // console.log('deliveryMethodID in Checkout delivery component is TRUE', this.cartService.cart()?.deliveryMethodID)
          const method = methods.find(x => x.id == this.cartService.cart()?.deliveryMethodID);
          if (method) {
            this.cartService.selectedDelivery.set(method);
            this.deliveryComplete.emit(true);
          }
        } 
        // else {
        //   console.log('deliveryMethodID in Checkout delivery component is false', this.cartService.cart()?.deliveryMethodID)
        // }
      }
    });
  }

  updateDeliveryMethod(method: DelibveryMethod) {
    this.cartService.selectedDelivery.set(method);
    const cart = this.cartService.cart();
    if (cart) {
      cart.deliveryMethodID = method.id;
      this.cartService.setCart(cart);
      this.deliveryComplete.emit(true);
    }
  }

}
