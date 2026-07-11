import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskService, TaskDetails } from '../../services/task.service';
import { EmployeeService, EmployeeListItem } from '../../services/employee.service';
import { TaskFormComponent } from '../task-form/task-form.component';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [CommonModule, FormsModule, TaskFormComponent, ConfirmDialogModule, DropdownModule],
  providers: [ConfirmationService],
  templateUrl: './task-list.component.html',
  styleUrl: './task-list.component.css'
})
export class TaskListComponent implements OnInit {
  tasks: TaskDetails[] = [];
  employees: EmployeeListItem[] = [];

  // Filter fields
  selectedStatus = '';
  selectedEmployeeId = 0;

  // Pagination fields
  pageNumber = 1;
  pageSize = 10;

  // Modals state
  formVisible = false;
  selectedTask: TaskDetails | null = null;

  constructor(
    private taskService: TaskService,
    private employeeService: EmployeeService,
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      if (params['status']) {
        this.selectedStatus = params['status'];
      } else {
        this.selectedStatus = '';
      }
      this.loadTasks();
    });
    this.loadEmployees();
  }

  loadTasks(): void {
    const apiStatus = (this.selectedStatus && this.selectedStatus !== 'Overdue') ? this.selectedStatus : undefined;
    
    this.taskService.getAllTasks(
      apiStatus,
      this.selectedEmployeeId || undefined
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          let list = res.data;
          if (this.selectedStatus === 'Overdue') {
            list = list.filter(t => this.isOverdue(t));
          }
          this.tasks = list;
          this.pageNumber = 1; // Reset to page 1 on search reload
        } else {
          this.showErrorAlert(res.message || 'Failed to load task list.');
        }
      },
      error: (err) => {
        this.showErrorAlert(err.error?.message || 'Error fetching tasks.');
      }
    });
  }

  loadEmployees(): void {
    // Fetch all active employees for selection dropdowns
    this.employeeService.getEmployeeList({ pageNumber: 1, pageSize: 1000, isActive: true }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.employees = res.data.employees;
        }
      }
    });
  }

  onFilterChange(): void {
    this.loadTasks();
  }

  onResetFilters(): void {
    this.selectedStatus = '';
    this.selectedEmployeeId = 0;
    this.pageNumber = 1;
    this.loadTasks();
  }

  openCreateForm(): void {
    this.selectedTask = null;
    this.formVisible = true;
  }

  openEditForm(task: TaskDetails): void {
    this.taskService.getTaskById(task.employeeTaskId).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.selectedTask = res.data;
          this.formVisible = true;
        } else {
          this.showErrorAlert(res.message || 'Failed to load task details.');
        }
      },
      error: (err) => {
        this.showErrorAlert(err.error?.message || 'Error fetching task details.');
      }
    });
  }

  onFormClose(): void {
    this.formVisible = false;
    this.selectedTask = null;
  }

  onFormSave(payload: Omit<TaskDetails, 'employeeTaskId' | 'createdDate' | 'employeeName'>): void {
    if (this.selectedTask) {
      // Update
      this.taskService.updateTask(this.selectedTask.employeeTaskId, payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSuccessAlert('Task updated successfully.');
            this.formVisible = false;
            this.selectedTask = null;
            this.loadTasks();
          } else {
            this.showErrorAlert(res.message || 'Failed to update task.');
          }
        },
        error: (err) => {
          this.showErrorAlert(err.error?.message || 'Error updating task.');
        }
      });
    } else {
      // Create
      this.taskService.createTask(payload).subscribe({
        next: (res) => {
          if (res.success) {
            this.showSuccessAlert('Task created successfully.');
            this.formVisible = false;
            this.loadTasks();
          } else {
            this.showErrorAlert(res.message || 'Failed to create task.');
          }
        },
        error: (err) => {
          this.showErrorAlert(err.error?.message || 'Error creating task.');
        }
      });
    }
  }

  onDelete(task: TaskDetails): void {
    this.confirmationService.confirm({
      message: `Are you sure you want to delete task "${task.title}"?`,
      header: 'Confirm Delete',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Yes',
      rejectLabel: 'No',
      acceptButtonStyleClass: 'p-button-danger p-button-sm',
      rejectButtonStyleClass: 'p-button-secondary p-button-outlined p-button-sm me-2',
      accept: () => {
        this.taskService.deleteTask(task.employeeTaskId).subscribe({
          next: (res) => {
            if (res.success) {
              this.showSuccessAlert('Task deleted successfully.');
              this.loadTasks();
            } else {
              this.showErrorAlert(res.message || 'Failed to delete task.');
            }
          },
          error: (err) => {
            this.showErrorAlert(err.error?.message || 'Error deleting task.');
          }
        });
      }
    });
  }

  // Allow inline status update
  onStatusUpdate(task: TaskDetails, nextStatus: string): void {
    if (task.status === 'Completed' && nextStatus === 'Pending') {
      this.showErrorAlert('A Completed task cannot be changed back to Pending.');
      return;
    }

    this.taskService.updateTaskStatus(task.employeeTaskId, nextStatus).subscribe({
      next: (res) => {
        if (res.success) {
          this.showSuccessAlert('Task status updated successfully.');
          this.loadTasks();
        } else {
          this.showErrorAlert(res.message || 'Failed to update task status.');
        }
      },
      error: (err) => {
        this.showErrorAlert(err.error?.message || 'Error updating task status.');
      }
    });
  }

  isOverdue(task: TaskDetails): boolean {
    if (task.status === 'Completed') return false;
    const due = new Date(task.dueDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return due < today;
  }

  showSuccessAlert(msg: string): void {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: msg });
  }

  showErrorAlert(msg: string): void {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: msg });
  }

  onPageChange(page: number): void {
    this.pageNumber = page;
  }

  get totalPages(): number {
    return Math.ceil(this.tasks.length / this.pageSize) || 1;
  }
}
