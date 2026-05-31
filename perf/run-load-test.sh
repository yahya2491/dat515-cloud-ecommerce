#!/usr/bin/env bash
#
# Simple helper to run the k6 load test defined in perf/basic-load-test.js
# and publish metrics to Prometheus/Grafana.

set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
COMPOSE_FILE="${ROOT_DIR}/infra/docker-compose.yml"

K6_VUS="${K6_VUS:-10}"
K6_DURATION="${K6_DURATION:-1m}"

echo "▶️  Ensuring core services are running (postgres, api, prometheus, grafana)..."
docker compose -f "${COMPOSE_FILE}" up -d postgres api prometheus grafana

echo "▶️  Running k6 load test with VUs=${K6_VUS}, duration=${K6_DURATION}..."
docker compose -f "${COMPOSE_FILE}" run --rm \
  -e K6_VUS="${K6_VUS}" \
  -e K6_DURATION="${K6_DURATION}" \
  k6

echo "✅ Load test finished."
echo "   • Prometheus: http://localhost:9090"
echo "   • Grafana:    http://localhost:3001 (Load Testing › API Load Overview)"
echo "   • k6 summary: perf/results/basic-load-summary.json & .html"
