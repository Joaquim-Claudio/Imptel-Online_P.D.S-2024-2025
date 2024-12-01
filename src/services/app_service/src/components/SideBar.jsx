import React from "react";
import MenuItem from "./MenuItem";
import ButtonLink from "./buttonLink";
import { useState } from "react";
import chartPieIcon from "../assets/images/fi-br-chart-pie-alt.svg";
import userAddIcon from "../assets/images/fi-br-user-add.svg";
import studentListIcon from "../assets/images/fi-br-line-width.svg";
import enrollmentIcon from "../assets/images/fi-br-apps-add.svg";
import paymentIcon from "../assets/images/fi-br-dollar.svg";
import reportIcon from "../assets/images/fi-br-diploma.svg";
import statisticsIcon from "../assets/images/fi-br-stats.svg";
import settingsIcon from "../assets/images/fi-br-settings.svg";
import signOutIcon from "../assets/images/fi-br-sign-out-alt.svg";
import logo from "../assets/images/logo.svg"



function SideBar(){

    const [activeId, setActiveId]= useState(null);

    const menuItems =[
        {id:1, icon: chartPieIcon, label: "Início", route: "/" },
        {id:2, icon: userAddIcon, label: "Alunos", route:"./Student"},
        {id:3, icon: studentListIcon, label: "Lista de Alunos", route:"./StudentList"},
        {id:4, icon: enrollmentIcon, label: "Matrículas", route:"./Enrollment"},
        {id:5, icon: paymentIcon, label:"Pagamentos", route:"/"},
        {id:6, icon: reportIcon, label:"Relatórios", route:"./Test"},
        {id:7, icon: statisticsIcon, label:"Estatísticas", route:"/"},
        {id:8, icon: settingsIcon, label:"Definições", route:"/"}
    ];

    const buttonLink=[ {id: menuItems.length +1, icon: signOutIcon, label:"Terminar sessão", route:"/"}]

    return(

        <nav className="sidebar">
            <img className="logo-sidebar" src={logo} alt="Logótipo Imptel" />
            
            <div className="menu">
                {menuItems.map((item) =>(
                    <MenuItem
                    key={item.id}
                    icon={item.icon}
                    label={item.label }
                    route={item.route}
                    isActive={item.id===activeId}
                    onClick={ ()=> setActiveId(item.id)} 
                    />
                ))}
            </div>

            {buttonLink.map((link) =>(
                <ButtonLink
                key={link.id}
                icon={link.icon}
                label={link.label}
                route={link.route}
                
                />

            ))}                  
        </nav>
    )
     
}

export default SideBar;