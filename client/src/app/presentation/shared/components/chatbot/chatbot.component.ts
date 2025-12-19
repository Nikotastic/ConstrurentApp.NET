import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewChecked,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  ChatService,
  ChatMessage,
} from '../../../../application/services/chat.service';

@Component({
  selector: 'app-chatbot',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chatbot.component.html',
  styleUrls: ['./chatbot.component.scss'],
})
export class ChatbotComponent implements AfterViewChecked {
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;

  isOpen = false;
  userInput = '';
  messages: ChatMessage[] = [
    {
      text: 'Hi! 👋 I am Firmness virtual assistant. How can I help you today?',
      isUser: false,
      timestamp: new Date(),
    },
  ];
  isTyping = false;
  errorMessage: string | null = null;

  constructor(private chatService: ChatService) {}

  toggleChat() {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.scrollToBottom();
    }
  }

  sendMessage() {
    if (!this.userInput.trim()) return;

    // Clear any previous error
    this.errorMessage = null;

    // Add user message
    this.messages.push({
      text: this.userInput,
      isUser: true,
      timestamp: new Date(),
    });

    const userQuestion = this.userInput;
    this.userInput = '';
    this.isTyping = true;
    this.scrollToBottom();

    // Send to real AI backend
    this.chatService
      .sendMessage(userQuestion, this.messages.slice(0, -1))
      .subscribe({
        next: (response: any) => {
          this.isTyping = false;
          this.messages.push({
            text: response.message,
            isUser: false,
            timestamp: new Date(response.timestamp),
          });
          this.scrollToBottom();
        },
        error: (error: any) => {
          this.isTyping = false;
          console.error('Error getting AI response:', error);

          // Show user-friendly error message
          let errorText =
            'Sorry, I am having technical difficulties right now. ';

          if (error.status === 0) {
            errorText +=
              'I cannot connect to the server. Please check your connection.';
          } else if (error.status === 500) {
            errorText += 'There is a problem with the AI service.';
          } else {
            errorText += ' Please try again.';
          }

          errorText +=
            '\n\nYou can contact our team directly at (350) 5045930 or contacto@firmness.com';

          this.messages.push({
            text: errorText,
            isUser: false,
            timestamp: new Date(),
          });
          this.scrollToBottom();
        },
      });
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    try {
      this.scrollContainer.nativeElement.scrollTop =
        this.scrollContainer.nativeElement.scrollHeight;
    } catch (err) {}
  }
}
