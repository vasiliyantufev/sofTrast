import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http'; // Импортируем HttpClient
import { Observable } from 'rxjs'; // Импортируем Observable (опционально)
import { RecaptchaModule } from 'ng-recaptcha';  // Импортируем RecaptchaModule

@Component({
  selector: 'app-feedback',
  templateUrl: './feedback.component.html',
  styleUrls: ['./feedback.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RecaptchaModule]
})
export class FeedbackComponent implements OnInit {
  feedbackForm: FormGroup;
  successMessageVisible: boolean = false;
  errorMessageVisible: boolean = false;

  // Внедряем HttpClient через конструктор
  constructor(private fb: FormBuilder, private http: HttpClient) {
    this.feedbackForm = this.fb.group({
      name: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\(\)-]{7,15}$/)]], // Валидация для номера телефона
      subject: ['support', Validators.required],
      message: ['', Validators.required],      
      recaptcha: ['', Validators.required] // Поле для токена reCAPTCHA
    });
  }

  ngOnInit(): void {}

  resolved(captchaResponse: string | null): void {
    // Проверяем, что значение не null
    if (captchaResponse) {
      this.feedbackForm.controls['recaptcha'].setValue(captchaResponse);
    } else {
      // Если капча не была решена, можно очистить или сбросить значение
      this.feedbackForm.controls['recaptcha'].setValue(null);
    }
  }
  


  onSubmit(): void {
    if (this.feedbackForm.valid) {
      const feedbackData = this.feedbackForm.value;

      // Отправляем HTTP-запрос на сервер
      this.sendFeedback(feedbackData).subscribe({
        next: (response) => {
          console.log('Feedback submitted:', response);

          // Если отправка прошла успешно
          this.successMessageVisible = true;
          this.errorMessageVisible = false;

          // Сброс формы после отправки
          this.feedbackForm.reset();
        },
        error: (error) => {
          console.error('Error submitting feedback:', error);
          this.errorMessageVisible = true;
          this.successMessageVisible = false;
        }
      });
    } else {
      this.errorMessageVisible = true;
      this.successMessageVisible = false;
    }
  }

  // Метод для отправки данных обратной связи
  sendFeedback(feedbackData: any): Observable<any> {
    const url = 'http://localhost:5006/feedback';
    return this.http.post(url, feedbackData); // Отправляем POST-запрос
  }
}