import { Component, EventEmitter, inject, Output, signal, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MapService } from '../../../core/services/map.service';
import { EventService } from '../../../core/services/event.service';
import { PaymentService } from '../../../core/services/payment.service';
import * as L from 'leaflet';

@Component({
  selector: 'app-create-event-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './create-event-modal.component.html',
  styleUrls: ['./create-event-modal.component.scss']
})
export class CreateEventModalComponent implements AfterViewInit, OnDestroy {
  @Output() closeModal = new EventEmitter<void>();
  @Output() eventCreated = new EventEmitter<string>();

  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private mapService = inject(MapService);
  private paymentService = inject(PaymentService);

  private readonly DRAFT_KEY = 'event_draft_form';

  isSubmitting = signal(false);
  createdEventId = signal<string | null>(null);

  private selectedLat: number | undefined = undefined;
  private selectedLon: number | undefined = undefined;
  private selectedOsmId: string | undefined = undefined;

  private map!: L.Map;
  private marker!: L.Marker;

  eventForm = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(300)]],
    description: ['', Validators.required],
    startDate: ['', Validators.required],
    locationName: ['', Validators.required],
    city: ['', Validators.required],
    street: ['', Validators.required],
    houseNumber: ['']
  });

  ngAfterViewInit() {
    this.initMap();
    this.loadDraft();

    this.eventForm.valueChanges.subscribe(() => this.saveDraft());
  }

  ngOnDestroy() {
    if (this.map) {
      this.map.remove();
    }
  }

  private saveDraft() {
    if (this.createdEventId()) return;
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
          
          if (this.marker && this.map) {
            this.marker.setLatLng([draft.lat, draft.lon]);
            this.map.setView([draft.lat, draft.lon], 15);
          }
        }
      } catch (e) {
        console.error('Ошибка чтения черновика', e);
      }
    }
  }

  private initMap() {
    const iconDefault = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
      shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
      iconSize: [25, 41],
      iconAnchor: [12, 41],
      popupAnchor: [1, -34],
      shadowSize: [41, 41]
    });
    L.Marker.prototype.options.icon = iconDefault;

    this.map = L.map('event-map').setView([50.4501, 30.5234], 13);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors'
    }).addTo(this.map);

    this.marker = L.marker([50.4501, 30.5234], { draggable: true }).addTo(this.map);

    this.marker.on('dragend', () => {
      const position = this.marker.getLatLng();
      this.updateLocationData(position.lat, position.lng);
    });

    this.map.on('click', (e: any) => {
      this.marker.setLatLng(e.latlng);
      this.updateLocationData(e.latlng.lat, e.latlng.lng);
    });

    setTimeout(() => {
      this.map.invalidateSize();
    }, 100);
  }

  private updateLocationData(lat: number, lon: number) {
    this.mapService.reverseGeocode(lat, lon).subscribe((res: any) => {
        if (res && res.address) {
          this.selectedLat = lat;
          this.selectedLon = lon;
          this.selectedOsmId = res.osm_id?.toString() || undefined;

          const addr = res.address;
          const locName = res.name || addr.amenity || addr.building || ''; 

          this.eventForm.patchValue({
              locationName: locName,
              city: addr.city || addr.town || addr.village || '',
              street: addr.road || '',
              houseNumber: addr.house_number || ''
          });
          
          this.saveDraft();
        }
    });
  }

  close() {
    this.closeModal.emit();
  }

  submit() {
    if (this.eventForm.invalid) return;
    this.isSubmitting.set(true);

    const formValue = this.eventForm.getRawValue();
    const payload = {
      ...formValue,
      startDate: new Date(formValue.startDate).toISOString(),
      latitude: this.selectedLat,
      longitude: this.selectedLon,
      osmId: this.selectedOsmId
    };

    this.eventService.createEvent(payload).subscribe({
      next: (res) => {
        this.isSubmitting.set(false);
        localStorage.removeItem(this.DRAFT_KEY);
        this.createdEventId.set(res.eventId);
      },
      error: (err) => {
        this.isSubmitting.set(false);
        console.error(err);
      }
    });
  }

  finishWithoutVip() {
    if (this.createdEventId()) {
      this.eventCreated.emit(this.createdEventId()!);
    }
    this.close();
  }

  buyVip() {
    const id = this.createdEventId();
    if (id) {
      this.paymentService.payForVip(id);
    }
  }
}