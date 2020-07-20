if object_id('usp_ObtenerRepresentacionImpresa', 'p') is not null
	drop procedure usp_ObtenerRepresentacionImpresa
go
create procedure usp_ObtenerRepresentacionImpresa
(
	@idComprobante bigint
)
as
begin
	select IdComprobante, NombreRepresentacionImpresa, Archivo from RepresentacionImpresa
	where
		IdComprobante = @idComprobante
end
go