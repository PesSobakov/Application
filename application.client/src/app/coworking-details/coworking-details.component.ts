import { Component, Input, OnInit } from '@angular/core';
import { Amenity } from '../../models/Amenity';
import { ApiService } from '../api.service';
import { Observable } from 'rxjs';
import { WorkspaceType } from '../../models/WorkspaceType';
import { WorkspaceGroupDto } from '../../DTOs/GetWorkspaces/WorkspaceGroupDto';

@Component({
  selector: 'app-coworking-details',
  templateUrl: './coworking-details.component.html',
  styleUrl: './coworking-details.component.css',
  standalone: false
})
export class CoworkingDetailsComponent implements OnInit
{
  constructor(private api: ApiService)  { }

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

  @Input() id? :string

  workspaceGroups$?: Observable<WorkspaceGroupDto[]>;

  getImageSrcs(type: WorkspaceType): string[]
  {
    const images = [['assets\\Open Space 1.jpg', 'assets\\Open Space 2.jpg', 'assets\\Open Space 3.jpg', 'assets\\Open Space 4.jpg'], ['assets\\Private Room 1.jpg', 'assets\\Private Room 2.jpg', 'assets\\Private Room 3.jpg'], ['assets\\Meeting Room 1.jpg', 'assets\\Meeting Room 2.jpg', 'assets\\Meeting Room 3.jpg', 'assets\\Meeting Room 4.jpg']];
    return images[type];
  }

  getName(type: WorkspaceType): string
  {
    const names = ["Open space", "Private Rooms","Meeting rooms"];
    return names[type];
  }

  getDescription(type: WorkspaceType): string
  {
    const descriptions = ["A vibrant shared area perfect for freelancers or small teams who enjoy a collaborative atmosphere. Choose any available desk and get to work with flexibility and ease.", "Ideal for focused work, video calls, or small team huddles. These fully enclosed rooms offer privacy and come in a variety of sizes to fit your needs.", "Designed for productive meetings, workshops, or client presentations. Equipped with screens, whiteboards, and comfortable seating to keep your sessions running smoothly."];
    return descriptions[type];
  }

  ngOnInit()
  {
    if (this.id != undefined && +this.id != undefined) {
      this.workspaceGroups$ = this.api.GetWorkspaces(+this.id);
    }
  }
}
