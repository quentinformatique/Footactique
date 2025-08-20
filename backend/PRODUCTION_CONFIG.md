# Deployment Configuration Template

Copy this file to `appsettings.Production.json` and update the values for your production environment.

## Required Configuration Changes:

### Database Connection
- Update `ConnectionStrings.footactique` with your production PostgreSQL connection string
- Ensure the database user has appropriate permissions

### CORS Origins
- Update `AllowedOrigins` array with your actual production domain(s)
- Remove any localhost entries

### JWT Security
- Generate a strong, random JWT key (minimum 32 characters)
- Never use the development key in production
- Keep this key secret and secure

### Logging
- Production logging is set to Warning level to reduce noise
- Consider using a structured logging provider for production

## Environment Variables (Alternative to appsettings.json)

You can also set these values using environment variables:

```bash
ConnectionStrings__footactique="Host=..."
AllowedOrigins__0="https://your-domain.com"
Jwt__Key="your-secret-key"
Jwt__Issuer="Footactique"
```

## Security Checklist

- [ ] Strong, unique JWT secret key
- [ ] Secure database credentials
- [ ] HTTPS-only domains in CORS
- [ ] No development URLs in production config
- [ ] Database connection uses SSL in production