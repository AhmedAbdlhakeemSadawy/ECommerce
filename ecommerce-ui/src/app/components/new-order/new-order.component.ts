import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormsModule, FormArray } from '@angular/forms';
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
  message: string = '';
  isSuccess: boolean = false;

  constructor(
    private fb: FormBuilder,
    private orderService: OrderService,
    public router: Router
  ) {
    this.orderForm = this.fb.group({
      items: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.products = this.orderService.getProducts();
    this.addProductItem(); // Add initial product selection
  }

  get items() {
    return this.orderForm.get('items') as FormArray;
  }

  createProductItem(): FormGroup {
    return this.fb.group({
      productId: ['', Validators.required],
      quantity: [1, [Validators.required, Validators.min(1)]]
    });
  }

  addProductItem(): void {
    this.items.push(this.createProductItem());
  }

  removeProductItem(index: number): void {
    this.items.removeAt(index);
    this.updateTotalAmount();
  }

  onProductChange(): void {
    this.updateTotalAmount();
  }

  updateTotalAmount(): void {
    this.totalAmount = 0;
    this.items.controls.forEach(item => {
      const productId = Number(item.get('productId')?.value);
      const quantity = Number(item.get('quantity')?.value);
      
      const selectedProduct = this.products.find(p => p.id === productId);
      if (selectedProduct && quantity > 0) {
        this.totalAmount += selectedProduct.price * quantity;
      }
    });
  }

  onSubmit(): void {
    if (this.orderForm.valid) {
      const orderRequest: OrderRequest = {
        products: this.items.controls.map(item => ({
          id: Number(item.get('productId')?.value),
          quantity: Number(item.get('quantity')?.value)
        })),
        customerId: 1
      };

      this.orderService.addOrder(orderRequest).subscribe({
        next: (response) => {
          this.message = response.message;
          this.isSuccess = true;
          // Navigate to orders list after a short delay
          setTimeout(() => {
            this.router.navigate(['/orders']);
          }, 2000);
        },
        error: (response) => {
          console.error('Error creating order:', response);

          this.message = 'Error creating order: ' + (response?.error.Message || 'Unknown error');
          this.isSuccess = false;
          console.error('Error creating order:', response);
        }
      });
    }
  }
} 