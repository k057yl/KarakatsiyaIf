export interface PendingOrganizer {
  userId: string;
  organizerId: string;
  name: string;
  phone?: string;
  email?: string;
  telegram?: string;
  appliedAt: string;
}