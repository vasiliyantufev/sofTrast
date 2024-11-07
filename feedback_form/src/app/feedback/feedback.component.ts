import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule]
})
export class FeedbackComponent implements OnInit {
  feedbackForm: FormGroup;
  successMessageVisible: boolean = false;
  errorMessageVisible: boolean = false;

  constructor(private fb: FormBuilder) {
    this.feedbackForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\(\)-]{7,15}$/)]], // Валидация для номера телефона
      subject: ['support', Validators.required],
      message: ['', Validators.required],
    });
  }

  ngOnInit(): void {}

  onSubmit(): void {
    if (this.feedbackForm.valid) {
      const feedbackData = this.feedbackForm.value;

      // Здесь можно сделать HTTP-запрос на сервер
      console.log('Feedback submitted:', feedbackData);
      
      // Если отправка прошла успешно
      this.successMessageVisible = true;
      this.errorMessageVisible = false;

      // Сброс формы после отправки
      this.feedbackForm.reset();
    } else {
      this.errorMessageVisible = true;
      this.successMessageVisible = false;
    }
  }
}