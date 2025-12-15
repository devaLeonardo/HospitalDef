-- TRIGGER PAGO DE CITAS
CREATE TRIGGER dbo.trCitaPagadaActualizarEstatus
ON dbo.Citas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    IF UPDATE(estatusAtencion)
    BEGIN
        INSERT INTO dbo.HistorialCitasMedicoPaciente
        (
            idCita,
            fecha,
            hora,
            estatus
        )
        SELECT
            i.FolioCitas,
            GETDATE(),
            CONVERT(time, GETDATE()),
            i.estatusAtencion
        FROM INSERTED i
        INNER JOIN DELETED d
            ON i.FolioCitas = d.FolioCitas
        WHERE d.estatusAtencion <> i.estatusAtencion;
    END
END;
GO

--TRIGGER CANCELACION DE CITAS
CREATE TRIGGER trCitaCanceladaActualizarEstatus
ON Citas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF ((SELECT TRIGGER_NESTLEVEL()) > 1) 
    BEGIN
        RETURN;
    END
    IF UPDATE(estatusAtencion)
    BEGIN
        -- Hacemos el update cruzando con la tabla 'inserted'
        UPDATE C
        SET estatusAtencion = 'Cancelada por el Paciente'
        FROM Citas C
        INNER JOIN inserted i ON C.FolioCitas = i.FolioCitas
        WHERE i.estatusAtencion = 'CANCELADO';
    END
END;
GO



--TRIGGER AGENDACION DE CITAS
CREATE TRIGGER trg_InsertHistorialCita
ON Citas
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO HistorialCitasMedicoPaciente
    (
        fecha,
        hora,
        estatus,
        idCita,
        doctor,
        consultorio,
        paciente
    )
    SELECT
        i.fechaCita,
        i.horaCita,
        i.estatusAtencion,
        i.FolioCitas,

        -- Nombre completo del doctor (Empleado)
        e.nombre + ' ' + e.apellidoP + ' ' + e.apellidoM AS doctor,

        -- Consultorio del doctor
        CAST(d.idConsultorio AS varchar(100)) AS consultorio,

        -- Nombre completo del paciente
        p.nombre + ' ' + p.apellidoP + ' ' + p.apellidoM AS paciente

    FROM inserted i
    INNER JOIN Doctor d
        ON i.idDoctor = d.idDoctor
    INNER JOIN Empleado e
        ON d.idEmpleado = e.idEmpleado
    INNER JOIN Paciente p
        ON i.idPaciente = p.idPaciente;
END;
GO






