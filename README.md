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
- `EMAIL_SETTINGS_RESEND_API_KEY`: Your Resend API key.
- `OPENAI_API_KEY`: OpenAI api key for recognizing images.

Copy `.env.example` for local tooling only. Never commit a populated `.env` file.

## Future plans: ESP32 watering devices

Kangla is planned to support physical watering devices built with ESP32 hardware.
Each device will be linked to a plant, allowing users to trigger watering for that
plant from the app. Devices will also send soil-humidity readings to Kangla, so
users can view the `HumidityMeasurement` data collected from their plants.

## ESP32 watering-device protocol

Kangla is ready for ESP32 watering devices. Each device has a fixed device access
key printed on its sticker. The user enters that key to claim and attach the device
to one plant. Kangla stores only a SHA-256 hash of the key; the key is sent by the
device in the `X-Device-Access-Key` header and is never returned by the API.

The device calls `POST /api/device/check-ins` every minute. Include `rawSoilMoisture`
from `0` through `4095` and its calibrated `soilMoisturePercentage` from `0` through
`100` together when reporting telemetry; Kangla stores both values for the linked
device. The response contains any pending
manual watering command. A device must acknowledge a command before activating its
pump, then report either completion or failure. Kangla records a watering event only
after confirmed completion.

Users create a manual watering request with
`POST /api/WateringDevices/{deviceId}/watering-commands`. Commands expire after 15
minutes if the device does not acknowledge them, and only one active command is
allowed per device.

For an ESP32 using the default 12-bit ADC resolution, start with dry and wet
calibration values of `3200` and `1500` and calibrate each physical sensor:

```cpp
analogReadResolution(12);
const int rawValue = analogRead(sensorPin);
int moisturePercentage = map(rawValue, 3200, 1500, 0, 100);
moisturePercentage = constrain(moisturePercentage, 0, 100);
```

Keep ADC attenuation unchanged after calibrating. Automatic watering is not
implemented yet; `wateringIntervalSetting` remains reserved for that future mode.

## Device simulator

`src/kangla.DeviceSimulator` is a local .NET console application that implements
the current device protocol against a running API. Its development-only sticker
access key is `kangla-simulator-01`. Attach that key to a plant in the UI, then run:

```powershell
dotnet run --project src/kangla.DeviceSimulator -- https://localhost:7049 2350 5
```

The optional final arguments are the simulated raw 12-bit soil-moisture value and
check-in interval in seconds. The simulator maps the raw value to a percentage with
the default 3200/1500 calibration, then acknowledges and completes any manual
watering command returned by a check-in using the device's configured duration.

On Windows, [run-device-simulator.bat](run-device-simulator.bat) starts the
simulator against the local HTTPS API address with its built-in development key.

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
- `EMAIL_SETTINGS_DEFAULT_FROM_EMAIL`, `EMAIL_SETTINGS_RESEND_API_KEY`, and
  `OPENAI_API_KEY`.

The Docker user must be able to run Docker and use passwordless `sudo` to create
the application data directories. They are owned by the container's non-root
user (UID/GID `1001`).

Development seeding creates predictable demonstration accounts and must never be
enabled on an internet-accessible deployment.
