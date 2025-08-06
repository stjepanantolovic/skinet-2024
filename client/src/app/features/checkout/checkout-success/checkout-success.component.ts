import { Component } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { RouterLink } from '@angular/router';
import { AddressPipe } from "../../../shared/pipes/address-pipe";
import { CurrencyPipe, DatePipe } from '@angular/common';
import { CardPipe } from "../../../shared/pipes/card-pipe";

@Component({
  selector: 'app-checkout-success',
  imports: [
    MatButton,
    RouterLink,
    AddressPipe,
    CurrencyPipe,
    CardPipe,
    DatePipe
],
  templateUrl: './checkout-success.component.html',
  styleUrl: './checkout-success.component.scss'
})
export class CheckoutSuccessComponent {

}
