import React from "react";

function ButtonLink({ route, icon, label }) {
    return (
    <div className="button-link mt-5">
        <button 
            className="link-item ps-2" 
            onClick={() => window.location.href = route} 
        >
            <img src={icon} alt={label} className="link-item-icon" />
            {label}
        </button>           
    </div>
    );
}
export default ButtonLink;