const express = require('express');

const APP_PORT = process.env.APP_PORT ?? 5500;
const DAPR_HTTP_PORT = process.env.DAPR_HTTP_PORT ?? 3501;
const DAPR_HOST = process.env.DAPR_HOST ?? "http://localhost";
const BASE_URL = `${DAPR_HOST}:${DAPR_HTTP_PORT}`;

const app = express();
app.use(express.json());
app.use( express.static( __dirname + '/client' ));

app.get('/get-ably-api-key', async (req, res) => {
  let response = await fetch(`${BASE_URL}/v1.0/secrets/localsecretstore/AblyApiKey`);
  let secret = await response.json();
  res.send(secret.AblyApiKey);
});

app.listen(APP_PORT);