import { of } from 'rxjs';
import { WateringCommandService } from '../../watering-command.service';
import { WateringCommandsTableComponent } from './watering-commands-table.component';

describe('WateringCommandsTableComponent', () => {
  it('loads the selected page and tracks the total record count', () => {
    const service = jasmine.createSpyObj<WateringCommandService>('WateringCommandService', ['getAll']);
    service.getAll.and.returnValue(of({
      pageNumber: 2,
      pageSize: 20,
      totalPages: 3,
      totalRecords: 42,
      data: []
    }));
    const component = new WateringCommandsTableComponent(service);
    component.deviceId = 7;

    component.handlePageEvent({ pageIndex: 1, pageSize: 20, length: 42, previousPageIndex: 0 });

    expect(service.getAll).toHaveBeenCalledOnceWith(7, 2, 20);
    expect(component.totalRecords).toBe(42);
  });
});
