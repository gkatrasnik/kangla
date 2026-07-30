export interface WateringDevice {
  id: number;
  minimumSoilHumidity: number;
  wateringIntervalSetting: number;
  wateringDurationSetting: number;
  plantId: number;
}
