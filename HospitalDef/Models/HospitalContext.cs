using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HospitalDef.Models;

public partial class HospitalContext : DbContext
{
    public HospitalContext()
    {
    }

    public HospitalContext(DbContextOptions<HospitalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BitacoraCita> BitacoraCitas { get; set; }

    public virtual DbSet<Cita> Citas { get; set; }

    public virtual DbSet<Consultorio> Consultorios { get; set; }

    public virtual DbSet<DetallesMedicamento> DetallesMedicamentos { get; set; }

    public virtual DbSet<DetallesServicio> DetallesServicios { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Empleado> Empleados { get; set; }

    public virtual DbSet<Especialidade> Especialidades { get; set; }

    public virtual DbSet<Farmaceutico> Farmaceuticos { get; set; }

    public virtual DbSet<HistorialCitasMedicoPaciente> HistorialCitasMedicoPacientes { get; set; }

    public virtual DbSet<HistorialMedicoPaciente> HistorialMedicoPacientes { get; set; }

    public virtual DbSet<HorarioEmpleado> HorarioEmpleados { get; set; }

    public virtual DbSet<Medicamento> Medicamentos { get; set; }

    public virtual DbSet<Paciente> Pacientes { get; set; }

    public virtual DbSet<Recepcionistum> Recepcionista { get; set; }

    public virtual DbSet<Recetum> Receta { get; set; }

    public virtual DbSet<Servicio> Servicios { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<VistaDoctorHorario> VistaDoctorHorario { get; set; }



    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("server=DESKTOP-OKP5T09\\SQLEXPRESS; database=Hospital; integrated security=true; TrustServerCertificate=Yes");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BitacoraCita>(entity =>
        {
            entity.HasKey(e => e.IdBitacoraCitas).HasName("PK__Bitacora__6F8088F7D3791E11");

            entity.Property(e => e.IdBitacoraCitas).HasColumnName("idBitacoraCitas");
            entity.Property(e => e.Costo)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("costo");
            entity.Property(e => e.Estatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.MontoDevuelto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("montoDevuelto");
            entity.Property(e => e.PoliticaCancelacion)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("politicaCancelacion");

            entity.HasOne(d => d.FolioCitasNavigation).WithMany(p => p.BitacoraCita)
                .HasForeignKey(d => d.FolioCitas)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BitacoraCitas_Citas");
        });

        modelBuilder.Entity<Cita>(entity =>
        {
            entity.HasKey(e => e.FolioCitas).HasName("PK__Citas__5E301A4972306A6B");

            entity.Property(e => e.estatusAtencion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estatusAtencion");
            entity.Property(e => e.fechaCita).HasColumnName("fechaCita");
            entity.Property(e => e.fechaCreacionCita)
                .HasColumnType("datetime")
                .HasColumnName("fechaCreacionCita");
            entity.Property(e => e.horaCita).HasColumnName("horaCita");
            entity.Property(e => e.horaInicio).HasColumnName("horaInicio");
            entity.Property(e => e.horaTermino).HasColumnName("horaTermino");
            entity.Property(e => e.idDoctor).HasColumnName("idDoctor");
            entity.Property(e => e.idPaciente).HasColumnName("idPaciente");

            entity.HasOne(d => d.IdDoctorNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.idDoctor)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Citas_Doctor");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.Cita)
                .HasForeignKey(d => d.idPaciente)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Citas_Paciente");
        });

        modelBuilder.Entity<Consultorio>(entity =>
        {
            entity.HasKey(e => e.IdConsultorio).HasName("PK__Consulto__230EBF0FD8BA9684");

            entity.ToTable("Consultorio");

            entity.Property(e => e.IdConsultorio).HasColumnName("idConsultorio");
            entity.Property(e => e.Edificio)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("edificio");
            entity.Property(e => e.Numero)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numero");
            entity.Property(e => e.Planta).HasColumnName("planta");
        });

        modelBuilder.Entity<DetallesMedicamento>(entity =>
        {
            entity.HasKey(e => e.IdDm).HasName("PK__Detalles__B77398AF4E228881");

            entity.ToTable("DetallesMedicamento");

            entity.Property(e => e.IdDm).HasColumnName("IdDM");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.FolioTicket).HasColumnName("folioTicket");
            entity.Property(e => e.IdMedicamento).HasColumnName("idMedicamento");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subTotal");

            entity.HasOne(d => d.FolioTicketNavigation).WithMany(p => p.DetallesMedicamentos)
                .HasForeignKey(d => d.FolioTicket)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DM_Ticekt");

            entity.HasOne(d => d.IdMedicamentoNavigation).WithMany(p => p.DetallesMedicamentos)
                .HasForeignKey(d => d.IdMedicamento)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DM_Medicamento");
        });

        modelBuilder.Entity<DetallesServicio>(entity =>
        {
            entity.HasKey(e => e.IdDs).HasName("PK__Detalles__B77398B19CF6C9DF");

            entity.ToTable("DetallesServicio");

            entity.Property(e => e.IdDs).HasColumnName("IdDS");
            entity.Property(e => e.FolioTicket).HasColumnName("folioTicket");
            entity.Property(e => e.IdServicio).HasColumnName("idServicio");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
            entity.Property(e => e.SubTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("subTotal");

            entity.HasOne(d => d.FolioTicketNavigation).WithMany(p => p.DetallesServicios)
                .HasForeignKey(d => d.FolioTicket)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DS_Ticket");

            entity.HasOne(d => d.IdServicioNavigation).WithMany(p => p.DetallesServicios)
                .HasForeignKey(d => d.IdServicio)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DM_Servicio");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.IdDoctor).HasName("PK__Doctor__418956C380E1B023");

            entity.ToTable("Doctor");

            entity.Property(e => e.IdDoctor).HasColumnName("idDoctor");
            entity.Property(e => e.CedulaProf)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cedulaProf");
            entity.Property(e => e.IdConsultorio).HasColumnName("idConsultorio");
            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");
            entity.Property(e => e.IdEspecialidad).HasColumnName("idEspecialidad");

            entity.HasOne(d => d.IdConsultorioNavigation).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.IdConsultorio)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Consultorio_Doctor");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Dotor_Empelado");

            entity.HasOne(d => d.IdEspecialidadNavigation).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.IdEspecialidad)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Dotor_Especialidad");
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.HasKey(e => e.IdEmpleado).HasName("PK__Empleado__5295297CD02B662B");

            entity.ToTable("Empleado");

            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");
            entity.Property(e => e.Activo)
                .HasDefaultValue(true)
                .HasColumnName("activo");
            entity.Property(e => e.ApellidoM)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoM");
            entity.Property(e => e.ApellidoP)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoP");
            entity.Property(e => e.Calle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.Colonia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("colonia");
            entity.Property(e => e.CuentaBancaria)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cuentaBancaria");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.IdHorario).HasColumnName("idHorario");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Municipio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Rfc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("RFC");
            entity.Property(e => e.Sexo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("sexo");

            entity.HasOne(d => d.IdHorarioNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdHorario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Empleado_Horario");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Empleados)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Empleado_Usuario");
        });

        modelBuilder.Entity<Especialidade>(entity =>
        {
            entity.HasKey(e => e.IdEspecialidad).HasName("PK__Especial__E8AB160020FAED9A");

            entity.Property(e => e.IdEspecialidad).HasColumnName("idEspecialidad");
            entity.Property(e => e.Especialidades)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("especialidades");
        });

        modelBuilder.Entity<Farmaceutico>(entity =>
        {
            entity.HasKey(e => e.IdFarmaceutico).HasName("PK__Farmaceu__4AF6012E77F39604");

            entity.ToTable("Farmaceutico");

            entity.Property(e => e.IdFarmaceutico).HasColumnName("idFarmaceutico");
            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Farmaceuticos)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Farmaceutico_Empleado");
        });

        modelBuilder.Entity<HistorialCitasMedicoPaciente>(entity =>
        {
            entity.HasKey(e => e.IdHcmp).HasName("PK__Historia__91465EB5FB82181A");

            entity.ToTable("HistorialCitasMedicoPaciente");

            entity.Property(e => e.IdHcmp).HasColumnName("idHCMP");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Hora).HasColumnName("hora");
        });

        modelBuilder.Entity<HistorialMedicoPaciente>(entity =>
        {
            entity.HasKey(e => e.IdHistorialMedicoPaciente).HasName("PK__Historia__4A0B6EFB22EAF4B5");

            entity.ToTable("HistorialMedicoPaciente");

            entity.Property(e => e.IdHistorialMedicoPaciente).HasColumnName("idHistorialMedicoPaciente");
            entity.Property(e => e.Alergias)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("alergias");
            entity.Property(e => e.Altura)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("altura");
            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");
            entity.Property(e => e.Padecimientos)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("padecimientos");
            entity.Property(e => e.Peso)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("peso");
            entity.Property(e => e.TipoSangre)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("tipoSangre");

            entity.HasOne(d => d.IdPacienteNavigation).WithMany(p => p.HistorialMedicoPacientes)
                .HasForeignKey(d => d.IdPaciente)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_HistorialMedicoPaciente_Paciente");
        });

        modelBuilder.Entity<HorarioEmpleado>(entity =>
        {
            entity.HasKey(e => e.IdHorario).HasName("PK__HorarioE__DE60F33AD626E6BB");

            entity.ToTable("HorarioEmpleado");

            entity.Property(e => e.IdHorario).HasColumnName("idHorario");
            entity.Property(e => e.Dias)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dias");
            entity.Property(e => e.HoraEntrada).HasColumnName("horaEntrada");
            entity.Property(e => e.HoraSalida).HasColumnName("horaSalida");
        });

        modelBuilder.Entity<Medicamento>(entity =>
        {
            entity.HasKey(e => e.IdMedicamento).HasName("PK__Medicame__42B24C58FB5A348F");

            entity.ToTable("Medicamento");

            entity.Property(e => e.IdMedicamento).HasColumnName("idMedicamento");
            entity.Property(e => e.Caducidad).HasColumnName("caducidad");
            entity.Property(e => e.Lote)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lote");
            entity.Property(e => e.NombreMedicamento)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreMedicamento");
            entity.Property(e => e.PrecioCompra)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioCompra");
            entity.Property(e => e.PrecioVenta)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precioVenta");
            entity.Property(e => e.Stock).HasColumnName("stock");
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ubicacion");
        });

        modelBuilder.Entity<Paciente>(entity =>
        {
            entity.HasKey(e => e.IdPaciente).HasName("PK__Paciente__F48A08F27B77D7DC");

            entity.ToTable("Paciente");

            entity.Property(e => e.IdPaciente).HasColumnName("idPaciente");
            entity.Property(e => e.ApellidoM)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoM");
            entity.Property(e => e.ApellidoP)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("apellidoP");
            entity.Property(e => e.Calle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("calle");
            entity.Property(e => e.Colonia)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("colonia");
            entity.Property(e => e.Edad).HasColumnName("edad");
            entity.Property(e => e.Estado)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estado");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fechaNacimiento");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Municipio)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("municipio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Sexo)
                .HasMaxLength(4)
                .IsUnicode(false)
                .HasColumnName("sexo");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Pacientes)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Paciente_Usuario");
        });

        modelBuilder.Entity<Recepcionistum>(entity =>
        {
            entity.HasKey(e => e.IdRecepcionista).HasName("PK__Recepcio__5298EDCC9DA514F3");

            entity.Property(e => e.IdRecepcionista).HasColumnName("idRecepcionista");
            entity.Property(e => e.IdEmpleado).HasColumnName("idEmpleado");

            entity.HasOne(d => d.IdEmpleadoNavigation).WithMany(p => p.Recepcionista)
                .HasForeignKey(d => d.IdEmpleado)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Recepcionista_empelado");
        });

        modelBuilder.Entity<Recetum>(entity =>
        {
            entity.HasKey(e => e.FolioReceta).HasName("PK__Receta__9972E6E0F6124A97");

            entity.Property(e => e.FolioReceta).HasColumnName("folioReceta");
            entity.Property(e => e.Diagnostico)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("diagnostico");
            entity.Property(e => e.FolioCita).HasColumnName("folioCita");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("observaciones");
            entity.Property(e => e.Tratamientos)
                .HasMaxLength(3000)
                .IsUnicode(false)
                .HasColumnName("tratamientos");

            entity.HasOne(d => d.FolioCitaNavigation).WithMany(p => p.Receta)
                .HasForeignKey(d => d.FolioCita)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Receta_Cita");
        });

        modelBuilder.Entity<Servicio>(entity =>
        {
            entity.HasKey(e => e.Idservicio).HasName("PK__servicio__46FDB9245A3242F8");

            entity.ToTable("servicio");

            entity.Property(e => e.Idservicio).HasColumnName("idservicio");
            entity.Property(e => e.Estatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("estatus");
            entity.Property(e => e.HoraF).HasColumnName("horaF");
            entity.Property(e => e.HoraI).HasColumnName("horaI");
            entity.Property(e => e.NombreServicio)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("nombreServicio");
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.FolioTicket).HasName("PK__Ticket__369F07D036DB7F6C");

            entity.ToTable("Ticket");

            entity.Property(e => e.FolioTicket).HasColumnName("folioTicket");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Hora).HasColumnName("hora");
            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.NumCliente).HasColumnName("numCliente");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Ticket_Usuario");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Usuario__645723A6D3C0305F");

            entity.ToTable("Usuario");

            entity.Property(e => e.IdUsuario).HasColumnName("idUsuario");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Contraseña)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contraseña");
            entity.Property(e => e.Correo)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("correo");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Registro");
            entity.Property(e => e.NombreUsuario)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("nombreUsuario");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("telefono");
        });
            modelBuilder.Entity<VistaDoctorHorario>()
            .HasNoKey()
            .ToView("VistaDoctoresHorario");

        OnModelCreatingPartial(modelBuilder);

    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
