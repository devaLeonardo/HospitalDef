 INSERT INTO Usuario (correo, contraseña, nombreUsuario, telefono, Activo)
VALUES
('doc1@hospital.com','1234','doc1','5500000001',1),
('doc2@hospital.com','1234','doc2','5500000002',1),
('doc3@hospital.com','1234','doc3','5500000003',1),
('doc4@hospital.com','1234','doc4','5500000004',1),
('doc5@hospital.com','1234','doc5','5500000005',1),
('doc6@hospital.com','1234','doc6','5500000006',1),
('doc7@hospital.com','1234','doc7','5500000007',1),
('doc8@hospital.com','1234','doc8','5500000008',1),
('doc9@hospital.com','1234','doc9','5500000009',1),
('doc10@hospital.com','1234','doc10','5500000010',1),
('doc11@hospital.com','1234','doc11','5500000011',1),
('doc12@hospital.com','1234','doc12','5500000012',1),
('doc13@hospital.com','1234','doc13','5500000013',1),
('doc14@hospital.com','1234','doc14','5500000014',1),
('doc15@hospital.com','1234','doc15','5500000015',1),
('doc16@hospital.com','1234','doc16','5500000016',1),
('doc17@hospital.com','1234','doc17','5500000017',1),
('doc18@hospital.com','1234','doc18','5500000018',1),
('doc19@hospital.com','1234','doc19','5500000019',1),
('doc20@hospital.com','1234','doc20','5500000020',1);


INSERT INTO Empleado
(idUsuario, idHorario, RFC, cuentaBancaria, nombre, apellidoP, apellidoM,
 fechaNacimiento, sexo, edad, calle, colonia, municipio, estado, activo)
VALUES
(21,1,'DOC001','1111','Juan','General','Uno','1980-01-01','M',45,'Calle 1','Centro','CDMX','CDMX',1),
(22,1,'DOC002','2222','Ana','General','Dos','1982-02-02','F',43,'Calle 2','Centro','CDMX','CDMX',1),

(23,1,'DOC003','3333','Carlos','Pediatra','Uno','1983-03-03','M',42,'Calle 3','Centro','CDMX','CDMX',1),
(24,1,'DOC004','4444','Laura','Pediatra','Dos','1984-04-04','F',41,'Calle 4','Centro','CDMX','CDMX',1),

(25,1,'DOC005','5555','Miguel','Cardio','Uno','1980-05-05','M',45,'Calle 5','Centro','CDMX','CDMX',1),
(26,1,'DOC006','6666','Paola','Cardio','Dos','1981-06-06','F',44,'Calle 6','Centro','CDMX','CDMX',1),

(27,1,'DOC007','7777','Jorge','Neuro','Uno','1982-07-07','M',43,'Calle 7','Centro','CDMX','CDMX',1),
(28,1,'DOC008','8888','Elena','Neuro','Dos','1983-08-08','F',42,'Calle 8','Centro','CDMX','CDMX',1),

(29,1,'DOC009','9999','Rosa','Gine','Uno','1984-09-09','F',41,'Calle 9','Centro','CDMX','CDMX',1),
(30,1,'DOC010','1010','María','Gine','Dos','1985-10-10','F',40,'Calle 10','Centro','CDMX','CDMX',1),

(31,1,'DOC011','1110','Luis','Oftal','Uno','1981-11-11','M',44,'Calle 11','Centro','CDMX','CDMX',1),
(32,1,'DOC012','1212','Claudia','Oftal','Dos','1982-12-12','F',43,'Calle 12','Centro','CDMX','CDMX',1),

(33,1,'DOC013','1313','Pedro','Trauma','Uno','1983-01-13','M',42,'Calle 13','Centro','CDMX','CDMX',1),
(34,1,'DOC014','1414','Diana','Trauma','Dos','1984-02-14','F',41,'Calle 14','Centro','CDMX','CDMX',1),

(35,1,'DOC015','1515','Raúl','Psiqui','Uno','1985-03-15','M',40,'Calle 15','Centro','CDMX','CDMX',1),
(36,1,'DOC016','1616','Sandra','Psiqui','Dos','1986-04-16','F',39,'Calle 16','Centro','CDMX','CDMX',1),

(37,1,'DOC017','1717','Iván','Onco','Uno','1987-05-17','M',38,'Calle 17','Centro','CDMX','CDMX',1),
(38,1,'DOC018','1818','Patricia','Onco','Dos','1988-06-18','F',37,'Calle 18','Centro','CDMX','CDMX',1),

(39,1,'DOC019','1919','Óscar','Endo','Uno','1989-07-19','M',36,'Calle 19','Centro','CDMX','CDMX',1),
(40,1,'DOC020','2020','Verónica','Endo','Dos','1990-08-20','F',35,'Calle 20','Centro','CDMX','CDMX',1);
select * from Empleado

INSERT INTO Doctor (idEmpleado, idEspecialidad, idConsultorio, cedulaProf)
VALUES
(93,1,1,'GEN001'),
(95,1,2,'GEN002'),

(97,2,3,'PED001'),
(99,2,4,'PED002'),

(101,3,5,'CAR001'),
(103,3,6,'CAR002'),

(105,4,7,'NEU001'),
(107,4,8,'NEU002'),

(109,5,9,'GIN001'),
(111,5,10,'GIN002'),

(113,6,11,'OFT001'),
(115,6,12,'OFT002'),

(117,7,13,'TRA001'),
(119,7,14,'TRA002'),

(121,8,15,'PSI001'),
(123,8,16,'PSI002'),

(125,9,17,'ONC001'),
(127,9,18,'ONC002'),

(129,10,19,'END001'),
(131,10,20,'END002');



select * from Doctor