import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { SessionStorageService } from '../services/session-storage.service';
import { RealEstateService } from '../services/real-estate-management.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(private fb: FormBuilder,
    private router: Router,
    private sessionStorageService: SessionStorageService,
    private realEstateService: RealEstateService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.realEstateService.login(this.loginForm).subscribe(
        {
          next: response => {
            this.sessionStorageService.setTokenSessionStorage(response.token);
            this.router.navigate(['/dashboard']);
          },
          error: err => {
            this.errorMessage = 'Invalid credentials!'
          }
        }
      )
    } else {
      this.errorMessage = 'Please fill in all fields.';
    }
  }
}
