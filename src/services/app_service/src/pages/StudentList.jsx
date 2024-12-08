import React from "react";
import DropdownCourse from "../components/DropdownCourse";
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"
import RadioInput from "../components/RadioInput";
import Alert from "../components/Alert";

import { PAGE } from "../assets/utils/PageIdMap";

import axios from "axios"

const registries = axios.create({
    baseURL: "http://localhost:5297/api/registries",
    withCredentials: true
})


function StudentList({user}) {

    const [result, setResult] = React.useState();
    const [netError, setNetError] = React.useState(false);
    const [acadYear, setAcadYear] = React.useState(user.acadYear);
    const [sessionExpired, setSessionExpired] = React.useState(false);
    const [notFoundError, setNotFoundError] = React.useState(false);
    const [isLoading, setIsLoading] = React.useState(true);

    function handleSelection(event) {
        setAcadYear(event.target.value);
    }

    React.useEffect(function(){

        setNotFoundError(false);
        setIsLoading(true);

        try {
            registries.get("all/", { params: { acadYear } })
            .then( (respose) => {
                setResult(respose.data.$values);
                setIsLoading(false);
            })
            .catch( (error) => {
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
                else if (error.response?.status == 404) {
                    setNotFoundError(true)
                    console.error("Response: " + error.response.status + " \"Not found\"");
                }
       
                setIsLoading(false)
            });

        } catch(err) {
            console.error(err)
        }

        
    }, [acadYear])


    if(result) {
        // Group elements by Course and by class
        var groupedByCourses = result.reduce((accumulator, row) => {
            if(!accumulator[row.course]) accumulator[row.course] = {};

            const classAndRoom = row._Class.name + "|" + row._Class.roomId;
            if(!accumulator[row.course][classAndRoom]) accumulator[row.course][classAndRoom] = [];
            
            accumulator[row.course][classAndRoom].push(row);
            return accumulator;
        }, {})

    }

    console.log(acadYear);
    
    
    return (
        <div className="container-fluid">

            <Alert
                fireOn={isLoading}
                text="A carregar lista de alunos..."
                icon="loader" 
                showBadge={true}
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

            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.STUDENT_LIST} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={"Lista de alunos"} />

                        <main>
                            <div className="container-fluid">
                                <header>
                                    <h2 className="section-header">Cursos | Turmas</h2>
                                </header>

                                <div className="filter" onChange={handleSelection}>
                                    <RadioInput value="2022/2023" name="acadyear"
                                        defaultChecked={"2022/2023" === user.acadYear}/>

                                    <RadioInput value="2023/2024" name="acadyear"
                                        defaultChecked={"2023/2024" === user.acadYear}/>

                                    <RadioInput value="2024/2025" name="acadyear"
                                        defaultChecked={"2024/2025" === user.acadYear}/>
                                </div>

                                <div>
                                    {notFoundError || !result ? 
                                    (
                                        <div className="not-found-msg">
                                            <p>Não foram encontrados alunos matriculados no ano lectivo {acadYear}.</p>
                                        </div>
                                    ) 
                                
                                    : (
                                        <div className="dropdown">
                                            {Object.keys(groupedByCourses).map((course) => {
                                                return(
                                                    <DropdownCourse
                                                        key={course}
                                                        course={course}
                                                        classes={groupedByCourses[course]}
                                                    />
                                                );
                                            })}
                                        </div>    
                                    )}           
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

export default StudentList;