


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_OperatorConfiguration_Insert]
	@OperatorID int,
	@Volume int,
	@CDRDirection int,
	@Percentage float,
	@Amount float,
	@Currency int,
	@FromDate datetime, 
	@ToDate datetime,
	@Notes nvarchar(max),
	@ServiceSubTypeSettings	nvarchar(MAX),
	@Id int out
AS
BEGIN
	Insert into dbo.OperatorConfiguration([OperatorID], [Volume], [CDRDirection]  ,	[Percentage] ,	[Amount] ,	[Currency], [FromDate], [ToDate], [Notes] ,[ServiceSubTypeSettings])
	Values(@OperatorID, @Volume, @CDRDirection ,	@Percentage ,	@Amount ,	@Currency, @FromDate, @ToDate, @Notes, @ServiceSubTypeSettings )

	Set @Id = @@IDENTITY
	END