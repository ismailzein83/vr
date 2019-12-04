CREATE TABLE [CRMFixedOper].[Case] (
    [ID]          BIGINT         NOT NULL,
    [CaseNumber]  BIGINT         NULL,
    [Description] NVARCHAR (900) NULL,
    [CreatedTime] DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);





