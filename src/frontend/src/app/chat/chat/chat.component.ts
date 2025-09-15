import {
  ChangeDetectionStrategy,
  Component,
  computed,
  ElementRef,
  inject,
  Input,
  OnDestroy,
  OnInit,
  signal,
  ViewChild
} from '@angular/core';
import {MatButtonModule} from '@angular/material/button';
import {MatInputModule} from '@angular/material/input';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {concatMap, filter, from, interval, map, Subject, take, takeUntil, tap} from 'rxjs';
import {ChatMessage, isMessageDelta, isMetaData} from '../services/dto/models';
import {ChatCompletionStreamingService} from '../services/chat-completion-streaming.service';
import {ChatApiService} from '../services/chat-api.service';
import {MessageComponent} from './ui/message/message.component';
import {MatSnackBar, MatSnackBarModule} from '@angular/material/snack-bar';
import {StreamingMessageComponent} from './ui/streaming-message/streaming-message.component';
import {ChatControlsComponent} from './ui/chat-controls/chat-controls.component';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';


@Component({
  selector: 'app-chat',
  imports: [CommonModule, MatInputModule, MatButtonModule, FormsModule, MessageComponent,
    MatSnackBarModule, StreamingMessageComponent, ChatControlsComponent, MatProgressSpinnerModule
  ],
  providers: [ChatCompletionStreamingService],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ChatComponent implements OnInit, OnDestroy {
  @Input() chatId?: string;

  private chatCompletionService = inject(ChatCompletionStreamingService);
  private snackBar = inject(MatSnackBar);
  private chatService = inject(ChatApiService);
  private stop$ = new Subject<void>();

  messages = signal<ChatMessage[]>([]);
  showIntent = computed(() => this.messages().length === 0);
  isLoading = signal(true);
  isStreaming = signal(false);

  sendEnabled = computed(() => !this.isStreaming());
  stopEnabled = computed(() => this.isStreaming());

  streamingContent = signal('');

  @ViewChild('messagesBox') messagesBox!: ElementRef<HTMLElement>;
  @ViewChild('controls', {static: false}) controls?: ChatControlsComponent;

  ngOnInit(): void {
    this.chatService.getChatDetails({chatId: this.chatId})
      .subscribe(history => {
        this.isLoading.set(false);
        if (!history) {
          return;
        }
        this.messages.set(history.messages);
        this.isLoading.set(false);
      });
  }

  ngOnDestroy(): void {
    this.stop$.next();
    this.stop$.complete();
  }

  onStop() {
    this.stop$.next();
  }

  private scrollToBottom() {
    setTimeout(() => {
      const box = this.messagesBox.nativeElement;
      box.scrollTop = box.scrollHeight;
    }, 0);
  }

  onReactionSet($event: "like" | "dislike", msg: ChatMessage) {
    this.chatService.setMessageReaction({messageId: msg.id, reaction: $event})
      .subscribe({
        next:
          () => {
            this.messages.update(messages =>
              [...messages.map(m => m.id === msg.id ? {...m, reaction: $event} : m)]
            );
          }, error: () => {
          this.snackBar.open('Failed to set reaction', 'Close', {duration: 3000});
        }
      });
  }

  onSend($event: string | null) {
    if (!$event) {
      return;
    }

    const userInput = $event?.trim();

    if (!userInput) return;

    this.streamingContent.set('');
    this.isStreaming.set(true);
    let scrolled = false;
    let streamingMessageId: string | undefined;

    this.chatCompletionService.startCompletion({chatId: this.chatId, text: userInput})
      .pipe(
        tap(event => {
          if (isMetaData(event)) {
            this.chatId = event.chatId;
            streamingMessageId = event.completionMessageId;
            this.acknowledgeUserInput(userInput, event);
            history.pushState({}, '', `/chat/${this.chatId}`);
          }
        }),
        filter(isMessageDelta),
        concatMap(chunk => {
          const chars = chunk.content.split('');
          return from(chars).pipe(
            concatMap(char => interval(10).pipe(
              take(1),
              map(() => {
                if (!scrolled) {
                  this.scrollToBottom();
                }
                scrolled = true;
                return char;
              })
            ))
          );
        }),
        takeUntil(this.stop$)
      )
      .subscribe({
        next: char => {
          this.streamingContent.update(val => (val ?? '') + char);
        },
        complete: () => {
          this.isStreaming.set(false);
          this.appendMessage({
            messageType: 'system' as const,
            content: this.streamingContent(),
            createdAt: new Date().toISOString(),
            id: streamingMessageId!,
            reaction: null
          });
          this.streamingContent.set('');
        },
        error: () => {
          this.snackBar.open('Something went wrong', 'Close', {duration: 3000});
          this.isStreaming.set(false);
        }
      });

  }

  private acknowledgeUserInput(userInput: string, metadata: {
    userMessageId: string;
    chatId: string }) {
    this.appendMessage({
      messageType: 'user',
      content: userInput,
      createdAt: new Date().toISOString(),
      id: metadata.userMessageId,
      reaction: null
    });
    this.controls?.clearInput();
  }

  private appendMessage(msg: ChatMessage) {
    this.messages.update((val) => [...val, msg]);
  }

  async startNewChat() {
    this.stop$.next();
    window.location.href = '/chat/new';
  }
}
