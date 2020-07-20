if object_id('usp_ObtenerRespuestaComprobante', 'p') is not null
	drop procedure usp_ObtenerRespuestaComprobante
go
create procedure usp_ObtenerRespuestaComprobante
(
	@idComprobante bigint
)
as
begin
	select c.IdComprobante, 'R_' + e.NumeroDocumentoIdentidad + '-' + c.TipoComprobante + '-' + c.SerieNumero + '.zip' NombreRespuesta, Archivo
	from ComprobanteRespuesta cr 
	inner join Comprobante c on cr.IdComprobante = c.IdComprobante
	inner join Empresa e on e.IdEmpresa = c.IdEmpresa
	cross apply (select top 1 IdArchivo from ComprobanteRespuestaHistorial where IdComprobante = cr.IdComprobante order by IdArchivo desc) crh
	cross apply (select Archivo from CDRSunatProcesado where IdArchivo = crh.IdArchivo)  cdr
	where 
		cr.IdComprobante = @idComprobante
end
go