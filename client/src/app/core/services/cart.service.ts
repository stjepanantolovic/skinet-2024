import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Cart, CartItem } from '../../shared/models/cart';
import { Product } from '../../shared/models/product';
import { map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CartService {
  baseUrl = environment.apiUrl;
  private http = inject(HttpClient);
  cart = signal<Cart | null>(null);
  itemCount = computed(() => {
    return this.cart()?.items.reduce((sum, item) => sum + item.quantity, 0)
  })

  totals = computed(() => {
    const cart = this.cart();
    if (!cart) {
      return null;
    }

    const subtotal = cart.items.reduce((sum, item) => sum + item.quantity * item.price, 0)
    const shipping = 0;
    const discount = 0;
    return {
      subtotal,
      shipping,
      discount,
      total: subtotal + shipping - discount
    }
  })

  getCart(id: string) {
    return this.http.get<Cart>(this.baseUrl + 'cart?id=' + id).pipe(
      map(cart => {
        this.cart.set(cart);
        return cart;
      })
    )
  }

  setCart(cart: Cart) {
    return this.http.post<Cart>(this.baseUrl + 'cart', cart).subscribe({
      next: cart => this.cart.set(cart)
    })
  }

  async addItemToCart(item: CartItem | Product, quantity = 1) {
    const cart = this.cart() ?? this.createCart();

    item = this.mapProductToCartItem(item);

    cart.items = this.addOrUpdateItem(cart.items, item, quantity);
    this.setCart(cart);
  }

  removeItemFromCart(productId: number, quantity = 1) {
    const cart = this.cart();
    if (!cart) {
      return;
    }
    const index = cart.items.findIndex(x => x.productId == productId)
    if (index !== -1) {
      if (cart.items[index].quantity > quantity) {
        cart.items[index].quantity -= quantity;
      }
      else {
        cart.items.splice(index, 1);
      }
      if (cart.items.length === 0) {
        this.deleteCart(cart.id);
      } else {
        this.setCart(cart);
      }
    }
  }

  private addOrUpdateItem(items: CartItem[], item: CartItem, quantity: number): CartItem[] {
    const index = items.findIndex(x => x.productId === item.productId)
    if (index === -1) {
      item.quantity = quantity
      items.push(item);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private mapProductToCartItem(item: Product | CartItem): CartItem {
    if (this.isProduct(item)) {
      var itemProduct = (item as Product)
      return {
        productId: itemProduct.id,
        productName: itemProduct.name,
        price: itemProduct.price,
        quantity: 0,
        pictureUrl: itemProduct.pictureUrl,
        brand: itemProduct.brand,
        type: itemProduct.type
      }
    }
    return item as CartItem;
  }

  private isProduct(item: CartItem | Product) {
    return (item as Product).id !== undefined;
  }

  private createCart(): Cart {
    const cart = new Cart();
    localStorage.setItem('cart_id', cart.id);
    return cart;
  }

  deleteCart(id: string) {
    return this.http.delete(this.baseUrl + 'cart?id=' + id).subscribe({
      next: () => {
        localStorage.removeItem('cart_id');
        this.cart.set(null);
      }
    })
  }
}
