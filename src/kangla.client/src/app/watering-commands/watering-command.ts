export type WateringCommandStatus = 'pending' | 'acknowledged' | 'completed' | 'failed' | 'cancelled' | 'expired' | 'timedOut';
export type ActiveWateringCommandStatus = Extract<WateringCommandStatus, 'pending' | 'acknowledged'>;

export function isActiveWateringCommandStatus(
  status: WateringCommandStatus | null | undefined
): status is ActiveWateringCommandStatus {
  return status === 'pending' || status === 'acknowledged';
}

export function getActiveWateringCommandLabel(status: ActiveWateringCommandStatus): string {
  return status === 'pending' ? 'Watering queued' : 'Watering now';
}

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
