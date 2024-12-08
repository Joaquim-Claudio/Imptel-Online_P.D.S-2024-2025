import React from "react";

import Alert from "../components/Alert";

import banner from "../assets/images/imgExemploIA.png"
import logo from "../assets/images/logo.svg"

import axios from "axios"

const accounts = axios.create({
    baseURL: import.meta.env.VITE_ACCOUNT_SERVICE_URL,
    withCredentials: true
})

function Login () {
    const [success, setSuccess] = React.useState(false);
    const [error, setError] = React.useState(false);
    const [isLoading, setIsLoading] = React.useState(false);
    const [username, setUsername] = React.useState("");
    const [password, setPassword] = React.useState("");

    const [inputClass, setInputClass] = React.useState("form-control");
    const [patternClass, setPaternClass] = React.useState("red-msg");
    const [failClass, setfailClass] = React.useState("red-msg");

    const regex = /^(?!.*\b(SELECT|INSERT|DELETE|UPDATE|DROP|TRUNCATE|ALTER|CREATE|REPLACE|EXEC|UNION|TABLE|DATABASE)\b).*\S.*$/i;

    function showPatternError() {
        setInputClass(current => current + " red-border");
        setPaternClass(current => current + " show");
    }

    function showFail() {
        setInputClass(current => current + " red-border");
        setfailClass(current => current + " show");
    }

    function hideErrors() {
        setInputClass(current => current.replaceAll(" red-border", ""));
        setPaternClass(current => current.replaceAll(" show", ""));
        setfailClass(current => current.replaceAll(" show", ""));
    }


    function handleUsernameChange(event) {
        setUsername(event.target.value);
        hideErrors();
    }

    function handelPasswordChange(event) {
        setPassword(event.target.value);
        hideErrors();
    }

    function handleSubmit(event) {
        event.preventDefault();

        if(!regex.test(username) || !regex.test(password)) {
            showPatternError();
            return;
        }

        try {
            setIsLoading(true);

            accounts.post("login", {
                username,
                password
            }) .then( (response) => {
                setIsLoading(false);

                setSuccess(true);

                setTimeout(function() {
                    setSuccess(false);
                    window.location.replace("/");
                }, 2000)


            }).catch( (error) => {
                    if(!error.response){
                        console.error("No error response");

                        setError(true)

                        setTimeout(function() {
                            setError(false);
                        }, 3000)
                    }
                    else if (error.response?.status == 401) {
                        console.error("Response: " + error.response.status + " \"Unauthorized\"");
                        showFail();
                    }
                    else console.error("Login failed");
           
                    setIsLoading(false)
                });

        } catch(err) {
            console.error(err)
        }
        
    }


    return (
        <div className="container-fluid login-container">

            <Alert 
                fireOn={isLoading}
                title="Autenticação em curso..."
                text="A verificar as suas credenciais."
                icon="loader"
                showBadge={true}
            />


            <Alert 
                fireOn={error}
                title="Upsss!"
                text="Algo correu mal... Tente outra vez dentro de alguns minutos."
                icon="warning"
                showBadge={true}
            />

            <Alert 
                fireOn={success}
                title="Bem-vindo"
                text="Autenticado com sucesso."
                icon="success"
                showBadge={true}
            />


            <div className="row">
                <div className="col-lg-7 d-none d-lg-block px-lg-0 py-lg-0 img-left ">
                    <img className="img-fluid img-blur h-100 w-100 " src={banner} alt=""/>

                </div>
                <div className="col-lg-5 col-12 px-lg-5">        

                    <div className="col-12 text-center pt-5">
                        <img className="img-fluid img-logo" src={logo} alt="" />
                    </div>
                    <div className="col-12">
                        <div className="fw-bold text-login">Atenção:</div>
                        <div className="col-12 text-login ">
                            Digite o seu número de utilizador e a palavra-passe
                        </div>

                        <form onSubmit={handleSubmit}>
                            <div className="input-group input-group-sm mb-3 box-login-size">
                                <input 
                                    id="usernameInput" 
                                    type="text" 
                                    className={inputClass} 
                                    placeholder="Nome de Utilizador" 
                                    aria-label="Username input" 
                                    autoComplete="username"
                                    value={username}
                                    onChange={handleUsernameChange}/>
                            </div>

                            <div className="input-group input-group-sm mb-3 box-login-size ">
                                <input 
                                    id="passwordInput"  
                                    type="password" 
                                    className={inputClass} 
                                    placeholder="Palavra-passe" 
                                    aria-label="Password input" 
                                    autoComplete="current-password"
                                    value={password}
                                    onChange={handelPasswordChange}/>
                            </div>

                            <span id="ptrnMsg" className={patternClass}>
                                <i className="fa-solid fa-circle-info me-2"></i>
                                Campos de preenchimento obrigatório.
                            </span>

                            <span id="failMsg" className={failClass}>
                                <i className="fa-solid fa-circle-info me-2"></i>
                                Nome de utilizador ou palavra-passe incorreta.
                            </span>

                            <div className="text-center mt-3">
                                <button type="submit" className="btn box-login-size btn-primary">
                                    Login
                                </button>
                            </div>
                        </form>
                        
                        <div className="text-center text-login py-5">
                            <a  href="">
                                Recuperar Password
                            </a>
                            
                        </div>
                        
                    </div>
                </div>
            </div>

            <link rel="stylesheet" href="../styles/login.scss" />
        </div>
    )
}

export default Login;