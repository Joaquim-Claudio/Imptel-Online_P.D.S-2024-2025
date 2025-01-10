import React from "react";
import Pending from "./Pending";
import ButtonNew from "./ButtonNew";
import moneyIcon from "../assets/images/fi-rr-money.svg";
import downloadIcon from "../assets/images/fi-rr-download.svg"

function TableInvoice({month ,paymentDueDate, price, date}){
    return(
        <table className="table custom-table ">
            <thead>
                <tr>
                    <th scope="col">Data</th>
                    <th scope="col">Descrição</th>
                    <th scope="col">Forma de<br /> pagamento</th>
                    <th scope="col">Valor</th>
                    <th scope="col">Ação</th>
                </tr>
            </thead>
            <tbody>
                <tr >
                <td className="pt-3">{date}</td>
                <td className="pt-3">Propina {month}</td>
                <td className="pt-3">{paymentDueDate}</td>
                <td className="pt-3">{price}</td>
                <td className="pt-3"><ButtonNew className="btn-download" icon={downloadIcon} label={"Transferir"} onClick={""}/></td>
                </tr>
                <tr>
                <td>{date}</td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-download" icon={downloadIcon} label={"Transferir"} onClick={""}/></td>
                </tr>
                <tr>
                
                <td>{date}</td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-download" icon={downloadIcon} label={"Transferir"} onClick={""}/></td>
                </tr>
                <tr>
                <td>{date}</td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-download" icon={downloadIcon} label={"Transferir"} onClick={""}/></td>
                </tr>
                <tr>
                <td>{date}</td>
                <td>Propina {month}</td>
                <td>{paymentDueDate}</td>
                <td>{price}</td>
                <td><ButtonNew className="btn-download" icon={downloadIcon} label={"Transferir"} onClick={""}/></td>
                </tr>
            </tbody>
    </table>

    );
}

export default TableInvoice;