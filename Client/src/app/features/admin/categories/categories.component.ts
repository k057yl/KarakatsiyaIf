import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { EventService } from '../../events/services/event.service';

@Component({
  selector: 'app-admin-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.component.html',
  styleUrls: ['./categories.component.scss']
})
export class AdminCategoriesComponent implements OnInit {
  private eventService = inject(EventService);
  private fb = inject(FormBuilder);

  public categories = signal<any[]>([]);
  public isLoading = signal(true);
  public isSubmitting = signal(false);

  public readonly availableIcons = [
    { value: 'music', label: '🎸' },
    { value: 'flash', label: '⚡' },
    { value: 'microphone', label: '🎤' },
    { value: 'image', label: '🎨' },
    { value: 'theater', label: '🎭' },
    { value: 'sport', label: '🏆' },
    { value: 'beer', label: '🍻' },
    { value: 'party', label: '🎉' }
  ];

  categoryForm = this.fb.nonNullable.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    icon: ['', [Validators.required]]
  });

  ngOnInit() {
    this.loadCategories();
  }

  private loadCategories() {
    this.eventService.getCategories().subscribe({
      next: (data) => { 
        this.categories.set(data); 
        this.isLoading.set(false); 
      },
      error: () => this.isLoading.set(false)
    });
  }

  public getIconLabel(iconValue: string): string {
    const found = this.availableIcons.find(i => i.value == iconValue);
    return found ? found.label : '🏷';
  }

  onCreate() {
    if (this.categoryForm.invalid) return;
    this.isSubmitting.set(true);

    this.eventService.createCategory(this.categoryForm.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.categoryForm.reset({ name: '', icon: '' });
        this.loadCategories();
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  onDelete(id: string) {
    if (!confirm('Уверен? Все связанные события лишатся категории.')) return;
    this.eventService.deleteCategory(id).subscribe({
      next: () => this.loadCategories()
    });
  }
}