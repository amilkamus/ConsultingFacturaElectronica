ALTER PROCEDURE ActualizarSerieCorrelativo
	@serie  VARCHAR(20),
	@tipoComprobante VARCHAR(50)
AS
BEGIN	
	update cm set
	cm.correlativo = cast(cm.correlativo as bigint) + 1
	FROM CorrelativoMast cm 
	inner join CO_TipoComprobante tc on cm.idTipoComprobante = tc.idTipoComprobante
	where tipoComprobante = @TipoComprobante and cm.serie = @serie
END
GO