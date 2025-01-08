import React from "react";
import SideBar from "../components/SideBar";
import Toolbar from "../components/Toolbar";
import { PAGE } from "../assets/utils/PageIdMap";
import CardPayment from "../components/CardPayment";
import Dropdown from "../components/Dropdown";

import ImageCard from "../assets/images/thumb-cartao-credito-loja-maquina.jpg"


import axios from "axios"

const registries = axios.create({
    baseURL: import.meta.env.VITE_REGISTRY_SERVICE_URL,
    withCredentials: true
});

const auxiliar = axios.create({
    baseURL: import.meta.env.VITE_AUXILIAR_SERVICE_URL,
    withCredentials: true
});

function Payments({user}){
    return(
       <div className="container-fluid">
            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.PAYMENTS} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={"Pagamentos"}/>

                        <main>
                            <div className="container-fluid">
                                <header className="payment-header">
                                    <h2 className="section-header pb-5">Área Financeira</h2>
                                </header>
                                <div className="cards">
                                    <CardPayment image={ImageCard}  title={"Valores a Pagamento"} />
                                    <CardPayment image={ImageCard}  title={"Recibos"} />

                                </div>
                                
                            </div>
                        </main>

                    </div>

                </div>
            </div>
       </div>
    )

}

export default Payments;