import { RoomCountDto } from "./RoomCountDto";

export interface CoworkingDto
{
  id: number,
  name: string,
  description: string,
  address: string,
  roomCountDtos: RoomCountDto[]
}
