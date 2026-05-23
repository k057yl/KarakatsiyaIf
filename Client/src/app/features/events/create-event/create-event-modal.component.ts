import { Component, EventEmitter, inject, Output, signal, AfterViewInit, OnDestroy, input, OnInit } from '@angular/core';
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
export class CreateEventModalComponent implements OnInit, AfterViewInit, OnDestroy {
  @Output() closeModal = new EventEmitter<void>();
  @Output() eventCreated = new EventEmitter<string>();

  public editEventData = input<any | null>(null);

  private fb = inject(FormBuilder);
  private eventService = inject(EventService);
  private mapService = inject(MapService);
  private paymentService = inject(PaymentService);

  private readonly DRAFT_KEY = 'event_draft_form';

  public isSubmitting = signal(false);
  public createdEventId = signal<string | null>(null);
  public isEditMode = signal<boolean>(false);
  public isSuccessScreen = signal<boolean>(false);

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

  ngOnInit() {
    const data = this.editEventData();
    if (data) {
      this.isEditMode.set(true);
      this.createdEventId.set(data.id);

      const formattedDate = data.startDate ? new Date(data.startDate).toISOString().slice(0, 16) : '';

      this.eventForm.patchValue({
        title: data.title || '',
        description: data.description || '',
        startDate: formattedDate,
        locationName: data.locationName || '',
        city: data.city || '',
        street: data.street || '',
        houseNumber: data.houseNumber || ''
      });

      this.selectedLat = data.latitude;
      this.selectedLon = data.longitude;
      this.selectedOsmId = data.osmId;
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

      if (!this.isEditMode()) {
        this.loadDraft();
        this.eventForm.valueChanges.subscribe(() => this.saveDraft());
      } else if (this.selectedLat && this.selectedLon) {
        this.moveMarkerAndSetView(this.selectedLat, this.selectedLon);
      }
    }, 50);
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
    if (this.createdEventId() && !this.isEditMode()) return;
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
          
          if (!this.isEditMode()) {
            this.saveDraft();
          }
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

    if (this.isEditMode()) {
      const id = this.createdEventId()!;
      this.eventService.updateEvent(id, payload).subscribe({
        next: () => {
          this.isSubmitting.set(false);
          this.eventCreated.emit(id);
          this.close();
        },
        error: (err) => {
          this.isSubmitting.set(false);
          console.error(err);
        }
      });
    } else {
      this.eventService.createEvent(payload).subscribe({
        next: (res) => {
          this.isSubmitting.set(false);
          localStorage.removeItem(this.DRAFT_KEY);
          this.createdEventId.set(res.eventId);
          this.isSuccessScreen.set(true);
        },
        error: (err) => {
          this.isSubmitting.set(false);
          console.error(err);
        }
      });
    }
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