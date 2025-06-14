import { Component } from '@angular/core';
import { ApiService } from '../api.service';
import { Observable } from 'rxjs';
import { CoworkingDto } from '../../DTOs/GetCoworkings/CoworkingDto';
import { RoomCountDto } from '../../DTOs/GetCoworkings/RoomCountDto';
import { WorkspaceType } from '../../models/WorkspaceType';

@Component({
  selector: 'app-coworking-list',
  templateUrl: './coworking-list.component.html',
  styleUrl: './coworking-list.component.css',
  standalone: false
})
export class CoworkingListComponent
{
  constructor(private api: ApiService) { }

  coworkings$?: Observable<CoworkingDto[]> = this.api.GetCoworkings();

  getImageSrc(index: number): string
  {
    const images = ["assets\\Coworking 1.jpg", "assets\\Coworking 2.jpg", "assets\\Coworking 3.jpg", "assets\\Coworking 4.jpg", "assets\\Coworking 5.jpg"];
    return images[index % images.length];
  }

  getRoomCount(roomCount:RoomCountDto[])
  {
    let openSpace = roomCount.find(room => room.workspaceType == WorkspaceType.OpenSpace);
    let privateRoom = roomCount.find(room => room.workspaceType == WorkspaceType.PrivateRoom);
    let meetingRoom = roomCount.find(room => room.workspaceType == WorkspaceType.MeetingRoom);
    return `🪑 ${openSpace?.rooms ?? 0} desk${openSpace?.rooms == 1 ? "" : "s"} · 🔒 ${privateRoom?.rooms ?? 0} private room${privateRoom?.rooms == 1 ? "" : "s"} · 👥 ${meetingRoom?.rooms ?? 0} meeting room${meetingRoom?.rooms == 1 ? "" : "s"}`;
  }
}
