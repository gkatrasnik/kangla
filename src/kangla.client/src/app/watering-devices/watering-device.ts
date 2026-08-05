import { ActiveWateringCommandStatus } from '../watering-commands/watering-command';

export interface WateringDevice {
  id: number;
  active: boolean;
  deleted: boolean;
  minimumSoilHumidity: number;
  wateringIntervalSetting: number;
  wateringDurationSetting: number;
  plantId: number | null;
  activeWateringCommandStatus: ActiveWateringCommandStatus | null;
}
