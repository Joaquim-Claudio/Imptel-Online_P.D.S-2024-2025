import React from "react";

function Alert({title, text, icon}) {

    return (
        <div className="alert-container">
            <div className="alert-base">
                {title ? <h2>{title}</h2> : <></>}
                {text ? <p>{text}</p> : <></>}
                <div className={icon}></div>
                <div className="alert-header">
                    Imptel-Online
                </div>
            </div>
        </div>
    )
}

export default Alert;