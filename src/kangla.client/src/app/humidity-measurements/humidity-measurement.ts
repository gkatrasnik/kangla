export interface HumidityMeasurement {
  id: number;
  dateTime: string;
  rawSoilMoisture: number;
  soilMoisturePercentage: number | null;
  wateringDeviceId: number;
}
