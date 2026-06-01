import { Component, EventEmitter, inject, Output, signal, AfterViewInit, OnDestroy, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { MapService } from '../../../core/services/map.service';
import { CreateEventPhotoDto } from '../../../core/models/dtos/event.dto';

@Component({
  selector: 'app-event-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, TranslateModule],
  templateUrl: './event-form.component.html',
  styleUrls: ['./event-form.component.scss']
})
export class EventFormComponent implements OnInit, AfterViewInit, OnDestroy {
  @Input() isEditMode = false;
  @Input() initialData: any = null;
  @Input() categoriesList: any[] = [];
  @Input() isSubmitting = false;
  @Input() isUploadingPhoto = false;

  @Output() formSubmit = new EventEmitter<{ basePayload: any, selectedFiles: { file: File, isMain: boolean }[] }>();
  @Output() cancel = new EventEmitter<void>();
  @Output() uploadPhotoOnFly = new EventEmitter<{ file: File, isMain: boolean }>();

  private fb = inject(FormBuilder);
  private mapService = inject(MapService);
  private translate = inject(TranslateService);

  private readonly DRAFT_KEY = 'event_draft_form';

  public uploadedPhotos = signal<CreateEventPhotoDto[]>([]);
  public selectedFiles: { file: File, isMain: boolean }[] = [];

  private selectedLat: number | undefined = undefined;
  private selectedLon: number | undefined = undefined;
  private selectedOsmId: string | undefined = undefined;

  private map!: any;
  private marker!: any;

  eventForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', Validators.required],
    startDate: ['', Validators.required],
    categoryId: ['', Validators.required],
    locationName: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    houseNumber: [''],
    externalTicketUrl: [''],
    contactLinks: ['']
  });

  ngOnInit() {
    if (this.isEditMode && this.initialData) {
      const data = this.initialData;
      const formattedDate = data.startDate ? new Date(data.startDate).toISOString().slice(0, 16) : '';

      this.eventForm.patchValue({
        title: data.title || '',
        description: data.description || '',
        startDate: formattedDate,
        categoryId: data.categoryId || '',
        locationName: data.locationName || '',
        city: data.city || '',
        street: data.street || '',
        houseNumber: data.houseNumber || '',
        externalTicketUrl: data.externalTicketUrl || '',
        contactLinks: data.contactLinks || ''
      });

      this.selectedLat = data.latitude;
      this.selectedLon = data.longitude;
      this.selectedOsmId = data.osmId;

      if (data.photos) {
        this.uploadedPhotos.set(data.photos);
      }
    }
  }

  ngAfterViewInit() {
    setTimeout(() => {
      const mapContainer = document.getElementById('event-map');
      if (!mapContainer) {
        console.error('Контейнер для карты не найден в DOM.');
        return;
      }
      
      this.initMap();

      if (!this.isEditMode) {
        this.loadDraft();
        this.eventForm.valueChanges.subscribe(() => this.saveDraft());
      } else if (this.selectedLat && this.selectedLon) {
        this.moveMarkerAndSetView(this.selectedLat, this.selectedLon);
      }
    }, 50);
  }

  public onFilesSelected(event: Event) {
    const target = event.target as HTMLInputElement;
    if (!target.files || target.files.length === 0) return;

    const files: File[] = Array.from(target.files);

    files.forEach(file => {
      const isFirst = this.uploadedPhotos().length === 0 && this.selectedFiles.length === 0;
      const reader = new FileReader();
      reader.onload = (e: any) => {
        const localPhoto: CreateEventPhotoDto = {
          imageUrl: e.target.result,
          publicId: '',
          isMain: isFirst
        };

        this.uploadedPhotos.update((list: CreateEventPhotoDto[]) => [...list, localPhoto]);
        this.selectedFiles.push({ file, isMain: isFirst });

        if (this.isEditMode) {
          this.uploadPhotoOnFly.emit({ file, isMain: isFirst });
        }
      };
      reader.readAsDataURL(file);
    });
  }

  public removePhoto(index: number) {
    this.uploadedPhotos.update((list: CreateEventPhotoDto[]) => {
    const updated = list.filter((_, i: number) => i !== index);
    if (list[index]?.isMain && updated.length > 0) {
        updated[0].isMain = true;
        if (!this.isEditMode && this.selectedFiles[0]) {
        this.selectedFiles[0].isMain = true;
        }
    }
    return updated;
    });

    if (!this.isEditMode) {
      this.selectedFiles.splice(index, 1);
    }
  }

  public setMainPhoto(index: number) {
    this.uploadedPhotos.update((list: CreateEventPhotoDto[]) => 
      list.map((p: CreateEventPhotoDto, i: number) => ({ ...p, isMain: i === index }))
    );
    if (!this.isEditMode) {
      this.selectedFiles.forEach((f, i) => f.isMain = i === index);
    }
  }

  private moveMarkerAndSetView(lat: number, lon: number) {
    if (this.marker && this.map) {
      this.marker.setLatLng([lat, lon]);
      this.map.setView([lat, lon], 15);
    }
  }

  ngOnDestroy() {
    if (this.map) {
      this.map.remove();
    }
  }

  private saveDraft() {
    if (this.isEditMode) return;
    const draft = {
      form: this.eventForm.getRawValue(),
      lat: this.selectedLat,
      lon: this.selectedLon,
      osmId: this.selectedOsmId
    };
    localStorage.setItem(this.DRAFT_KEY, JSON.stringify(draft));
  }

  private loadDraft() {
    const saved = localStorage.getItem(this.DRAFT_KEY);
    if (saved) {
      try {
        const draft = JSON.parse(saved);
        this.eventForm.patchValue(draft.form);
        
        if (draft.lat && draft.lon) {
          this.selectedLat = draft.lat;
          this.selectedLon = draft.lon;
          this.selectedOsmId = draft.osmId;
          this.moveMarkerAndSetView(draft.lat, draft.lon);
        }
      } catch (e) {
        console.error(this.translate.instant('EVENT_MODAL.DRAFT_ERROR'), e);
      }
    }
  }

  private async initMap() {
    const L = await import('leaflet');

    const iconDefault = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
      shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      shadowSize: [41, 41]
    });
    L.Marker.prototype.options.icon = iconDefault;

    const startLat = this.selectedLat || 50.4501;
    const startLon = this.selectedLon || 30.5234;

    this.map = L.map('event-map').setView([startLat, startLon], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);

    this.marker = L.marker([startLat, startLon], { draggable: true }).addTo(this.map);

    this.marker.on('dragend', () => {
      const position = this.marker.getLatLng();
      this.updateLocationData(position.lat, position.lng);
    });

    this.map.on('click', (e: any) => {
      this.marker.setLatLng(e.latlng);
      this.updateLocationData(e.latlng.lat, e.latlng.lng);
    });

    this.map.invalidateSize();
  }

  private updateLocationData(lat: number, lon: number) {
    this.mapService.reverseGeocode(lat, lon).subscribe({
      next: (res: any) => {
        if (res && res.address) {
          this.selectedLat = lat;
          this.selectedLon = lon;
          this.selectedOsmId = res.osmId?.toString() || undefined;

          const addr = res.address;
          const locName = res.displayName ? res.displayName.split(',')[0] : ''; 

          this.eventForm.patchValue({
            locationName: locName,
            city: addr.city || addr.town || addr.village || '',
            street: addr.road || '',
            houseNumber: addr.houseNumber || ''
          });
        }
      },
      error: (err: unknown) => {
        console.error(this.translate.instant('EVENT_MODAL.ERROR_GEOCODER'), err);
      }
    });
  }

  public submitForm() {
    const hasMainPhoto = this.uploadedPhotos().some(p => p.isMain);
    if (!hasMainPhoto) {
      alert(this.translate.instant('EVENT_MODAL.ERROR_NO_MAIN_PHOTO'));
      return;
    }

    if (this.eventForm.invalid) return;

    const formValue = this.eventForm.getRawValue();
    const basePayload = {
      ...formValue,
      startDate: new Date(formValue.startDate).toISOString(),
      latitude: this.selectedLat,
      longitude: this.selectedLon,
      osmId: this.selectedOsmId
    };

    this.formSubmit.emit({ basePayload, selectedFiles: this.selectedFiles });
  }

  public clearFormAndStorage() {
    this.selectedFiles = [];
    this.uploadedPhotos.set([]);
    this.eventForm.reset();
  }
}