import {Component, input, output} from '@angular/core';
import {MessageTypePipe} from "../../utils/message-type.pipe";
import {ChatMessage} from '../../../services/dto/models';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';

@Component({
  selector: 'app-message',
  imports: [
    MessageTypePipe,
    MatIconModule,
    MatButtonModule
  ],
  templateUrl: './message.component.html',
  styleUrl: './message.component.scss'
})
export class MessageComponent {
  message = input.required<ChatMessage>();
  reaction = output<'like' | 'dislike'>();
}
