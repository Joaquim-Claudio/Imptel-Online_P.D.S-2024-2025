import React from 'react';

function CardPayment({image, title}){
    return(
        <div className="card-payments">
            <img className="card-img-top" src={image}/>
                <div className="card-text">
                    {title}
                </div>

        </div>


    )
}

export default CardPayment;