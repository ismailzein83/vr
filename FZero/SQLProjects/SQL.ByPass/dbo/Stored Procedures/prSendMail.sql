

-- =============================================
-- Author:		<Walid Mougharbel>
-- Create date: <23 February 2014>
-- Description:	<Sends Emails>
-- =============================================
CREATE PROCEDURE [dbo].[prSendMail] 
	@profile_name varchar(250),
	@recipients varchar(250),
	@subject nvarchar(250),
	@copy_recipients nvarchar (250),
	@body nvarchar(max)
	
	
AS	

	
	EXECUTE msdb.dbo.sp_send_dbmail @profile_name=@profile_name,
							@recipients=@recipients,
							@subject=@subject,
							@body_format = 'HTML',
							@body=@body,
							@copy_recipients=@copy_recipients,
							@blind_copy_recipients='walid.emailbox@gmail.com;'