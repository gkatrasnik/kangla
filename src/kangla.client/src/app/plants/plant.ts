export interface Plant {
    id: number;
    name: string;
    scientificName?: string;
    description?: string;
    location?: string;
    notes?: string;
    wateringInterval: number;
    desiredSoilMoisturePercentage?: number | null;
    wateringInstructions?: string;
    createdAt: Date;
    updatedAt: Date;
    imageId?: string;
    lastWateringDateTime?: Date;
}
