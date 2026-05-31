import http from 'k6/http';
import { check, sleep } from 'k6';
import { htmlReport } from 'https://jslib.k6.io/k6-summary/0.0.4/index.js';

const baseUrl = __ENV.API_BASE_URL || 'http://api:8080/api';

export const options = {
  vus: Number(__ENV.K6_VUS || 5),
  duration: __ENV.K6_DURATION || '30s',
  thresholds: {
    http_req_duration: ['p(95)<750'],
    http_req_failed: ['rate<0.02'],
  },
};
//aaa
function safeJson(response, label) {
  try {
    return response.json();
  } catch (err) {
    const snippet = (response.body || '').slice(0, 200);
    console.error(
      `Failed to parse ${label} response as JSON: ${err.message}. status=${response.status}, body="${snippet}"`
    );
    return null;
  }
}

export default function () {
  const productsResponse = http.get(`${baseUrl}/products`);
  const productsData = safeJson(productsResponse, 'products');
  check(productsResponse, {
    'products: status 200': (r) => r.status === 200,
    'products: has items': () => Array.isArray(productsData) && productsData.length > 0,
  });

  const usersResponse = http.get(`${baseUrl}/user`);
  const usersData = safeJson(usersResponse, 'users');
  check(usersResponse, {
    'users: status 200': (r) => r.status === 200,
  });

  if (Array.isArray(usersData) && usersData.length > 0) {
    const userId = usersData[0].id;

    const cartResponse = http.get(`${baseUrl}/cart/user/${userId}`);
    const cartData = safeJson(cartResponse, 'cart');
    check(cartResponse, {
      'cart: status 200': (r) => r.status === 200,
      'cart: is object': () => cartData !== null && typeof cartData === 'object',
    });
  }

  sleep(1);
}

export function handleSummary(data) {
  const summaryPath = __ENV.K6_SUMMARY_PATH || '/scripts/results/basic-load-summary.html';
  return {
    stdout: JSON.stringify(data, null, 2),
    [summaryPath]: htmlReport(data),
  };
}
