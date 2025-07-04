import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // ✅ Add this

@Component({
  selector: 'app-form',
  standalone: true,
  imports: [FormsModule, CommonModule], // ✅ Add CommonModule here
  templateUrl: './form.html',
  styleUrl: './form.css'
})
export class Form {
  email: string = '';
  password: string = '';
  message: string = '';

  login() {
    if (this.email !='' && this.password != '') {
      console.log(this.email + "/n" + this.password);
      
    } else {
      console.log("Enter credentials");
      
    }
  }
}
