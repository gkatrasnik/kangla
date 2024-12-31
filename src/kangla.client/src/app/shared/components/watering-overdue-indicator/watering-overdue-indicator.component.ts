import { Component } from '@angular/core';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'app-watering-overdue-indicator',
    imports: [MatChipsModule, MatIconModule],
    templateUrl: './watering-overdue-indicator.component.html',
    styleUrl: './watering-overdue-indicator.component.scss'
})
export class WateringOverdueIndicatorComponent {

}
