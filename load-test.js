import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
    scenarios: {
        hpa_test: {
            executor: 'ramping-arrival-rate',
            timeUnit: '1m',                             // unidade de tempo para o arrival rate
            preAllocatedVUs: 5,
            maxVUs: 30,
            stages: [
                // 🔹 Fase 1 — carga baixa (baseline)
                { target: 800, duration: '1m' },         // começa com 120 RPM durante 1 minuto (2 por segundo)

                // 🔹 Fase 2 — começa a pressionar
                { target: 1500, duration: '1m' },         // aumenta para 240 RPM durante 1 minuto (4 por segundo)
                { target: 2000, duration: '1m' },         // aumenta para 480 RPM durante 1 minuto (8 por segundo)
                { target: 3500, duration: '3m' },         // aumenta para 720 RPM durante 3 minutos (12 por segundo)

                // 🔻 Fase 3 — mantém carga alta
                { target: 1000, duration: '2m' },         // mantém carga alta durante 2 minutos (8 por segundo)
                { target: 600, duration: '2m' },        // mantém 240 RPM durante 2 minutos (4 por segundo)

                // 🧊 Fase 4 — cai quase a zero (força scale down)
                { target: 60, duration: '5m' },         // reduz para 60 RPM durante 5 minutos
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
            'Ocp-Apim-Subscription-Key': '6096bd1760144b52a5aa06462dcb0013'
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

    const res = http.get(RUN_URL, params);

    check(res, {
        'status 200': r => r.status === 200,
    });

    // mantém o ritmo do arrival-rate
    sleep(1);
}
