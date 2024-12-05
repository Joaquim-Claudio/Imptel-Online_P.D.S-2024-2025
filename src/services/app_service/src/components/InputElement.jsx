import React from "react";



function InputElement({forId, type, placeholder, 
                        aria_label, 
                        autoComplete, 
                        value, 
                        onChange,
                        disabled, 
                        icon, 
                        label, 
                        classInput}){ 


    return(
    <div className={classInput || "form-item"}>
        {label != null ? <label className="d-block" htmlFor={forId} >{label}</label> : ""}
        <input id={forId}
            type={type} 
            placeholder={placeholder}
            aria-label={aria_label}
            autoComplete={autoComplete}
            value={value}
            onChange={onChange}
            disabled={disabled}
        />
    </div>    

    );

    

}

export default InputElement;
