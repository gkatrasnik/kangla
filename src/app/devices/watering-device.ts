export interface WateringDevice {
    id: number;
    name: string;
    description?: string;
    location?: string;
    notes?: string;
    active?: boolean;
    deleted?: boolean;
    waterNow?: boolean;
    minimumSoilHumidity: number;
    wateringIntervalSetting: number;
    wateringDurationSetting: number;
    deviceToken?: string;
    createdAt?: Date;
    updatedAt?: Date;
}
