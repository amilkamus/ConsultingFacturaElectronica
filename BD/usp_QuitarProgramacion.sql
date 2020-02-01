if OBJECT_ID('usp_QuitarProgramacion', 'P') is not null
	drop procedure usp_QuitarProgramacion
go
create procedure usp_QuitarProgramacion
as
begin
	delete from Programacion
end
go