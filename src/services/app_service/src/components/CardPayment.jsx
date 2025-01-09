import React from 'react';
import { Link } from 'react-router-dom';

function CardPayment({image, title, to}){
    return(
        <Link  to={to}>      
            <div className="card-payments">
                <img className="card-img-top" src={image}/>
                    <div className="card-text">
                        {title}
                    </div>

            </div>
        </Link>


    )
}

export default CardPayment;