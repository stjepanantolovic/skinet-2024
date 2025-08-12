import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Order, OrderTocreate } from '../../shared/models/order';

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  orderComplete = false;


  createOrder(orderTocreate: OrderTocreate) {
    return this.http.post<Order>(this.baseUrl + 'orders', orderTocreate);
  }

  getOrdersForUser() {
    return this.http.get<Order[]>(this.baseUrl + 'orders');
  }

  getOrderDetail(id: number) {
    return this.http.get<Order>(this.baseUrl + 'orders/' + id);
  }
}
