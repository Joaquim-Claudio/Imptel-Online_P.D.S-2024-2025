import React from "react";
import { useState } from "react";
import SideBar from "../components/SideBar";
import Toolbar from "../components/Toolbar";
import { PAGE } from "../assets/utils/PageIdMap";
import CardPayment from "../components/CardPayment";
import educationImage from "../assets/images/education-growth-concept-assortment.png";
import calculatorImage from "../assets/images/woman-doing-accounting.png"




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
                            <div className="container-fluid payment-Container">
                                    <>
                                    <header className="payment-header">
                                        <h2 >Área Financeira</h2>
                                    </header>

                                    <div className="cards ">
                                        <div className=" cards-p col-5">
                                            <CardPayment 
                                                image={educationImage} 
                                                to="/paymentsValues" 
                                                title={"Valores a Pagamento"} 
                                            />
                                            <div className="textPayment ">Verifique os seus pagamentos pendentes e mantenha-os em dia.</div>
                                        </div>
                                        <div className="col-5  ">
                                            <CardPayment 
                                                image={calculatorImage} 
                                                to="/paymentsInvoice" 
                                                title={"Recibos"} 
                                            />
                                            <div className="textPayment ">Consulte os seus pagamentos anteriores e obtenha faturas/recibos.</div>
                                        </div>
                                    </div>
                                    </>

                            </div>
                        </main>


                    </div>

                </div>
            </div>
       </div>
    )

}

export default Payments;