import { HttpInterceptorFn } from '@angular/common/http';

export const serverUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('/') || req.url.startsWith('assets/') || req.url.startsWith('api/')) {
    const baseUrl = 'http://localhost:4200'; 
    const cleanUrl = req.url.startsWith('/') ? req.url : `/${req.url}`;
    
    const clone = req.clone({
      url: `${baseUrl}${cleanUrl}`
    });
    return next(clone);
  }
  
  return next(req);
};