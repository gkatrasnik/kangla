export type WateringCommandStatus = 'pending' | 'acknowledged' | 'completed' | 'failed' | 'cancelled' | 'expired';

export interface WateringCommand {
  id: number;
  deviceId: number;
  status: WateringCommandStatus;
  durationSeconds: number;
  requestedAtUtc: string;
  expiresAtUtc: string;
  acknowledgedAtUtc: string | null;
  startedAtUtc: string | null;
  finishedAtUtc: string | null;
  failureReason: string | null;
  wateringEventId: number | null;
}
