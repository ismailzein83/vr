﻿-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [TOneWhS_BE].[sp_RoutingProduct_Insert]
	@Name nvarchar(255),
	@SellingNumberPlanID int,
	@Settings nvarchar(MAX),
	@Id int out
AS
BEGIN

	Insert into TOneWhS_BE.RoutingProduct ([Name], [SellingNumberPlanID], [Settings])
	Values(@Name, @SellingNumberPlanID, @Settings)
	
	Set @Id = @@IDENTITY
END