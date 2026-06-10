import { HttpInterceptorFn } from '@angular/common/http';
import { environment } from '../../../environments/environment';

export const serverUrlInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.url.startsWith('http://') || req.url.startsWith('https://')) {
    return next(req);
  }

  let targetUrl = environment.siteUrl;

  if (req.url.startsWith('api/')) {
    targetUrl = environment.apiUrl;

    const cleanUrl = req.url.substring(4); 
    const finalUrl = cleanUrl.startsWith('/') ? cleanUrl : `/${cleanUrl}`;

    const clone = req.clone({
      url: `${targetUrl}${finalUrl}`
    });
    return next(clone);
  }

  const cleanUrl = req.url.startsWith('/') ? req.url : `/${req.url}`;
  const clone = req.clone({
    url: `${targetUrl}${cleanUrl}`
  });

  return next(clone);
};