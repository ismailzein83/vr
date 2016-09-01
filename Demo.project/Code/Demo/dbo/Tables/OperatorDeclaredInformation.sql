CREATE TABLE [dbo].[OperatorDeclaredInformation] (
    [ID]                                  INT           IDENTITY (1, 1) NOT NULL,
    [OperatorID]                          INT           NOT NULL,
    [FromDate]                            DATETIME      NOT NULL,
    [ToDate]                              DATETIME      NULL,
    [DestinationGroup]                    INT           NULL,
    [Volume]                              INT           NOT NULL,
    [AmountType]                          INT           NOT NULL,
    [Notes]                               VARCHAR (MAX) NULL,
    [Attachment]                          BIGINT        NULL,
    [SourceOperatorDeclaredInformationID] VARCHAR (255) NULL,
    [timestamp]                           ROWVERSION    NULL,
    CONSTRAINT [PK_OperatorDeclaredInformation] PRIMARY KEY CLUSTERED ([ID] ASC)
);



