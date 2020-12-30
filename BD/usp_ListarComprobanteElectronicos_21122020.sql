alter procedure usp_ListarComprobanteElectronicos
(
	@estado int,
	@nroDocumentoEmisor varchar(30),
	@nroDocumentoReceptor varchar(30),
	@fechaInicial varchar(10),
	@fechaFinal varchar(10),
	@tipoComprobante varchar(10),
	@serieNumero varchar(20)
)
/*
usp_ListarComprobanteElectronicos 0, '20602034675', '', '2020-07-18', '2020-07-19'
*/
as
begin
	set nocount on
	set dateformat dmy
	declare @fechaIni date = cast(@fechaInicial as date)
	declare @fechaFin date = cast(@fechaFinal as date)
	
	select 
		c.IdComprobante, 
		NroDocumentoIdentidad NumeroDocumentoIdentidad, 
		c.RazonSocial,
		case c.TipoComprobante
			when '01' then 'Factura'
			when '03' then 'Boleta'
			when '07' then 'Nota de crédito'
			when '08' then 'Nota de débito'
		end TipoComprobante,
		c.SerieNumero,
		convert(char(10),cast(FechaEmison as date),103) FechaEmision,
		Moneda,
		TotalPrecioVenta,		
		tr.Descripcion Estado,
		cr.DescripcionSUNAT DescripcionEstado,
		tr.IdTipoRespuesta IdEstado,
		isnull(crn.SerieNumero, '') ComprobanteReferenciado,
		c.FechaRegistro,
		c.TotalImpuesto,
		c.TotalValorVenta,
		c.TotalDescuento
		--into #temporal
	from comprobante c
	inner join ComprobanteRespuesta cr on c.IdComprobante = cr.IdComprobante
	inner join TipoRespuesta tr on tr.IdTipoRespuesta = cr.IdTipoRespuesta
	inner join Empresa e on c.IdEmpresa = e.IdEmpresa
	left join ComprobanteReferenciaNota crn on crn.IdComprobante = c.IdComprobante
	where
			(tr.Codigo = @estado or @estado = -1)
		and (e.NumeroDocumentoIdentidad = @nroDocumentoEmisor)
		and (c.NroDocumentoIdentidad like isnull(@nroDocumentoReceptor, '') + '%')
		and (CAST(FechaEmison as date) between @fechaIni and @fechaFin)
		and (c.TipoComprobante = @tipoComprobante or @tipoComprobante = '0')
		and (c.SerieNumero like isnull(@serieNumero, '') + '%')

	if @estado = -1 or @estado = 4
	begin
		insert into #temporal
		select 
			c.IdComprobante, 
			NroDocumentoIdentidad NumeroDocumentoIdentidad, 
			c.RazonSocial,
			case c.TipoComprobante
				when '01' then 'Factura'
				when '03' then 'Boleta'
				when '07' then 'Nota de crédito'
				when '08' then 'Nota de débito'
			end TipoComprobante,
			c.SerieNumero,
			convert(char(10),cast(FechaEmison as date),103) FechaEmision,
			Moneda,
			TotalPrecioVenta,			
			'Sin Respuesta' Estado,
			'' DescripcionEstado,
			-2 Idestado,
			isnull(crn.SerieNumero, '') ComprobanteReferenciado,
			c.FechaRegistro,
			c.TotalImpuesto,
			c.TotalValorVenta,
			c.TotalDescuento
		from comprobante c
		inner join PendienteEnvio pe on pe.IdComprobante = c.IdComprobante
		inner join Empresa e on c.IdEmpresa = e.IdEmpresa
		left join ComprobanteReferenciaNota crn on crn.IdComprobante = c.IdComprobante
		where
				(e.NumeroDocumentoIdentidad = @nroDocumentoEmisor)
			and (c.NroDocumentoIdentidad like isnull(@nroDocumentoReceptor, '') + '%')
			and (CAST(FechaEmison as date) between @fechaIni and @fechaFin)
			and (c.TipoComprobante = @tipoComprobante or @tipoComprobante = '0')
			and (c.SerieNumero like isnull(@serieNumero, '') + '%')
	end

	select * from #temporal
	order by FechaRegistro desc

end
go