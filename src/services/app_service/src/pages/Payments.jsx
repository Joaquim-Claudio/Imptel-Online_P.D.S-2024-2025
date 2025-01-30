import React from "react";
import { useState } from "react";
import SideBar from "../components/SideBar";
import Toolbar from "../components/Toolbar";
import Footer from "../components/Footer"
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
                            <div className="container-fluid">
                                    <>
                                    <header className="main-header">
                                        <h2 >Área Financeira</h2>
                                    </header>

                                    <div className="cards ">
                                        <div className="col-4">
                                            <CardPayment 
                                                image={educationImage} 
                                                to="/payments/pending-payments"
                                                title={"Valores a Pagamento"} 
                                            />
                                            <div className="textPayment ">Verifique os seus pagamentos pendentes e mantenha-os em dia.</div>
                                        </div>
                                        <div className="col-4">
                                            <CardPayment 
                                                image={calculatorImage} 
                                                to="/payments/invoices" 
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

            <Footer acadYear={user.acadYear}/>
       </div>
    )

}

export default Payments;