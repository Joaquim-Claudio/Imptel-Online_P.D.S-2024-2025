import React from "react";
import { PAGE } from "../assets/utils/PageIdMap";
import SideBar from "../components/SideBar";
import Toolbar from "../components/Toolbar";
import InputElement from "../components/InputElement";
import searchIcon from "../assets/images/Frame34.svg";
import { Link } from "react-router-dom";
import backIcon from "../assets/images/backIcon.svg";
import Footer from "../components/Footer";
import Table from "../components/Table";
import BoxInfo from "../components/BoxInfo";
import rightIcon from "../assets/images/right.svg";
import leftIcon from "../assets/images/left.svg";
function PaymentsValues(){


        const [keywords, setKeywords] = React.useState("");

        function handleKeywordsChange(event) {
            setKeywords(event.target.value);
        }

    function handleSearch(event) {
        event.preventDefault();

        setIsLoading(true);

        try {
            accounts.post("find/student", {keywords})

            .then( (response) => {
                setStudent(response.data);
                populate(response.data);
                setKeywords("");
                setIsLoading(false);

                setIsEditing(false);
                setIsCreating(false);

            }).catch( (error) => {
                if(!error.response){
                    console.error("No error response");

                    setNetError(true)

                    setTimeout(function() {
                        setNetError(false);
                    }, 3000)
                }
                else if (error.response?.status == 401) {
                    console.error("Response: " + error.response.status + " \"Unauthorized\"");
                    setSessionExpired(true)

                    setTimeout(function(){
                        setSessionExpired(false)
                    }, 3000)
                }
                else if (error.response?.status == 404 || error.response?.status == 409) {
                    console.error("Response: " + error.response.status + " \"Not found or Conflict\"");

                    setNotFoundError(true)

                    setTimeout(function() {
                        setNotFoundError(false);
                    }, 3000)

                }
                else console.error("Something went wrong!");
                
                setKeywords("");
                setIsLoading(false)
            });

        } catch(err) {
            console.error(err)
        }
    }

    return(
        <div className="container-fluid padd">
            <div className="row">
                    <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                        <SideBar activeId={PAGE.PAYMENTS} />
                    </div>
                    <div className="col-10 col-md-9 col-xl-95">
                        <div className="container-fluid">
                            <Toolbar header={"Pagamentos"} />
                            <main>                        
                                <div className=" container-fluid ">
                                    <div>
                                    <div >
                                        <Link to={"/payments"} className="backLink-Payment">
                                            <img src={backIcon} className="pr-5" /> Voltar
                                        </Link>
                                    </div>
                                        <form onSubmit={handleSearch} className="row justify-content-center">
                                            <div className="col-5 pe-3">
                                                <InputElement 
                                                    forId={"search"}
                                                    icon={searchIcon} 
                                                    placeholder={"Pesquisar: nome ou número de estudante" }
                                                    type={"text"}
                                                    aria_label={"Search query"}
                                                    autoComplete={"off"}
                                                    value={keywords}
                                                    onChange={handleKeywordsChange}/>
                                            </div>
                                            <div className="col-1">
                                                <button type="submit" className="btn-search">
                                                    <img src={searchIcon}  alt="" />
                                                </button>
                                            </div>

                                        </form>
                                    </div>   
                                    <div className="box">
                                    <BoxInfo name={"Érica Machado da Silva"} id={20240012}/>                             

                                    </div>
                                    <div className="row justify-content-center">
                                        <div className="col-8">
                                            <header className="text-center">
                                                <h2  className="section-header pb-5 ">Valores a Pagamento</h2>
                                            </header>
                                                                                                                                
                                        </div>
                                    </div>
                                    <Table
                                    /> 
                                </div>
                                <div className="row">
                                    <div className="pai ">
                                        <Link to={""}  className="links" > <img src={leftIcon} alt="" /> Anterior</Link>
                                        <Link to={""} className="links">Próximo <img src={rightIcon} alt="" /></Link>
                                    </div>
                                </div>

                            </main>
                        </div>
                    </div>
                </div>
        </div>

        )
}

export default PaymentsValues;