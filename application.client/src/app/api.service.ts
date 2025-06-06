import { Injectable } from '@angular/core';
import { environment } from './../environments/environment';


import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

import { StringDto } from '../DTOs/StringDto';
import { WorkspaceDto } from '../DTOs/GetWorkspaces/WorkspaceDto';
import { LoginDto } from '../DTOs/LoginDto';
import { CreateBookingDto } from '../DTOs/CreateBookingDto';
import { BookingDto } from '../DTOs/GetBooking/BookingDto';

@Injectable({
  providedIn: 'root'
})

@Injectable()
export class ApiService
{
  readonly endpoint: string = `https://${environment.server}/`;

  constructor(private http: HttpClient) { }

  login(dto: LoginDto)
  {
    let url = `${this.endpoint}api/auth/user`;
    return this.http
      .post(url, dto, { withCredentials: true });
  }
  getUser(): Observable<StringDto>
  {
    let url = `${this.endpoint}api/auth/user`;
    return this.http
      .get<StringDto>(url, { withCredentials: true });
  }

  GetWorkspaces(): Observable<WorkspaceDto[]> 
  {
    let url = `${this.endpoint}api/workspace`;
    return this.http
      .get<WorkspaceDto[]>(url, { withCredentials: true });
  }

  CreateBooking(dto: CreateBookingDto)
  {
    let url = `${this.endpoint}api/booking`;
    return this.http
      .post(url, dto, { withCredentials: true });
  }
  DeleteBooking(id: number)
  {
    let url = `${this.endpoint}api/booking/${id}`;
    return this.http
      .delete(url, { withCredentials: true });
  }
  EditBooking(id: number, dto: CreateBookingDto)
  {
    let url = `${this.endpoint}api/booking/${id}`;
    return this.http
      .patch(url, dto, { withCredentials: true });
  }
  GetBooking(id: number): Observable<BookingDto> 
  {
    let url = `${this.endpoint}api/booking/${id}`;
    return this.http
      .get<BookingDto>(url, { withCredentials: true });
  }
  GetBookings(): Observable<BookingDto[]> 
  {
    let url = `${this.endpoint}api/booking`;
    return this.http
      .get<BookingDto[]>(url, { withCredentials: true });
  }
}
