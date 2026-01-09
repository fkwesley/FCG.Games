import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    scenarios: {
        hpa_test: {
            executor: 'ramping-arrival-rate',
            timeUnit: '1m',
            preAllocatedVUs: 5,
            maxVUs: 500,
            stages: [
                { target: 1000, duration: '1m' },
                { target: 2000, duration: '1m' },
                { target: 5000, duration: '1m' },
                { target: 10000, duration: '3m' },

                { target: 1000, duration: '2m' },
                { target: 600, duration: '2m' },

                { target: 60, duration: '5m' },
            ],
        },
    },

    thresholds: {
        http_req_failed: ['rate<0.05'],
        http_req_duration: ['p(95)<2000'],
    },
};

/**
 * =========================
 * ENDPOINTS
 * =========================
 */
const LOGIN_URL = 'http://4.239.149.248/Auth/Login';
const RUN_URL = 'http://apimfcg.azure-api.net/games/games/';

/**
 * =========================
 * SETUP – LOGIN
 * =========================
 */
export function setup() {
    const payload = JSON.stringify({
        userId: 'frank.vieira',
        password: 'Password1*',
    });

    const params = {
        headers: {
            accept: 'text/plain',
            'Content-Type': 'application/json',
            'Ocp-Apim-Subscription-Key': '6096bd1760144b52a5aa06462dcb0013',
        },
    };

    const res = http.post(LOGIN_URL, payload, params);

    check(res, {
        'login ok': r => r.status === 200,
        'token ok': r => r.json('token') !== undefined,
    });

    return { token: res.json('token') };
}

/**
 * =========================
 * TESTE PRINCIPAL – POST
 * =========================
 */
export default function (data) {
    // 🔐 nome 100% único
    const uniqueName = `FIFA-${__VU}-${__ITER}-${Date.now()}`;

    const body = JSON.stringify({
        name: uniqueName,
        description: 'Soccer game',
        genre: 'Soccer',
        price: 223,
        releaseDate: '1985-09-13',
        rating: 10,
    });

    const params = {
        headers: {
            accept: 'text/plain',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${data.token}`,
        },
    };

    const res = http.post(RUN_URL, body, params);

    check(res, {
        'status sucesso': r => r.status === 201 || r.status === 200,
    });

    //sleep(1);
}
