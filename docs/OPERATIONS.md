## Footactique - Operations Runbook

This runbook is the single source of truth for operating Footactique in production. It covers architecture, deployment on Debian/OVH, configuration, updates/rollbacks, backups and disaster recovery, monitoring, and troubleshooting.

Use this as a step-by-step guide during incidents and routine operations.

---

### 1) System Overview

- **Frontend**: React app, built with `react-scripts`, served by Caddy.
- **API**: ASP.NET Core 9 (`net9.0`), Entity Framework Core with PostgreSQL, JWT auth.
- **Database**: PostgreSQL 16.
- **Reverse proxy & TLS**: Caddy v2 provides HTTPS via Let’s Encrypt and reverse-proxies `/api` to the API.
- **Orchestration**: Docker Compose (prod stack lives in `deploy/`).

Key repo paths:
- `deploy/docker-compose.yml` – production stack (web/api/db)
- `deploy/Caddyfile` – TLS/reverse-proxy and static hosting
- `backend/Footactique.ApiService/Program.cs` – API bootstrapping; proxy headers; EF migrations autoload
- `backend/Footactique.ApiService/appsettings.json` – defaults for API; prod overrides via env vars
- `backend/Footactique.Services/appsettings.json` – dev connection strings (not used in prod)
- `frontend/src/services/api.ts` – frontend API base URL (built with `REACT_APP_API_URL`; in prod we use `/api`)

Runtime containers (Compose service names):
- `web` – Caddy + React build output
- `api` – ASP.NET Core application (listens on `8080` inside container)
- `db` – PostgreSQL instance (persistent volume `pgdata`)

---

### 2) Production Deployment (Debian/OVH)

Prerequisites on VPS (Debian 11/12):
```bash
apt-get update -y
apt-get install -y ca-certificates curl gnupg

install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /etc/apt/keyrings/docker.gpg
chmod a+r /etc/apt/keyrings/docker.gpg

echo "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
https://download.docker.com/linux/debian $(. /etc/os-release; echo $VERSION_CODENAME) stable" \
> /etc/apt/sources.list.d/docker.list

apt-get update -y
apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
systemctl enable --now docker
```

Optional firewall:
```bash
apt-get install -y ufw
ufw allow 80
ufw allow 443
```

If Apache/Nginx is occupying 80/443, stop/remove it:
```bash
ss -tulpn | grep -E ':80|:443' || true
systemctl stop apache2 nginx || true
apt-get remove -y apache2 nginx || true
```

DNS (OVH):
- Create `A` record for `footactique.<your-domain>` pointing to VPS IPv4
- Optional: create `AAAA` if VPS has functional IPv6; otherwise leave it out
- Propagation: usually 5–30 minutes; extreme cases up to 24h

Environment file (`deploy/.env`):
```bash
DOMAIN=footactique.costesquentin.fr  # your FQDN (no protocol)
ACME_EMAIL=admin@example.com         # email for Let’s Encrypt
POSTGRES_PASSWORD=StrongPassword     # postgres superuser password
JWT_KEY=LongRandomSecret             # JWT signing key for API
```
Generate secure secrets:
```bash
openssl rand -base64 48
```

Initial deploy:
```bash
cd /opt
git clone <repo_url> Footactique
cd Footactique/deploy
cp .env.example .env && nano .env   # adjust values
docker compose build --pull
docker compose up -d
```

Access:
- Frontend: `https://$DOMAIN`
- API: proxied at `https://$DOMAIN/api`

Certificates are provisioned automatically by Caddy. If DNS is not propagated yet, cert issuance will retry automatically; you can reload Caddy once DNS is good:
```bash
docker compose exec web caddy reload --config /etc/caddy/Caddyfile
```

---

### 3) Configuration Reference

Environment variables (configured in `deploy/docker-compose.yml` and `.env`):
- `DOMAIN` – host served by Caddy
- `ACME_EMAIL` – Let’s Encrypt account email
- `POSTGRES_PASSWORD` – postgres DB password (user: `postgres`, db: `footactique`)
- `JWT_KEY` – API JWT signing key
- API connection string is passed via `ConnectionStrings__footactique` to .NET, pointing to `db` service

