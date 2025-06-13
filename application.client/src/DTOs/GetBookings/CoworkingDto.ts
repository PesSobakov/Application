import { BookingDto } from "./BookingDto";

export interface CoworkingDto
{
  id: number,
  name: string,
  description: string,
  address: string,
  bookings: BookingDto[]
}
