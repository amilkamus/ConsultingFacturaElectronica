ALTER PROCEDURE ListarSerieCorrelativo
	@TipoComprobante VARCHAR(50)
AS
/*
ListarSerieCorrelativo 'BOLETA'
ListarSerieCorrelativo 'FACTURA'
ListarSerieCorrelativo 'NOTA DE CREDITO'
*/
BEGIN
	--SELECT TOP 1 Serie, Numero FROM SerieCorrelativo WHERE Estado = 1
	SELECT cm.Serie, cast(cm.correlativo as bigint) Numero FROM CorrelativoMast cm 
	inner join CO_TipoComprobante tc on cm.idTipoComprobante = tc.idTipoComprobante
	where tipoComprobante = @TipoComprobante
END
GO