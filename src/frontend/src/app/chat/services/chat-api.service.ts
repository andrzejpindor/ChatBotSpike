import {HttpClient} from '@angular/common/http';
import {inject, Injectable} from '@angular/core';
import {Observable, of} from 'rxjs';
import {environment} from '../../../environments/environment';
import {ChatDetailsDto, ChatListItemDto} from './dto/models';

@Injectable({ providedIn: 'root' })
export class ChatApiService {
  private readonly httpClient = inject(HttpClient);

  getChatList(): Observable<ChatListItemDto[]> {
    return this.httpClient.get<ChatListItemDto[]>(
      `${environment.apiBaseUrl}/chats`
    );
  }

  getChatDetails(request: { chatId?: string }): Observable<ChatDetailsDto | null> {
    if (!request.chatId) {
      return of(null);
    }

    return this.httpClient.get<ChatDetailsDto>(
      `${environment.apiBaseUrl}/chats/${request.chatId}`
    );
  }

  setMessageReaction(request: {messageId: string, reaction: 'like' | 'dislike'}): Observable<void> {
    return this.httpClient.post<void>(
      `${environment.apiBaseUrl}/messages/${request.messageId}/reaction`,
      {reactionType: request.reaction}
    );
  }
}
