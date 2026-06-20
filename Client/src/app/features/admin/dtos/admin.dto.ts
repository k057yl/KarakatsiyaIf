export interface PendingOrganizer {
  userId: string;
  organizerId: string;
  name: string;
  phone?: string;
  email?: string;
  telegram?: string;
  appliedAt: string;
}

export interface AdminOrganizer {
  userId: string;
  organizerId: string;
  name: string;
  email: string;
  phone?: string;
  website?: string;
  telegram?: string;
  instagram?: string;
  isApproved: boolean;
}