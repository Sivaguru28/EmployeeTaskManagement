import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/login/login.component';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { EmployeeListComponent } from './components/employee-list/employee-list.component';
import { TaskListComponent } from './components/task-list/task-list.component';

export const routes: Routes = [
  {
    path: 'login',
    component : LoginComponent
  },
  {
    path: '',
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component : DashboardComponent
       //loadComponent: () => import('./components/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },
      {
        path: 'employees',
        component : EmployeeListComponent
        //loadComponent: () => import('./components/employee-list/employee-list.component').then(m => m.EmployeeListComponent)
      },
      {
        path: 'tasks',
        component : TaskListComponent
       // loadComponent: () => import('./components/task-list/task-list.component').then(m => m.TaskListComponent)
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
