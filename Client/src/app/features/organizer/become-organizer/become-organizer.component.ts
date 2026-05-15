import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { OrganizerService } from '../../../core/services/organizer.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-become-organizer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './become-organizer.component.html',
  styleUrls: ['./become-organizer.component.scss']
})
export class BecomeOrganizerComponent {
  private fb = inject(FormBuilder);
  private orgService = inject(OrganizerService);
  private authService = inject(AuthService);

  isSubmitting = signal(false);
  isSuccess = signal(false);

  applyForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    phone: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    telegram: [''],
    instagram: ['']
  });

  submit() {
    if (this.applyForm.invalid) return;

    this.isSubmitting.set(true);
    const data = this.applyForm.getRawValue() as any;

    this.orgService.apply(data).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.isSuccess.set(true);
      },
      error: () => this.isSubmitting.set(false)
    });
  }
}