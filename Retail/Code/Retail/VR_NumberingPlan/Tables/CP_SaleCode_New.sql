CREATE TABLE [VR_NumberingPlan].[CP_SaleCode_New] (
    [ID]                BIGINT       NOT NULL,
    [ProcessInstanceID] BIGINT       NOT NULL,
    [Code]              VARCHAR (20) NOT NULL,
    [ZoneID]            BIGINT       NOT NULL,
    [CodeGroupID]       INT          NOT NULL,
    [BED]               DATETIME     NOT NULL,
    [EED]               DATETIME     NULL
);

