import React from 'react';
import { useState } from 'react';

function Dropdown({course, grade, group, name, internId}){
    
  const [isOpen, setIsOpen] = useState(false);

  return (
    <div>
      <button onClick={() => setIsOpen(!isOpen)}>{course}{grade}{group}</button>
      {isOpen && (
        <div>
          <p>{name}{internId}</p>
         
        </div>
      )}
    </div>
  );
}

export default Dropdown;

