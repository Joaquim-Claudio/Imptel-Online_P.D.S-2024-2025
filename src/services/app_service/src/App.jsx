import React from "react";
import { BrowserRouter , Routes, Route, Navigate } from "react-router-dom";
import Alert from "./components/Alert";
import Homepage from "./pages/Homepage";
import Student from "./pages/Student";
import StudentList from "./pages/StudentList";
import Enrollment from "./pages/Enrollment";
import Test from "./pages/Test";
import Login from "./pages/Login";

import axios from "axios"

const http = axios.create({
    baseURL: "http://localhost:5293/api/accounts",
    withCredentials: true
})

function App() {
    const [user, setUser] = React.useState();
    const [isLoading, setIsLoading] = React.useState(true);

    React.useEffect(function() {
        try {
            http.get("/auth")
                .then( (response) => {
                    setUser(response.data)
                    setIsLoading(false)
                })
        
                .catch( (error) => {
                    if(!error.response) console.error("No error response");
                    else if (error.response?.status == 401) console.error("Response: " + error.response.status + " \"Unauthorized\"");
                    else console.error("Authentication failed");
           
                    setIsLoading(false)
                });
                
        } catch(err) {
            console.error(err)
        }

        
    }, [])

    if(isLoading) {
        return (
            <Alert 
                text="A carregar a página..."
                icon="loader" 
                showBadge={true}
            />
        )
    }

    return (
        <BrowserRouter>

            <Routes>
                <Route path="/" element={user ? <Homepage user={user} /> : <Navigate to="/login" replace/>} />
                <Route path="/student" element={user ? <Student user={user} /> : <Navigate to="/login" replace/>} />
                <Route path="/student_list" element={user ? <StudentList user={user}  /> : <Navigate to="/login" replace/>} />
                <Route path="/enrollment" element={user ? <Enrollment user={user}  /> : <Navigate to="/login" replace/>} />
                <Route path="/test" element={user ? <Test /> : <Navigate to="/login" replace/>} />
                <Route path="/login" element={!user ? <Login /> : <Navigate to="/" replace/>} />
            </Routes>
            
        </BrowserRouter>
    );
}

export default App;