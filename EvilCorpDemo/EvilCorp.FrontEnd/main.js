const express = require('express');

let APP_PORT = process.env.APP_PORT ?? 5500;

const app = express();
app.use( express.static( __dirname + '/client' ));

app.get('/get-ably-api-key', (req, res) => {
  res.send(process.env.ABLY_API_KEY);
});

app.listen(APP_PORT);