import React from "react";
import { BrowserRouter , Routes, Route, Navigate } from "react-router-dom";

import Alert from "./components/Alert";

import Homepage from "./pages/Homepage";
import Student from "./pages/Student";
import StudentList from "./pages/StudentList";
import Enrollment from "./pages/Enrollment";
import Payments from "./pages/Payments";
import Login from "./pages/Login";
import Invoices from "./pages/Payments.Invoices";
import axios from "axios"
import PendingPayments from "./pages/Payments.Pending";

const accounts = axios.create({
    baseURL: import.meta.env.VITE_ACCOUNT_SERVICE_URL,
    withCredentials: true
})

function App() {
    const [user, setUser] = React.useState();
    const [netError, setNetError] = React.useState(false);
    const [sessionExpired, setSessionExpired] = React.useState(false);
    const [isLoading, setIsLoading] = React.useState(true);

    React.useEffect(function() {
        try {
            accounts.get("/auth")
                .then( (response) => {
                    setUser(response.data)
                    setIsLoading(false)
                })
        
                .catch( (error) => {
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
                    else if (error.response?.status == 404) {
                        console.error("Response: " + error.response.status + " \"Not found\"");
                    }
           
                    setIsLoading(false)
                });
                
        } catch(err) {
            console.error(err)
        }

        
    }, [])

    if(isLoading) {
        return (
            <div className="container-fluid">
                <Alert 
                    fireOn={isLoading}
                    text="A carregar a página..."
                    icon="loader" 
                    showBadge={true}
                />
                <Alert 
                    fireOn={netError}
                    title="Upsss!"
                    text="Algo correu mal... Tente outra vez dentro de alguns minutos."
                    icon="warning"
                    showBadge={true}
                />

                <Alert 
                    fireOn={sessionExpired}
                    title="Sessão expirada"
                    text="Utilize as suas credenciais para inciar uma nova sessão."
                    icon="error"
                    showBadge={true}
                />

            </div>
        )
    }

    return (
        <BrowserRouter>

            <Routes>
                <Route path="/" element={user ? <Homepage user={user} /> : <Navigate to="/login" replace/>} />
                <Route path="/student" element={user ? <Student user={user} /> : <Navigate to="/login" replace/>} />
                <Route path="/student_list" element={user ? <StudentList user={user}  /> : <Navigate to="/login" replace/>} />
                <Route path="/enrollment" element={user ? <Enrollment user={user}  /> : <Navigate to="/login" replace/>} />
                <Route path="/payments" element={user ? <Payments user={user} /> : <Navigate to="/login" replace/> } />
                <Route path="/payments/pending-payments" element={user ? <PendingPayments /> : <Navigate to="/login" replace/>} />
                <Route path="/payments/invoices" element={user ? <Invoices /> : <Navigate to="/login" replace/>} />
                <Route path="/login" element={!user ? <Login /> : <Navigate to="/" replace/>} />
            </Routes>
            
        </BrowserRouter>
    );
}

export default App;