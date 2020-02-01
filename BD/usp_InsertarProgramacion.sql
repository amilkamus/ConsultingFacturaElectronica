if OBJECT_ID('usp_InsertarProgramacion', 'P') is not null
	drop procedure usp_InsertarProgramacion
go
create procedure usp_InsertarProgramacion
as
begin

	if exists(select 1 from Programacion)
	begin
		raiserror('La tarea se encuentra en curso, se podrá continuar al finalizar.', 16, 1)
	end
	else
	begin
		insert into Programacion values (GETDATE())
	end
end
go