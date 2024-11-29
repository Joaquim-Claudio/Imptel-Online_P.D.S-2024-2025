import React from "react";

function ButtonNew({route, icon, label}){
    return (
        <div className="button-new ">
        <button 
            className="button-item " 
            onClick={() => window.location.href = route} 
        >
            <img src={icon} alt={label} className="button-item-icon" />
             {label}
        </button>           
    </div>
    );
}

export default ButtonNew;