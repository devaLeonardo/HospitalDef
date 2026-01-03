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
    12,                          -- idDoctor
    GETDATE(),                  -- fechaCreacionCita (hoy)
    CAST(GETDATE() AS DATE),    -- fechaCita (hoy)
    '17:05',                    -- horaCita
    '17:05',                    -- horaInicio
    '17:50',                    -- horaTermino
    'PAGADO'                 -- estatusAtencion
);
