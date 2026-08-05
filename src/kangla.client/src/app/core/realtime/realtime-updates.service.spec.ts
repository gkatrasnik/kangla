import { reconnectDelay } from './realtime-updates.service';

describe('RealtimeUpdatesService reconnect backoff', () => {
  it('uses the configured progression and caps at sixty seconds', () => {
    expect(reconnectDelay(0, 0.5)).toBe(0);
    expect(reconnectDelay(1, 0.5)).toBe(2000);
    expect(reconnectDelay(2, 0.5)).toBe(5000);
    expect(reconnectDelay(3, 0.5)).toBe(10000);
    expect(reconnectDelay(4, 0.5)).toBe(30000);
    expect(reconnectDelay(5, 0.5)).toBe(60000);
    expect(reconnectDelay(20, 0.5)).toBe(60000);
  });

  it('adds bounded jitter to non-zero retry delays', () => {
    expect(reconnectDelay(1, 0)).toBe(1600);
    expect(reconnectDelay(1, 1)).toBe(2400);
  });
});
