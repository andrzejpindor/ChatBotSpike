import { Pipe, PipeTransform } from '@angular/core';
import {MessageType} from '../../services/dto/models';

@Pipe({
  name: 'messageType'
})
export class MessageTypePipe implements PipeTransform {

  transform(value: MessageType): unknown {
    switch (value) {
      case 'system':
        return 'ChatBot: ';
      case 'user':
        return 'You: ';
      default:
        return '?: ';
    }
  }

}
