import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from './employee.service';
import { environment } from '../../environments/environment';

export interface TaskDetails {
  employeeTaskId: number;
  employeeId: number;
  title: string;
  description: string;
  priority: string;
  status: string;
  startDate: string;
  dueDate: string;
  estimatedHours: number;
  createdDate: string;
  employeeName: string;
}

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private readonly apiUrl = `${environment.apiUrl}/Task`;

  constructor(private http: HttpClient) { }

  getAllTasks(status?: string, employeeId?: number): Observable<ApiResponse<TaskDetails[]>> {
    const payload = {
      status: status || null,
      employeeId: employeeId && employeeId > 0 ? employeeId : null
    };
    return this.http.post<ApiResponse<TaskDetails[]>>(`${this.apiUrl}/List`, payload);
  }

  getTaskById(id: number): Observable<ApiResponse<TaskDetails>> {
    return this.http.post<ApiResponse<TaskDetails>>(`${this.apiUrl}/Get`, { id });
  }

  getTasksByEmployee(employeeId: number): Observable<ApiResponse<TaskDetails[]>> {
    return this.http.post<ApiResponse<TaskDetails[]>>(`${this.apiUrl}/GetByEmployee`, { employeeId });
  }

  createTask(task: Omit<TaskDetails, 'employeeTaskId' | 'createdDate' | 'employeeName'>): Observable<ApiResponse<TaskDetails>> {
    return this.http.post<ApiResponse<TaskDetails>>(`${this.apiUrl}/Create`, task);
  }

  updateTask(id: number, task: Omit<TaskDetails, 'employeeTaskId' | 'createdDate' | 'employeeName'>): Observable<ApiResponse<TaskDetails>> {
    const payload = { id, ...task };
    return this.http.post<ApiResponse<TaskDetails>>(`${this.apiUrl}/Update`, payload);
  }

  updateTaskStatus(id: number, status: string): Observable<ApiResponse<TaskDetails>> {
    return this.http.post<ApiResponse<TaskDetails>>(`${this.apiUrl}/UpdateStatus`, { id, status });
  }

  deleteTask(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/Delete`, { id });
  }
}
