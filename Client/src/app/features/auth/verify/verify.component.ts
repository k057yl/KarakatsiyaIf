import { Component, inject, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-verify',
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './verify.component.html',
  styleUrls: ['../auth-shared.scss']
})
export class VerifyComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  errorMessage = signal<string>('');
  emailToVerify = signal<string>('');

  verifyForm = this.fb.group({
    code: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(6)]]
  });

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      if (params['email']) {
        this.emailToVerify.set(params['email']);
      } else {
        this.errorMessage.set('Email потерялся. Попробуй залогиниться.');
      }
    });
  }

  onSubmit() {
    if (this.verifyForm.invalid) {
      this.verifyForm.markAllAsTouched();
      return;
    }

    if (!this.emailToVerify()) {
      this.errorMessage.set('Непонятно, чью почту подтверждаем. Вернись на шаг назад.');
      return;
    }

    this.errorMessage.set('');
    const code = this.verifyForm.getRawValue().code as string;

    this.authService.verifyCode({ email: this.emailToVerify(), code }).subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message || 'Код хуйня. Проверь почту еще раз.');
      }
    });
  }
}