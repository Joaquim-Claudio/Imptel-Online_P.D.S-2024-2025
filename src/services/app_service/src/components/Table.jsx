import React from "react";
import Pending from "./Pending";
import ButtonNew from "./ButtonNew";
import moneyIcon from "../assets/images/fi-rr-money.svg";

function Table({month ,paymentDueDate, price}){
    return(
        <table className="table custom-table ">
            <thead>
                <tr>
                    <th scope="col">Estado</th>
                    <th scope="col">Descrição</th>
                    <th scope="col">Data limite de<br /> pagamento</th>
                    <th scope="col">Valor</th>
                    <th scope="col">Ação</th>
                </tr>
            </thead>
            <tbody>
                <tr >
                <td className="pt-3"> <Pending/></td>
                <td className="pt-3">Propina {month}</td>
                <td className="pt-3">{paymentDueDate}</td>
                <td className="pt-3">{price}</td>
                <td className="pt-3"><ButtonNew className="btn-pay" icon={moneyIcon} label={"Pagar"} onClick={""}/></td>
                </tr>
                <tr>
                <td><Pending/></td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-pay" icon={moneyIcon} label={"Pagar"} onClick={""}/></td>
                </tr>
                <tr>
                
                <td><Pending/></td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-pay" icon={moneyIcon} label={"Pagar"} onClick={""}/></td>
                </tr>
                <tr>
                <td><Pending/></td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-pay" icon={moneyIcon} label={"Pagar"} onClick={""}/></td>
                </tr>
                <tr>
                <td><Pending/></td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-pay" icon={moneyIcon} label={"Pagar"} onClick={""}/></td>
                </tr>
            </tbody>
    </table>

    );
}

export default Table;