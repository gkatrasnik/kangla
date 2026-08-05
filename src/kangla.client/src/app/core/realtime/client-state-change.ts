export type ClientStateResource =
  | 'plant'
  | 'wateringCommands'
  | 'wateringEvents'
  | 'humidityMeasurements';

export interface ClientStateChange {
  plantId: number | null;
  deviceId: number | null;
  resources: ClientStateResource[];
  occurredAtUtc: string;
}
