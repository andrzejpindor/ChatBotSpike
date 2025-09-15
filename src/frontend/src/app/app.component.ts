import {Component} from '@angular/core';
import {RouterLink, RouterOutlet} from '@angular/router';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatButton} from '@angular/material/button';
import {MatDividerModule} from '@angular/material/divider';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, MatToolbarModule, MatButton, MatDividerModule, RouterLink],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  onNewChat() {
    window.location.href = '/chat/new';
  }
}
