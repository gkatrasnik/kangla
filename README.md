# Kangla

Kangla is a plant care app designed as a hobby project. Users can upload photos of plants, and the app identifies them, adding them to a personal collection with basic information and recommended watering intervals. Users can log watering events, and Kangla will notify them when it’s time to water their plants again, making plant care simple and enjoyable.

## Tech stack
Dotnet web api + angular PWA client application.

Client app:
  - Angular 18
  - Scss
  - Angular Material
  - PWA
    
Api:
  - Dotnet 8
  - Clean architecture
  - Code first Entity framework with Sqlite
  - ASP.NET Core Identity Endpoints
  - Serilog logging
  - OpenAI api integration for plant recognition
  - Swagger

## Environment Variables

To run this application, you need to set up the following environment variables:

- `EMAIL_SETTINGS_DEFAULT_FROM_EMAIL`: The email address from which emails will be sent.
- `EMAIL_SETTINGS_HOST`: The SMTP server hostname.
- `EMAIL_SETTINGS_PORT`: The port number used by the SMTP server (e.g., 587 for TLS).
- `EMAIL_SETTINGS_USERNAME`: The username for SMTP authentication.
- `EMAIL_SETTINGS_PASSWORD`: The password for SMTP authentication.
- `OPENAI_API_KEY`: OpenAI api key for recognizing images.
