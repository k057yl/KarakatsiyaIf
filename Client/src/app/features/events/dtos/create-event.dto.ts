import { CreateEventPhotoDto } from "./create-event-photo.dto";

export interface CreateEventDto {
  title: string;
  description: string;
  startDate: string;
  locationName: string;
  city: string;
  street: string;
  categoryId: string;
  performers: string[];
  houseNumber?: string;
  latitude?: number;
  longitude?: number;
  osmId?: string;
  externalTicketUrl?: string;
  contactLinks?: string;
  isVip?: boolean;
  photos: CreateEventPhotoDto[];
}