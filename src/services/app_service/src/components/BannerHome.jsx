import React from "react";
import imageHome from "../assets/images/working-on-laptop.png";
import globeIcone from "../assets/images/fi-rr-globe.svg"

function Banner() {
    return (
        <div className="banner-container" style={{ backgroundImage: `url(${imageHome})` }}>
            <div className="banner-content">
                <p className="banner-title">Imptel-Online <img style={{width: '35px'}} src={globeIcone}  /></p>
                <p className="banner-description">
                A plataforma integrada do <span className="fw-bold"> Instituto Médio Privado de Tecnologias - IMPTEL </span>, projectada para melhorar a interação entre alunos, professores e as secretarias.
                </p>
                <p className="banner-description">
                Esta plataforma é parte de uma iniciativa tecnológica do IMPTEL, iniciada em 2024, com o objectivo de integrar e concentrar as informações académicas e administrativas essenciais para as actividades lectivas no IMPTEL.
                </p>
                <p className="banner-footer">Novembro, 2024.</p>
            </div>
        </div>
    );
}

export default Banner;
