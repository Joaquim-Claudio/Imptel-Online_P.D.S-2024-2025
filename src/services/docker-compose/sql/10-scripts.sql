-- Updated

CREATE TYPE Role AS ENUM ('Secretary', 'Teacher', 'Student', 'Helpdesk', 'Admin');
CREATE TYPE Level AS ENUM ('10', '11', '12', '13');
CREATE TYPE FeeRole AS ENUM ('Tuition', 'Subscription', 'Renewal', 'Late');
CREATE TYPE Status AS ENUM ('Active', 'Inactive', 'Finished');

CREATE TABLE Building (
    id SERIAL PRIMARY KEY,
    name TEXT,
    address TEXT,
    phone TEXT,
    email TEXT
);

CREATE TABLE "User" (
    id SERIAL PRIMARY KEY,
    internId TEXT,
    name TEXT NOT NULL,
    email TEXT,
    address TEXT,
    phone TEXT,
    birthDate DATE NOT NULL,
    hashPassword TEXT NOT NULL,
    role Role NOT NULL,
    docId TEXT NOT NULL DEFAULT '0000000000'
);


CREATE TABLE Secretary (
    position TEXT,
    building_id INTEGER REFERENCES building(id)
) INHERITS ("User");


CREATE TABLE Teacher (
    academicLevel TEXT,
    course TEXT
) INHERITS ("User");


CREATE TABLE Student (
) INHERITS ("User");


CREATE TABLE Course (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    duration INTEGER
);

CREATE TABLE "Class" (
    id SERIAL PRIMARY KEY,
    name TEXT,
    roomId TEXT
);


CREATE TABLE Enrollment (
    id SERIAL PRIMARY KEY,
    class INTEGER REFERENCES "Class" (id) ON DELETE SET NULL,
    course_id INTEGER REFERENCES Course (id) ON DELETE SET NULL,
    acadYear TEXT,
    level Level
);


CREATE TABLE Registry (
    id SERIAL PRIMARY KEY,
    date DATE,
    status Status,
    approved bool,
    student_id INTEGER,
    building_id INTEGER REFERENCES Building (id) ON DELETE SET NULL,
    enrollment_id INTEGER REFERENCES Enrollment (id) ON DELETE SET NULL
);


CREATE TABLE Fee (
    id SERIAL PRIMARY KEY,
    description TEXT,
    price DECIMAL
);


CREATE TABLE EnrollmentFee (
    role FeeRole,
    limitDate DATE
) inherits (Fee);


CREATE TABLE Invoice (
    id SERIAL PRIMARY KEY,
    date DATE,
    reference TEXT,
    value DECIMAL,
    fee_id INTEGER REFERENCES Fee (id)
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
    triId INTEGER,
    classification_id INTEGER REFERENCES Classification (id)
);


CREATE TABLE Unit (
    id SERIAL PRIMARY KEY,
    name TEXT
);


CREATE TABLE StudyPlan (
    id SERIAL PRIMARY KEY,
    acadYear TEXT,
    teacher_id INTEGER,
    unit_id INTEGER REFERENCES Unit(id) ON DELETE SET NULL,
    class_id INTEGER REFERENCES "Class"(id) ON DELETE SET NULL
);


CREATE TABLE "Module" (
    id SERIAL PRIMARY KEY,
    classification_id INTEGER REFERENCES Classification (id),
    studyPlan_id INTEGER REFERENCES StudyPlan (id) ON DELETE SET NULL
);


CREATE TABLE Enrollment_module (
    enrollment_id INTEGER REFERENCES Enrollment (id),
    module_id INTEGER REFERENCES "Module" (id),
    PRIMARY KEY (enrollment_id, module_id)
);


CREATE TABLE TimeSlot (
    id SERIAL PRIMARY KEY,
    starts TIME,
    ends TIME,
    stuplan_id INTEGER REFERENCES StudyPlan (id)
);



-- ############################################################

INSERT INTO  Building (name, address, phone, email)
VALUES ('Imptel Viana', 'Rua Muito Longa', '+244 923 726 895', 'imptel.angola@gmail.com');

INSERT INTO "User" (internid, name, email, address, phone, birthdate, hashpassword, role)
VALUES ('mario.igreja', 'Mário André de Oliveira Igreja', 'mario.igreja@gmail.com',
        'Rua Comandante Eurico', '+244 921 067 921', '1976-05-29',
        'AQAAAAIAAYagAAAAEGu8ksVFSNNWDWcuVAv+tjJ8di3kJDDRJSoitHX+wuqh6VsMt/JcycoydDH60StSyg==', 'Admin');

INSERT INTO Course (name, duration)
VALUES ('Médio Técnico de Electrónica e Telecomunicações', 4);

INSERT INTO "Class" (name, roomid)
VALUES ('ET10A', 3),
       ('ET10B', 5),
       ('ET10C', 1),
       ('I10A', 2),
       ('I10B', 4);

INSERT INTO Enrollment (class, course_id, acadyear, level)
VALUES (1, 1, '2024/2025', '10');

INSERT INTO Registry (date, status, student_id, building_id, enrollment_id)
VALUES ('2024-11-14', 'Active', 1, 1, 1);

INSERT INTO Unit (name)
VALUES ('Matemática'),
       ('Física'),
       ('Electricidade e Eletrónica'),
       ('Tecnologias de Informação e Comunicação'),
       ('Desenho Técnico');


INSERT INTO StudyPlan (acadyear, teacher_id, unit_id, class_id)
VALUES ('2024/2025', 2, 1, 1),
       ('2024/2025', 2, 2, 2),
       ('2024/2025', 2, 4, 3),
       ('2024/2025', 2, 1, 2);


INSERT INTO "Module" (studyplan_id)
VALUES (1),
       (2),
       (3);
