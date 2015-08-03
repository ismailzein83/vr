-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DeleteDuplicateSystemParameters] 
	-- Add the parameters for the stored procedure here
	@PName	varchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	delete from systemparameter where name = @PName AND [timestamp] not in (select min([timestamp]) from systemparameter group by name having name = @PName)


--declare @PName varchar(50)
--declare cur Cursor local for select distinct name from systemparameter

--open cur

--fetch next from cur into @PName

--while @@FETCH_STATUS = 0 BEGIN

--exec DeleteDuplicateSystemParameters @PName
--fetch next from cur into @PName

--End
--close cur
--deallocate cur

END