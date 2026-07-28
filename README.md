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
  - .NET 9
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

## Docker deployment

Pushing to `main` builds the combined .NET 9 API and Angular PWA, publishes both
`latest` images to GitHub Container Registry, then deploys the image to the OCI
VPS. The Hetzner workflow is run manually and expects an existing Docker network
named `web_network` for the reverse proxy.

Add these GitHub repository secrets:

- `OCI_SSH_PRIVATE_KEY`, `OCI_INSTANCE_IP`, `OCI_USERNAME`, and `GHCR_PAT` for
  the automatic OCI VPS deployment.
- `HETZNER_INSTANCE_IP` and `HETZNER_USERNAME` for the manual Hetzner deployment
  (it reuses `OCI_SSH_PRIVATE_KEY` and `GHCR_PAT`).
- `EMAIL_SETTINGS_DEFAULT_FROM_EMAIL`, `EMAIL_SETTINGS_HOST`,
  `EMAIL_SETTINGS_PORT`, `EMAIL_SETTINGS_USERNAME`, `EMAIL_SETTINGS_PASSWORD`,
  and `OPENAI_API_KEY`.

The Docker user must be able to run Docker and use passwordless `sudo` to create
the application data directories. They are owned by the container's non-root
user (UID/GID `1001`).
