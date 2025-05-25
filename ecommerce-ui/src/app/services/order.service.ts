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
    { id: 1, name: 'Laptop', price: 999.99, description: 'High-performance laptop' },
    { id: 2, name: 'Smartphone', price: 599.99, description: 'Latest smartphone model' },
    { id: 3, name: 'Headphones', price: 199.99, description: 'Wireless noise-canceling headphones' },
    { id: 4, name: 'Tablet', price: 399.99, description: '10-inch tablet' }
  ];

  constructor(private http: HttpClient) { }

  getProducts(): Product[] {
    return this.products;
  }

  addOrder(order: OrderRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/orders`, order);
  }
} 