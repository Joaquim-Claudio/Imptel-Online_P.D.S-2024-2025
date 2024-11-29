import express from "express";
import cors from "cors";

const app = express();

app.use(cors());
app.use(express.static('public'));

app.get('/', (req, res) => {
    return res.render('index');
})

app.listen(3000);