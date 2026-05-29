export interface CreateEventPhotoDto {
  imageUrl: string;
  publicId: string;
  isMain: boolean;
}

export interface CreateEventDto {
  title: string;
  description: string;
  startDate: string;
  locationName: string;
  city: string;
  street: string;
  categoryId: string;
  houseNumber?: string;
  latitude?: number;
  longitude?: number;
  osmId?: string;
  externalTicketUrl?: string;
  contactLinks?: string;
  isVip?: boolean;
  photos: CreateEventPhotoDto[];
}