import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'login',
    loadComponent: () => import('./components/login/login.component')
      .then(m => m.LoginComponent)
  },
  {
    path: 'orders/new',
    loadComponent: () => import('./components/new-order/new-order.component')
      .then(m => m.NewOrderComponent)
  },
  { path: '', redirectTo: '/login', pathMatch: 'full' }
];
