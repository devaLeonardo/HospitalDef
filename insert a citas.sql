INSERT INTO Citas
(
    idPaciente,
    idDoctor,
    fechaCreacionCita,
    fechaCita,
    horaCita,
    horaInicio,
    horaTermino,
    estatusAtencion
)
VALUES
(
    20,                          -- idPaciente
    1,                          -- idDoctor
    GETDATE(),                  -- fechaCreacionCita (hoy)
    CAST(GETDATE() AS DATE),    -- fechaCita (hoy)
    '1:52',                    -- horaCita
    '1:52',                    -- horaInicio
    '2:52',                    -- horaTermino
    'PAGADO'                 -- estatusAtencion
);



---update
UPDATE nombre_tabla
SET columna1 = valor1,
    columna2 = valor2,
    ...
WHERE condicion;
--updete con join 
UPDATE e
SET e.Salario = e.Salario * 1.10
FROM Empleados e
INNER JOIN Departamentos d ON e.IdDepartamento = d.IdDepartamento
WHERE d.Nombre = 'Sistemas';



----delete
DELETE FROM nombre_tabla
WHERE condicion;
---delete con join 
DELETE c
FROM Citas c
INNER JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
WHERE p.Activo = 0;
