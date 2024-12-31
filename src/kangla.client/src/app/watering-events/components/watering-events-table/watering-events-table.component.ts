import { Component, Input, OnInit, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { CommonModule } from '@angular/common';
import { WateringEvent } from '../../watering-event';
import { WateringEventService } from '../../watering-event.service';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NotificationService } from '../../../core/notifications/notification.service';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { DialogData } from '../../../shared/interfaces/dialog-data';
import { PagedResponse } from '../../../shared/interfaces/paged-response';
import { MatCardModule } from '@angular/material/card';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

@Component({
    selector: 'app-watering-events-table',
    imports: [
        MatTableModule,
        CommonModule,
        MatButtonModule,
        MatIconModule,
        MatCardModule,
        MatPaginatorModule
    ],
    templateUrl: './watering-events-table.component.html',
    styleUrl: './watering-events-table.component.scss'
})
export class WateringEventsTableComponent implements OnInit, OnChanges {
  displayedColumns: string[] = ['createdAt', 'delete'];
  dataSource = new MatTableDataSource<WateringEvent>([]);
  wateringEvents: WateringEvent[] = [];

  @Input() plantId: number | undefined;
  @Input() reloadTrigger: number = 0; //when watering event in "details" page is added reload trigger is changed.

  // Pagination
  wateringEventsListLength = 0;
  pageSize = 10;
  pageIndex = 0;
  pageSizeOptions = [10, 20, 50];
  hidePageSize = false;
  showPageSizeOptions = true;
  showFirstLastButtons = true;
  disabled = false;

  constructor(
    private wateringEventService: WateringEventService,
    private notificationService: NotificationService,
    public dialog: MatDialog
  ) { }  

  handlePageEvent(e: PageEvent) {
    this.wateringEventsListLength = e.length;
    this.pageSize = e.pageSize;
    this.pageIndex = e.pageIndex;
    this.loadWateringEvents(this.pageIndex, this.pageSize);
  }

  ngOnInit(): void {
    this.loadWateringEvents(this.pageIndex, this.pageSize);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['reloadTrigger'] && !changes['reloadTrigger'].firstChange) {
      this.loadWateringEvents(this.pageIndex, this.pageSize);
    }
  }

  loadWateringEvents(pageIndex: number, pageSize: number): void {
    if (!this.plantId) {
      return;
    }

    this.wateringEventService.getAllWateringEventsByPlantId(this.plantId, pageIndex + 1, pageSize).subscribe((pagedResponse: PagedResponse<WateringEvent>) => {
      this.wateringEvents = pagedResponse.data;
      this.dataSource.data = this.wateringEvents;

      this.wateringEventsListLength = pagedResponse.totalRecords;
    });
  }

  deleteWateringEvent(eventId: number): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Delete Watering Event',
        message: 'Are you sure you want to delete this watering event?'
      } as DialogData
    });         

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Watering event deleted:', result);
        this.doDeleteWateringEvent(eventId);        
      }
    });
  }
  
  doDeleteWateringEvent(eventId: number): void {
    this.wateringEventService.deleteWateringEvent(eventId).subscribe({
      next: () => {
        this.loadWateringEvents(this.pageIndex, this.pageSize);
        this.notificationService.showNonErrorSnackBar('Watering event deleted');
      },
      error: (err) => {
        console.error('Error deleting event:', err);
      }
    });
  }
}