export interface PlantRecognizeResponseDto {    
    commonName?: string 
    latinName?: string 
    description?: string
    additionalTips?: string
    wateringInterval?: number
    wateringInstructions?: string 
    imageId?: string
    error?: string
}
