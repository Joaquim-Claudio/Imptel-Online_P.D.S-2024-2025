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
    const [isLoading, setLoading] = React.useState(true);

    React.useEffect(function() {
        try {
            http.get("/auth")
                .then( (response) => {
                    console.log(response.data)
                    setUser(response.data)
                    setLoading(false)
                })
        
                .catch( (error) => {
                    if(!error.response) console.error("No server response");
                    else if (error.response?.status == 401) console.error("Response: " + error.response.status + " \"Unauthorized\"");
                    else console.error("Authentication failed");
           
                    setLoading(false)
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
            />
        )
    }

    return (
        <BrowserRouter>

            <Routes>
                <Route path="/" element={user ? <Homepage user={user} /> : <Navigate to="/login" replace/>} />
                <Route path="/student" element={user ? <Student /> : <Navigate to="/login" replace/>} />
                <Route path="/student_list" element={user ? <StudentList /> : <Navigate to="/login" replace/>} />
                <Route path="/enrollment" element={user ? <Enrollment /> : <Navigate to="/login" replace/>} />
                <Route path="/test" element={user ? <Test /> : <Navigate to="/login" replace/>} />
                <Route path="/login" element={!user ? <Login /> : <Navigate to="/" replace/>} />
            </Routes>
            
        </BrowserRouter>
    );
}

export default App;