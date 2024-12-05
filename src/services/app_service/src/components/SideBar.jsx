import React from "react";
import MenuItem from "./MenuItem";
import ButtonLink from "./ButtonLink";
import Alert from "./Alert";
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

import axios from "axios";

const http = axios.create({
    baseURL: "http://localhost:5293/api/accounts",
    withCredentials: true
})

function SideBar({activeId}){

    const menuItems =[
        {id:1, icon: chartPieIcon, label: "Início", route: "/" },
        {id:2, icon: userAddIcon, label: "Alunos", route:"/student"},
        {id:3, icon: studentListIcon, label: "Lista de Alunos", route:"/student_list"},
        {id:4, icon: enrollmentIcon, label: "Matrículas", route:"/enrollment"},
        {id:5, icon: paymentIcon, label:"Pagamentos", route:"/"},
        {id:6, icon: reportIcon, label:"Relatórios", route:"/test"},
        {id:7, icon: statisticsIcon, label:"Estatísticas", route:"/"},
        {id:8, icon: settingsIcon, label:"Definições", route:"/"}
    ];

    const buttonLink=[ {id: menuItems.length +1, icon: signOutIcon, label:"Terminar sessão", route:"/"}]

    const [error, setError] = React.useState(false);
    const [inConfirm, setInConfirm] = React.useState(false);
    const [isLoading, setIsLoading] = React.useState(false);

    function handleClose() {
        setInConfirm(false);
    }

    function handleConfirm() {

        setInConfirm(false)
        setIsLoading(true)
        
        http.get("logout")
            .then((response) => {
                if(response.status == 200) {
                    setIsLoading(false)
                    window.location.replace("/login")
                }
            })
            .catch(error => {
                if(!error.response) {
                    console.error("No server response");
                    setError(true)

                    setTimeout(function() {
                        setError(false);
                    }, 3000)
                }
                else if (error.response?.status == 401) console.error("Response: " + error.response.status + " \"Unauthorized\"");
                else console.error("Logout failed");
        
                setIsLoading(false)

            });

    }


    return(

        <nav className="sidebar">

            <Alert 
                fireOn={inConfirm}
                title="Terminar sessão"
                text="Está prestes a terminar a sessão."
                icon="warning"
                showCancelButton={true}
                showConfirmButton={true}
                confirmButtonColor="danger"
                confirmButtonText="Confirmar"
                cancelButtonText="Cancelar"
                onClose={handleClose}
                onConfirm={handleConfirm}
            />

            <Alert 
                fireOn={isLoading}
                text="A terminar sessão..."
                icon="loader"
                showBadge={true}
            />


            <Alert 
                fireOn={error}
                title="Upsss!"
                text="Algo correu mal... Tente outra vez dentro de alguns minutos."
                icon="warning"
                showBadge={true}
            />

            <img className="logo-sidebar" src={logo} alt="Logótipo Imptel" />
            
            <div className="menu">
                {menuItems.map((item) =>(
                    <MenuItem
                        key={item.id}
                        icon={item.icon}
                        label={item.label }
                        route={item.route}
                        isActive={item.id===activeId}
                    />
                ))}
            </div>

            {buttonLink.map((link) =>(
                <ButtonLink
                    key={link.id}
                    icon={link.icon}
                    label={link.label}
                    route={link.route}
                    handleClick={() => setInConfirm(true)}
                />

            ))}                  
        </nav>
    )
     
}

export default SideBar;