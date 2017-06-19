CREATE TABLE [dbo].[Address] (
    [ID]    INT            IDENTITY (1, 1) NOT NULL,
    [Line1] NVARCHAR (100) NULL,
    [Line2] NVARCHAR (100) NULL,
    [State] NVARCHAR (50)  NULL,
    [Zip]   NVARCHAR (14)  NULL,
    [City]  NVARCHAR (50)  NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC)
);

