## Kangla

Dotnet web api + angular client application.

User can upload photo of a plant, application will recognize the plant via openAi api, and add it to the database, with some basic info about the plant.
User can mark when the plant was watered, and after recomended watering period for the plant, the warning will appear.

## Environment Variables

To run this application, you need to set up the following environment variables:

- `EMAIL_SETTINGS_DEFAULT_FROM_EMAIL`: The email address from which emails will be sent.
- `EMAIL_SETTINGS_HOST`: The SMTP server hostname.
- `EMAIL_SETTINGS_PORT`: The port number used by the SMTP server (e.g., 587 for TLS).
- `EMAIL_SETTINGS_USERNAME`: The username for SMTP authentication.
- `EMAIL_SETTINGS_PASSWORD`: The password for SMTP authentication.
- `OPENAI_API_KEY`: OpenAI api key for recognizing images.
