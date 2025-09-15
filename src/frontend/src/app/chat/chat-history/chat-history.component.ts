import {Component, inject} from '@angular/core';
import {ChatApiService} from '../services/chat-api.service';
import {MatListModule} from '@angular/material/list';
import {MatCardModule} from '@angular/material/card';
import {AsyncPipe} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-chat-history',
  imports: [MatListModule, MatCardModule, AsyncPipe, RouterLink],
  templateUrl: './chat-history.component.html',
  styleUrl: './chat-history.component.scss'
})
export class ChatHistoryComponent {
  chatService = inject(ChatApiService);
  chatList$ = this.chatService.getChatList();
}
