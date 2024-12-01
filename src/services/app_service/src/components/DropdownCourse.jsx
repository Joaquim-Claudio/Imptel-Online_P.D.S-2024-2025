import React from "react";
import { useState } from "react";


function DropdownCourse({course, group, grade, courseIn}){
    const [isOpen, setIsOpen]= useState(false);
    return(
        <div>
            <button onClick={()=> setIsOpen(!isOpen)}>{course}</button>
            {isOpen &&(
                <div>
                    <p>{courseIn}{grade}{group}</p>

                </div>

            )
        }
            
        </div>
       
    );

}

export default DropdownCourse;