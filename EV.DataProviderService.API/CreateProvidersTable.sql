-- Script to create Providers table if it doesn't exist
USE FA25BearDB;
GO

-- Check if Provider table exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Provider]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Provider] (
        [ProviderId] UNIQUEIDENTIFIER NOT NULL,
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL,
        [ContactEmail] NVARCHAR(320) NULL,
        [Verified] BIT NOT NULL DEFAULT 0,
        [OnboardedAt] DATETIME NULL,
        CONSTRAINT [PK__Provider__B54C687D9D7A8C1A] PRIMARY KEY ([ProviderId]),
        CONSTRAINT [UQ__Provider__CADB0B13751B52EF] UNIQUE ([OrganizationId])
    );
    
    PRINT 'Table Provider created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Provider already exists.';
END
GO

-- Check if Organizations table exists, if not create it
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Organizations]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Organizations] (
        [OrganizationId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Name] NVARCHAR(250) NOT NULL,
        [OrgType] NVARCHAR(100) NULL,
        [Description] NVARCHAR(1000) NULL,
        [Country] NVARCHAR(100) NULL,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK__Organiza__CADB0B12F7843142] PRIMARY KEY ([OrganizationId])
    );
    
    PRINT 'Table Organizations created successfully.';
END
ELSE
BEGIN
    PRINT 'Table Organizations already exists.';
END
GO

-- Add foreign key constraint if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Providers_Org')
BEGIN
    ALTER TABLE [dbo].[Provider]
    ADD CONSTRAINT [FK_Providers_Org] 
    FOREIGN KEY ([OrganizationId]) 
    REFERENCES [dbo].[Organizations]([OrganizationId]);
    
    PRINT 'Foreign key FK_Providers_Org created successfully.';
END
ELSE
BEGIN
    PRINT 'Foreign key FK_Providers_Org already exists.';
END
GO

