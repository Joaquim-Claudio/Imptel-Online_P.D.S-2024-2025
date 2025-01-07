import React, { useState } from 'react';

import InputElement from "../components/InputElement";
import ButtonNew from "../components/ButtonNew";
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"
import Alert from "../components/Alert";


import plusIcon from "../assets/images/fi-br-plus.svg"
import trashIcon from "../assets/images/fi-br-trash.svg"
import chIcon from "../assets/images/fi-br-change.svg"
import searchIcon from "../assets/images/Frame34.svg";
import { PAGE } from "../assets/utils/PageIdMap";


import axios from "axios"

const accounts = axios.create({
    baseURL: import.meta.env.VITE_ACCOUNT_SERVICE_URL,
    withCredentials: true
})


function Student({user}) {

//Botão novo

const [isCreating, setIsCreating]= React.useState(false);
const handleNewStudent =() =>{
    setIsCreating(true);
    setInternId('');
    setName('');
    setDocId('');
    setBirthDate('');
    setAddress('');
    setEmail('');
    setPhone('');
};

const handleSaveStudent =()=>{
    //setIsCreating(false);
    setIsCreating(false);
    setIsEditing(false);
};

const handleCancel=()=>{
    //setIsCancel(false);
    setIsCreating(false);
    setIsEditing(false);
    setStudent(null);
    setInternId('');
    setName('');
    setDocId('');
    setBirthDate('');
    setAddress('');
    setEmail('');
    setPhone(''); 
};



/////////////////////////////////////////////////////////////////////

//Boão editar 
const [isEditing, setIsEditing] = React.useState(false);

const handleEditStudent =() =>{
    setIsEditing(true);
};


////////////




    const [isLoading, setIsLoading] = React.useState(false);
    const [netError, setNetError] = React.useState(false);
    const [sessionExpired, setSessionExpired] = React.useState(false);
    const [notFoundError, setNotFoundError] = React.useState(false);

    const [keywords, setKeywords] = React.useState("");
    const [student, setStudent] = React.useState(null);

    const [internId, setInternId] = React.useState("");
    const [name, setName] = React.useState("");
    const [docId, setDocId] = React.useState("");
    const [birthDate, setBirthDate] = React.useState("");
    const [address, setAddress] = React.useState("");
    const [email, setEmail] = React.useState("");
    const [phone, setPhone] = React.useState("");

    function populate(data) {
        setInternId(data.internId);
        setName(data.name);
        setDocId(data.docId);
        setBirthDate(data.birthDate);
        setAddress(data.address);
        setEmail(data.email);
        setPhone(data.phone);
    }

    function handleInternIdChange(event){
        setInternId(event.target.value);
    }
    function handleNameChange(event){
        setName(event.target.value);
    }
    function handleDocIdChange(event){
        setDocId(event.target.value);
    }
    function handleBirthDateChange(event){
        setBirthDate(event.target.value);
    }
    function handleAddressChange(event){
        setAddress(event.target.value);
    }
    function handleEmailChange(event){
        setEmail(event.target.value);
    }
    function handlePhoneChange(event){
        setPhone(event.target.value);
    }
    
    function handleKeywordsChange(event) {
        setKeywords(event.target.value);
    }

    

    function handleSearch(event) {
        event.preventDefault();

        setIsLoading(true);

        try {
            accounts.post("find/student", {keywords})

            .then( (response) => {
                setStudent(response.data);
                populate(response.data);
                setKeywords("");
                setIsLoading(false);

                setIsEditing(false);
                setIsCreating(false);

            }).catch( (error) => {
                if(!error.response){
                    console.error("No error response");

                    setNetError(true)

                    setTimeout(function() {
                        setNetError(false);
                    }, 3000)
                }
                else if (error.response?.status == 401) {
                    console.error("Response: " + error.response.status + " \"Unauthorized\"");
                    setSessionExpired(true)

                    setTimeout(function(){
                        setSessionExpired(false)
                    }, 3000)
                }
                else if (error.response?.status == 404 || error.response?.status == 409) {
                    console.error("Response: " + error.response.status + " \"Not found or Conflict\"");

                    setNotFoundError(true)

                    setTimeout(function() {
                        setNotFoundError(false);
                    }, 3000)

                }
                else console.error("Something went wrong!");
                
                setKeywords("");
                setIsLoading(false)
            });

        } catch(err) {
            console.error(err)
        }
    }

   
    return (

        <div className="container-fluid">

            <Alert 
                fireOn={isLoading}
                title="A pesquisar..."
                text="Aguarde pelo resultado da sua pesquisa."
                icon="loader"
            />

            <Alert 
                fireOn={netError}
                title="Upsss!"
                text="Algo correu mal... Tente outra vez dentro de alguns minutos."
                icon="warning"
                showBadge={true}
            />

            <Alert 
                fireOn={sessionExpired}
                title="Sessão expirada"
                text="Utilize as suas credenciais para inciar uma nova sessão."
                icon="error"
                showBadge={true}
            />

            <Alert 
                fireOn={notFoundError}
                title="Não encontrado!"
                text="Tente introduzir o nome completo ou o número de estudante."
                icon="error"
                showBadge={true}
            />


            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.STUDENT} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={"Alunos"} />

                        <main>
                        
                            <div className=" container-fluid ">
                                <div>
                                    <form onSubmit={handleSearch} className="row justify-content-center">
                                        <div className="col-5 pe-3">
                                            <InputElement 
                                                forId={"search"}
                                                icon={searchIcon} 
                                                placeholder={"Pesquisar: nome ou número de estudante" }
                                                type={"text"}
                                                aria_label={"Search query"}
                                                autoComplete={"off"}
                                                value={keywords}
                                                onChange={handleKeywordsChange}/>
                                        </div>
                                        <div className="col-1">
                                            <button type="submit" className="btn-search">
                                                <img src={searchIcon}  alt="" />
                                            </button>
                                        </div>

                                    </form>
                                </div>
                                <div className="row justify-content-center">
                                    <div className="col-8">

                                        <header className="text-center">
                                            <h2  className="section-header ">Dados do Aluno</h2>
                                        </header>

                                        <form className="form-parent" action="/" method="">

                                            <div className="col-3 pe-2" >
                                            <InputElement 
                                                forId={"idNumber"}
                                                label={"Número de estudante"} 
                                                placeholder={"Exemplo: 20240022" }
                                                type={"text"}
                                                aria_label={"Student intern ID."}
                                                autoComplete={"off"}
                                                value={internId}
                                                onChange={handleInternIdChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>
                                            <div className="col-9 ps-2">
                                            <InputElement 
                                                forId={"name"}
                                                label={"Nome"} 
                                                placeholder={"Nome completo" }
                                                type={"text"}
                                                aria_label={"Student fullname."}
                                                autoComplete={"off"}
                                                value={name}
                                                onChange={handleNameChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>

                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                forId={"bi"}
                                                label={"Número do documento de identificação"}  
                                                placeholder="Exemplo: 004557777HO087" 
                                                type={"text"}
                                                aria_label={"Student national id number."}
                                                autoComplete={"off"}
                                                value={docId}
                                                onChange={handleDocIdChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                forId={"birthdate"}
                                                label={"Data de nascimento"} 
                                                placeholder="dd/mm/aaaa"  
                                                type={"date"}
                                                aria_label={"Student birthdate."}
                                                autoComplete={"off"}
                                                value={birthDate}
                                                onChange={handleBirthDateChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>


                                            <div className="col-12">
                                            <InputElement 
                                                forId={"address"}
                                                label={"Morada completa"} 
                                                placeholder="Morada completa" 
                                                type={"text"}
                                                aria_label={"Student full address."}
                                                autoComplete={"off"}
                                                value={address}
                                                onChange={handleAddressChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>

                                            
                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                forId={"mail"}
                                                label={"Email"} 
                                                placeholder="exemplo@imptel.com" 
                                                type={"email"}
                                                aria_label={"Student email address."}
                                                autoComplete={"off"}
                                                value={email}
                                                onChange={handleEmailChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                forId={"phone"}
                                                label={"Telefone"} 
                                                placeholder="900 000 000" 
                                                type={"number"}
                                                aria_label={"Student phone number."}
                                                autoComplete={"off"}
                                                value={phone}
                                                onChange={handlePhoneChange}
                                                disabled={!isCreating && !isEditing}

                                            />
                                            </div>
                                            
                                        </form>
                                        
                                        
                                    </div>


                                    {/* #######################################3333333############################33###################### */}
                                    <div className="col-3 text-end">
                                        <div className="action-group">
                                            {!isCreating && !isEditing && !student  &&(
                                                <ButtonNew icon={plusIcon} label={"Novo"} onClick={handleNewStudent} />
                                            )}
                                            {(student && !isCreating && !isEditing) &&(
                                                <>
                                                    <ButtonNew className="btn-editar" icon={chIcon} label={"Editar"} onClick={handleEditStudent} />
                                                    <ButtonNew className="btn-cancel" icon={trashIcon} label={"Cancelar"} onClick={handleCancel} />
                                                </>
                                            )}
                                            {(student && isEditing) && (
                                                <>
                                                    <ButtonNew className="btn-save" icon={plusIcon} label={"Guardar"} onClick={handleSaveStudent} />                                                    
                                                    <ButtonNew className="btn-cancel" icon={trashIcon} label={"Cancelar"} onClick={handleCancel} />
                                                </>
                                            )}
                                            {(isCreating) &&(
                                                <>
                                                    <ButtonNew className="btn-save" icon={plusIcon} label={"Guardar"} onClick={handleSaveStudent} />                                                    
                                                    <ButtonNew className="btn-cancel" icon={trashIcon} label={"Cancelar"} onClick={handleCancel} />
                                                </>

                                            )}                                        
                                        </div>
                                    </div>






                                </div>


                            </div>
                        </main>
                    </div>
                </div>
            </div>

            <Footer acadYear={user.acadYear}/>
        </div>



       
    );
}

export default Student;
