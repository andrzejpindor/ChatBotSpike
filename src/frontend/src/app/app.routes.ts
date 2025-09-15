import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: 'chats',
    loadComponent: () =>
      import('./chat/chat-history/chat-history.component').then((x) => x.ChatHistoryComponent),
    title: 'ChatBot - history',
  },{
    path: 'chat/new',
    loadComponent: () =>
      import('./chat/chat/chat.component').then((x) => x.ChatComponent),
    title: 'ChatBot - Chat',
  },
  {
    path: 'chat/:chatId',
    loadComponent: () =>
      import('./chat/chat/chat.component').then((x) => x.ChatComponent),
    title: 'ChatBot - Chat',
  },
  {
    path: '**',
    redirectTo: '/chat/new',
    pathMatch: 'full',
  },

];
