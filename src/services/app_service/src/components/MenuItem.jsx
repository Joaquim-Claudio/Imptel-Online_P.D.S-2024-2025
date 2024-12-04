import { useState } from "react";
import React from "react";
import { useNavigate } from "react-router-dom";

function MenuItem({icon, label, isActive, route, onClick}){
    const navigate=useNavigate();

    return(
        <button className={`menu-item   ${isActive?"active":"" }`} 
            onClick={() => {
                navigate(route); // Navega para a rota
            }}>
            <img src={icon}  alt={label} className="menu-item-icon "  />
            {label}
        </button>
)}

export default MenuItem;