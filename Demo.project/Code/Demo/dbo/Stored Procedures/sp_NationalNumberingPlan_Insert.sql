
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[sp_NationalNumberingPlan_Insert]
	@OperatorID int,
	@FromDate datetime,
	@ToDate datetime,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into dbo.NationalNumberingPlan([OperatorID],[FromDate],[ToDate],[Settings])
	Values(@OperatorID,@FromDate,@ToDate,@Settings)

	Set @Id = @@IDENTITY
	END