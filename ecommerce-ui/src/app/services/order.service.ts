import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  quantity: number;
}


export interface ProductRequest {
  id: number;
  quantity: number;
}


export interface OrderRequest {
  customerId: number;
  products: ProductRequest[];
}

@Injectable({
  providedIn: 'root'
}) 
export class OrderService {
  private apiUrl = environment.apiUrl; // Update this with your actual API URL

  // Static product list for demonstration
  readonly products: Product[] = [
    { id: 1, name: 'Product One', price: 20.00, description: 'Description For Product One' ,quantity: 5},
    { id: 2, name: 'Product Two', price: 10.00, description: 'Description For Product Two',quantity: 5 },
    { id: 3, name: 'Product Three', price: 50.00, description: 'Description For Product Three',quantity: 5 }
  ];

  constructor(private http: HttpClient) { }

  getProducts(): Product[] {
    return this.products;
  }

  addOrder(order: OrderRequest): Observable<any> {
    console.log(order);
    return this.http.post(`${this.apiUrl}/order`, order);
  }
} 