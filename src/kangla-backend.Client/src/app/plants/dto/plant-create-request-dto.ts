export interface PlantCreateRequestDto {
    name: string;                        // Required, max length 30
    scientificName?: string;             // Optional, max length 50
    description?: string;                // Optional, max length 100
    location?: string;                   // Optional, max length 100
    notes?: string;                      // Optional, max length 500
    wateringInterval: number;            // Required, range 1-365
    wateringInstructions?: string;       // Optional, max length 500
    imageId?: number | null;             // Optional, nullable
  }
  