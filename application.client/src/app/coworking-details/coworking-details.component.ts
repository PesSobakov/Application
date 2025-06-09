import { Component } from '@angular/core';
import { Amenity } from '../../models/Amenity';
import { ApiService } from '../api.service';
import { WorkspaceDto } from '../../DTOs/GetWorkspaces/WorkspaceDto';
import { Observable } from 'rxjs';
import { WorkspaceType } from '../../models/WorkspaceType';
import { BookingDto } from '../../DTOs/GetBooking/BookingDto';

@Component({
  selector: 'app-coworking-details',
  templateUrl: './coworking-details.component.html',
  styleUrl: './coworking-details.component.css',
  standalone: false
})
export class CoworkingDetailsComponent
{
  constructor(private api: ApiService) { }

  testAmenities = [Amenity.Coffee, Amenity.GameRoom, Amenity.WiFi, Amenity.Conditioner, Amenity.Microphone, Amenity.Headphones]
  getAmenitySrc(amenity: Amenity): string
  {
    switch (amenity) {
      case Amenity.Coffee:
        return "assets\\coffee.svg"
      case Amenity.GameRoom:
        return "assets\\device-gamepad-2.svg"
      case Amenity.WiFi:
        return "assets\\wifi.svg"
      case Amenity.Conditioner:
        return "assets\\air-conditioning.svg"
      case Amenity.Microphone:
        return "assets\\microphone.svg"
      case Amenity.Headphones:
        return "assets\\headphones.svg"
      default:
        return ""
    }
  }

  workspaces$?: Observable<WorkspaceDto[]> = this.api.GetWorkspaces();
  bookings$?: Observable<BookingDto[]> = this.api.GetBookings();

  getAmenities(workspaces: WorkspaceDto[], workspaceType: WorkspaceType): Amenity[]
  {
    return workspaces.find(x => x.workspaceType == workspaceType)?.amenities ?? [];
  }

  getFreeDesks(workspaces: WorkspaceDto[]): number
  {
    let now = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
    let totalDesks = workspaces.find(x => x.workspaceType == WorkspaceType.OpenSpace)?.capacity ?? 0;
    let reserved = 0;
    workspaces.find(x => x.workspaceType == WorkspaceType.OpenSpace)?.bookings.forEach((y) =>
    {
      let startDate = new Date(new Date(y.startDate).getFullYear(), new Date(y.startDate).getMonth(), new Date(y.startDate).getDate(), 12, 0, 0);
      let endDate = new Date(new Date(y.endDate).getFullYear(), new Date(y.endDate).getMonth(), new Date(y.endDate).getDate(), 12, 0, 0);
      if (startDate <= now && endDate >= now) {
        reserved += y.seats;
      }
    });
    return totalDesks - reserved;
  }

  getFreeRooms(workspaces: WorkspaceDto[], workspaceType: WorkspaceType, capacity: number): number
  {
    let now = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
    return workspaces.filter((x) =>
    {
      return x.workspaceType == workspaceType &&
        x.capacity == capacity &&
        x.bookings.find(y =>
        {
          let startDate = new Date(new Date(y.startDate).getFullYear(), new Date(y.startDate).getMonth(), new Date(y.startDate).getDate(), 12, 0, 0);
          let endDate = new Date(new Date(y.endDate).getFullYear(), new Date(y.endDate).getMonth(), new Date(y.endDate).getDate(), 12, 0, 0);
          return (startDate <= now && endDate >= now)
        }) == undefined;
    }).length
  }

  getBookings(bookings: BookingDto[], workspaceType: WorkspaceType): BookingDto[]
  {
    let now = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate(), 12, 0, 0);
    return bookings.filter((x) =>
    {
      let endDate = new Date(new Date(x.endDate).getFullYear(), new Date(x.endDate).getMonth(), new Date(x.endDate).getDate(), 12, 0, 0);
      return x.workspace.workspaceType == workspaceType &&
        endDate >= now;
    });
  }


}
