import React, { useState } from "react";
import dropdownIcon from "../assets/images/fi-rr-caret-right.svg"

function DropdownCourse({ course, classes }) {
    const [isOpen, setIsOpen] = useState(false);
    const [openClass, setOpenClass]= useState(null);

    const toogleClassDropdown = (_className)=>{
        if(openClass===_className){
            setOpenClass(null);
        }else{
            setOpenClass(_className);
        }

    }


    return (
        <div>

            {/* Course is the name of a course */}

            <button className="dropdown-btn" onClick={() => setIsOpen(!isOpen)}> <img className="pe-3" src={dropdownIcon} /> 
                {course}
            </button>

            {isOpen && (
                <div>

                    {/* Take the keys of the 'classes' object returning an array of keys and iterates through this array taking the '_className' */}
                    {Object.keys(classes).map((_className) => {

                        return (
                            <div key={_className}>

                                <button className="dropdownClass" onClick={() =>toogleClassDropdown(_className)} > <img className="pe-3" src={dropdownIcon} />
                                    {/* _className is in the format 'className|roomId' */}
                                    {_className.split("|")[0]} &bull; SALA {_className.split("|")[1].padStart(2, "0")}

                                </button  >

                                    {openClass == _className &&

                                        // Access the object 'classes' returning an array and iterates through the array
                                        (classes[_className].map( (row) => (
                                            <ul className="dropdownClass-item" key={row.student.id}>
                                                <li >
                                                    {row.student.name} &bull; {row.student.internId}
                                                </li>                                            
                                            </ul>
                                            ))
                                        )
                                    }
                            
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}
export default DropdownCourse;
