import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';  // Импортируем CommonModule для использования директивы *ngIf

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './feedback.component.html',
  styleUrl: './feedback.component.css'
})
export class FeedbackComponent {

}
