import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';

import { ToastComponent } from './presentation/shared/components/toast/toast.component';
import { ConfirmationModalComponent } from './presentation/shared/components/confirmation-modal/confirmation-modal.component';
import { ChatbotComponent } from './presentation/shared/components/chatbot/chatbot.component';

/**
 * Root App Component
 * Simple container for global components and router outlet
 */
@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    ToastComponent,
    ConfirmationModalComponent,
    ChatbotComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class AppComponent {
  title = 'Firmness - Construction Rental';
}
