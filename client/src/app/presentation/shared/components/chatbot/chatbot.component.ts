import {
  Component,
  ElementRef,
  ViewChild,
  AfterViewChecked,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Message {
  text: string;
  isUser: boolean;
  timestamp: Date;
}

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
  messages: Message[] = [
    {
      text: 'Hi! 👋 I am Firmness virtual assistant. How can I help you today?',
      isUser: false,
      timestamp: new Date(),
    },
  ];
  isTyping = false;

  toggleChat() {
    this.isOpen = !this.isOpen;
    if (this.isOpen) {
      this.scrollToBottom();
    }
  }

  sendMessage() {
    if (!this.userInput.trim()) return;

    // add user message
    this.messages.push({
      text: this.userInput,
      isUser: true,
      timestamp: new Date(),
    });

    const userQuestion = this.userInput;
    this.userInput = '';
    this.isTyping = true;
    this.scrollToBottom();

    // Simulate response (we'll connect this to the backend later)
    setTimeout(() => {
      this.isTyping = false;
      this.messages.push({
        text: this.getMockResponse(userQuestion),
        isUser: false,
        timestamp: new Date(),
      });
      this.scrollToBottom();
    }, 1500);
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

  // Temporal logic of responses (Mock)
  private getMockResponse(question: string): string {
    const q = question.toLowerCase();
    if (q.includes('excavadora') || q.includes('maquinaria')) {
      return 'We currently have 5 excavators available for immediate rent. Would you like to see the models?';
    }
    if (q.includes('precio') || q.includes('costo')) {
      return 'Our prices vary depending on the equipment and rental period. For example, a CAT 320 excavator starts at $450/day.';
    }
    if (q.includes('contacto') || q.includes('telefono')) {
      return 'You can contact us at (350) 5045930 or write to us at contacto@firmness.com';
    }
    return 'I understand your question. Could you be a little more specific? I\'m here to help you with information about our machinery and services.';
  }
}
