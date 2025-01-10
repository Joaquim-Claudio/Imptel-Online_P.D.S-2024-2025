import React from "react";
function ButtonNew({ icon, label, onClick, className }) {
    return (
        <div className="button-new pb-4">
            <button 
                className={`button-item ${className}`}
                onClick={onClick} // Agora executa a função passada como prop
            >
                <img src={icon} alt={label} className="button-item-icon" />
                {label}
            </button>           
        </div>
    );
}

export default ButtonNew;
