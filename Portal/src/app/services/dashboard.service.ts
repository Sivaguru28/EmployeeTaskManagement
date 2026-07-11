import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from './employee.service';
import { environment } from '../../environments/environment';

export interface DashboardStats {
  totalEmployees: number;
  activeEmployees: number;
  pendingTasks: number;
  completedTasks: number;
  overdueTasks: number;
}

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private readonly apiUrl = `${environment.apiUrl}/Dashboard`;

  constructor(private http: HttpClient) { }

  getDashboardStats(): Observable<ApiResponse<DashboardStats>> {
    return this.http.post<ApiResponse<DashboardStats>>(`${this.apiUrl}/GetStats`, {});
  }
}
