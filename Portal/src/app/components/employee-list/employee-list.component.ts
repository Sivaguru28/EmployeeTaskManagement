import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeService, EmployeeListItem, EmployeeDetails, GetEmployeeListRequest } from '../../services/employee.service';
import { EmployeeFormComponent } from '../employee-form/employee-form.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule, EmployeeFormComponent, ConfirmDialogModule],
  providers: [ConfirmationService],
  templateUrl: './employee-list.component.html',
  styleUrl: './employee-list.component.css'
})
export class EmployeeListComponent implements OnInit {
  employees: EmployeeListItem[] = [];
  totalRecords = 0;

  // Search and Filter fields
  searchText = '';
  selectedDepartment = '';
  showInactive = false;

  // Pagination fields
  pageNumber = 1;
  pageSize = 5;
  departments: string[] = ['IT', 'HR', 'Finance', 'Marketing', 'Operations', 'Sales'];

  // Form Modal fields
  formVisible = false;
  selectedEmployee: EmployeeDetails | null = null;

  constructor(
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) { }

  ngOnInit(): void {
    this.loadEmployees();
  }

  loadEmployees(): void {
    const isActiveVal = !this.showInactive;

    const request: GetEmployeeListRequest = {
      searchText: this.searchText || undefined,
      department: this.selectedDepartment || undefined,
      isActive: isActiveVal,
      pageNumber: this.pageNumber,
      pageSize: this.pageSize
    };

    this.employeeService.getEmployeeList(request).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employees = res.data.employees;
          this.totalRecords = res.data.totalRecords;
        } else {
          this.showErrorAlert(res.message || 'Failed to load employee list.');
        }
      },
      error: (err) => {
        this.showErrorAlert(err.error?.message || 'Error occurred while fetching employees.');
      }
    });
  }

  onSearch(): void {
    this.pageNumber = 1; // Reset to page 1 for new search
    this.loadEmployees();
  }

  onResetFilters(): void {
    this.searchText = '';
    this.selectedDepartment = '';
    this.showInactive = false;
    this.pageNumber = 1;
    this.loadEmployees();
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
    this.loadEmployees();
  }

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize) || 1;
  }

  openCreateForm(): void {
    this.selectedEmployee = null;
    this.formVisible = true;
  }

  openEditForm(item: EmployeeListItem): void {
    // Fetch detailed model first
    this.employeeService.getEmployeeById(item.employeeId).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.selectedEmployee = res.data;
          this.formVisible = true;
        } else {
          this.showErrorAlert(res.message || 'Failed to load employee details.');
        }
      },
      error: (err) => {
        this.showErrorAlert(err.error?.message || 'Error fetching employee details.');
      }
    });
  }

  onFormClose(): void {
    this.formVisible = false;
    this.selectedEmployee = null;
  }

  onFormSave(payload: Omit<EmployeeDetails, 'employeeId' | 'createdDate'>): void {
    if (this.selectedEmployee) {
      // Update
      const id = this.selectedEmployee.employeeId;
      this.employeeService.updateEmployee(id, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSuccessAlert('Employee updated successfully.');
            this.formVisible = false;
            this.selectedEmployee = null;
            this.loadEmployees();
          } else {
            this.showErrorAlert(res.message || 'Failed to update employee.');
          }
        },
        error: (err) => {
          this.showErrorAlert(err.error?.message || 'Error updating employee.');
        }
      });
    } else {
      // Create
      this.employeeService.createEmployee(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSuccessAlert('Employee created successfully.');
            this.formVisible = false;
            this.loadEmployees();
          } else {
            this.showErrorAlert(res.message || 'Failed to create employee.');
          }
        },
        error: (err) => {
          this.showErrorAlert(err.error?.message || 'Error creating employee.');
        }
      });
    }
  }

  onDelete(item: EmployeeListItem): void {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete employee ${item.firstName} ${item.lastName}? (Note : Task allocated with this Employee also got deleted)`,
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'p-button-danger p-button-sm',
      rejectButtonStyleClass: 'p-button-secondary p-button-outlined p-button-sm me-2',
      accept: () => {
        this.employeeService.deleteEmployee(item.employeeId).subscribe({
          next: (res) => {
            if (res.success) {
              this.showSuccessAlert('Employee deleted successfully.');
              this.loadEmployees();
            } else {
              this.showErrorAlert(res.message || 'Failed to delete employee.');
            }
          },
          error: (err) => {
            this.showErrorAlert(err.error?.message || 'Error deleting employee.');
          }
        });
      }
    });
  }

  showSuccessAlert(msg: string): void {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: msg });
  }

  showErrorAlert(msg: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: msg });
  }
}
