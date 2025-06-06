import { WorkspaceType } from "../models/WorkspaceType"

export interface CreateBookingDto
{
  name: string
  email: string
  workspaceType: WorkspaceType
  seats: number
  startDate: string
  endDate: string
  startTime: string
  endTime: string
}
