export interface AuthErrorResponse {
    type: string;
    title: string;
    status: number;
    errors?: { [key: string]: string[] };
    detail?: string;
}
