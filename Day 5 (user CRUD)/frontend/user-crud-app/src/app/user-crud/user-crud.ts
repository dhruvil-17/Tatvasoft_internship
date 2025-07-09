import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-user-crud',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './user-crud.html',
  styleUrls: ['./user-crud.css']
})
export class UserCrudComponent {
  users: any[] = [];
  user = { id: 0, email: '', password: '' };
  isEdit = false;
  apiUrl = 'http://localhost:5173/api/Users';

  constructor(private http: HttpClient) {
    this.loadUsers();
  }

  loadUsers() {
    this.http.get<any[]>(this.apiUrl).subscribe(data => {
      this.users = data;
    });
  }

  saveUser() {
    if (this.isEdit) {
      this.http.put(`${this.apiUrl}/${this.user.id}`, this.user).subscribe(() => {
        this.loadUsers();
        this.cancelEdit();
      });
    } else {
      this.http.post(this.apiUrl, this.user).subscribe(() => {
        this.loadUsers();
        this.resetForm();
      });
    }
  }

  editUser(user: any) {
    this.user = { ...user };
    this.isEdit = true;
  }

  deleteUser(id: number) {
    if (confirm('Are you sure you want to delete this user?')) {
      this.http.delete(`${this.apiUrl}/${id}`).subscribe(() => {
        this.loadUsers();
      });
    }
  }

  cancelEdit() {
    this.isEdit = false;
    this.resetForm();
  }

  resetForm() {
    this.user = { id: 0, email: '', password: '' };
  }
}
