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


