import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { TaskDetails } from '../../services/task.service';
import { EmployeeListItem } from '../../services/employee.service';
import { DropdownModule } from 'primeng/dropdown';

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, DropdownModule],
  templateUrl: './task-form.component.html',
  styleUrl: './task-form.component.css'
})
export class TaskFormComponent implements OnChanges {
  @Input() task: TaskDetails | null = null;
  @Input() employees: EmployeeListItem[] = [];
  @Input() visible = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Omit<TaskDetails, 'employeeTaskId' | 'createdDate' | 'employeeName'>>();

  taskForm: FormGroup;
  priorities: string[] = ['Low', 'Medium', 'High'];
  statuses: string[] = ['Pending', 'In Progress', 'Completed'];

  constructor(private fb: FormBuilder) {
    this.taskForm = this.fb.group({
      employeeId: ['', Validators.required],
      title: ['', [Validators.required, Validators.maxLength(200)]],
      description: [''],
      priority: ['', Validators.required],
      status: ['', Validators.required],
      startDate: ['', Validators.required],
      dueDate: ['', Validators.required],
      estimatedHours: [0, [Validators.required, Validators.min(0), Validators.max(1000)]]
    }, { validators: [this.dateRangeValidator, this.completedStatusValidator.bind(this)] });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['task'] && this.task) {
      this.taskForm.patchValue({
        employeeId: this.task.employeeId,
        title: this.task.title,
        description: this.task.description,
        priority: this.task.priority,
        status: this.task.status,
        startDate: this.formatDate(this.task.startDate),
        dueDate: this.formatDate(this.task.dueDate),
        estimatedHours: this.task.estimatedHours
      });
    } else if (changes['visible'] && this.visible && !this.task) {
      this.taskForm.reset({
        employeeId: '',
        title: '',
        description: '',
        priority: '',
        status: 'Pending',
        startDate: '',
        dueDate: '',
        estimatedHours: 0
      });
    }
  }

  // Date validation: Due Date cannot be earlier than Start Date
  dateRangeValidator(group: FormGroup): { [key: string]: boolean } | null {
    const start = group.get('startDate')?.value;
    const due = group.get('dueDate')?.value;
    if (start && due) {
      const startDate = new Date(start);
      const dueDate = new Date(due);
      if (dueDate < startDate) {
        return { invalidDateRange: true };
      }
    }
    return null;
  }

  // Status validation: A Completed task cannot be changed back to Pending
  completedStatusValidator(group: FormGroup): { [key: string]: boolean } | null {
    if (this.task && this.task.status === 'Completed') {
      const nextStatus = group.get('status')?.value;
      if (nextStatus === 'Pending') {
        return { completedToPending: true };
      }
    }
    return null;
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    const date = new Date(dateStr);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  onSubmit(): void {
    if (this.taskForm.valid) {
      const formValue = this.taskForm.value;
      formValue.employeeId = +formValue.employeeId;
      formValue.startDate = new Date(formValue.startDate).toISOString();
      formValue.dueDate = new Date(formValue.dueDate).toISOString();
      formValue.estimatedHours = +formValue.estimatedHours;
      this.save.emit(formValue);
    } else {
      this.taskForm.markAllAsTouched();
    }
  }

  onCancel(): void {
    this.close.emit();
  }
}
