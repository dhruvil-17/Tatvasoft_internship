import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="max-width: 500px; margin: auto; padding: 2rem;">

    <h2>Register</h2>
      <form (ngSubmit)="onRegister()" #registerForm="ngForm">
        <label>Email:</label>
        <input type="email" [(ngModel)]="registerEmail" name="registerEmail" required /><br /><br />

        <label>Password:</label>
        <input type="password" [(ngModel)]="registerPassword" name="registerPassword" required /><br /><br />

        <button type="submit">Register</button>
      </form>
      <p *ngIf="registerMessage">{{ registerMessage }}</p>

      <hr>
      <h2>Login</h2>
      <form (ngSubmit)="onLogin()" #loginForm="ngForm">
        <label>Email:</label>
        <input type="email" [(ngModel)]="loginEmail" name="loginEmail" required /><br /><br />

        <label>Password:</label>
        <input type="password" [(ngModel)]="loginPassword" name="loginPassword" required /><br /><br />

        <button type="submit">Login</button>
      </form>
      <p *ngIf="loginMessage">{{ loginMessage }}</p>

      <hr />
    <hr />
<h2>Access Secure Data</h2>
<button (click)="getSecureData()">Fetch Secure Data</button>
<p *ngIf="secureDataMessage">{{ secureDataMessage }}</p>

      
    </div>
  `
})
export class LoginComponent {
  loginEmail = '';
  loginPassword = '';
  registerEmail = '';
  registerPassword = '';

  loginMessage = '';
  registerMessage = '';

  constructor(private http: HttpClient) { }

  onLogin() {
    const payload = {
      email: this.loginEmail,
      password: this.loginPassword
    };

    this.http.post<any>('http://localhost:5173/api/Users/login', payload).subscribe({
      next: (res) => {
        this.loginMessage = res.message || 'Login successful!';
        localStorage.setItem('authToken', res.token);  // ✅ Save JWT Token
      },
      error: (err) => {
        this.loginMessage = 'Login failed!';
        console.error(err);
      }
    });
  }


  onRegister() {
    const payload = {
      id: 0,
      email: this.registerEmail,
      password: this.registerPassword
    };

    this.http.post<any>('http://localhost:5173/api/Users/register', payload).subscribe({
      next: (res) => {
        this.registerMessage = res.message || 'Registration successful!';
      },
      error: (err) => {
        this.registerMessage = 'Registration failed!';
        console.error(err);
      }
    });
  }
  secureDataMessage = '';

  getSecureData() {
    const token = localStorage.getItem('authToken'); // get saved token

    if (!token) {
      this.secureDataMessage = 'No token found. Please login first.';
      return;
    }

    const headers = {
      'Authorization': 'Bearer ' + token
    };

    this.http.get<any>('http://localhost:5173/api/Users/secure-data', { headers }).subscribe({
      next: (res) => {
        this.secureDataMessage = res.message;
      },
      error: (err) => {
        this.secureDataMessage = 'Access denied or invalid token';
        console.error(err);
      }
    });
  }

}
