import React from "react";
import { useState } from "react";
import SideBar from "../components/SideBar";
import Toolbar from "../components/Toolbar";
import { PAGE } from "../assets/utils/PageIdMap";
import CardPayment from "../components/CardPayment";
import InputElement from "../components/InputElement";
import searchIcon from "../assets/images/Frame34.svg";
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
    const [activeSection, setActiveSection]=useState("home");
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
                                {activeSection === "home" ? (
                                    <>
                                    <header className="payment-header">
                                        <h2 className="section-header pb-5">Área Financeira</h2>
                                    </header>

                                    <div className="cards">
                                        <CardPayment 
                                            image={ImageCard} 
                                            onClick={() => setActiveSection("valores")} 
                                            title={"Valores a Pagamento"} 
                                        />
                                        <CardPayment 
                                            image={ImageCard} 
                                            onClick={() => setActiveSection("recibos")} 
                                            title={"Recibos"} 
                                        />
                                    </div>
                                    </>
                                ) : activeSection === "valores" ? (
                                    <>

                                    <div className=" container-fluid ">
                                            <div className="row justify-content-center">
                                                <div className="col-5 pe-3">
                                                    <InputElement 
                                                        placeholder="Pesquisar: nome ou número de estudante" 
                                                        icon={searchIcon} 
                                                        forId={"search"}/>
                                                </div>
                                                <div className="col-1">
                                                    <button className=" btn-search">
                                                        <img src={searchIcon}  alt="" />
                                                    </button>
                                                </div>
                                            </div>
                                            <div className="row justify-content-center">
                                                <div className="col-8">

                                                        <header className="text-center">
                                                            <h2  className="section-header ">Valores a pagamento</h2>
                                                        </header>
                                                </div>
                                            </div>
                                    </div>

                                    </>

                                    
                                ) : activeSection === "recibos" ? (
                                    <>

                                    <div className=" container-fluid ">
                                        <div className="row justify-content-center">
                                            <div className="col-5 pe-3">
                                                <InputElement 
                                                    placeholder="Pesquisar: nome ou número de estudante" 
                                                    icon={searchIcon} 
                                                    forId={"search"}/>
                                            </div>
                                            <div className="col-1">
                                                <button className=" btn-search">
                                                    <img src={searchIcon}  alt="" />
                                                </button>
                                            </div>
                                        </div>
                                        <div className="row justify-content-center">
                                            <div className="col-8">

                                                    <header className="text-center">
                                                        <h2  className="section-header ">Recibos</h2>
                                                    </header>
                                            </div>
                                        </div>
                                    </div>
                                    </>
                                ) : null}
                            </div>
                        </main>


                    </div>

                </div>
            </div>
       </div>
    )

}

export default Payments;