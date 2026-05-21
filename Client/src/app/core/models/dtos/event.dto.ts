export interface CreateEventDto {
  title: string;
  description: string;
  startDate: string;
  locationName: string;
  city: string;
  street: string;
  houseNumber?: string;
  latitude?: number;
  longitude?: number;
  osmId?: string;
  externalTicketUrl?: string;
  contactLinks?: string;
  isVip?: boolean;
}