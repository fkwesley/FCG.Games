import http from 'k6/http';
import { check, sleep } from 'k6';
import { Counter } from 'k6/metrics';

// Contadores customizados
export const post201 = new Counter('post_201');
export const post403 = new Counter('post_403');
export const post400 = new Counter('post_400');

export const put200 = new Counter('put_200');
export const put403 = new Counter('put_403');
export const put400 = new Counter('put_400');

export const get200 = new Counter('get_200');
export const delete204 = new Counter('delete_204');

export let options = {
    //vus: 5,           // 10 usuários simultâneos
    //iterations: 100, // Use essa linha para rodar por número de iterações
    //duration: '1m',  // Use essa linha para rodar por tempo (ex: 10 minutos)
    stages: [        // Use essa seção para ramp-up e ramp-down
        { duration: '2m', target: 30 }, // ramp-up para 10 usuários em 2 minutos
        { duration: '5m', target: 60 }, // mantém 10 usuários por 5 minutos
        { duration: '2m', target: 10 },  // ramp-down para 0 usuários em 2 minutos
    ]
};

const users = [
    { userId: 'CJ', password: 'Password1*' },
    { userId: 'max.payne', password: 'teste123*'},
    { userId: 'frank.vieira', password: 'Password1*' }
];

// 🔐 Autenticação única
export function setup() {
    const tokens = {};

    for (const user of users) {
        const loginPayload = JSON.stringify(user);
        const loginHeaders = {
            'accept': 'text/plain',
            'Content-Type': 'application/json',
        };

        const loginRes = http.post(
            'https://aca-fcg-uat.agreeablemushroom-99bd6ac3.brazilsouth.azurecontainerapps.io/Auth/Login',
            loginPayload,
            { headers: loginHeaders }
        );

        check(loginRes, {
            [`Login do ${user.userId} realizado com sucesso (200)`]: (res) => res.status === 200,
        });

        try {
            const json = JSON.parse(loginRes.body);
            tokens[user.userId] = json.token;
            console.log(`Token obtido para ${user.userId}:`, json.token);
        } catch (err) {
            console.error(`Erro ao parsear o token para ${user.userId}:`, err, loginRes.body);
        }
        console.log('\n');
    }

    return { tokens };
}

export default function (data) {
    const user = users[Math.floor(Math.random() * users.length)];
    const token = data.tokens[user.userId];

    const userHeaders = {
        'accept': 'text/plain',
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    };

    // GET /Games - consulta todos
    const getRes = http.get(
        'https://aca-fcg-uat.agreeablemushroom-99bd6ac3.brazilsouth.azurecontainerapps.io/Games',
        { headers: userHeaders }
    );

    const gameName = `GTA_${__VU}_${__ITER}`;
    let foundGame;
    if (getRes.status === 200) {
        get200.add(1);
        try {
            const games = JSON.parse(getRes.body);
            foundGame = games.find(g => g.name === gameName);

            // DELETE /Games (caso já exista o registro e usuário seja frank.vieira)
            if (foundGame && user.userId === 'frank.vieira' && foundGame.gameId) {
                const deleteRes = http.del(
                    `https://aca-fcg-uat.agreeablemushroom-99bd6ac3.brazilsouth.azurecontainerapps.io/Games/${foundGame.gameId}`,
                    null,
                    { headers: userHeaders }
                );

                check(deleteRes, { 'DELETE /games realizado com sucesso (204)': (r) => r.status === 204 });

                if (deleteRes.status !== 204) {
                    console.error(`Erro ao deletar o jogo ${gameName}: status ${deleteRes.status}, body: ${deleteRes.body}`);
                    delete204.add(1);
                }
                sleep(0.2);
            }
        } catch (err) {
            console.error('Erro ao parsear resposta do GET /Games:', err, getRes.body);
        }
    } else {
        console.warn(`Falha no GET /Games para buscar jogo: status ${getRes.status}, body: ${getRes.body}`);
    }

    // POST /Games - insere
    const postPayload = JSON.stringify({ name: gameName, description: "Grand Theft Auto", genre: "Third-person shooter" });
    const postRes = http.post('https://aca-fcg-uat.agreeablemushroom-99bd6ac3.brazilsouth.azurecontainerapps.io/Games', postPayload, { headers: userHeaders });

    if (user.userId === 'frank.vieira')
        check(postRes, { 'POST /games realizado com sucesso com o usuário frank.vieira (201)' : (r) => r.status === 201 });
    else
        check(postRes, { 'POST /games não autenticado para outro usuário (403)': (r) => r.status === 403 });

    if ((user.userId === 'frank.vieira' && postRes.status !== 201) || (user.userId !== 'frank.vieira' && postRes.status !== 403))
        console.error(`Erro no POST /Games para usuário ${user.userId}: status ${postRes.status}, body: ${postRes.body}`);

    if (user.userId === 'frank.vieira' && postRes.status === 201)
        post201.add(1);
    if (user.userId !== 'frank.vieira' && postRes.status === 403)
        post403.add(1);

    // PUT /Games - atualiza rating e releaseDate se iteração for múltiplo de 2
    sleep(0.2);
    const gameToUpdate = postRes.status === 201 ? JSON.parse(postRes.body).gameId : 1 ;

    const futureDate = new Date(Date.now() + 50 * 24 * 60 * 60 * 1000).toISOString().slice(0, 10);
    const putPayload = JSON.stringify({
        name: __ITER % 5 === 0 ? gameName : `update test ${__VU}_${__ITER}`,
        description: "update test",
        genre: "soccer",
        rating: 3,
        releaseDate: futureDate
    });

    const putRes = http.put(
        `https://aca-fcg-uat.agreeablemushroom-99bd6ac3.brazilsouth.azurecontainerapps.io/Games/${gameToUpdate}`,
        putPayload,
        { headers: userHeaders }
    );

    //console.log(`PUT /Games para usuário ${user.userId} , status:  ${putRes.status} , body: ${putRes.body}`);

    if (putRes.status === 200) {
        check(putRes, { 'PUT /games realizado com sucesso (200)': (r) => r.status === 200 });
        put200.add(1);
    } else if (putRes.body && putRes.body.includes('already exists') && putRes.status === 400) {
        check(putRes, { 'PUT /games já existente (400)': (r) => r.status === 400 });
        put400.add(1);
    } else if (user.userId !== 'frank.vieira' && postRes.status === 403) {
        check(putRes, { 'PUT /games não autenticado para outro usuário (403)': (r) => r.status === 403 });
        put403.add(1);
    }
    else
        console.error(`Erro no PUT /Games para usuário ${user.userId}: status ${putRes.status}, body: ${putRes.body}`);

    sleep(0.5);
}