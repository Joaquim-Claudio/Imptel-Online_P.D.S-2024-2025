import React from "react";
import { BrowserRouter , Routes, Route } from "react-router-dom";
import SideBar from "./components/SideBar";
import Toolbar from "./components/Toolbar";
import Banner from "./components/BannerHome";
import Student from "./pages/Student";
import StudentList from "./pages/StudentList";
import Footer from "./components/Footer";


function App() {
    return (
        <BrowserRouter>
            <div className="container-fluid">
                <div className="row">
                    <div className="col-2 col-md-3 col-xl-25 px-4 bkg-white">
                        <SideBar />
                    </div>
                    <div className="col-10 col-md-9 col-xl-95">
                        <div className="container-fluid">
                            <Toolbar />
                            
                            <main>
                                <Routes>
                                    <Route path="/" element={<Banner />} />
                                    <Route path="Student" element={<Student />} />
                                    <Route path="StudentList" element={<StudentList/>}/>
                                </Routes>
                                
                            </main>
                            <Footer/>
                        </div>
                        
                    </div>
                </div>
                
            </div>
        </BrowserRouter>
    );
}

export default App;