import React from "react";

function RadioInput({value, name, defaultChecked}) {


    return (
        <div className="radio-box">
            <input type="radio" id={value} name={name} value={value} defaultChecked={defaultChecked}/>
            <label htmlFor={value}>{value}</label>
        </div>
    )
} 

export default RadioInput;