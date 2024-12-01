import React, { useState } from "react";

function DropdownCourse({ course, grades }) {
    const [isOpen, setIsOpen] = useState(false);

    

    return (
        <div>
            <button onClick={() => setIsOpen(!isOpen)}>{course}</button>
            {isOpen && (
                <div>
                    {Object.keys(grades).map((classKey) => {
                        const [courseIn, grade, group] = classKey.split('-');
                        return (
                            <div key={classKey}>
                                <h4>{`${courseIn}${grade}${group}`}</h4>
                                
                                    {grades[classKey].map((student) => (
                                        <div key={student.id}>
                                            {student.name} {student.internId}
                                        </div>
                                    ))}
                            
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}
export default DropdownCourse;
