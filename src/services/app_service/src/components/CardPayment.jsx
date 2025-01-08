import React from 'react';

function CardPayment({image, title, onClick}){
    return(
        <button  onClick={onClick}>      
            <div className="card-payments">
                <img className="card-img-top" src={image}/>
                    <div className="card-text">
                        {title}
                    </div>

            </div>
        </button>


    )
}

export default CardPayment;