import { ActiveWateringCommandStatus } from '../watering-commands/watering-command';

export interface WateringDevice {
  id: number;
  active: boolean;
  deleted: boolean;
  wateringIntervalSetting: number;
  wateringDurationSetting: number;
  plantId: number | null;
  activeWateringCommandStatus: ActiveWateringCommandStatus | null;
  latestSoilMoistureMeasurement?: LatestSoilMoistureMeasurement | null;
}

export interface LatestSoilMoistureMeasurement {
  rawSoilMoisture: number;
  soilMoisturePercentage: number;
  measuredAtUtc: Date;
}
