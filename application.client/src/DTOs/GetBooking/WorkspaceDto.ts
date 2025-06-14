import { Amenity } from "../../models/Amenity"
import { WorkspaceType } from "../../models/WorkspaceType"
import { BookingDto } from "./BookingDto";

export interface WorkspaceDto
{
  id: number,
  capacity: number,
  amenities: Amenity[]
  workspaceType: WorkspaceType
}
