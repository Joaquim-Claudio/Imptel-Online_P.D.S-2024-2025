import React from "react";
import imageHome from "../assets/images/working-on-laptop.png";
import globeIcone from "../assets/images/fi-rr-globe.svg"
import SideBar from "../components/SideBar"
import Toolbar from "../components/Toolbar"
import Footer from "../components/Footer"
import { PAGE } from "../assets/utils/PageIdMap";

function Homepage ({user}) {
    return (

        <div className="container-fluid">
            <div className="row">
                <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                    <SideBar activeId={PAGE.HOMEPAGE} />
                </div>
                <div className="col-10 col-md-9 col-xl-95">
                    <div className="container-fluid">
                        <Toolbar header={`Bem-vindo(a), ${user.name.split(" ")[0]}`} />

                        <main>
                            <div className="banner-container" style={{ backgroundImage: `url(${imageHome})` }}>
                                <div className="banner-content">
                                    <p className="banner-title">Imptel-Online <img style={{width: '35px'}} src={globeIcone}  /></p>
                                    <p className="banner-description">
                                    A plataforma integrada do <span className="fw-bold"> Instituto Médio Privado de Tecnologias - IMPTEL</span>, projectada para melhorar a interação entre alunos, professores e as secretarias.
                                    </p>
                                    <p className="banner-description">
                                    Esta plataforma é parte de uma iniciativa tecnológica do IMPTEL, iniciada em 2024, com o objectivo de integrar e concentrar as informações académicas e administrativas essenciais para as actividades lectivas no IMPTEL.
                                    </p>
                                    <p className="banner-footer">Novembro, 2024.</p>
                                </div>
                            </div>

                        </main>
                    </div>
                </div>
            </div>
            <Footer/> 
        </div>

    );
}

export default Homepage;
