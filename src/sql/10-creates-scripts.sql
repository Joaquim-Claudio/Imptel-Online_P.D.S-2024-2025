-- Updated

CREATE TYPE Role AS ENUM ('Secretary', 'Teacher', 'Student');
CREATE TYPE Level AS ENUM ('10', '11', '12', '13');
CREATE TYPE FeeRole AS ENUM ('Secretary', 'Teacher', 'Student');

CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    email TEXT NOT NULL,
    address TEXT,
    phone TEXT,
    birthDate DATE,
    hashPassword TEXT NOT NULL,
    role Role NOT NULL
);


CREATE TABLE Secretary (
    id INTEGER PRIMARY KEY REFERENCES "User" (id),
    mec TEXT,
    position TEXT
);


CREATE TABLE Teacher (
    id INTEGER PRIMARY KEY REFERENCES "User" (id),
    mec TEXT,
    academicLevel TEXT,
    course TEXT
);


CREATE TABLE Student (
    id INTEGER PRIMARY KEY REFERENCES "User" (id),
    internalId TEXT
);


CREATE TABLE Course (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    duration INTEGER,
    level Level
);


CREATE TABLE Registry (
    id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES Student (id),
    date DATE,
    building_id INTEGER REFERENCES Building (id)
);


CREATE TABLE Building (
    id SERIAL PRIMARY KEY,
    name TEXT,
    address TEXT,
    phone TEXT,
    email TEXT
);


CREATE TABLE Enrollment (
    id SERIAL PRIMARY KEY,
    student_id INTEGER REFERENCES Student (id),
    course_id INTEGER REFERENCES Course (id),
    level Level
);


CREATE TABLE Fee (
    id SERIAL PRIMARY KEY,
    description TEXT,
    price DECIMAL
);


CREATE TABLE EnrollmentFee (
    id INTEGER PRIMARY KEY REFERENCES Fee (id),
    role FeeRole
);


CREATE TABLE Invoice (
    id SERIAL PRIMARY KEY,
    date DATE,
    reference TEXT,
    value DECIMAL,
    fee_id INTEGER REFERENCES Fee (id)
);


CREATE TABLE "Module" (
    id SERIAL PRIMARY KEY,
    finalGrade DECIMAL
);


CREATE TABLE Classification (
    id SERIAL PRIMARY KEY,
    finalGrade DECIMAL,
    retaked BOOLEAN,
    retakeGrade DECIMAL
);


CREATE TABLE TrimesterAssessment (
    id SERIAL PRIMARY KEY,
    p1 DECIMAL,
    p2 DECIMAL,
    mt DECIMAL,
    classification_id INTEGER REFERENCES Classification (id)
);


CREATE TABLE StudyPlan (
    id SERIAL PRIMARY KEY,
    acadYear TEXT
);


CREATE TABLE Unit (
    id SERIAL PRIMARY KEY,
    name TEXT
);


CREATE TABLE "Class" (
    id SERIAL PRIMARY KEY,
    name TEXT,
    roomId TEXT
);


CREATE TABLE TimeSlot (
    id SERIAL PRIMARY KEY,
    start TIME,
    end TIME,
    stuplan_id INTEGER REFERENCES StudyPlan (id)
);



CREATE TABLE Enrollment_module (
    enrollment_id INTEGER REFERENCES Enrollment (id),
    module_id INTEGER REFERENCES Module (id),
    PRIMARY KEY (enrollment_id, module_id)
);


ALTER TABLE Student ADD COLUMN registry_id INTEGER REFERENCES Registry (id);
ALTER TABLE StudyPlan ADD COLUMN class_id INTEGER REFERENCES "Class" (id);
ALTER TABLE StudyPlan ADD COLUMN module_id INTEGER REFERENCES "Module" (id);