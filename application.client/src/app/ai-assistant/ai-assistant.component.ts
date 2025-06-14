import { Component } from '@angular/core';
import { ApiService } from '../api.service';
import { StringDto } from '../../DTOs/StringDto';

@Component({
  selector: 'app-ai-assistant',
  templateUrl: './ai-assistant.component.html',
  styleUrl: './ai-assistant.component.css',
  standalone: false
})
export class AiAssistantComponent
{
  constructor(private api: ApiService) { }

  response: string | undefined;
  responseQuestion: string | undefined;
  question: string = "";

  exampleQuestions: string[] = ["How many bookings do I have?", "Which bookings was active last week?", "Do I have anything on May 18?"];

  submit(question: string)
  {
    let responseQuestion = question;
    this.api.BookingsQuestion(<StringDto>{ value: question }).subscribe({
      error: () => { this.response = undefined },
      next: (response) =>
      {
        this.response = response.value
        this.responseQuestion = responseQuestion
      }
    })
  }
}
