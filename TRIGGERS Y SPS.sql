
create trigger actualizarHCMP on Citas 
after update
as
	begin
	SET NOCOUNT ON;

	if update(estatusAtencion)
		begin
			update t1
			set t1.estatus = case
								 when t2.estatusAtencion = 'PAGADO' then 'Cita pagada pendiente por atender'
								 else t2.estatusAtencion
							   end


			from HistorialCitasMedicoPaciente t1 INNER JOIN 
			inserted t2 ON t1.idCita = t2.FolioCitas;
			
		end

	end;


CREATE TRIGGER tr_ActualizarStock_Medicamento
ON DetallesMedicamento
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE M
    SET M.stock = M.stock - I.cantidad
    FROM Medicamento M
    INNER JOIN INSERTED I ON M.idMedicamento = I.idMedicamento;
END

CREATE TRIGGER tr_EliminarSinStock
ON Medicamento
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Medicamento
    WHERE stock <= 0;
END


CREATE or ALTER PROCEDURE spBuscador_MS (@Data varchar(100))
	AS
	BEGIN
		SELECT idMedicamento AS id, nombreMedicamento AS nombre, precioVenta AS precio, 'Medicamento' AS tipo
		FROM Medicamento
		WHERE nombreMedicamento LIKE '%' + @Data + '%'

		UNION 

		SELECT idservicio, nombreServicio, precio, 'Servicio' AS tipo
		FROM Servicio
		WHERE nombreServicio LIKE '%' + @Data + '%'

		ORDER BY nombre;

	END






CREATE or ALTER PROCEDURE spMedicamentoServicio (@Data int)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        dm.idMedicamento as id,
        dm.folioTicket as idTicket, 
        m.nombreMedicamento as nombre, 
        dm.precio as precio, 
        dm.cantidad as cantidad, 
        dm.subTotal as subtotal,
        'Medicamento' as tipo
    FROM DetallesMedicamento dm
    INNER JOIN Medicamento m ON dm.idMedicamento = m.idMedicamento
    WHERE dm.folioTicket = @Data

    UNION ALL 

    SELECT 
        ds.idServicio, 
        ds.folioTicket, 
        s.nombreServicio, 
        ds.precio,
        1 as cantidad, 
        ds.subTotal,
        'Servicio' as tipo
    FROM DetallesServicio ds
    INNER JOIN servicio s ON ds.idServicio = s.idservicio
    WHERE ds.folioTicket = @Data;
END



	
