import { DatePipe } from '@angular/common';
import { Component, Input } from '@angular/core';
import { LatestSoilMoistureMeasurement } from '../../../watering-devices/watering-device';

@Component({
  selector: 'app-soil-moisture-gauge',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './soil-moisture-gauge.component.html',
  styleUrl: './soil-moisture-gauge.component.scss'
})
export class SoilMoistureGaugeComponent {
  @Input() measurement: LatestSoilMoistureMeasurement | null = null;
  @Input() target: number | null | undefined = null;
  @Input() compact = false;

  get percentage(): number | null {
    return this.measurement?.soilMoisturePercentage ?? null;
  }

  get gaugeColor(): string {
    if (this.percentage === null || this.target === null || this.target === undefined) {
      return 'var(--app-text-muted)';
    }

    const deviation = Math.abs(this.percentage - this.target);
    const hue = 120 * Math.max(0, 1 - Math.max(0, deviation - 15) / 15);
    return `hsl(${hue} 68% 40%)`;
  }

  get statusLabel(): string {
    if (this.percentage === null) {
      return 'Waiting for sensor reading';
    }

    if (this.target === null || this.target === undefined) {
      return 'Set moisture target';
    }

    const difference = this.percentage - this.target;
    if (difference === 0) {
      return '✓ Target';
    }

    return `${difference < 0 ? '↓' : '↑'} ${Math.abs(difference)}%`;
  }

  get statusDescription(): string {
    if (this.percentage === null) {
      return 'Waiting for sensor reading';
    }

    if (this.target === null || this.target === undefined) {
      return 'Set moisture target';
    }

    const difference = this.percentage - this.target;
    if (difference === 0) {
      return 'At target';
    }

    const points = Math.abs(difference);
    return `${points} ${points === 1 ? 'point' : 'points'} ${difference < 0 ? 'below' : 'above'} target`;
  }

  get ariaLabel(): string {
    if (this.percentage === null) {
      return 'Soil moisture: waiting for sensor reading.';
    }

    const targetText = this.target === null || this.target === undefined
      ? 'No target is set.'
      : `Target is ${this.target} percent.`;
    return `Soil moisture is ${this.percentage} percent. ${targetText} ${this.statusDescription}.`;
  }

  get targetMarker(): { x1: number; y1: number; x2: number; y2: number } | null {
    if (this.target === null || this.target === undefined) {
      return null;
    }

    const clampedTarget = Math.min(100, Math.max(0, this.target));
    const angle = Math.PI - clampedTarget / 100 * Math.PI;
    return {
      x1: 60 + 42 * Math.cos(angle),
      y1: 60 - 42 * Math.sin(angle),
      x2: 60 + 53 * Math.cos(angle),
      y2: 60 - 53 * Math.sin(angle)
    };
  }
}
