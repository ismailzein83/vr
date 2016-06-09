CREATE TABLE [QM_BE].[ConnectorResultMapping] (
    [ID]               INT             IDENTITY (1, 1) NOT NULL,
    [ConnectorType]    VARCHAR (50)    NOT NULL,
    [ResultID]         INT             NOT NULL,
    [ResultName]       VARCHAR (50)    NOT NULL,
    [ConnectorResults] NVARCHAR (1000) NOT NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_ConnectorResultMapping\_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]        ROWVERSION      NULL,
    CONSTRAINT [PK_ConnectorResultMapping\] PRIMARY KEY CLUSTERED ([ID] ASC)
);

