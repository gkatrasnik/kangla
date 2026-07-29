import { LoadingInterceptor } from './loading.interceptor';
import { LoadingService } from './loading.service';

describe('LoadingInterceptor', () => {
  let interceptor: LoadingInterceptor;

  beforeEach(() => {
    const loadingService = jasmine.createSpyObj<LoadingService>(
      'LoadingService',
      ['loadingOn', 'loadingOff']
    );
    interceptor = new LoadingInterceptor(loadingService);
  });

  it('should be created', () => {
    expect(interceptor).toBeTruthy();
  });
});
