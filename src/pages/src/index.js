import express from "express";
import Login from "./api.js";

const app = express();

app.use(express.static('public'));

app.get('/', (req, res) => {
    return res.send('Hello world!');
})

app.get('/login', Login);

app.listen(3000);