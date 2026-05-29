import { Component, inject, OnInit, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService } from '../../../core/services/user.service';
import { UpdateContactsDto } from '../../../core/models/dtos/user.dto';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit, OnDestroy {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);

  public isLoading = signal<boolean>(true);
  public isSaving = signal<boolean>(false);
  public successMessage = signal<string>('');
  public errorMessage = signal<string>('');
  public telegramChatId = signal<number | null>(null);
  public telegramOtpCode = signal<string | null>(null);
  public isGeneratingOtp = signal<boolean>(false);
  public otpTimeLeft = signal<number>(600);
  private timerInterval: any = null;

  profileForm = this.fb.group({
    phone: ['', [Validators.maxLength(20)]],
    website: ['', [Validators.maxLength(500)]],
    telegram: ['', [Validators.maxLength(100)]],
    instagram: ['', [Validators.maxLength(100)]]
  });

  ngOnInit() {
    this.loadProfile();
  }

  ngOnDestroy() {
    this.clearOtpTimer();
  }

  private loadProfile() {
    this.userService.getMyProfile().subscribe({
      next: (user) => {
        this.telegramChatId.set(user.telegramChatId || null);

        if (user.contacts) {
          this.profileForm.patchValue({
            phone: user.contacts.phone || '',
            website: user.contacts.website || '',
            telegram: user.contacts.telegram || '',
            instagram: user.contacts.instagram || ''
          });
        }
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  public getTelegramCode() {
    this.isGeneratingOtp.set(true);
    this.errorMessage.set('');

    this.userService.generateTelegramOtp().subscribe({
      next: (res) => {
        this.telegramOtpCode.set(res.code);
        this.isGeneratingOtp.set(false);
        this.startOtpTimer();
      },
      error: () => {
        this.isGeneratingOtp.set(false);
        this.errorMessage.set('Не удалось сгенерировать код. Попробуй позже.');
      }
    });
  }

  private startOtpTimer() {
    this.clearOtpTimer();
    this.otpTimeLeft.set(600);
    
    this.timerInterval = setInterval(() => {
      if (this.otpTimeLeft() > 0) {
        this.otpTimeLeft.update(t => t - 1);

        if (this.otpTimeLeft() % 3 === 0) {
          this.checkTelegramStatus();
        }
      } else {
        this.telegramOtpCode.set(null);
        this.clearOtpTimer();
      }
    }, 1000);
  }

  private checkTelegramStatus() {
    this.userService.getMyProfile().subscribe({
      next: (user) => {
        if (user.telegramChatId) {
          this.telegramChatId.set(user.telegramChatId);
          this.telegramOtpCode.set(null);
          this.clearOtpTimer();
          this.successMessage.set('🚀 Бот успешно привязан! Пуши активированы.');
          setTimeout(() => this.successMessage.set(''), 4000);
        }
      }
    });
  }

  public unlinkTelegram() {
    if (!confirm('Точно отключить уведомления в Telegram?')) return;

    this.userService.unlinkTelegram().subscribe({
      next: () => {
        this.telegramChatId.set(null);
        this.successMessage.set('Уведомления в Telegram отключены.');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: () => {
        this.errorMessage.set('Не удалось отключить бот. Попробуй позже.');
      }
    });
  }

  private clearOtpTimer() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
      this.timerInterval = null;
    }
  }

  public get formattedTimeLeft(): string {
    const minutes = Math.floor(this.otpTimeLeft() / 60);
    const seconds = this.otpTimeLeft() % 60;
    return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
  }

  public saveContacts() {
    if (this.profileForm.invalid) return;

    this.isSaving.set(true);
    this.successMessage.set('');
    this.errorMessage.set('');

    const formValue = this.profileForm.getRawValue();
    const payload: UpdateContactsDto = {
      phone: formValue.phone || null,
      website: formValue.website || null,
      telegram: formValue.telegram || null,
      instagram: formValue.instagram || null
    };

    this.userService.updateContacts(payload).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.successMessage.set('Соцсети успешно сохранены! Теперь можешь флексить в комментах.');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (err) => {
        this.isSaving.set(false);
        this.errorMessage.set(err.error?.message || 'Произошла ошибка при сохранении');
      }
    });
  }
}