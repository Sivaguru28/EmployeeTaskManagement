import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface EmployeeListItem {
  employeeId: number;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber: string;
  department: string;
  designation: string;
  dateOfJoining: string;
  isActive: boolean;
  totalAssignedTasks: number;
}

export interface EmployeeDetails {
  employeeId: number;
  employeeCode: string;
  firstName: string;
  lastName: string;
  email: string;
  mobileNumber: string;
  department: string;
  designation: string;
  dateOfJoining: string;
  isActive: boolean;
  createdDate: string;
}

export interface GetEmployeeListRequest {
  searchText?: string;
  department?: string;
  isActive?: boolean;
  pageNumber: number;
  pageSize: number;
}

export interface GetEmployeeListResponse {
  employees: EmployeeListItem[];
  totalRecords: number;
}

export interface ApiResponse<T> {
  success: boolean;
  statusCode: number;
  message: string;
  data?: T;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private readonly apiUrl = `${environment.apiUrl}/Employee`;

  constructor(private http: HttpClient) { }

  getEmployeeList(request: GetEmployeeListRequest): Observable<ApiResponse<GetEmployeeListResponse>> {
    return this.http.post<ApiResponse<GetEmployeeListResponse>>(`${this.apiUrl}/GetEmployeeList`, request);
  }

  getEmployeeById(id: number): Observable<ApiResponse<EmployeeDetails>> {
    return this.http.post<ApiResponse<EmployeeDetails>>(`${this.apiUrl}/Get`, { id });
  }

  createEmployee(employee: Omit<EmployeeDetails, 'employeeId' | 'createdDate'>): Observable<ApiResponse<EmployeeDetails>> {
    return this.http.post<ApiResponse<EmployeeDetails>>(`${this.apiUrl}/Create`, employee);
  }

  updateEmployee(id: number, employee: Omit<EmployeeDetails, 'employeeId' | 'createdDate'>): Observable<ApiResponse<EmployeeDetails>> {
    const payload = { employeeId: id, ...employee };
    return this.http.post<ApiResponse<EmployeeDetails>>(`${this.apiUrl}/Update`, payload);
  }

  deleteEmployee(id: number): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/Delete`, { id });
  }

  getNextEmployeeCode(): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/NextCode`, {});
  }
}
