CREATE TYPE [Retail_EDR].[RingoMessageType] AS TABLE (
    [Sender]                    NVARCHAR (100) NULL,
    [Recipient]                 NVARCHAR (100) NULL,
    [SenderNetwork]             NVARCHAR (100) NULL,
    [RecipientNetwork]          NVARCHAR (100) NULL,
    [MSISDN]                    NVARCHAR (100) NULL,
    [RecipientRequestCode]      NVARCHAR (100) NULL,
    [MessageType]               SMALLINT       NULL,
    [FileName]                  NVARCHAR (200) NULL,
    [MessageDate]               DATETIME       NULL,
    [ACKMessageFileName]        NVARCHAR (100) NULL,
    [ACKMessageDate]            DATETIME       NULL,
    [StateRequest]              SMALLINT       NULL,
    [FlagCredit]                SMALLINT       NULL,
    [TransferredCredit]         INT            NULL,
    [FlagRequestCreditTransfer] SMALLINT       NULL,
    [AccountID]                 BIGINT         NULL,
    [NovercaFileName]           NVARCHAR (200) NULL);





