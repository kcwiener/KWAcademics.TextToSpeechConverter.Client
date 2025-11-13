# KWAcademics.TextToSpeechConverter.CLient

This application transforms written text into spoken audio, allowing users to adjust the speech speed and voice type. It also provides the option to download the generated audio file for offline use.

## API Integration

The client communicates with a backend API service that handles the actual text-to-speech conversion. The API expects:

**Request:**
- Text content (string, max 200 words)
- Prosody rate (integer, -50 to +50)

**Response:**
- Audio data (base64-encoded WAV)
- Duration (seconds)
- Word count
- WPM (words per minute)
- Success status and message

## Deployment

The application is configured for deployment to Azure Static Web Apps. The `staticwebapp.config.json` file includes:

- Route caching strategies
- Navigation fallback for SPA routing
- MIME type configurations
- Security headers (X-Content-Type-Options, X-Frame-Options, X-XSS-Protection)

## Security Features

- Authorization required for all pages
- Secure token-based API authentication
- HTTPS enforcement
- Security headers for XSS and clickjacking protection
- Local storage caching for authentication tokens

## Limitations

- Maximum text length: 200 words
- Supported file format: .txt only
- Maximum file size: 1MB
- Output format: WAV audio

## License

Copyright 2025 KWAcademics. All rights reserved.

## Author

Kyle Wiener (kcwiener)

## Repository

https://github.com/kcwiener/KWAcademics.TextToSpeechConverter.Client
