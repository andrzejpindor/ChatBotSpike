import {Component, input, output} from '@angular/core';
import {FormsModule} from "@angular/forms";
import {MatButton} from "@angular/material/button";
import {MatInput} from "@angular/material/input";
import {MatFormFieldModule} from '@angular/material/form-field';

@Component({
  selector: 'app-chat-controls',
  imports: [
    FormsModule,
    MatButton,
    MatFormFieldModule,
    MatInput
  ],
  templateUrl: './chat-controls.component.html',
  styleUrl: './chat-controls.component.scss'
})
export class ChatControlsComponent {
  sendEnabled = input.required<boolean>();
  stopEnabled = input.required<boolean>();
  protected userInput: string | null = null;

  sendMessage = output<string | null>();
  stop = output();

  clearInput() {
    this.userInput = '';
  }

  protected onKeyDown($event: KeyboardEvent) {
    if (!this.sendEnabled() || $event.key !== 'Enter') {
      return;
    }

    $event.preventDefault();
    this.send();
  }

  protected send() {
    if (!this.sendEnabled() || !this.userInput?.trim()) {
      return;
    }
    this.sendMessage.emit(this.userInput);
  }
}