Caddyfile behavior (`deploy/Caddyfile`):
- Serves the React build from `/usr/share/caddy`
- Proxies any path starting with `/api` to `api:8080` (prefix preserved)
- Automatically handles HTTP→HTTPS redirects and cert renewals

API boot behavior (`Program.cs`):
- Respects proxy headers via `UseForwardedHeaders` (prevents HTTPS loops)
- Applies EF Core migrations automatically on startup (`Database.Migrate()`)

---

### 4) Routine Operations

Start/stop/restart stack:
```bash
cd /opt/Footactique/deploy
docker compose up -d          # start
docker compose stop           # stop
docker compose restart web    # restart only web
docker compose restart api    # restart only api
docker compose restart        # restart all
```

View logs:
```bash
docker compose logs -f web
docker compose logs -f api
docker compose logs -f db
```

Container health and resources:
```bash
docker ps
docker stats
docker inspect footactique-api | jq '.[0].State'
```

Disk usage and cleanup:
```bash
docker system df
docker image prune -f
docker builder prune -f
```

---

### 5) Updating the Application

Safe update procedure (with build cache refresh):
```bash
cd /opt/Footactique
git pull
cd deploy
docker compose build --pull --no-cache web api
docker compose up -d
```

Notes:
- Always ensure DNS `DOMAIN` still matches your host.
- EF Core migrations run automatically on API start; ensure new migrations are included in the code if schema changed.
- For major DB changes, back up DB before update (see Backups) and have a rollback plan.

Rollback (previous commit):
```bash
cd /opt/Footactique
git log --oneline -n 5
git checkout <previous_commit>
cd deploy
docker compose build --pull --no-cache web api
docker compose up -d
```

Revert to main afterward:
```bash
cd /opt/Footactique
git checkout main
```

---

### 6) Database Operations

Connect to psql shell:
```bash
docker exec -it footactique-postgres psql -U postgres -d footactique
```

Manual backup:
```bash
docker exec -t footactique-postgres \
  pg_dump -U postgres -d footactique > /opt/footactique_dump_$(date +%F).sql
```

Manual restore:
```bash
cat /opt/footactique_dump_YYYY-MM-DD.sql | \
  docker exec -i footactique-postgres psql -U postgres -d footactique
```

Automated daily backup via cron (example):
```bash
crontab -e
# Add (runs at 02:15 every day):
15 2 * * * docker exec -t footactique-postgres pg_dump -U postgres -d footactique > /opt/footactique_dump_$(date +\%F).sql
```

Storage location: ensure `/opt` has sufficient disk space or move dumps to another volume.

---

### 7) TLS, DNS, and Domains

- Caddy automatically issues and renews certificates via Let’s Encrypt.
- Requirements: valid public DNS `A` (and optional `AAAA`) records for `DOMAIN`.
- If VPS has no IPv6, avoid creating `AAAA` to prevent failed handshakes.
- Change of `DOMAIN`:
  1. Update DNS records to point to VPS
  2. Update `.env` `DOMAIN=<new_domain>`
  3. `docker compose up -d --force-recreate web`
  4. Verify logs: `docker compose logs -f web`

Add `www` host (optional):
```caddy
# In deploy/Caddyfile, add second site block or a list of hosts
{your-domain} www.{your-domain} {
    # same handlers as primary block
}
```

Common TLS/DNS issues:
- `NXDOMAIN` in logs → DNS not propagated or record missing
- IPv6 present in DNS but not on VPS → remove `AAAA`
- Another service on 80/443 → stop Apache/Nginx; free ports

---

### 8) Troubleshooting Guide

Symptoms → Checks → Fixes.

- 404 on `POST /api/auth/register` behind proxy
  - Check `deploy/Caddyfile` uses `handle /api*` (not `handle_path`) so `/api` prefix is preserved.
  - Reload Caddy: `docker compose exec web caddy reload --config /etc/caddy/Caddyfile`.

- 502/Bad Gateway
  - `docker compose logs -f api` → API crashing? connection refused?
  - Ensure `api` is healthy and listening on 8080; `docker ps`, `docker logs footactique-api`.
  - `ConnectionStrings__footactique` correct? DB up? Check `db` logs.

- HTTPS redirect loops
  - Reverse proxy header handling must be enabled; `Program.cs` contains `UseForwardedHeaders` (already configured).
  - Confirm Caddy handles TLS termination (it does by default).

