alter procedure dbo.usp_RegistraRespuestaSUNAT
@CodigoSUNAT varchar(50),
@IdComprobante bigint,
@Archivo varbinary(max),
@Descripcion varchar(max),
@FechaSunat varchar(20),
@HoraSunat varchar(20)
as
/*
Del 0100 al 1999 Excepciones
Del 2000 al 3999 Errores que generan rechazo
Del 4000 en adelante Observaciones
*/
declare @temp table(Id bigint)

declare @IdMax bigint
DECLARE @IdCatalogo bigint
DECLARE @Respuesta bigint
DECLARE @TipoRespuesta bigint
DECLARE @IdTipoRespuesta bigint

 

--registrando un nuevo error
IF not exists(select 1 from CatalogoErrorSunat where codigoRespuesta=@CodigoSUNAT)
BEGIN
   INSERT INTO CatalogoErrorSunat(codigoRespuesta,Descripcion,Reintentar) VALUES(@CodigoSUNAT,@Descripcion,0)
END
else
begin
	if @CodigoSUNAT <> '0'
	begin
		set @Descripcion = (select Descripcion from CatalogoErrorSunat where codigoRespuesta = @CodigoSUNAT)
	end
end

SET @IdCatalogo=(select IdCatalogo from CatalogoErrorSunat where codigoRespuesta=@CodigoSUNAT)
--Clasificar el tipo de respuesta
SET @Respuesta=convert(bigint,@CodigoSUNAT)

if (@Respuesta=0)  SET @TipoRespuesta=0
if (@Respuesta>=100 and @Respuesta<=1999)  SET @TipoRespuesta=2
if (@Respuesta>=2000 and @Respuesta<=3999)  SET @TipoRespuesta=1
if (@Respuesta>=4000)  SET @TipoRespuesta=3

SET @IdTipoRespuesta=(select IdTipoRespuesta from TipoRespuesta where Codigo=@TipoRespuesta)

--Registrando la ultima respuesta
if not exists(select 1 from ComprobanteRespuesta where IdComprobante=@IdComprobante)
	BEGIN
       insert into ComprobanteRespuesta(IdComprobante,IdCatalogo,DescripcionSUNAT,IdTipoRespuesta,FechaRegistro,FechaSUNAT,HoraSUNAT)
	   VALUES(@IdComprobante,@IdCatalogo,@Descripcion,@IdTipoRespuesta,GETDATE(),@FechaSunat,@HoraSunat)
	END
ELSE
BEGIN
	update comprobanteRespuesta set IdCatalogo=@IdCatalogo,DescripcionSUNAT=@Descripcion,IdTipoRespuesta=@IdTipoRespuesta,FechaRegistro=GETDATE(),FechaSUNAT=@FechaSunat,HoraSUNAT=@HoraSunat
	where IdComprobante=@IdComprobante    
END
--Registrando el historial de respuesta
insert into CDRSunatProcesado(Archivo,FechaRegistro) OUTPUT INSERTED.IdArchivo into @temp(Id)
values(@Archivo,getdate())

SET @IdMax=(select Id from @temp)

insert into ComprobanteRespuestaHistorial(IdArchivo,IdCatalogo,Descripcion,FechaRespuestaSunat,FechaRegistro,HoraRespuestaSunat,IdComprobante)
values(@IdMax,@IdCatalogo,@Descripcion,@FechaSunat,getdate(),@HoraSunat,@IdComprobante)
--Eliminado de la tabla pendientes
delete from CDRSunatPendiente where IdComprobante=@IdComprobante


