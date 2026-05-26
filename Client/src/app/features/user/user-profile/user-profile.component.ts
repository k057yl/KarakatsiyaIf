import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { UserService} from '../../../core/services/user.service';
import { UpdateContactsDto } from '../../../core/models/dtos/user.dto';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.scss']
})
export class UserProfileComponent implements OnInit {
  private fb = inject(FormBuilder);
  private userService = inject(UserService);

  public isLoading = signal<boolean>(true);
  public isSaving = signal<boolean>(false);
  public successMessage = signal<string>('');
  public errorMessage = signal<string>('');

  profileForm = this.fb.group({
    phone: ['', [Validators.maxLength(20)]],
    website: ['', [Validators.maxLength(500)]],
    telegram: ['', [Validators.maxLength(100)]],
    instagram: ['', [Validators.maxLength(100)]]
  });

  ngOnInit() {
    this.loadProfile();
  }

  private loadProfile() {
    this.userService.getMyProfile().subscribe({
      next: (user) => {
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
      error: () => {
        this.isLoading.set(false);
      }
    });
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
      next: (res) => {
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