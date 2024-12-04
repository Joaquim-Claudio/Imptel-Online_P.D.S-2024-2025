import React from "react";
import InputElement from "../components/InputElement";
import searchIcon from "../assets/images/Frame34.svg";
import ButtonNew from "../components/ButtonNew";
import plusIcon from "../assets/images/fi-br-plus.svg"
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"

import { PAGE } from "../assets/utils/PageIdMap";

function Enrollment({user}) {
   
    return (

        <div className="container-fluid">
            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.ENROLLMENT} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={"Matrículas"} />

                        <main>


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
                                                <h2  className="section-header ">Dados da Matrícula</h2>
                                            </header>

                                        <form className="form-parent" action="/" method="">

                                            <div className="col-3 pe-2" >
                                            <InputElement 
                                                placeholder="Exemplo: 20240022" 
                                                label={"Número de estudante"} 
                                                forId={"idNumber"}
                                                type={"text"}
                                                />
                                            </div>
                                            <div className="col-9 ps-2">
                                            <InputElement 
                                                placeholder="Nome completo" 
                                                label={"Nome"} 
                                                forId={"name"}
                                                type={"text"}
                                                />
                                            </div>


                                            <div className="col-12">
                                            <InputElement 
                                                placeholder="Médio Técnico de Informática" 
                                                label={"Curso"} 
                                                forId={"course"}
                                                type={"text"}
                                                />
                                            </div>

                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                placeholder="Exemplo: 004557777HO087" 
                                                label={"Classe"}  
                                                forId={"grade"}
                                                type={"text"}
                                                />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                placeholder="I10A"  
                                                label={"Turma"} 
                                                forId={"class"}
                                                type={"text"}
                                                />
                                            </div>
                                        
                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                placeholder="22-11-2024" 
                                                label={"Data"} 
                                                forId={"dateEn"}
                                                type={"date"}
                                                />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                placeholder="Activa" 
                                                label={"Estado"} 
                                                forId={"status"}
                                                type={"text"}
                                                />
                                            </div>
                                            
                                        </form>
                                        
                                        
                                    </div>


                                    {/* #######################################3333333############################33###################### */}
                                    <div className="col-3 text-end ">
                                        <div className="action-group">
                                            <ButtonNew
                                            icon={plusIcon} 
                                            label={"Novo"}                   
                                            />
                                        </div>
                                            
                                    </div>
                                </div>            
                            </div>

                        </main>
                    </div>
                </div>
            </div>

            <Footer/>
            
        </div>

    );
}

export default Enrollment;

