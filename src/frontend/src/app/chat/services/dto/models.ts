export interface ChatDetailsDto {
  id: string;
  title?: string;
  messages: ChatMessage[];
}

export interface ChatListItemDto {
  id: string;
  title?: string;
}

export type MessageReaction = 'like' | 'dislike' | null;

export type MessageType = 'user' | 'system';

export interface ChatMessage {
  id: string;
  messageType: MessageType;
  content: string;
  reaction: MessageReaction;
  createdAt: string;
}

export type CompletionRequest = {
  chatId?: string;
  text: string;
};

export type MessageDelta = {
  type: 'delta';
  content: string;
}

export type ChatId = {
  type: 'metadata';
  chatId: string;
  completionMessageId: string;
  userMessageId: string;
}

export type Finish = {
  type: 'done';
  done: true;
}

export type CompletionChunk = MessageDelta | ChatId | Finish;

export const isMessageDelta = (chunk: CompletionChunk): chunk is MessageDelta => {
  return chunk.type === 'delta';
}

export const isMetaData = (chunk: CompletionChunk): chunk is ChatId => {
  return chunk.type === 'metadata';
}
