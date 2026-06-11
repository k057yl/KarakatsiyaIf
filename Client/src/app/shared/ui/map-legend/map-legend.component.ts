import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-map-legend',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './map-legend.component.html',
  styleUrls: ['./map-legend.component.scss']
})
export class MapLegendComponent {
  @Input() public activeFilter: string = 'all';
  @Output() public filterChanged = new EventEmitter<string>();

  public selectFilter(filterType: string): void {
    this.filterChanged.emit(filterType);
  }
}