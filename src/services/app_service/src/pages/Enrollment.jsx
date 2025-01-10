import React from "react";
import InputElement from "../components/InputElement";
import searchIcon from "../assets/images/Frame34.svg";
import ButtonNew from "../components/ButtonNew";
import plusIcon from "../assets/images/fi-br-plus.svg"
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"
import trashIcon from "../assets/images/fi-br-trash.svg"
import chIcon from "../assets/images/fi-br-change.svg"

import { PAGE } from "../assets/utils/PageIdMap";
import axios from "axios"

const accounts = axios.create({
    baseURL: import.meta.env.VITE_REGISTRY_SERVICE_URL,
    withCredentials: true
})
function Enrollment({user}) {


    //Botão novo
    
    function handleKeywordsChange(event) {
        setKeywords(event.target.value);
    }
    
    const [isCreating, setIsCreating]= React.useState(false);
    const handleNewStudent =() =>{
        setIsCreating(true);
        setInternId('');
        setName('');
        setCourse('');
        setGrade('');
        setGroup('');
        setDate('');
        setStatus('');
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
        setStudentRegistry(null);
        setInternId('');
        setName('');
        setCourse('');
        setGrade('');
        setGroup('');
        setDate('');
        setStatus(''); 
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
        const [studentRegistry, setStudentRegistry] = React.useState(null);
    
        const [internId, setInternId] = React.useState("");
        const [name, setName] = React.useState("");
        const [course, setCourse] = React.useState("");
        const [grade, setGrade] = React.useState("");
        const [group, setGroup] = React.useState("");
        const [date, setDate] = React.useState("");
        const [status, setStatus] = React.useState("");

      

    
        function populate(data) {            
            setInternId(data.internId);
            setName(data.name);
            
            console.log(data.registries.$values);

            data.registries.$values.forEach(registry=>{
                if(registry.status==="Active"){   
                    console.log(registry)                 
                    setCourse(registry.enrollment.course.name);
                    setGrade(registry.enrollment.level);
                    setGroup(registry.enrollment._Class.name);
                    setDate(registry.date);
                    setStatus(registry.status);                
                }
            })

        }
    
        function handleInternIdChange(event){
            setInternId(event.target.value);
        }
        function handleNameChange(event){
            setName(event.target.value);
        }
        function handleCourseChange(event){
            setCourse(event.target.value);
        }
        function handleGradeChange(event){
            setGrade(event.target.value);
        }
        function handleGroupChange(event){
            setGroup(event.target.value);
        }
        function handleDateChange(event){
            setDate(event.target.value);
        }
        function handleStatusChange(event){
            setStatus(event.target.value);
        }

        function handleSearch(event) {
            event.preventDefault();
    
            setIsLoading(true);
    
            try {
                accounts.post("find", {keywords})
    
                .then( (response) => {
                    setStudentRegistry(response.data);
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
            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.ENROLLMENT} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={"Matrículas"} />

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
                                                <h2  className="section-header ">Dados da Matrícula</h2>
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


                                            <div className="col-12">
                                            <InputElement 
                                                forId={"course"}
                                                placeholder="Médio Técnico de Informática" 
                                                label={"Curso"} 
                                                type={"text"}
                                                aria_label={"Student course"}
                                                autoComplete={"off"}
                                                value={course}
                                                onChange={handleCourseChange}
                                                disabled={!isCreating && !isEditing}
                                                />
                                            </div>

                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                placeholder="Exemplo: 004557777HO087" 
                                                label={"Classe"}  
                                                forId={"grade"}
                                                type={"text"}
                                                aria_label={"Grade"}
                                                autoComplete={"off"}
                                                value={grade}
                                                onChange={handleGradeChange}
                                                disabled={!isCreating && !isEditing}
                                                />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                forId={"class"}
                                                placeholder="I10A"  
                                                label={"Turma"} 
                                                type={"text"}
                                                aria_label={"Group"}
                                                autoComplete={"off"}
                                                value={group}
                                                onChange={handleGroupChange}
                                                disabled={!isCreating && !isEditing}
                                                />
                                            </div>
                                        
                                            
                                            <div className="col-6 pe-2">
                                            <InputElement 
                                                forId={"dateEn"}
                                                placeholder="22-11-2024" 
                                                label={"Data"} 
                                                type={"date"}
                                                aria_label={"Enrollment date"}
                                                autoComplete={"off"}
                                                value={date}
                                                onChange={handleDateChange}
                                                disabled={!isCreating && !isEditing}
                                                />
                                            </div>
                                            <div className="col-6 ps-2">
                                            <InputElement 
                                                forId={"status"}
                                                placeholder="Activa" 
                                                label={"Estado"} 
                                                type={"text"}
                                                aria_label={"StatusEnrollment status"}
                                                autoComplete={"off"}
                                                value={status}
                                                onChange={handleStatusChange}
                                                disabled={!isCreating && !isEditing}
                                                />
                                            </div>
                                            
                                        </form>
                                        
                                        
                                    </div>


                                    {/* #######################################3333333############################33###################### */}
                                    <div className="col-3 text-end">
                                        <div className="action-group">
                                            {!isCreating && !isEditing && !studentRegistry  &&(
                                                <ButtonNew icon={plusIcon} label={"Novo"} onClick={handleNewStudent} />
                                            )}
                                            {(studentRegistry && !isCreating && !isEditing) &&(
                                                <>
                                                    <ButtonNew className="btn-editar" icon={chIcon} label={"Editar"} onClick={handleEditStudent} />
                                                    <ButtonNew className="btn-cancel" icon={trashIcon} label={"Cancelar"} onClick={handleCancel} />
                                                </>
                                            )}
                                            {(studentRegistry && isEditing) && (
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

export default Enrollment;

