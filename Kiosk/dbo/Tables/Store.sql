CREATE TABLE [dbo].[Store] (
    [ID]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (100) NOT NULL,
    [AddressID] INT            NOT NULL,
    CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Store_Address] FOREIGN KEY ([AddressID]) REFERENCES [dbo].[Address] ([ID])
);

