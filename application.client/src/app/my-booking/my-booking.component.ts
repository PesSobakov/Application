import { Component } from '@angular/core';
import { BookingDto } from '../../DTOs/GetBooking/BookingDto';
import { Observable } from 'rxjs';
import { ApiService } from '../api.service';
import { WorkspaceType } from '../../models/WorkspaceType';

@Component({
  selector: 'app-my-booking',
  templateUrl: './my-booking.component.html',
  styleUrl: './my-booking.component.css',
  standalone: false
})
export class MyBookingComponent
{
  constructor(private api: ApiService) { }
  bookings$?: Observable<BookingDto[]> = this.api.GetBookings();
  workspaceImages = ["assets\\Open Space 1.jpg", "assets\\Private Room 1.jpg", "assets\\Meeting Room 1.jpg"];
  getImageSrc(type: WorkspaceType)
  {
    switch (type) {
      case (WorkspaceType.OpenSpace):
        return this.workspaceImages[0];
      case (WorkspaceType.PrivateRoom):
        return this.workspaceImages[1];
      case (WorkspaceType.MeetingRoom):
        return this.workspaceImages[2];
      default:
        return "assets\\armchair.svg"
    }
  }
  getName(booking: BookingDto)
  {
    let name = "";
    switch (booking.workspace.workspaceType) {
      case (WorkspaceType.OpenSpace):
        name += "Open space";
        break;
      case (WorkspaceType.PrivateRoom):
        name += "Private room";
        break;
      case (WorkspaceType.MeetingRoom):
        name += "Meeting room";
        break;
    }
    name += " for "
    name += booking.seats
    if (booking.seats == 1) {
      name += " person"
    }
    else {
      name += " people"
    }
    return name;
  }
  getDate(booking: BookingDto)
  {
    if (booking.workspace.workspaceType == WorkspaceType.MeetingRoom) {
      return `${booking.startDate}`;
    }
    else {
      let startDate = new Date(new Date(booking.startDate).getFullYear(), new Date(booking.startDate).getMonth(), new Date(booking.startDate).getDate(), 12, 0, 0);
      let endDate = new Date(new Date(booking.endDate).getFullYear(), new Date(booking.endDate).getMonth(), new Date(booking.endDate).getDate(), 12, 0, 0);
      let difference = Math.round((endDate.getTime() - startDate.getTime()) / (1000 * 3600 * 24)) + 1;
      return `${booking.startDate} - ${booking.endDate} (${difference} ${difference == 1 ? "day" : "days"})`;
    }
  }
  getTime(booking: BookingDto)
  {
    let startHour = +booking.startTime.slice(0, 2);
    let startMinute = +booking.startTime.slice(3, 5);
    let startTime = `${startHour > 12 ? startHour - 12 : startHour}:${booking.startTime.slice(3, 5)} ${startHour > 12 ? "PM" : "AM"}`;
    let endHour = +booking.endTime.slice(0, 2);
    let endMinute = +booking.endTime.slice(3, 5);
    let endTime = `${endHour > 12 ? endHour - 12 : endHour}:${booking.endTime.slice(3, 5)} ${endHour > 12 ? "PM" : "AM"}`;
    if (booking.workspace.workspaceType == WorkspaceType.MeetingRoom) {
      let duration = (endHour * 60 + endMinute) - (startHour * 60 + startMinute);
      let durationHours = Math.floor(duration / 60);
      let durationminutes = duration % 60;
      let durationString = "";
      if (durationHours != 0) {
        if (durationminutes != 0) {
          durationString = `${durationHours} ${durationHours == 1 ? "hour" : "hours"} ${durationminutes} ${durationminutes == 1 ? "minute" : "minutes"}`
        }
        else {
          durationString = `${durationHours} ${durationHours == 1 ? "hour" : "hours"}`
        }
      }
      else {
        durationString = `${durationminutes} ${durationminutes == 1 ? "minute" : "minutes"}`
      }
      return `from ${startTime} to ${endTime} (${durationString})`;
    }
    else {
      return `from ${startTime} to ${endTime}`;
    }
  }

  deleteOpened = false;
  bookingForDeletion: BookingDto | undefined;
  showDelete(booking: BookingDto)
  {
    this.bookingForDeletion = booking;
    this.deleteOpened = true;
  }
  deleteBooking(booking: BookingDto | undefined)
  {
    if (booking != undefined) {
      this.api.DeleteBooking(booking.id).subscribe(() =>
      {
        this.deleteOpened = false;
        this.bookings$ = this.api.GetBookings();
      });
    }
  }
}
