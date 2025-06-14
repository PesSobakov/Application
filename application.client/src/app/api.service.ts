import { Injectable } from '@angular/core';
import { environment } from './../environments/environment';


import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';

import { StringDto } from '../DTOs/StringDto';
import { LoginDto } from '../DTOs/LoginDto';
import { CreateBookingDto } from '../DTOs/CreateBookingDto';
import { BookingDto } from '../DTOs/GetBooking/BookingDto';
import { CoworkingDto } from '../DTOs/GetCoworkings/CoworkingDto';
import { CoworkingDto as CoworkingDto2 } from '../DTOs/GetBookings/CoworkingDto';
import { WorkspaceGroupDto } from '../DTOs/GetWorkspaces/WorkspaceGroupDto';
import { EditBookingDto } from '../DTOs/EditBookingDto';

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

  GetCoworkings(): Observable<CoworkingDto[]> 
  {
    let url = `${this.endpoint}api/coworking`;
    return this.http
      .get<CoworkingDto[]>(url, { withCredentials: true });
  }

  GetWorkspaces(id:number): Observable<WorkspaceGroupDto[]> 
  {
    let url = `${this.endpoint}api/workspace/${id}`;
    return this.http
      .get<WorkspaceGroupDto[]>(url, { withCredentials: true });
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
  EditBooking(id: number, dto: EditBookingDto)
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
  GetBookings(): Observable<CoworkingDto2[]> 
  {
    let url = `${this.endpoint}api/booking`;
    return this.http
      .get<CoworkingDto2[]>(url, { withCredentials: true });
  }

  BookingsQuestion(dto: StringDto): Observable<StringDto> 
  {
    let url = `${this.endpoint}api/booking/question`;
    return this.http
      .post<StringDto>(url, dto, { withCredentials: true });
  }
}
