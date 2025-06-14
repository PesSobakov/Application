import { WorkspaceType } from "../../models/WorkspaceType"
import { Amenity } from "../../models/Amenity"
import { BookingDto } from "./BookingDto"
import { FreeRoomsDto } from "./FreeRoomsDto"

export interface WorkspaceGroupDto
{
  workspaceType: WorkspaceType
  amenities: Amenity[]
  freeRooms: FreeRoomsDto[],
  bookings: BookingDto[]
}
