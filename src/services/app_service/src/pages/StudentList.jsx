import React from "react";
import { useState } from "react";
import Dropdown from "../components/Dropdown";
import DropdownCourse from "../components/DropdownCourse";



function StudentList(){
    const students = [
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

    const groupedCourses= students.reduce((acumulador, course))



    
    return(
        <div className="container-fluid">
            <header>
            <h2 className="section-header">Cursos | Turmas</h2>
            </header>
            <div>
                
            </div>
            <div className="dropdown">
                {students.map((item)=>
                    <DropdownCourse
                        key={item.id}
                        course={item.course}
                        grade={item.grade}
                        group={item.group}
                        courseIn={item.courseIn}
                    />
                
                )}
                {students.map((item)=>(
                    <Dropdown
                    key={item.id}
                    course={item.course}
                    grade={item.grade}
                    group={item.group}
                    name={item.name}
                    internId={item.internId}                    
                    />

                ))}

                
            </div>               
                

            
        </div>

    );
}
export default StudentList;




