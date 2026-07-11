import { Component, Input, Output, EventEmitter, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { EmployeeDetails, EmployeeService } from '../../services/employee.service';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './employee-form.component.html',
  styleUrl: './employee-form.component.css'
})
export class EmployeeFormComponent implements OnChanges {
  @Input() employee: EmployeeDetails | null = null;
  @Input() visible = false;
  @Output() close = new EventEmitter<void>();
  @Output() save = new EventEmitter<Omit<EmployeeDetails, 'employeeId' | 'createdDate'>>();

  employeeForm: FormGroup;
  departments: string[] = ['IT', 'HR', 'Finance', 'Marketing', 'Operations', 'Sales'];

  constructor(private fb: FormBuilder, private employeeService: EmployeeService) {
    this.employeeForm = this.fb.group({
      employeeCode: ['', [Validators.required, Validators.maxLength(50)]],
      firstName: ['', [Validators.required, Validators.maxLength(100)]],
      lastName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(256)]],
      mobileNumber: ['', [Validators.required, Validators.pattern(/^[0-9+\-\s()]*$/), Validators.maxLength(20)]],
      department: ['', Validators.required],
      designation: ['', [Validators.required, Validators.maxLength(100)]],
      dateOfJoining: ['', [Validators.required, this.futureDateValidator]],
      isActive: [true]
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['employee'] && this.employee) {
      this.employeeForm.patchValue({
        employeeCode: this.employee.employeeCode,
        firstName: this.employee.firstName,
        lastName: this.employee.lastName,
        email: this.employee.email,
        mobileNumber: this.employee.mobileNumber,
        department: this.employee.department,
        designation: this.employee.designation,
        dateOfJoining: this.formatDate(this.employee.dateOfJoining),
        isActive: this.employee.isActive
      });
    } else if (changes['visible'] && this.visible && !this.employee) {
      this.employeeForm.reset({
        employeeCode: '',
        firstName: '',
        lastName: '',
        email: '',
        mobileNumber: '',
        department: '',
        designation: '',
        dateOfJoining: '',
        isActive: true
      });
      // Auto-bind Next Employee Code from DB
      this.employeeService.getNextEmployeeCode().subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.employeeForm.patchValue({ employeeCode: response.data });
          }
        },
        error: (err) => console.error('Failed to auto-bind employee code', err)
      });
    }
  }

  futureDateValidator(control: any): { [key: string]: boolean } | null {
    if (control.value) {
      const selectedDate = new Date(control.value);
      const today = new Date();
      today.setHours(0, 0, 0, 0);
      if (selectedDate > today) {
        return { futureDate: true };
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
    if (this.employeeForm.valid) {
      const formValue = this.employeeForm.value;
      // Map back input timezone properly
      formValue.dateOfJoining = new Date(formValue.dateOfJoining).toISOString();
      this.save.emit(formValue);
    } else {
      this.employeeForm.markAllAsTouched();
    }
  }

  onCancel(): void {
    this.close.emit();
  }
}
