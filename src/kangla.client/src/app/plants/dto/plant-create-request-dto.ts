export interface PlantCreateRequestDto {
  name: string;
  scientificName?: string;
  description?: string;
  location?: string;
  notes?: string;
  wateringInterval: number;
  desiredSoilMoisturePercentage: number;
  wateringInstructions?: string;
  imageId?: string | null;
}
