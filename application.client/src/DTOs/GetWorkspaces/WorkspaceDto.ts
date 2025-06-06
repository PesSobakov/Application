import { WorkspaceType } from "../../models/WorkspaceType"
import { Amenity } from "../../models/Amenity"
import { BookingDto } from "./BookingDto"

export interface WorkspaceDto
{
  id: number,
  capacity: number,
  amenities: Amenity[]
  workspaceType: WorkspaceType
  bookings: BookingDto[]
}
