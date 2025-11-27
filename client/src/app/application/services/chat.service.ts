import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
export interface ChatMessage {
  text: string;
  isUser: boolean;
  timestamp: Date;
}

export interface ChatRequest {
  message: string;
  conversationHistory?: { text: string; isUser: boolean }[];
}

export interface ChatResponse {
  message: string;
  timestamp: Date;
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private apiUrl = `${environment.apiUrl}/chat`;

  constructor(private http: HttpClient) {}

  sendMessage(
    message: string,
    conversationHistory?: ChatMessage[]
  ): Observable<ChatResponse> {
    const request: ChatRequest = {
      message,
      conversationHistory: conversationHistory?.map((msg) => ({
        text: msg.text,
        isUser: msg.isUser,
      })),
    };

    return this.http.post<ChatResponse>(`${this.apiUrl}/message`, request);
  }

  checkHealth(): Observable<any> {
    return this.http.get(`${this.apiUrl}/health`);
  }
}
