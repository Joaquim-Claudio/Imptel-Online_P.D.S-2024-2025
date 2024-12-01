import React from "react";
import InputElement from "../components/InputElement";
import searchIcon from "../assets/images/Frame34.svg";
import ButtonNew from "../components/ButtonNew";
import plusIcon from "../assets/images/fi-br-plus.svg"
import Footer from "../components/Footer";

function Student(){
   
    return(
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
                            <h2  className="section-header ">Dados do Aluno</h2>
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

                        
                        <div className="col-6 pe-2">
                        <InputElement 
                            placeholder="Exemplo: 004557777HO087" 
                            label={"Número do documento de identificação"}  
                            forId={"bi"}
                            type={"text"}
                            />
                        </div>
                        <div className="col-6 ps-2">
                        <InputElement 
                            placeholder="dd-mm-aaaa"  
                            label={"Data de nascimento"} 
                            forId={"birthdate"}
                            type={"date"}
                            />
                        </div>


                        <div className="col-12">
                        <InputElement 
                            placeholder="Morada completa" 
                            label={"Morada completa"} 
                            forId={"address"}
                            type={"text"}
                            />
                        </div>

                        
                        
                        <div className="col-6 pe-2">
                        <InputElement 
                            placeholder="exemplo@imptel.com" 
                            label={"Email"} 
                            forId={"mail"}
                            type={"email"}
                            />
                        </div>
                        <div className="col-6 ps-2">
                        <InputElement 
                            placeholder="900 000 000" 
                            label={"Telefone"} 
                            forId={"phone"}
                            type={"number"}
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
            <Footer/>

    


        </div>



       
    );
}

export default Student;
