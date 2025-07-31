import { nanoid } from 'nanoid';

export type CartType = {
    id: string;
    items: CartItem[];
}

// we could use class Product
export type CartItem = {
    productId: number;
    productName: string;
    price: number;
    quantity: number;
    pictureUrl: string;
    brand: string;
    type: string;
}

export class Cart implements CartType {
    id = nanoid();
    items: CartItem[] = [];
}