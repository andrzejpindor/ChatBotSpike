import {Injectable} from "@angular/core";
import {Observable, Subscriber, throwError} from "rxjs";
import {environment} from '../../../environments/environment';
import {EventStreamContentType, fetchEventSource} from '@microsoft/fetch-event-source';
import {CompletionChunk, CompletionRequest} from './dto/models';


@Injectable()
export class ChatCompletionStreamingService {
  private completionInProgress = false;

  startCompletion(request: CompletionRequest): Observable<CompletionChunk> {
    if (!request) return throwError(() => new Error('Request is required'));
    if (!request.text?.trim()) {
      return throwError(() => new Error('Text is required'));
    }

    if (this.completionInProgress) {
      return throwError(() => new Error('Completion already in progress'));
    }


    return this.startServerSentEvents(request);
  }

  private startServerSentEvents(request: CompletionRequest): Observable<CompletionChunk> {
    const controller = new AbortController();

    return new Observable<CompletionChunk>((subscriber: Subscriber<CompletionChunk>) => {
      this.completionInProgress = true;

      let url = `${environment.apiBaseUrl}/chats/completions`;
      if (request.chatId) {
        url += `/${request.chatId}`;
      }

      void fetchEventSource(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({message: request.text}),
        signal: controller.signal,
        async onopen(response) {
          if (response.ok && response.headers.get('content-type') === EventStreamContentType) {
            return;
          } else {
            throw new Error(`Failed to open SSE connection. Status: ${response.status}`);
          }
        },
        onmessage(msg) {
          if (msg.event === 'error') {
            throw new Error(msg.data);
          }

          switch (msg.event) {
            case 'delta':
              subscriber.next({type: 'delta', content: JSON.parse(msg.data).content});
              break;
            case 'metadata':
              const metadata = JSON.parse(msg.data)
              subscriber.next({
                type: 'metadata',
                ...metadata
              });
              break;
            case 'finish':
              subscriber.next({type: 'done', done: true});
              subscriber.complete();
              break;
            default:
              console.warn('Unknown event type:', msg.event);
          }

        },
        onerror(err) {
          subscriber.error(err.message);
          throw err;
        }
      });

      return () => {
        this.cleanup(controller);
      };
    });
  }

  private cleanup(cancellation: AbortController) {
    try {
      cancellation.abort();
    } catch (e) {
      console.error(e);
    } finally {
      this.completionInProgress = false;
    }
  }
}
