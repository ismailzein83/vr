CREATE TABLE [dbo].[AlertCriteria] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [ClassName]            VARCHAR (512) NULL,
    [SerializationInfo]    IMAGE         NULL,
    [Updated]              DATETIME      NULL,
    [IsEnabled]            CHAR (1)      NULL,
    [Tag]                  VARCHAR (50)  NULL,
    [UserID]               INT           NULL,
    [XMLSerializationInfo] VARCHAR (MAX) NULL,
    [timestamp]            ROWVERSION    NOT NULL,
    CONSTRAINT [PK_PersistedAlertCriteria2] PRIMARY KEY CLUSTERED ([ID] ASC)
);

