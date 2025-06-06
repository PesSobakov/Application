import { WorkspaceDto } from "./WorkspaceDto"

export interface BookingDto
{
  id: number,
  name: string
  email: string
  workspace: WorkspaceDto
  seats: number
  startDate: string
  endDate: string
  startTime: string
  endTime: string
}
