import React, { useState } from "react";
import dropdownIcon from "../assets/images/fi-rr-caret-right.svg"

function DropdownCourse({ course, grades }) {
    const [isOpen, setIsOpen] = useState(false);
    const [openClassKey, setOpenClassKey]= useState(null);

    const toogleClassKeyDropdown= (classKey)=>{
        if(openClassKey===classKey){
            setOpenClassKey(null);
        }else{
            setOpenClassKey(classKey);
        }

    }


    

    return (
        <div>
            <button className="dropdown-btn" onClick={() => setIsOpen(!isOpen)}> <img className="pe-3" src={dropdownIcon} /> {course}</button>
            {isOpen && (
                <div>
                    {Object.keys(grades).map((classKey) => {
                        const [courseIn, grade, group] = classKey.split('-');
                        return (
                            <div key={classKey}>
                                <button className="dropdownClassKey " onClick={() =>toogleClassKeyDropdown(classKey)} > <img className="pe-3" src={dropdownIcon} />
                                {`${courseIn}${grade}${group}`}
                                </button  >

                                {openClassKey==classKey &&
                                (grades[classKey].map((student) => (
                                        <ul className="dropdownClasskey-item" key={student.id}>
                                            <li >
                                            {student.name} {student.internId}
                                            </li>                                            
                                        </ul>
                                    )))}
                                
                                
                                
                                    
                            
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}
export default DropdownCourse;
