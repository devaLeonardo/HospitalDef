use Hospital

CREATE VIEW VistaEmpleados AS
SELECT 
    nombre,
    apellidoP,
    cuentaBancaria,
    activo
FROM Empleado;

SELECT * FROM VistaEmpleados


CREATE VIEW VistaDoctoresHorario AS
SELECT
    d.idDoctor AS IdDoctor,
    CONCAT(e.nombre, ' ', e.apellidoP, ' ', ISNULL(e.apellidoM, '')) AS NombreCompleto,
    es.especialidades AS Especialidad,
    c.numero AS Consultorio,
    h.dias AS DiaSemana,              -- Lunes, Martes, etc.
    h.horaEntrada AS HoraInicio,      -- Time
    h.horaSalida AS HoraFin           -- Time
FROM Doctor d
INNER JOIN Empleado e ON d.idEmpleado = e.idEmpleado
INNER JOIN Especialidades es ON d.idEspecialidad = es.idEspecialidad
INNER JOIN HorarioEmpleado h ON e.idHorario = h.idHorario
LEFT JOIN Consultorio c ON d.idConsultorio = c.idConsultorio;


CREATE VIEW VistaDoctores AS
SELECT 
    d.idDoctor,
    e.nombre + ' ' + e.apellidoP + ' ' + e.apellidoM AS NombreCompleto,
    e.nombre,
    e.apellidoP,
    e.apellidoM,
    u.nombreUsuario AS Usuario,
    u.contraseña AS Contraseña,
    e.cuentaBancaria,
    es.especialidades AS Especialidad,
    d.cedulaProf,
    u.correo,
    u.telefono,
    c.numero AS Consultorio,
    c.idConsultorio,
	e.activo
FROM Doctor d
INNER JOIN Empleado e ON d.idEmpleado = e.idEmpleado
INNER JOIN Usuario u ON e.idUsuario = u.idUsuario
INNER JOIN Especialidades es ON d.idEspecialidad = es.idEspecialidad
LEFT JOIN Consultorio c ON d.idConsultorio = c.idConsultorio;

SELECT * FROM VistaDoctores




CREATE VIEW VistaPacientes AS
SELECT 
    p.idPaciente,
    u.nombreUsuario AS Usuario,
    p.nombre + ' ' + p.apellidoP + ' ' + p.apellidoM AS NombreCompleto,
    p.nombre,
    p.apellidoP,
    p.apellidoM,
    u.contraseña AS Contraseña,
    u.correo,
    u.telefono
FROM Paciente p
INNER JOIN Usuario u ON p.idUsuario = u.idUsuario;


SELECT * FROM VistaPacientes


CREATE VIEW DoctorEspecialidad AS 
SELECT 
	d.idDoctor,
	u.nombreUsuario AS Usuario,
	e.nombre + ' ' + e.apellidoP + ' ' + e.apellidoM AS NombreCompleto,
	es.especialidades AS Especialidad
FROM Usuario u
INNER JOIN Empleado e ON u.idUsuario=e.idUsuario
INNER JOIN Doctor d ON e.idEmpleado=d.idEmpleado
INNER JOIN Especialidades es ON d.idEspecialidad=es.idEspecialidad

SELECT * FROM DoctorEspecialidad


CREATE VIEW RecetasAtendidas AS
SELECT
    d.idDoctor,
    ud.nombreUsuario AS UsuarioDoctor,
    e.nombre + ' ' + e.apellidoP + ' ' + e.apellidoM AS NombreCompletoDoctor,
    r.folioReceta AS numReceta,
    r.observaciones,
    r.tratamientos AS tratamiento,
    r.diagnostico,
    p.idPaciente,
    up.nombreUsuario AS UsuarioPaciente,
    p.nombre + ' ' + p.apellidoP + ' ' + p.apellidoM AS NombreCompletoPaciente,
	c.fechaCita as Fecha
FROM Citas c
INNER JOIN Doctor d ON c.idDoctor = d.idDoctor
INNER JOIN Empleado e ON d.idEmpleado = e.idEmpleado
INNER JOIN Usuario ud ON e.idUsuario = ud.idUsuario
INNER JOIN Paciente p ON c.idPaciente = p.idPaciente
INNER JOIN Usuario up ON p.idUsuario = up.idUsuario
INNER JOIN Receta r ON c.folioCitas = r.folioCita;






--vista para historial del paciente
CREATE VIEW HistorialMedicoPacienteParaDoctor
AS
SELECT
    r.folioReceta AS IdBitacoraCitas,
    c.fechaCita AS FechaMovimiento,
    -- Doctor
    d.idDoctor,
    ud.nombreUsuario AS UsuarioDoctor,
    e.nombre + ' ' + e.apellidoP + ' ' + e.apellidoM AS NombreDoctor,
    -- Especialidad
    es.especialidades AS Especialidad,
    -- Paciente
    p.idPaciente,
    up.nombreUsuario AS UsuarioPaciente,
    p.nombre + ' ' + p.apellidoP + ' ' + p.apellidoM AS NombrePaciente,
    -- Receta
    r.observaciones,
    r.tratamientos,
    r.diagnostico,
    -- Consultorio
    CONCAT(co.numero, ' - ', co.planta, ' - ', co.edificio) AS Consultorio
FROM Citas c
INNER JOIN Doctor d ON c.idDoctor = d.idDoctor
INNER JOIN Especialidades es ON d.idEspecialidad = es.idEspecialidad
INNER JOIN Consultorio co ON d.idConsultorio = co.idConsultorio
INNER JOIN Empleado e ON d.idEmpleado = e.idEmpleado
INNER JOIN Usuario ud ON e.idUsuario = ud.idUsuario
INNER JOIN Paciente p ON c.idPaciente = p.idPaciente
INNER JOIN Usuario up ON p.idUsuario = up.idUsuario
INNER JOIN Receta r ON c.folioCitas = r.folioCita;

