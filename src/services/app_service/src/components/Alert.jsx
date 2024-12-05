import React from "react";

function Alert({fireOn=true, title, text, icon, 
                showBadge, 
                showConfirmButton, 
                confirmButtonText,
                confirmButtonColor,
                showCancelButton,
                cancelButtonText,
                onClose,
                onConfirm}) {
    
    if(!fireOn) {
        return null;
    }

    return (
        <div className="alert-container">
            <div className="alert-base">
                {title ? <h2>{title}</h2> : <></>}
                {text ? <p>{text}</p> : <></>}
                <div className={icon}></div>
                {showBadge ? 
                    (<div className="alert-badge">
                        Imptel-Online
                    </div>)
                    : <></>
                }
                <div className="container">
                    <div className="row justify-content-center">
                        {showCancelButton ?
                            <button type="button" 
                                className={"alert-btn btn btn-secondary me-3"}
                                onClick={onClose}>
                                {cancelButtonText || "Cancel"}
                            </button>
                            : <></>
                        }

                        {showConfirmButton ?
                            <button type="button" 
                                className={`alert-btn btn btn-${confirmButtonColor || "success"}`}
                                onClick={onConfirm}>
                                {confirmButtonText || "Confirm"}
                            </button>
                            : <></>
                        }

                    </div>
                </div>

            </div>
        </div>
    )
}

export default Alert;