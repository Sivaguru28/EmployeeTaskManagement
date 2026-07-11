import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  username = 'admin';
  password = 'Admin@123';
  loading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private messageService: MessageService
  ) {
    // If already logged in, redirect to dashboard
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(): void {
    if (!this.username || !this.password) {
      this.messageService.add({ severity: 'error', summary: 'Validation Error', detail: 'Please enter both username and password.' });
      return;
    }

    this.loading = true;

    this.authService.login(this.username, this.password).subscribe({
      next: (res) => {
        this.loading = false;
        if (res.success) {
          this.router.navigate(['/dashboard']);
        } else {
          this.messageService.add({ severity: 'error', summary: 'Login Failed', detail: res.message || 'Login failed.' });
        }
      },
      error: (err) => {
        this.loading = false;
        this.messageService.add({ severity: 'error', summary: 'System Error', detail: err.error?.message || 'An error occurred during authentication.' });
      }
    });
  }
}
