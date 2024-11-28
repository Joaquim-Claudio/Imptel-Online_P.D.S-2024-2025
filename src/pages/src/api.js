import axios from "axios";
import { response } from "express";

const http = axios.create({
    baseURL: "http://localhost:5293/api/accounts/auth"
})

const Login = async (req, res) => {

    return res.send(
        http.get()
    )
}

export default Login;