- Cert issuance fails with `urn:acme:error:dns`
  - DNS not propagated; verify with `getent hosts <domain>`
  - Remove AAAA if IPv6 not configured
  - Open ports 80/443 through firewall

- DB connection errors
  - Verify `POSTGRES_PASSWORD` in `.env`
  - `docker compose logs -f db` for Postgres startup errors
  - Ensure `api` env var `ConnectionStrings__footactique` points to `Host=db;Port=5432;...`

- Disk full / image pull failures
  - Clean up: `docker system df`, `docker image prune -f`, `docker builder prune -f`
  - Move backups off the VPS or rotate older dumps

- Permission denied on docker socket
  - Use `sudo` or add user to `docker` group: `usermod -aG docker $USER && newgrp docker`

- HTTP/3 UDP buffer warnings in Caddy
  - Harmless. Optional tuning:
    ```bash
    sysctl -w net.core.rmem_max=2500000
    sysctl -w net.core.rmem_default=2500000
    ```

---

### 9) Observability and Logging

- Logs are accessible via `docker compose logs -f <service>`.
- ASP.NET logging levels can be configured in `appsettings.json` or environment.
- Consider centralizing logs in the future (e.g., Loki/Promtail, ELK) if needed.

Basic health checks:
```bash
curl -I https://$DOMAIN         # HTTP 200 for index.html
curl -I https://$DOMAIN/api     # should return 404/empty (no route), but server headers present
```

---

### 10) Security and Hardening

- Keep Debian, Docker, and images up-to-date (`--pull` builds).
- Store secrets only in `.env` on the server; never commit `.env` to git.
- Rotate `JWT_KEY` periodically; doing so invalidates existing refresh tokens.
- Ensure only ports 80/443 are publicly open; DB is internal-only within the Docker network.
- Limit SSH access; use key-based auth.
- Regular database backups and offsite copies.

---

### 11) Disaster Recovery (DR)

Scenario: VPS total loss.
1. Provision a new Debian VPS.
2. Reinstall Docker/Compose (see Section 2).
3. Restore repo and `.env`:
   ```bash
   cd /opt && git clone <repo_url> Footactique
   cd Footactique/deploy && nano .env   # fill values
   ```
4. Restore DB from backup:
   ```bash
   docker compose up -d db
   cat /path/to/backup.sql | docker exec -i footactique-postgres psql -U postgres -d footactique
   ```
5. Start API and web:
   ```bash
   docker compose up -d --build api web
   ```
6. Update DNS if IP changed; wait for propagation; Caddy will re-issue certs.

---

### 12) Local Development

Frontend:
```bash
cd frontend
npm install
npm start
```

Backend:
```bash
cd backend/Footactique.ApiService
dotnet run
```

Database (local container):
```bash
cd backend
docker compose up -d postgres
```

Tests:
```bash
cd backend
dotnet test -c Release
```

---

### 13) Known Conventions and Preferences

- Controllers are thin; business logic in services.
- Use appsettings for connection strings, not hard-coded.
- Avoid null-forgiving operator; prefer `required` properties.
- MSTest for unit/integration tests, Arrange-Act-Assert format.
- Extensive logging at controller/service layers.
- Clean file organization: DTOs, models, services in separate folders.

---

### 14) Change Log (Ops)

- 2025-08: Introduced Caddy + Compose deployment (`deploy/`), API `UseForwardedHeaders`, and auto EF migrations at startup.
- 2025-08: Fixed reverse-proxy path to preserve `/api` prefix (use `handle /api*`).

---

### 15) Quick Command Cheatsheet

Deploy/update:
```bash
cd /opt/Footactique && git pull
cd deploy && docker compose build --pull --no-cache web api && docker compose up -d
```

Logs:
```bash
docker compose logs -f web | sed -u 's/.*/[WEB] &/'
docker compose logs -f api | sed -u 's/.*/[API] &/'
```

Backups:
```bash
docker exec -t footactique-postgres pg_dump -U postgres -d footactique > /opt/footactique_dump_$(date +%F).sql
```

Certs reload:
```bash
docker compose exec web caddy reload --config /etc/caddy/Caddyfile
```

---

If you need environment-specific overrides or additional domains, update `.env` and `deploy/Caddyfile`, rebuild if necessary, and reload Caddy.


