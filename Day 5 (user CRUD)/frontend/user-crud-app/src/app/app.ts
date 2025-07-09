import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { UserCrudComponent } from './user-crud/user-crud'; // ✅ Import your component

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, UserCrudComponent], // ✅ Add to imports
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'user-crud-app';
}
