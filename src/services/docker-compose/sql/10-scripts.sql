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
    class_id INTEGER REFERENCES "Class" (id) ON DELETE SET NULL,
    course_id INTEGER REFERENCES Course (id) ON DELETE SET NULL,
    acadYear TEXT,
    level Level
);


CREATE TABLE Registry (
    id SERIAL PRIMARY KEY,
    date DATE,
    update_date DATE DEFAULT CURRENT_DATE,
    status Status,
    approved bool,
    student_id INTEGER,
    building_id INTEGER REFERENCES Building (id) ON DELETE SET NULL,
    enrollment_id INTEGER REFERENCES Enrollment (id) ON DELETE SET NULL
);


CREATE TABLE Fee (
    id SERIAL PRIMARY KEY,
    description TEXT,
    price DECIMAL,
    limitDate DATE
);


CREATE TABLE EnrollmentFee (
    role FeeRole,
    enrollment_id INTEGER REFERENCES Enrollment (id)
) inherits (Fee);


CREATE TABLE Payment (
    id SERIAL PRIMARY KEY,
    reference TEXT,
    payed BOOLEAN,
    date DATE,
    student_id INTEGER NOT NULL,
    fee_id INTEGER NOT NULL
);


CREATE TABLE Invoice (
    id SERIAL PRIMARY KEY,
    date DATE,
    reference TEXT,
    value DECIMAL,
    method TEXT,
    employee_id INTEGER NOT NULL,
    payment_id INTEGER REFERENCES Payment (id)
);


CREATE TABLE Classification (
    id SERIAL PRIMARY KEY,
    finalGrade DECIMAL,
    retaken BOOLEAN,
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
    studyplan_id INTEGER REFERENCES StudyPlan (id) ON DELETE SET NULL
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
    studyplan_id INTEGER REFERENCES StudyPlan (id)
);


CREATE TABLE AcadYear (
    id SERIAL PRIMARY KEY,
    name TEXT,
    started_at DATE DEFAULT CURRENT_DATE,
    finished_at DATE DEFAULT NULL
);


-- ############################################################

INSERT INTO  Building (name, address, phone, email)
VALUES ('Imptel Viana', 'Rua Muito Longa', '+244 923 726 895', 'imptel.angola@gmail.com');

INSERT INTO "User" (internid, name, email, address, phone, birthdate, hashpassword, role)
VALUES ('mario.igreja', 'Mário André de Oliveira Igreja', 'mario.igreja@gmail.com',
        'Rua Comandante Eurico', '+244 921 067 921', '1976-05-29',
        'AQAAAAIAAYagAAAAEGu8ksVFSNNWDWcuVAv+tjJ8di3kJDDRJSoitHX+wuqh6VsMt/JcycoydDH60StSyg==', 'Admin');

INSERT INTO Student (internid, name, email, address, phone, birthdate, hashpassword, role, docId)
VALUES ('20240001', 'Joaquim Manuel Igreja Cláudio', 'jclaudio223@gmail.com',
        'Rua Doutor João 10 1 FRT 1500-435, Odivelas', '+244 935 328 921', '2001-11-13',
        'AQAAAAEAACcQAAAAEOwHiHt9nBz9dIWXmtLZt3wNPbJS+onmHR1pwdHMrBM094ztTjJ5OzH/9i+m3D2TSA==', 'Student', '295698745');

INSERT INTO Course (name, duration)
VALUES ('Médio Técnico de Electrónica e Telecomunicações', 4);

INSERT INTO "Class" (name, roomid)
VALUES ('ET10A', 3),
       ('ET10B', 5),
       ('ET10C', 1),
       ('I10A', 2),
       ('I10B', 4);

INSERT INTO Enrollment (class_id, course_id, acadyear, level)
VALUES (1, 1, '2024/2025', '10');

INSERT INTO Registry (date, status, student_id, building_id, enrollment_id)
VALUES ('2024-11-14', 'Active', 2, 1, 1);

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

INSERT INTO AcadYear (name, started_at, finished_at)
VALUES ('2013', '2013-01-01', '2013-12-20'),
       ('2014', '2014-01-01', '2014-12-20'),
       ('2015', '2015-01-01', '2015-12-20'),
       ('2016', '2016-01-01', '2016-12-20'),
       ('2017', '2017-01-01', '2017-12-20'),
       ('2018', '2018-01-01', '2018-12-20'),
       ('2019', '2019-01-01', '2019-12-20'),
       ('2020/2021', '2020-01-01', '2021-07-20'),
       ('2021/2022', '2021-09-01', '2022-07-20'),
       ('2022/2023', '2022-09-01', '2023-07-20'),
       ('2023/2024', '2023-09-01', '2024-07-20'),
       ('2024/2025', '2024-09-01', null);

INSERT INTO EnrollmentFee (description, price, limitdate, role, enrollment_id)
VALUES ('Propina Setembro', 26500, '2024-09-16', 'Tuition', 1),
       ('Propina Outubro', 26500, '2024-10-07', 'Tuition', 1),
       ('Propina Novembro', 26500, '2024-11-05', 'Tuition', 1),
       ('Propina Dezembro', 26500, '2024-12-05', 'Tuition', 1),
       ('Propina Janeiro', 26500, '2025-01-06', 'Tuition', 1),
       ('Propina Fevereiro', 26500, '2025-02-05', 'Tuition', 1),
       ('Propina Março', 26500, '2025-03-05', 'Tuition', 1),
       ('Propina Abril', 26500, '2025-04-07', 'Tuition', 1),
       ('Propina Maio', 26500, '2025-05-05', 'Tuition', 1),
       ('Propina Junho', 26500, '2025-06-05', 'Tuition', 1),
       ('Propina Julho', 26500, '2025-07-07', 'Tuition', 1);

INSERT INTO Payment (reference, payed, date, student_id, fee_id)
VALUES ('PRP000001', true, '2024-09-10', 2, 1),
       ('PRP000002', true, '2024-10-01', 2, 2),
       ('PRP000003', true, '2024-11-01', 2, 3),
       ('PRP000004', true, '2024-11-29', 2, 4),
       ('PRP000005', true, '2024-11-29', 2, 5),
       ('PRP000006', false, null, 2, 6),
       ('PRP000007', false, null, 2, 7),
       ('PRP000008', false, null, 2, 8),
       ('PRP000009', false, null, 2, 9),
       ('PRP000010', false, null, 2, 10),
       ('PRP000011', false, null, 2, 11);

INSERT INTO Invoice (date, reference, value, method, employee_id, payment_id)
VALUES ('2024-09-10', '099', 26500, 'TPA', 1, 1),
       ('2024-10-01', '111', 26500, 'TPA', 1, 2),
       ('2024-11-01', '003', 26500, 'TPA', 1, 3),
       ('2024-11-29', '755', 26500, 'TPA', 1, 4),
       ('2024-11-29', '039', 26500, 'TPA', 1, 5);