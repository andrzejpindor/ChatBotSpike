import {Component, input} from '@angular/core';
import {MessageTypePipe} from '../../utils/message-type.pipe';

@Component({
  selector: 'app-streaming-message',
  imports: [
    MessageTypePipe
  ],
  templateUrl: './streaming-message.component.html',
  styleUrl: './streaming-message.component.scss'
})
export class StreamingMessageComponent {
  streamingContent = input.required<string>();
}
