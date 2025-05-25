import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OrderService, Product, OrderRequest } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-new-order',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container mt-4">
      <h2>Create New Order</h2>
      <form [formGroup]="orderForm" (ngSubmit)="onSubmit()" class="mt-4">
        <div class="mb-3">
          <label for="product" class="form-label">Select Product</label>
          <select 
            id="product" 
            formControlName="productId" 
            class="form-select"
            (change)="onProductChange()">
            <option value="">Choose a product...</option>
            <option *ngFor="let product of products" [value]="product.id">
              {{product.name}} - ${{product.price}}
            </option>
          </select>
        </div>

        <div class="mb-3">
          <label for="quantity" class="form-label">Quantity</label>
          <input 
            type="number" 
            id="quantity" 
            formControlName="quantity" 
            class="form-control"
            (change)="updateTotalAmount()">
        </div>

        <div class="mb-3">
          <label class="form-label">Total Amount</label>
          <p class="form-control-static">${{totalAmount}}</p>
        </div>

        <button 
          type="submit" 
          class="btn btn-primary"
          [disabled]="!orderForm.valid">
          Place Order
        </button>
      </form>
    </div>
  `,
  styles: [`
    .container {
      max-width: 600px;
    }
    .form-control-static {
      padding: 0.375rem 0;
      font-weight: bold;
    }
  `]
})
export class NewOrderComponent implements OnInit {
  orderForm: FormGroup;
  products: Product[] = [];
  totalAmount: number = 0;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    private router: Router
  ) {
    this.orderForm = this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.products = this.orderService.getProducts();
  }

  onProductChange(): void {
    this.updateTotalAmount();
  }

  updateTotalAmount(): void {
    const productId = Number(this.orderForm.get('productId')?.value);
    const quantity = Number(this.orderForm.get('quantity')?.value);
    
    const selectedProduct = this.products.find(p => p.id === productId);
    if (selectedProduct && quantity > 0) {
      this.totalAmount = selectedProduct.price * quantity;
    } else {
      this.totalAmount = 0;
    }
  }

  onSubmit(): void {
    if (this.orderForm.valid) {
      const orderRequest: OrderRequest = {
        productId: Number(this.orderForm.get('productId')?.value),
        quantity: Number(this.orderForm.get('quantity')?.value),
        totalAmount: this.totalAmount
      };

      this.orderService.addOrder(orderRequest).subscribe({
        next: (response) => {
          console.log('Order created successfully:', response);
          // Navigate to orders list or show success message
          this.router.navigate(['/orders']);
        },
        error: (error) => {
          console.error('Error creating order:', error);
          // Handle error (show error message to user)
        }
      });
    }
  }
} 