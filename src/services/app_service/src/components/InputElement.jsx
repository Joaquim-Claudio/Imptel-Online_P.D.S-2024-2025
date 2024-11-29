import React from "react";



function InputElement({placeholder , icon, label, forId, classInput, type}){ 
    return(
    <div className={classInput || "form-item"}>
        {label != null ? <label className="d-block" htmlFor={forId} >{label}</label> : ""}
        <input type={type}  id={forId}   placeholder={placeholder}/>
    </div>    

    );

    

}

export default InputElement;