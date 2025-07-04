import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./header/header";
import { Footer } from "./footer/footer";
import {Form} from "./form/form"


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Footer , Form],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected title = 'my-angular-app';
}
