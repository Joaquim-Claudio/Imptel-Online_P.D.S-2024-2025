import React from "react";
import DropdownCourse from "../components/DropdownCourse";
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"

import { PAGE } from "../assets/utils/PageIdMap";
import Alert from "../components/Alert";

import axios from "axios"

const http = axios.create({
    baseURL: "http://localhost:5297/api/registries",
    withCredentials: true
})

function StudentList({user}) {

    const studentList = [
        { id:1 , course:"Técnico de Informática", courseIn:"I", grade: "10", group:"C", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:2 , course:"Técnico de Informática", courseIn:"I", grade: "10", group:"A", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:3 , course:"Técnico de Informática", courseIn:"I", grade: "11", group:"B", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:4 , course:"Técnico de Informática", courseIn:"I", grade: "11", group:"C", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:5 , course:"Técnico de Informática", courseIn:"I", grade: "12", group:"A", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:6 , course:"Técnico de Informática", courseIn:"I", grade: "12", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:7 , course:"Técnico de Informática", courseIn:"I", grade: "13", group:"C", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:8 , course:"Técnico de Informática", courseIn:"I", grade: "13", group:"A", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:9 , course:"Técnico de Informática", courseIn:"I", grade: "10", group:"B", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:10 , course:"Técnico de Informática", courseIn:"I", grade: "10", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:11 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "11", group:"A", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:12 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "11", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:13 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "10", group:"A", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:14 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "12", group:"C", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:15 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "13", group:"A", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:16 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "12", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:17 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "11", group:"C", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:18 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "10", group:"A", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:19 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "12", group:"C", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:20 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "10", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        { id:21 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "13", group:"A", name: 'Matias Rocha Paulo Miguel', internId: '20240201'},
        { id:22 , course:"Técnico de Electrónica e Telecomunicações", courseIn:"T", grade: "10", group:"B", name: 'Érica Machado da Silva', internId: '202401001'},
        
    ]

    const groupedByCourses= studentList.reduce((acumulator, student)=> {
        if(!acumulator[student.course]){
            acumulator[student.course]={};
        }
        const classKey= `${student.courseIn}-${student.grade}-${student.group}`;
        if(!acumulator[student.course][classKey]){
            acumulator[student.course][classKey]=[]
        }
        acumulator[student.course][classKey].push(student);
        return acumulator;
            

    }, {});


    const [students, setStudents] = React.useState();
    const [isLoading, setIsLoading] = React.useState(true);

    React.useEffect(function(){
        try {
            http.get("active")
            .then( (respose) => {
                setStudents(respose.data);
                setIsLoading(false);
            })
            .catch( (error) => {
                if(!error.response) console.error("No error response");
                else if (error.response?.status == 401) console.error("Response: " + error.response.status + " \"Unauthorized\"");
                else console.error("Failed loading students");
       
                setIsLoading(false)
            });

        } catch(err) {
            console.error(err)
        }


    }, [])


    
    return (
        <div className="container-fluid">

            <Alert
                fireOn={isLoading}
                text="A carregar lista de alunos..."
                icon="loader" 
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
                                <div>
                                    
                                </div>
                                <div className="dropdown">
                                    {Object.keys(groupedByCourses).map((course)=>
                                    { const [courseIn, grade, group ]= course.split('-');
                                        return(
                                            <DropdownCourse
                                            key={course}
                                            course={course}
                                            courseIn={courseIn}
                                            grade={grade}
                                            group={group}
                                            grades={groupedByCourses[course]}
                                            
                                            
                                            />

                                        );

                                    }
                                    
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