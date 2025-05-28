import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormsModule } from '@angular/forms';
import { OrderService, Product, OrderRequest } from '../../services/order.service';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-new-order',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule
  ],
  templateUrl: './new-order.component.html',
  styleUrl: './new-order.component.css'
})
export class NewOrderComponent implements OnInit {
  orderForm: FormGroup;
  products: Product[] = [];
  totalAmount: number = 0;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    public router: Router
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
        products: this.products.map(product => ({
          id: product.id,
          quantity: product.quantity
        })),
        customerId: 1
      };

      this.orderService.addOrder(orderRequest).subscribe({
        
        next: (response) => {
          console.log('Order created successfully:', response);
          // Navigate to orders list or show success message
          this.router.navigate(['/orders']);
        },
        error: (error) => {
          console.log(orderRequest);
          console.error('Error creating order:', error);
          // Handle error (show error message to user)
        }
      });
    }
  }
} 