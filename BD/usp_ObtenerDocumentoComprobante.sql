if object_id('usp_ObtenerDocumentoComprobante', 'p') is not null
	drop procedure usp_ObtenerDocumentoComprobante
go
create procedure usp_ObtenerDocumentoComprobante
(
	@idComprobante bigint
)
as
begin
	select IdComprobante, NombreXML, ArchivoXML from ComprobanteDocumento
	where
		IdComprobante = @idComprobante
end
go