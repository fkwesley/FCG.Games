import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    scenarios: {
        hpa_test: {
            executor: 'ramping-arrival-rate',
            timeUnit: '1s',
            preAllocatedVUs: 5,
            maxVUs: 150,
            stages: [
                // 🔹 Fase 1 — carga baixa (baseline)
                { target: 5, duration: '1m' },

                // 🔹 Fase 2 — começa a pressionar
                { target: 10, duration: '1m' },
                { target: 20, duration: '1m' },
                { target: 30, duration: '3m' },

                // 🔻 Fase 3 — mantém carga alta
                { target: 40, duration: '3m' },
                { target: 50, duration: '2m' },

                // 🧊 Fase 4 — cai quase a zero (força scale down)
                { target: 5, duration: '4m' },
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
const LOGIN_URL = 'http://4.239.177.16/Auth/Login';
const USERS_URL = 'http://4.239.177.16/Users';

/**
 * =========================
 * SETUP – LOGIN DINÂMICO
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
        },
    };

    const res = http.post(LOGIN_URL, payload, params);

    check(res, {
        'login status 200': r => r.status === 200,
        'token retornado': r => r.json('token') !== undefined,
    });

    return {
        token: res.json('token'),
    };
}

/**
 * =========================
 * TESTE PRINCIPAL
 * =========================
 */
export default function (data) {
    const params = {
        headers: {
            accept: 'text/plain',
            Authorization: `Bearer ${data.token}`,
        },
    };

    const res = http.get(USERS_URL, params);

    check(res, {
        'status 200': r => r.status === 200,
    });

    // mantém o ritmo do arrival-rate
    sleep(0.01);
}