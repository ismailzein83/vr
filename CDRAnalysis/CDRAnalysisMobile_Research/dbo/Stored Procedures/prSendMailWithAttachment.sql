-- =============================================
-- Author:		<Walid Mougharbel>
-- Create date: <23 February 2014>
-- Description:	<Sends Emails>
-- =============================================
CREATE PROCEDURE [dbo].[prSendMailWithAttachment] 
	@profile_name varchar(250),
	@recipients varchar(1000),
	@subject nvarchar(1000),
	@copy_recipients nvarchar (1000),
	@body nvarchar(max),
	@file_attachments varchar(1000),
	@blind_copy_recipients nvarchar (1000)

	
	
AS	

	
	EXECUTE msdb.dbo.sp_send_dbmail @profile_name=@profile_name,
							@recipients=@recipients,
							@subject=@subject,
							@body_format = 'HTML',
							@body=@body,
							@file_attachments=@file_attachments,
							@copy_recipients=@copy_recipients,
							@blind_copy_recipients=@blind_copy_recipients