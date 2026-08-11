export interface PlantRecognizeResponseDto {    
    commonName?: string 
    latinName?: string 
    description?: string
    additionalTips?: string
    wateringInterval?: number
    desiredSoilMoisturePercentage?: number | null
    wateringInstructions?: string
    imageId?: string
    identificationConfidence?: 'low' | 'medium' | 'high'
    error?: string
}
