import React from "react";

function BoxInfo({name, id}){
    return(
    <div className="row justify-content-start box-info">
        <div className="col-8">
            <ul>
                Aluno(a) encontrado(a)
                    <li>{name}</li>
                    <li>{id}</li>
            </ul>                                                                                  
        </div>
    </div>
    )
}

export default BoxInfo;