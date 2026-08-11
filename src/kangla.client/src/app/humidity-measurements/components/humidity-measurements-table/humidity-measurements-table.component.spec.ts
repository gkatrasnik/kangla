import { of } from 'rxjs';
import { HumidityMeasurementService } from '../../humidity-measurement.service';
import { HumidityMeasurementsTableComponent } from './humidity-measurements-table.component';

describe('HumidityMeasurementsTableComponent', () => {
  it('loads the selected page and tracks the total record count', () => {
    const service = jasmine.createSpyObj<HumidityMeasurementService>('HumidityMeasurementService', ['getAll']);
    service.getAll.and.returnValue(of({
      pageNumber: 3,
      pageSize: 20,
      totalPages: 4,
      totalRecords: 61,
      data: []
    }));
    const component = new HumidityMeasurementsTableComponent(service);
    component.deviceId = 9;

    component.handlePageEvent({ pageIndex: 2, pageSize: 20, length: 61, previousPageIndex: 1 });

    expect(service.getAll).toHaveBeenCalledOnceWith(9, 3, 20);
    expect(component.totalRecords).toBe(61);
  });
});
