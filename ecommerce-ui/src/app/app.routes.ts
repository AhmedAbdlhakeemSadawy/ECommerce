import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'orders/new',
    loadComponent: () => import('./components/new-order/new-order.component')
      .then(m => m.NewOrderComponent)
  },
  { path: '', redirectTo: '/orders/new', pathMatch: 'full' }
];
