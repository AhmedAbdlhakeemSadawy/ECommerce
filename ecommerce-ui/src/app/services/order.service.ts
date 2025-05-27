import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
}

export interface OrderRequest {
  productId: number;
  quantity: number;
  totalAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = 'http://localhost:5000/api'; // Update this with your actual API URL

  // Static product list for demonstration
  readonly products: Product[] = [
    { id: 1, name: 'Product One', price: 20.00, description: 'Description For Product One' },
    { id: 2, name: 'Product Two', price: 10.00, description: 'Description For Product Two' },
    { id: 3, name: 'Product Three', price: 50.00, description: 'Description For Product Three' }
  ];

  constructor(private http: HttpClient) { }

  getProducts(): Product[] {
    return this.products;
  }

  addOrder(order: OrderRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders`, order);
  }
} 