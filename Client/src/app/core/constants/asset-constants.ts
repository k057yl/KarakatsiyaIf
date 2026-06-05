import { environment } from '../../../environments/environment';

export const ASSET_CONSTANTS = {
  DEFAULT_AVATARS: [
    'assets/images/default1.png',
    'assets/images/default2.png',
    'assets/images/default3.png'
  ],

  getPerformerAvatar(avatarUrl: string | null | undefined, performerId: string): string {
    if (avatarUrl && avatarUrl.trim() !== '') {
      if (avatarUrl.startsWith('/uploads')) {
        const baseHost = environment.apiUrl.replace(/\/api$/, '').replace(/\/api\/$/, '');
        return `${baseHost}${avatarUrl}`;
      }
      return avatarUrl;
    }
    
    if (!performerId) return this.DEFAULT_AVATARS[0];

    let hash = 0;
    for (let i = 0; i < performerId.length; i++) {
      hash = performerId.charCodeAt(i) + ((hash << 5) - hash);
    }
    
    const index = Math.abs(hash) % this.DEFAULT_AVATARS.length;
    return this.DEFAULT_AVATARS[index];
  }
};