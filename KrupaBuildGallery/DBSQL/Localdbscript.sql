USE [master]
GO
/****** Object:  Database [DB_krupagallery]    Script Date: 4/19/2020 11:20:10 AM ******/
CREATE DATABASE [DB_krupagallery]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DB_krupagallery', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER12\MSSQL\DATA\DB_krupagallery.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'DB_krupagallery_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER12\MSSQL\DATA\DB_krupagallery_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [DB_krupagallery] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [DB_krupagallery].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [DB_krupagallery] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [DB_krupagallery] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [DB_krupagallery] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [DB_krupagallery] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [DB_krupagallery] SET ARITHABORT OFF 
GO
ALTER DATABASE [DB_krupagallery] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [DB_krupagallery] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [DB_krupagallery] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [DB_krupagallery] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [DB_krupagallery] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [DB_krupagallery] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [DB_krupagallery] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [DB_krupagallery] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [DB_krupagallery] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [DB_krupagallery] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [DB_krupagallery] SET  DISABLE_BROKER 
GO
ALTER DATABASE [DB_krupagallery] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [DB_krupagallery] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [DB_krupagallery] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [DB_krupagallery] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [DB_krupagallery] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [DB_krupagallery] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [DB_krupagallery] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [DB_krupagallery] SET RECOVERY FULL 
GO
ALTER DATABASE [DB_krupagallery] SET  MULTI_USER 
GO
ALTER DATABASE [DB_krupagallery] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DB_krupagallery] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DB_krupagallery] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DB_krupagallery] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
EXEC sys.sp_db_vardecimal_storage_format N'DB_krupagallery', N'ON'
GO
USE [DB_krupagallery]
GO
/****** Object:  Table [dbo].[tbl_AdminRoleModules]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_AdminRoleModules](
	[AdminRoleModuleId] [int] NOT NULL,
	[ModuleName] [nvarchar](100) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[None] [bit] NOT NULL,
	[View] [bit] NOT NULL,
	[Add] [bit] NOT NULL,
	[Edit] [bit] NOT NULL,
	[Full] [bit] NOT NULL,
 CONSTRAINT [PK_AdminRoleModule] PRIMARY KEY CLUSTERED 
(
	[AdminRoleModuleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_AdminRolePermissions]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_AdminRolePermissions](
	[AdminRolePermissionId] [int] IDENTITY(1,1) NOT NULL,
	[AdminRoleId] [int] NOT NULL,
	[AdminRoleModuleId] [int] NOT NULL,
	[Permission] [int] NOT NULL,
 CONSTRAINT [PK_tbl_AdminRolePermissions] PRIMARY KEY CLUSTERED 
(
	[AdminRolePermissionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_AdminRoles]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_AdminRoles](
	[AdminRoleId] [int] IDENTITY(1,1) NOT NULL,
	[AdminRoleName] [nvarchar](150) NULL,
	[AdminRoleDescription] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_AdminRole] PRIMARY KEY CLUSTERED 
(
	[AdminRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_AdminUsers]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_AdminUsers](
	[AdminUserId] [bigint] IDENTITY(1,1) NOT NULL,
	[AdminRoleId] [int] NOT NULL,
	[FirstName] [nvarchar](350) NOT NULL,
	[LastName] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](350) NULL,
	[Password] [nvarchar](150) NOT NULL,
	[MobileNo] [nvarchar](20) NULL,
	[AlternateMobile] [nvarchar](50) NULL,
	[Address] [nvarchar](450) NULL,
	[City] [nvarchar](200) NULL,
	[Designation] [nvarchar](350) NULL,
	[Dob] [datetime] NULL,
	[DateOfJoin] [datetime] NULL,
	[BloodGroup] [nvarchar](50) NULL,
	[WorkingTime] [nvarchar](50) NULL,
	[AdharCardNo] [nvarchar](50) NULL,
	[DateOfIdCardExpiry] [datetime] NULL,
	[Remarks] [nvarchar](600) NULL,
	[ProfilePicture] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_AdminUser] PRIMARY KEY CLUSTERED 
(
	[AdminUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Cart]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Cart](
	[Cart_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CartItemId] [bigint] NULL,
	[CartItemQty] [bigint] NULL,
	[CartSessionId] [nvarchar](500) NULL,
	[ClientUserId] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Cart] PRIMARY KEY CLUSTERED 
(
	[Cart_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Categories]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Categories](
	[CategoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](450) NOT NULL,
	[CategoryImage] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_Category] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ClientOtherDetails]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ClientOtherDetails](
	[ClientOtherDetailsId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientUserId] [bigint] NOT NULL,
	[Addharcardno] [nvarchar](50) NULL,
	[Pancardno] [nvarchar](50) NULL,
	[GSTno] [nvarchar](50) NULL,
	[City] [nvarchar](150) NULL,
	[State] [nvarchar](150) NULL,
	[Address] [nvarchar](250) NULL,
	[ShipCity] [nvarchar](150) NULL,
	[ShipState] [nvarchar](100) NULL,
	[ShipAddress] [nvarchar](250) NULL,
	[CreditLimitAmt] [numeric](18, 2) NULL,
	[AmountDue] [numeric](18, 2) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[ShipFirstName] [nvarchar](300) NULL,
	[ShipLastName] [nvarchar](300) NULL,
	[ShipPhoneNumber] [nvarchar](20) NULL,
	[ShipEmail] [nvarchar](150) NULL,
	[ShipPostalcode] [nvarchar](50) NULL,
	[Dob] [datetime] NULL,
	[ShopPhoto] [nvarchar](max) NULL,
	[ShopName] [nvarchar](250) NULL,
	[PanCardPhoto] [nvarchar](450) NULL,
	[GSTPhoto] [nvarchar](450) NULL,
	[DistributorCode] [nvarchar](150) NULL,
	[AddharPhoto] [nvarchar](max) NULL,
 CONSTRAINT [PK_Tbl_ClientOtherDetails] PRIMARY KEY CLUSTERED 
(
	[ClientOtherDetailsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ClientRoles]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ClientRoles](
	[ClientRoleId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientRoleName] [nvarchar](150) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ClientRole] PRIMARY KEY CLUSTERED 
(
	[ClientRoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ClientUsers]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ClientUsers](
	[ClientUserId] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](250) NOT NULL,
	[LastName] [nvarchar](250) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[Email] [nvarchar](150) NOT NULL,
	[Password] [nvarchar](150) NOT NULL,
	[MobileNo] [nvarchar](50) NULL,
	[ClientRoleId] [bigint] NOT NULL,
	[CompanyName] [nvarchar](150) NULL,
	[ProfilePicture] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[AlternateMobileNo] [nvarchar](max) NULL,
	[Prefix] [nvarchar](10) NULL,
 CONSTRAINT [PK_Tbl_ClientUsers] PRIMARY KEY CLUSTERED 
(
	[ClientUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ContactFormData]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ContactFormData](
	[ContactForm_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](250) NULL,
	[Message] [nvarchar](max) NOT NULL,
	[PhoneNumber] [nvarchar](20) NULL,
	[Email] [nvarchar](50) NULL,
	[FromWhere] [nvarchar](50) NULL,
	[ClientUserId] [bigint] NULL,
	[MessageDate] [datetime] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_tbl_ContactFormData] PRIMARY KEY CLUSTERED 
(
	[ContactForm_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_DistributorRequestDetails]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DistributorRequestDetails](
	[DistributorRequestId] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](250) NULL,
	[LastName] [nvarchar](250) NULL,
	[Email] [nvarchar](150) NULL,
	[MobileNo] [nvarchar](20) NULL,
	[CompanyName] [nvarchar](150) NULL,
	[City] [nvarchar](150) NULL,
	[State] [nvarchar](150) NULL,
	[AddharcardNo] [nvarchar](25) NULL,
	[PanCardNo] [nvarchar](25) NULL,
	[GSTNo] [nvarchar](35) NULL,
	[IsDelete] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [bigint] NULL,
	[Dob] [datetime] NULL,
	[AlternateMobileNo] [nvarchar](50) NULL,
	[Prefix] [nvarchar](10) NULL,
	[ShopName] [nvarchar](200) NULL,
	[ShopPhoto] [nvarchar](max) NULL,
	[PanCardPhoto] [nvarchar](max) NULL,
	[GSTPhoto] [nvarchar](max) NULL,
	[AddharPhoto] [nvarchar](450) NULL,
	[ProfilePhoto] [nvarchar](450) NULL,
 CONSTRAINT [PK_tbl_DistributorRequestDetails] PRIMARY KEY CLUSTERED 
(
	[DistributorRequestId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_GeneralSetting]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_GeneralSetting](
	[GeneralSettingId] [bigint] IDENTITY(1,1) NOT NULL,
	[InitialPointCustomer] [decimal](18, 2) NULL,
	[ShippingMessage] [nvarchar](max) NULL,
 CONSTRAINT [PK_tbl_GeneralSetting] PRIMARY KEY CLUSTERED 
(
	[GeneralSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Godown]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Godown](
	[GodownId] [int] IDENTITY(1,1) NOT NULL,
	[GodownName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDeleted] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [int] NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_Godown] PRIMARY KEY CLUSTERED 
(
	[GodownId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_GSTMaster]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_GSTMaster](
	[GST_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[GSTText] [nvarchar](15) NULL,
	[GSTPer] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tbl_GSTMaster] PRIMARY KEY CLUSTERED 
(
	[GST_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_HomeImages]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_HomeImages](
	[HomeImageId] [int] IDENTITY(1,1) NOT NULL,
	[HomeImageName] [nvarchar](100) NOT NULL,
	[HeadingText1] [nvarchar](100) NULL,
	[HeadingText2] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_HomeImages] PRIMARY KEY CLUSTERED 
(
	[HomeImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ItemStocks]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemStocks](
	[StockId] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[SubProductId] [bigint] NULL,
	[ProductItemId] [bigint] NOT NULL,
	[Qty] [bigint] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ItemStocks] PRIMARY KEY CLUSTERED 
(
	[StockId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Itemtext_master]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Itemtext_master](
	[Item_Text_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemText] [nvarchar](max) NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Itemtext_master] PRIMARY KEY CLUSTERED 
(
	[Item_Text_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_LoginHistory]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_LoginHistory](
	[LoginHistoryId] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [bigint] NULL,
	[DateAction] [datetime] NULL,
	[IPAddress] [nvarchar](150) NULL,
	[Type] [nvarchar](500) NULL,
 CONSTRAINT [PK_tbl_LoginHistory] PRIMARY KEY CLUSTERED 
(
	[LoginHistoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Offers]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Offers](
	[OfferId] [bigint] IDENTITY(1,1) NOT NULL,
	[OfferName] [nvarchar](350) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[SubproductId] [bigint] NULL,
	[ProductItemId] [bigint] NOT NULL,
	[OfferPrice] [numeric](18, 2) NOT NULL,
	[OfferPriceforDistributor] [numeric](18, 2) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_Offers] PRIMARY KEY CLUSTERED 
(
	[OfferId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_OrderItemDetails]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_OrderItemDetails](
	[OrderDetailId] [bigint] IDENTITY(1,1) NOT NULL,
	[OrderId] [bigint] NULL,
	[ProductItemId] [bigint] NULL,
	[ItemName] [nvarchar](250) NULL,
	[Qty] [bigint] NULL,
	[Price] [numeric](18, 2) NULL,
	[Sku] [nvarchar](50) NULL,
	[GSTAmt] [numeric](18, 2) NULL,
	[IGSTAmt] [numeric](18, 2) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[Discount] [decimal](18, 2) NULL,
	[GSTPer] [decimal](18, 2) NULL,
 CONSTRAINT [PK_Tbl_OrderItemDetails] PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Orders]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Orders](
	[OrderId] [bigint] IDENTITY(1,1) NOT NULL,
	[ClientUserId] [bigint] NOT NULL,
	[OrderAmount] [decimal](18, 2) NOT NULL,
	[OrderShipCity] [nvarchar](100) NULL,
	[OrderShipState] [nvarchar](100) NULL,
	[OrderShipAddress] [nvarchar](250) NOT NULL,
	[OrderShipPincode] [nvarchar](10) NULL,
	[OrderShipClientName] [nvarchar](150) NULL,
	[OrderShipClientPhone] [nvarchar](50) NULL,
	[OrderStatusId] [bigint] NOT NULL,
	[PaymentType] [nvarchar](150) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[AmountDue] [decimal](18, 2) NULL,
	[RazorpayOrderId] [nvarchar](300) NULL,
	[RazorpayPaymentId] [nvarchar](300) NULL,
	[RazorSignature] [nvarchar](300) NULL,
	[ShippingCharge] [decimal](18, 2) NULL,
	[ShippingStatus] [int] NULL,
	[PointsUsed] [decimal](18, 2) NULL,
	[InvoiceNo] [bigint] NULL,
	[InvoiceYear] [nvarchar](150) NULL,
 CONSTRAINT [PK_Tbl_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_PaymentHistory]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PaymentHistory](
	[PaymentHistory_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[AmountPaid] [decimal](18, 2) NOT NULL,
	[AmountDue] [decimal](18, 2) NOT NULL,
	[DateOfPayment] [datetime] NOT NULL,
	[OrderId] [bigint] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
	[PaymentBy] [nvarchar](250) NULL,
	[RazorpayOrderId] [nvarchar](300) NULL,
	[RazorpayPaymentId] [nvarchar](300) NULL,
	[RazorSignature] [nvarchar](300) NULL,
	[PaymentFor] [nvarchar](250) NULL,
 CONSTRAINT [PK_tbl_PaymentHistory] PRIMARY KEY CLUSTERED 
(
	[PaymentHistory_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_PointDetails]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PointDetails](
	[PointId] [bigint] IDENTITY(1,1) NOT NULL,
	[Points] [decimal](18, 2) NULL,
	[ClientUserId] [bigint] NULL,
	[ExpiryDate] [datetime] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [bigint] NULL,
	[UsedPoints] [decimal](18, 2) NULL,
 CONSTRAINT [PK_tbl_PointDetails] PRIMARY KEY CLUSTERED 
(
	[PointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ProductItemImages]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ProductItemImages](
	[ProductItemImageId] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductItemId] [bigint] NOT NULL,
	[ItemImage] [nvarchar](450) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_ProductItemImages] PRIMARY KEY CLUSTERED 
(
	[ProductItemImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_ProductItems]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ProductItems](
	[ProductItemId] [bigint] IDENTITY(1,1) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[SubProductId] [bigint] NULL,
	[ItemName] [nvarchar](200) NOT NULL,
	[ItemDescription] [nvarchar](max) NOT NULL,
	[MRPPrice] [decimal](18, 2) NOT NULL,
	[DistributorPrice] [decimal](18, 2) NOT NULL,
	[CustomerPrice] [decimal](18, 2) NOT NULL,
	[GST_Per] [decimal](18, 2) NOT NULL,
	[IGST_Per] [decimal](18, 2) NOT NULL,
	[Cess] [decimal](18, 2) NULL,
	[MainImage] [nvarchar](350) NULL,
	[Notification] [nvarchar](200) NULL,
	[Sku] [nvarchar](50) NULL,
	[IsPopularProduct] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
	[ShippingCharge] [decimal](18, 2) NULL,
	[Tags] [nvarchar](max) NULL,
	[GodownId] [bigint] NULL,
	[HSNCode] [nvarchar](150) NULL,
 CONSTRAINT [PK_Tbl_ProductItems] PRIMARY KEY CLUSTERED 
(
	[ProductItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_Products]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Products](
	[Product_Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ProductName] [nvarchar](450) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ProductImage] [nvarchar](450) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_Product] PRIMARY KEY CLUSTERED 
(
	[Product_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_SubProducts]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_SubProducts](
	[SubProductId] [bigint] IDENTITY(1,1) NOT NULL,
	[SubProductName] [nvarchar](450) NOT NULL,
	[CategoryId] [bigint] NOT NULL,
	[ProductId] [bigint] NOT NULL,
	[SubProductImage] [nvarchar](350) NULL,
	[IsActive] [bit] NOT NULL,
	[IsDelete] [bit] NOT NULL,
	[CreatedBy] [bigint] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy] [bigint] NULL,
	[UpdatedDate] [datetime] NULL,
 CONSTRAINT [PK_Tbl_SubProduct] PRIMARY KEY CLUSTERED 
(
	[SubProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[tbl_WishList]    Script Date: 4/19/2020 11:20:11 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_WishList](
	[PK_WishListId] [bigint] IDENTITY(1,1) NOT NULL,
	[ItemId] [bigint] NULL,
	[ClientUserId] [bigint] NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [PK_tbl_WishList] PRIMARY KEY CLUSTERED 
(
	[PK_WishListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (1, N'Role', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (2, N'Category', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (3, N'Product', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (4, N'Sub Product', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (5, N'Product Item', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (6, N'Stock', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (7, N'Order', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (8, N'Offer', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (9, N'Customers', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (10, N'Distibutors', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (11, N'Distibutor Request', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (12, N'Contact Request', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (13, N'Setting', 0, 1, 1, 1, 1, 1)
INSERT [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId], [ModuleName], [DisplayOrder], [None], [View], [Add], [Edit], [Full]) VALUES (14, N'Manage Page Content', 0, 1, 1, 1, 1, 1)
SET IDENTITY_INSERT [dbo].[tbl_AdminRolePermissions] ON 

INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (1, 2, 1, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (2, 2, 2, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (3, 2, 3, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (4, 2, 4, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (5, 2, 5, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (6, 2, 6, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (7, 2, 7, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (8, 2, 8, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (9, 2, 9, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (10, 2, 10, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (11, 2, 11, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (12, 2, 12, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (13, 2, 13, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (14, 2, 14, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (15, 3, 1, 4)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (16, 3, 2, 3)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (17, 3, 3, 2)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (18, 3, 4, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (19, 3, 5, 0)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (20, 3, 6, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (21, 3, 7, 2)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (22, 3, 8, 3)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (23, 3, 9, 4)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (24, 3, 10, 3)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (25, 3, 11, 2)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (26, 3, 12, 1)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (27, 3, 13, 0)
INSERT [dbo].[tbl_AdminRolePermissions] ([AdminRolePermissionId], [AdminRoleId], [AdminRoleModuleId], [Permission]) VALUES (28, 3, 14, 1)
SET IDENTITY_INSERT [dbo].[tbl_AdminRolePermissions] OFF
SET IDENTITY_INSERT [dbo].[tbl_AdminRoles] ON 

INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Super Admin', NULL, 1, 0, -1, CAST(0x0000AB5500000000 AS DateTime), -1, CAST(0x0000AB5500000000 AS DateTime))
INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Staff', N'This is staff role for few module rights', 1, 0, 1, CAST(0x0000AB8C00E78D84 AS DateTime), 1, CAST(0x0000AB94006114A0 AS DateTime))
INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Manager', NULL, 1, 0, 1, CAST(0x0000AB8C00F7A569 AS DateTime), 1, CAST(0x0000AB940093B386 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_AdminRoles] OFF
SET IDENTITY_INSERT [dbo].[tbl_AdminUsers] ON 

INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, N'Nilesh', N'Prajapati', N'prajapati.nileshbhai@gmail.com', N'12345', N'9824936252', N'9033550453', N'100 foot road,, Nani khodiyar,', N'Anand', N'Sr. Asp.net Developer', CAST(0x0000800F00000000 AS DateTime), CAST(0x0000AB9300000000 AS DateTime), N'AB+', N'10 AM to 7 PM', N'1234 5678 9123', CAST(0x0000ABCC00000000 AS DateTime), N'This is super admin user', N'avatar04-210a10e7-205f-4e1c-9da3-382dd4b72db5.png', 1, 0, -1, CAST(0x0000AB5500000000 AS DateTime), 1, CAST(0x0000AB9400B69AC1 AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 1, N'Kamlesh', N'Lalwani', N'kamleshlalwani152@gmail.com', N'12345', N'9033550453', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, -1, CAST(0x0000AB7400000000 AS DateTime), -1, CAST(0x0000AB7400000000 AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, 3, N'Vanita', N'Prajapati', N'vanita007444@gmail.com', N'Admin@123', N'7575036252', N'9033550453', N'100 foot road,, Nani khodiyar,', N'Anand', N'Staff', CAST(0x0000827700000000 AS DateTime), CAST(0x0000AB9300000000 AS DateTime), N'O+', N'10 AM to 7 PM', N'0000 1111 2222 3333', NULL, N'testing', N'user2-160x160-e7478cd6-6bd9-4300-b4b4-679580240d19.jpg', 1, 0, 1, CAST(0x0000AB93017C77C4 AS DateTime), 1, CAST(0x0000AB9400EE1CC7 AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, 2, N'test', N'test', N'test@gmail.com', N'9033550453', N'9999999999', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, 1, CAST(0x0000AB9301895918 AS DateTime), 1, CAST(0x0000AB93012ED2DF AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_AdminUsers] OFF
SET IDENTITY_INSERT [dbo].[tbl_Cart] ON 

INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (20, 1, 1, N'c4f19e00-6719-4699-aa0a-983a81558648', 0, CAST(0x0000AB7F0030976B AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (21, 4, 3, N'c4f19e00-6719-4699-aa0a-983a81558648', 0, CAST(0x0000AB7F0030ACEF AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (24, 17, 28, N'257cf5f6-20eb-4799-8c62-c84a714b3e32', 12, CAST(0x0000AB7F003A8510 AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (44, 26, 3, N'268e0d70-4853-4c9c-bfa2-dd0a179552c7', 6, CAST(0x0000AB810033B011 AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (10059, 25, 1, N'16836579-1f1f-47bf-9eba-4ed77d02291f', 5, CAST(0x0000AB9C01566AE8 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Cart] OFF
SET IDENTITY_INSERT [dbo].[tbl_Categories] ON 

INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Catergory1', NULL, 1, 0, 1, CAST(0x0000AB7E00C2EB3F AS DateTime), 1, CAST(0x0000AB7E00C2EB3F AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Color', N'9edffaa3-94a4-4bfd-a8c8-5e59420fda0f-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(0x0000AB7F004B9AE2 AS DateTime), 1, CAST(0x0000AB7F004BB7D5 AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Water Tank', N'3209236e-695e-40ae-a4c7-b20df3d10722-asian-paints-blue-colour-500x500.png', 1, 0, 1, CAST(0x0000AB7F004C0248 AS DateTime), 1, CAST(0x0000AB7F007B1F1F AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'PTMP FAUCET', N'5e58ae27-54d9-4437-8e2c-96e3b82a60ce-1.jpg', 1, 0, 1, CAST(0x0000AB7F007BB231 AS DateTime), 1, CAST(0x0000AB7F007CAF85 AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'FAUCETS NEW', NULL, 1, 0, 1, CAST(0x0000AB7F0085AB30 AS DateTime), 1, CAST(0x0000AB7F0085AB30 AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'Nilesh test', N'ca79335c-383e-45b0-9afa-6066607a5422-IMG-20200313-WA0008.jpg', 0, 0, 1, CAST(0x0000AB7F00FD0110 AS DateTime), 1, CAST(0x0000AB7F00FD837E AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'GOLD', N'aee373b3-c111-44db-bb59-77856cd128eb-prehistorich.jpg', 1, 0, 1, CAST(0x0000AB8100A428E2 AS DateTime), 1, CAST(0x0000AB8100A428E2 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Categories] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientOtherDetails] ON 

INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (1, 1, NULL, NULL, NULL, N'Anand', N'Gujarat', N'B-101,Laksh Residency', NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (2, 2, NULL, NULL, NULL, N'Vadodara', N'Gujarat', N'A101,Prime Buglows', NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10003, 3, NULL, NULL, NULL, N'Nadiad', N'Gujarat', N'A5,Res Houese', NULL, NULL, NULL, CAST(5000.00 AS Numeric(18, 2)), NULL, 1, 0, NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10004, 4, NULL, NULL, NULL, N'Godhra', N'Gujarat', N'K Nivas,MG road', NULL, NULL, NULL, CAST(4000.00 AS Numeric(18, 2)), CAST(1200.00 AS Numeric(18, 2)), 1, 0, NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10005, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 5, CAST(0x0000AB7A00CEF0D4 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10006, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 6, CAST(0x0000AB7B002686DB AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10007, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 7, CAST(0x0000AB7B002774EA AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10008, 8, N'112121212', NULL, N'154544545', N'Anand', N'Gujarat', NULL, NULL, NULL, NULL, CAST(10000.00 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 1, CAST(0x0000AB7F009DA34E AS DateTime), 1, CAST(0x0000AB7F009DA34E AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10009, 9, N'12345', NULL, NULL, N'Anand', N'Gujarat', NULL, N'ANand', N'Gujarat', N'Shop 1, M G ROad', CAST(20000.00 AS Numeric(18, 2)), CAST(13194.00 AS Numeric(18, 2)), 1, 0, 1, CAST(0x0000AB7F009E7F9A AS DateTime), 1, CAST(0x0000AB7F009E7F9A AS DateTime), N'ramesh', N'patel', N'8897987877', N'krutik@gmail.com', N'388770', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10010, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 10, CAST(0x0000AB7E01749010 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10011, 11, N'zx12345uhdhh', NULL, NULL, N'GODHRA', N'GUJARAT', NULL, NULL, NULL, NULL, CAST(10000.00 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 1, CAST(0x0000AB7E0179AF69 AS DateTime), 1, CAST(0x0000AB7E0179AF69 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10012, 12, N'123', NULL, NULL, N'godhra', N'gujarat', NULL, NULL, NULL, NULL, CAST(50000.00 AS Numeric(18, 2)), CAST(3700.00 AS Numeric(18, 2)), 1, 0, 1, CAST(0x0000AB7F00338977 AS DateTime), 1, CAST(0x0000AB7F00338977 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10013, 13, N'123455', NULL, NULL, N'GODHRA', N'GUJARAT', NULL, N'asc', N'acsd', N'asx', CAST(50000.00 AS Numeric(18, 2)), CAST(30000.00 AS Numeric(18, 2)), 1, 0, 1, CAST(0x0000AB810034EA55 AS DateTime), 1, CAST(0x0000AB810034EA55 AS DateTime), N'kam', N'asx', N'9106490735', N'kamleshlalwani152@gmail.com', N'389001', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10014, 14, NULL, NULL, NULL, NULL, NULL, NULL, N'ANand', N'Gujarat', N'Shop 1, M G ROad', NULL, NULL, 1, 0, 14, CAST(0x0000AB9A014E4249 AS DateTime), NULL, NULL, N'ramesh', N'patel', N'7016232958', N'krutik@gmail.com', N'380001', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ClientOtherDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientRoles] ON 

INSERT [dbo].[tbl_ClientRoles] ([ClientRoleId], [ClientRoleName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Customer', 1, 0, 1, CAST(0x0000AB5601294DC9 AS DateTime), 1, CAST(0x0000AB5601294DC9 AS DateTime))
INSERT [dbo].[tbl_ClientRoles] ([ClientRoleId], [ClientRoleName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Distributor', 1, 0, 1, CAST(0x0000AB56012E8079 AS DateTime), 1, CAST(0x0000AB56012E8079 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ClientRoles] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientUsers] ON 

INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (1, N'John', N'Doe', N'johndoe', N'johndoe@gmail.com', N'1ZQRVQG3sX0=', N'9999898989', 1, NULL, NULL, 1, 0, 1, CAST(0x0000AB56012E8079 AS DateTime), 1, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (2, N'Ramesh', N'Patel', N'rameshpatel', N'ramesh@gmail.com', N'1ZQRVQG3sX0=', N'8787878787', 1, NULL, NULL, 1, 0, 1, CAST(0x0000AB56012E8079 AS DateTime), 1, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (3, N'Rakesh', N'Shah', N'rakeshshah11', N'rakesh@gmail.com', N'1ZQRVQG3sX0=', N'9998707070', 2, N'RK Center', NULL, 1, 0, 1, CAST(0x0000AB56012E8079 AS DateTime), 1, CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (4, N'Nil', N'Thakkar', N'nil1212', N'nil1287@gmail.com', N'1ZQRVQG3sX0=', N'9977447744', 2, N'Nil Trader', NULL, 1, 0, 1, CAST(0x0000AB56012E8079 AS DateTime), 1, CAST(0x0000AB7F0040F787 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (5, N'Nilesh', N'Prajapati', N'NileshPrajapati', N'nilesh007444@gmail.com', N'AOx_m27p__8=', N'09824936252', 1, NULL, N'', 1, 0, 0, CAST(0x0000AB7A00CEEF3B AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (6, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'kamleshlalwani152@gmail.com', N'FczZJFnW_R0=', N'9106490735', 1, NULL, N'', 1, 0, 0, CAST(0x0000AB7B002686D2 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (7, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'kamleshlalwani152@yahoo.com', N'FczZJFnW_R0=', N'9510105513', 1, NULL, N'', 1, 0, 0, CAST(0x0000AB7B002774E5 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (8, N'Raj', N'Thakker', N'RajThakker', N'reaj@gmail.com', N'AOx_m27p__8=', N'878888788', 2, N'Raj Traders', NULL, 1, 0, 1, CAST(0x0000AB7F009DA1BA AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (9, N'Nilesh', N'Prajapati', N'NileshPrajapati', N'prajapati.nileshbhai1@gmail.com', N'AOx_m27p__8=', N'9033331394', 2, N'Nilesh Software', NULL, 1, 0, 1, CAST(0x0000AB7F009E7E7F AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (10, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'abc@gmail.com', N'FczZJFnW_R0=', N'9106490735', 1, NULL, N'', 1, 0, 0, CAST(0x0000AB7E01749006 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (11, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'redspeech@rediffmail.com', N'Sj3ERF8C/LY=', N'9106490735', 2, N'UNIVERSAL INFOTECH', NULL, 1, 0, 1, CAST(0x0000AB7E0179AF65 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (12, N'ashwain', N'khatri', N'ashwainkhatri', N'prakashmagnani@gmail.com', N'KZ152Wn/Rqw=', N'9824143334', 2, N'HARI OM TREADERS', NULL, 1, 0, 1, CAST(0x0000AB7F0033896E AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (13, N'BURHAN', N'BURHAN', N'BURHANBURHAN', N'kachwala63@gmail.com', N'KZ152Wn/Rqw=', N'9016050614', 2, N'BURHAN', NULL, 1, 0, 1, CAST(0x0000AB810034EA4B AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (14, N'krutik', N'shah', N'krutikshah', N'krutik.shah1310@gmail.com', N'keR6gLPDWCAYWKAnpSw_CQ==', N'7016232958', 1, NULL, N'', 1, 0, 0, CAST(0x0000AB9A014E4246 AS DateTime), NULL, NULL, N'77777777', N'Mr')
SET IDENTITY_INSERT [dbo].[tbl_ClientUsers] OFF
SET IDENTITY_INSERT [dbo].[tbl_ContactFormData] ON 

INSERT [dbo].[tbl_ContactFormData] ([ContactForm_Id], [Name], [Message], [PhoneNumber], [Email], [FromWhere], [ClientUserId], [MessageDate], [IsDelete]) VALUES (3, N'kamlesh lalwani', N'd', N'9106490735', N'kamleshlalwani152@gmail.com', N'Web', 0, CAST(0x0000AB7E017AD7C5 AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[tbl_ContactFormData] OFF
SET IDENTITY_INSERT [dbo].[tbl_DistributorRequestDetails] ON 

INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (1, N'Raj', N'Thakker', N'reaj@gmail.com', N'878888788', N'Raj Traders', N'Anand', N'Gujarat', N'112121212', NULL, N'154544545', 1, CAST(0x0000AB56012E8079 AS DateTime), CAST(0x0000AB56012E8079 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (2, N'Nilesh', N'Prajapati', N'prajapati.nileshbhai@gmail.com', N'09824936252', N'Nilesh Software', N'Anand', N'Gujarat', N'12345', NULL, NULL, 1, CAST(0x0000AB7F009CF87E AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (3, N'kamlesh', N'LALWANI', N'redspeech@rediffmail.com', N'9106490735', N'UNIVERSAL INFOTECH', N'GODHRA', N'GUJARAT', N'zx12345uhdhh', NULL, NULL, 1, CAST(0x0000AB7E01787B10 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (4, N'ashwain', N'khatri', N'prakashmagnani@gmail.com', N'9824143334', N'HARI OM TREADERS', N'godhra', N'gujarat', N'123', NULL, NULL, 1, CAST(0x0000AB7F0030DB2B AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (5, N'BURHAN', N'BURHAN', N'kachwala63@gmail.com', N'9016050614', N'BURHAN', N'GODHRA', N'GUJARAT', N'123455', NULL, NULL, 1, CAST(0x0000AB8100349262 AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_DistributorRequestDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_GeneralSetting] ON 

INSERT [dbo].[tbl_GeneralSetting] ([GeneralSettingId], [InitialPointCustomer], [ShippingMessage]) VALUES (1, CAST(250.00 AS Decimal(18, 2)), N'Shipping charge will be taken later')
SET IDENTITY_INSERT [dbo].[tbl_GeneralSetting] OFF
SET IDENTITY_INSERT [dbo].[tbl_Godown] ON 

INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'ABC1', 1, 1, 1, CAST(0x0000AB8C00648E5B AS DateTime), 1, CAST(0x0000AB8C0064AC4E AS DateTime))
INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Godown 2', 1, 0, 1, CAST(0x0000AB8C0066D4AB AS DateTime), 1, CAST(0x0000AB8C006AAC38 AS DateTime))
INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Godown 1', 1, 0, 1, CAST(0x0000AB8C006A8123 AS DateTime), 1, CAST(0x0000AB8C006A9935 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Godown] OFF
SET IDENTITY_INSERT [dbo].[tbl_GSTMaster] ON 

INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (1, N'5%', CAST(5.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (2, N'12%', CAST(12.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (3, N'18%', CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (4, N'28%', CAST(28.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[tbl_GSTMaster] OFF
SET IDENTITY_INSERT [dbo].[tbl_HomeImages] ON 

INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'e3aa1a4a-eb1a-4868-8e35-1076f86be5f6-user4-128x128.jpg', N'ABC1', N'XYZ1', 1, 1, CAST(0x0000AB8B011AF39C AS DateTime), 1, CAST(0x0000AB8B01210156 AS DateTime))
INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'00120c0d-6af4-45a7-9131-45da74c20d49-user7-128x128.jpg', N'ABC', N'XYZ', 1, 1, CAST(0x0000AB8C0071F050 AS DateTime), 1, CAST(0x0000AB8C0071F050 AS DateTime))
INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'2374a71a-076d-4505-af18-8c6a465f225d-20190428101259445950437.jpg', N'Hi', N'Hello', 1, 1, CAST(0x0000AB91012E16DA AS DateTime), 1, CAST(0x0000AB91012E16DA AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_HomeImages] OFF
SET IDENTITY_INSERT [dbo].[tbl_ItemStocks] ON 

INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, 1, 1, 1, 50, 1, 0, 1, CAST(0x0000AB7E00C8D271 AS DateTime), 1, CAST(0x0000AB7E00C8D271 AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, 1, 2, 2, 3, 10, 1, 0, 1, CAST(0x0000AB7E00C8E00C AS DateTime), 1, CAST(0x0000AB7E00C8E00C AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, 3, 5, 0, 6, 100, 1, 0, 1, CAST(0x0000AB7F004FFEEF AS DateTime), 1, CAST(0x0000AB7F004FFEEF AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 4, 7, 0, 8, 50, 1, 0, 1, CAST(0x0000AB7F00804EEB AS DateTime), 1, CAST(0x0000AB7F00804EEB AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, 4, 7, 0, 8, 50, 1, 0, 1, CAST(0x0000AB7F00805C8A AS DateTime), 1, CAST(0x0000AB7F00805C8A AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, 4, 7, 0, 8, 25, 1, 0, 1, CAST(0x0000AB7F008102A7 AS DateTime), 1, CAST(0x0000AB7F008102A7 AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, 5, 10, 12, 25, 10, 1, 0, 1, CAST(0x0000AB9A00F59B13 AS DateTime), 1, CAST(0x0000AB9A00F59B13 AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, 5, 10, 12, 24, 5, 1, 0, 1, CAST(0x0000AB9A00F62301 AS DateTime), 1, CAST(0x0000AB9A00F62301 AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10008, 5, 10, 12, 24, 15, 1, 0, 1, CAST(0x0000AB9C010B9972 AS DateTime), 1, CAST(0x0000AB9C010B9973 AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10009, 5, 10, 12, 25, 12, 1, 0, 1, CAST(0x0000AB9C010BAA70 AS DateTime), 1, CAST(0x0000AB9C010BAA70 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ItemStocks] OFF
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] ON 

INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (1, 1, CAST(0x0000AB8A01825867 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (2, 1, CAST(0x0000AB8B0170EF3A AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (3, 1, CAST(0x0000AB8B0170FA19 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (4, 1, CAST(0x0000AB8B01748961 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (5, 1, CAST(0x0000AB8B0174A699 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (6, 1, CAST(0x0000AB8B017B5AB7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (7, 1, CAST(0x0000AB8B017F5996 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (8, 1, CAST(0x0000AB8B017F5993 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (9, 1, CAST(0x0000AB8C00BF0590 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (10, 1, CAST(0x0000AB8C00C15519 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (11, 1, CAST(0x0000AB8C00C4C81F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (12, 1, CAST(0x0000AB8C00C91B8C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (13, 1, CAST(0x0000AB8C011E6339 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (14, 1, CAST(0x0000AB8C01259664 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (15, 1, CAST(0x0000AB8C013081DB AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (16, 1, CAST(0x0000AB8C0136060D AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (17, 1, CAST(0x0000AB8C013C22AD AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (18, 1, CAST(0x0000AB8C0141A8AB AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (19, 1, CAST(0x0000AB8C014DB19C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20, 1, CAST(0x0000AB8C0151CDC7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (21, 1, CAST(0x0000AB8C01541863 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (22, 1, CAST(0x0000AB8C015E7A79 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (23, 1, CAST(0x0000AB8C01726595 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (24, 1, CAST(0x0000AB910187DD5E AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (25, 1, CAST(0x0000AB9101888CB7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (26, 1, CAST(0x0000AB930007374F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (27, 1, CAST(0x0000AB9300EE3F27 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (28, 1, CAST(0x0000AB9300FDB45F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (29, 1, CAST(0x0000AB930101EA5F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30, 1, CAST(0x0000AB93012FFCF8 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (31, 1, CAST(0x0000AB930130809F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (32, 1, CAST(0x0000AB930131DCDD AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (33, 1, CAST(0x0000AB930132A447 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (34, 1, CAST(0x0000AB9301370329 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (35, 1, CAST(0x0000AB9301475620 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (36, 1, CAST(0x0000AB93014936AB AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (37, 1, CAST(0x0000AB930157AE22 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (38, 1, CAST(0x0000AB930157B0E2 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (39, 1, CAST(0x0000AB9301721C33 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (40, 1, CAST(0x0000AB930174E99E AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (41, 1, CAST(0x0000AB9301796849 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (42, 1, CAST(0x0000AB93017A4D26 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (43, 1, CAST(0x0000AB93017ACE00 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (44, 1, CAST(0x0000AB93017C4ADE AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (45, 1, CAST(0x0000AB93018310E7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (46, 1, CAST(0x0000AB9301836B87 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (47, 1, CAST(0x0000AB9301857040 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (48, 1, CAST(0x0000AB93018874EF AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (49, 1, CAST(0x0000AB9301890C03 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50, 1, CAST(0x0000AB940000AF1A AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (51, 1, CAST(0x0000AB9400025F15 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (52, 1, CAST(0x0000AB940003D687 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (53, 5, CAST(0x0000AB940003EC7C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (54, 1, CAST(0x0000AB940005EA52 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (55, 1, CAST(0x0000AB9400060FEB AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (56, 5, CAST(0x0000AB9400062441 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (57, 1, CAST(0x0000AB9400B0D243 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (58, 1, CAST(0x0000AB9400B38940 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (59, 1, CAST(0x0000AB9400B434E8 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (60, 1, CAST(0x0000AB9400B8EA34 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (61, 1, CAST(0x0000AB9400B96CF2 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (62, 5, CAST(0x0000AB9400B9975F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (63, 5, CAST(0x0000AB9400BB378A AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (64, 1, CAST(0x0000AB9400BB47E1 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (65, 1, CAST(0x0000AB9400BB7AAB AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (66, 1, CAST(0x0000AB9400BBAFF3 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (67, 1, CAST(0x0000AB9400D7815B AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (68, 1, CAST(0x0000AB9400E6EFA0 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (69, 5, CAST(0x0000AB9400E755B5 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (70, 5, CAST(0x0000AB9400ED4D1F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (71, 1, CAST(0x0000AB9400ED9600 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (72, 5, CAST(0x0000AB9400EE26FC AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (73, 1, CAST(0x0000AB9400EE34DA AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (74, 5, CAST(0x0000AB9400EE62B4 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (75, 1, CAST(0x0000AB9400F36F57 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (76, 1, CAST(0x0000AB95016812F9 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (77, 1, CAST(0x0000AB9501754727 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (78, 1, CAST(0x0000AB950175EBE7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (79, 1, CAST(0x0000AB95012E38A1 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (80, 1, CAST(0x0000AB9900F50468 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (81, 1, CAST(0x0000AB990111F767 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (82, 1, CAST(0x0000AB990113EBBD AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (83, 1, CAST(0x0000AB99011636A5 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (10082, 1, CAST(0x0000AB9A004CE7B4 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20082, 1, CAST(0x0000AB9A009CF25B AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20083, 1, CAST(0x0000AB9A009E0E43 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20084, 1, CAST(0x0000AB9A00C16723 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20085, 1, CAST(0x0000AB9A00C41A77 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20086, 1, CAST(0x0000AB9A00CE445C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20087, 1, CAST(0x0000AB9A00DABED6 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20088, 1, CAST(0x0000AB9A00EC499E AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20089, 1, CAST(0x0000AB9A00F312AF AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20090, 1, CAST(0x0000AB9A00F3D5A9 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20091, 1, CAST(0x0000AB9B0099E63D AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20092, 1, CAST(0x0000AB9B00CCB57D AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20093, 1, CAST(0x0000AB9B00CEFE9A AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20094, 1, CAST(0x0000AB9B00DC989C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20095, 1, CAST(0x0000AB9B00E73A1C AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20096, 1, CAST(0x0000AB9B00E86CDF AS DateTime), N'::1', N'Login')
GO
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20097, 1, CAST(0x0000AB9B00F5222F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20098, 1, CAST(0x0000AB9B011AE8AF AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20099, 1, CAST(0x0000AB9C00F41159 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20100, 1, CAST(0x0000AB9C00FA998E AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20101, 1, CAST(0x0000AB9C010B8227 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30101, 1, CAST(0x0000AB9D0107EED9 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30102, 1, CAST(0x0000AB9D011419B2 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30103, 1, CAST(0x0000AB9D011419B2 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30104, 1, CAST(0x0000AB9D01144109 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30105, 1, CAST(0x0000AB9D01270E5D AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30106, 1, CAST(0x0000AB9D01277DA7 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30107, 1, CAST(0x0000AB9D013884A9 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (40101, 1, CAST(0x0000AB9D015A64B9 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (40102, 1, CAST(0x0000AB9E011C9832 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (40103, 1, CAST(0x0000AB9E0120C319 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50101, 1, CAST(0x0000ABA000C70C9F AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50102, 1, CAST(0x0000ABA000CA6457 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50103, 1, CAST(0x0000ABA000CD3254 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50104, 1, CAST(0x0000ABA000CD72A1 AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50105, 1, CAST(0x0000ABA000CDA20B AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (60101, 1, CAST(0x0000ABA1006A5AAA AS DateTime), N'::1', N'Login')
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] OFF
SET IDENTITY_INSERT [dbo].[tbl_Offers] ON 

INSERT [dbo].[tbl_Offers] ([OfferId], [OfferName], [CategoryId], [ProductId], [SubproductId], [ProductItemId], [OfferPrice], [OfferPriceforDistributor], [StartDate], [EndDate], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Offer Water Tank', 1, 2, 2, 3, CAST(200.00 AS Numeric(18, 2)), CAST(190.00 AS Numeric(18, 2)), CAST(0x0000AB7D00000000 AS DateTime), CAST(0x0000AB8200000000 AS DateTime), 1, 0, 1, CAST(0x0000AB7E00C7DA63 AS DateTime), 1, CAST(0x0000AB7E00C7DA63 AS DateTime))
INSERT [dbo].[tbl_Offers] ([OfferId], [OfferName], [CategoryId], [ProductId], [SubproductId], [ProductItemId], [OfferPrice], [OfferPriceforDistributor], [StartDate], [EndDate], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'HEADING', 4, 6, 0, 9, CAST(900.00 AS Numeric(18, 2)), CAST(800.00 AS Numeric(18, 2)), CAST(0x0000AB7F00000000 AS DateTime), CAST(0x0000AB8000000000 AS DateTime), 1, 0, 1, CAST(0x0000AB7F00821E03 AS DateTime), 1, CAST(0x0000AB7F00821E03 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Offers] OFF
SET IDENTITY_INSERT [dbo].[tbl_OrderItemDetails] ON 

INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (1, 1, 5, N'Orange Item 1', 5, CAST(900.00 AS Numeric(18, 2)), N'4554', CAST(686.44 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 10, CAST(0x0000AB7E017C8EA8 AS DateTime), 10, CAST(0x0000AB7E017C8EA8 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (2, 2, 9, N'SHEET COVER ITEM', 1, CAST(2900.00 AS Numeric(18, 2)), N'P3', CAST(138.10 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 6, CAST(0x0000AB7F002DC057 AS DateTime), 6, CAST(0x0000AB7F002DC057 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (3, 2, 15, N'BIB COCK', 2, CAST(1386.00 AS Numeric(18, 2)), N'ARA2103', CAST(422.84 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 6, CAST(0x0000AB7F002DC057 AS DateTime), 6, CAST(0x0000AB7F002DC057 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (4, 3, 9, N'SHEET COVER ITEM', 3, CAST(2900.00 AS Numeric(18, 2)), N'P3', CAST(414.28 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 12, CAST(0x0000AB7F0039EC51 AS DateTime), 12, CAST(0x0000AB7F0039EC51 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (5, 4, 26, N'10 gm 24 carat bis', 1, CAST(30000.00 AS Numeric(18, 2)), N'p001', CAST(4576.28 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 13, CAST(0x0000AB82001C8CC4 AS DateTime), 13, CAST(0x0000AB82001C8CC4 AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (6, 5, 24, N'BIB COCK1', 2, CAST(1014.00 AS Numeric(18, 2)), N'XO102', CAST(309.36 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00B3C16B AS DateTime), 14, CAST(0x0000AB9B00B3C16B AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (7, 5, 25, N'PILLAR COCK  NEW1', 3, CAST(1138.00 AS Numeric(18, 2)), N'XO101', CAST(520.78 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00B3C16D AS DateTime), 14, CAST(0x0000AB9B00B3C16D AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (8, 6, 25, N'PILLAR COCK  NEW1', 1, CAST(1138.00 AS Numeric(18, 2)), N'XO101', CAST(173.60 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00B6745A AS DateTime), 14, CAST(0x0000AB9B00B6745A AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (9, 6, 24, N'BIB COCK1', 1, CAST(1014.00 AS Numeric(18, 2)), N'XO102', CAST(154.68 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00B6745A AS DateTime), 14, CAST(0x0000AB9B00B6745A AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (10, 7, 25, N'PILLAR COCK  NEW1', 1, CAST(964.41 AS Numeric(18, 2)), N'XO101', CAST(164.88 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00EEA0D2 AS DateTime), 14, CAST(0x0000AB9B00EEA0D2 AS DateTime), CAST(48.22 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (11, 7, 24, N'BIB COCK1', 1, CAST(859.32 AS Numeric(18, 2)), N'XO102', CAST(146.88 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B00EEA0D3 AS DateTime), 14, CAST(0x0000AB9B00EEA0D3 AS DateTime), CAST(42.97 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (12, 8, 25, N'PILLAR COCK  NEW1', 1, CAST(964.41 AS Numeric(18, 2)), N'XO101', CAST(164.88 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B0141C77D AS DateTime), 14, CAST(0x0000AB9B0141C77D AS DateTime), CAST(48.22 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (13, 8, 24, N'BIB COCK1', 1, CAST(859.32 AS Numeric(18, 2)), N'XO102', CAST(146.88 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9B0141C77D AS DateTime), 14, CAST(0x0000AB9B0141C77D AS DateTime), CAST(42.97 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (14, 9, 25, N'PILLAR COCK  NEW1', 1, CAST(889.83 AS Numeric(18, 2)), N'XO101', CAST(160.20 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C00FE88E1 AS DateTime), 9, CAST(0x0000AB9C00FE88E1 AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (15, 12, 25, N'PILLAR COCK  NEW1', 1, CAST(889.83 AS Numeric(18, 2)), N'XO101', CAST(160.20 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C01090A7F AS DateTime), 9, CAST(0x0000AB9C01090A7F AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (16, 14, 25, N'PILLAR COCK  NEW1', 1, CAST(889.83 AS Numeric(18, 2)), N'XO101', CAST(160.20 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C0109EA1A AS DateTime), 9, CAST(0x0000AB9C0109EAA0 AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (17, 16, 25, N'PILLAR COCK  NEW1', 1, CAST(889.83 AS Numeric(18, 2)), N'XO101', CAST(160.20 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C010A99CE AS DateTime), 9, CAST(0x0000AB9C010A99CE AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (18, 18, 24, N'BIB COCK1', 1, CAST(793.22 AS Numeric(18, 2)), N'XO102', CAST(142.74 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C010BEAA7 AS DateTime), 9, CAST(0x0000AB9C010BEAA7 AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (19, 20, 24, N'BIB COCK1', 1, CAST(793.22 AS Numeric(18, 2)), N'XO102', CAST(142.74 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C010C7BA3 AS DateTime), 9, CAST(0x0000AB9C010C7BA3 AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (20, 22, 25, N'PILLAR COCK  NEW1', 1, CAST(889.83 AS Numeric(18, 2)), N'XO101', CAST(160.20 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 9, CAST(0x0000AB9C010D7DAF AS DateTime), 9, CAST(0x0000AB9C010D7DAF AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (10017, 10016, 25, N'PILLAR COCK  NEW1', 1, CAST(964.41 AS Numeric(18, 2)), N'XO101', CAST(166.86 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9D01895D36 AS DateTime), 14, CAST(0x0000AB9D01895D36 AS DateTime), CAST(37.62 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [Discount], [GSTPer]) VALUES (10018, 10017, 24, N'BIB COCK1', 2, CAST(859.32 AS Numeric(18, 2)), N'XO102', CAST(309.42 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 14, CAST(0x0000AB9D018B3FC8 AS DateTime), 14, CAST(0x0000AB9D018B3FC8 AS DateTime), CAST(0.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[tbl_OrderItemDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_Orders] ON 

INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (1, 10, CAST(4500.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 3, N'upi', 1, 0, 10, CAST(0x0000AB7E017C8E95 AS DateTime), 10, CAST(0x0000AB7E017C8E95 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_ESOAFBLOJidBKr', N'pay_ESOBtkoArnI7p3', N'6d045ac62b9caef0f362559ac7a6426bb48c037a9c879f55d9c8fbc8b9e50831', NULL, NULL, NULL, 1, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (2, 6, CAST(5672.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 2, N'upi', 1, 0, 6, CAST(0x0000AB7F002DC049 AS DateTime), 6, CAST(0x0000AB7F002DC049 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_ESRwuMTRcw5bmH', N'pay_ESRxLuzvAUbwvq', N'3261bb465b1a0e8a635b3a3644158617ecb2303b8a97d184e37f4be48e10c604', NULL, NULL, NULL, 2, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (3, 12, CAST(8700.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 1, N'ByCredit', 1, 0, 12, CAST(0x0000AB7F0039EC48 AS DateTime), 12, CAST(0x0000AB7F0039EC48 AS DateTime), CAST(3700.00 AS Decimal(18, 2)), N'3', N'', N'', NULL, NULL, NULL, 3, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (4, 13, CAST(30000.00 AS Decimal(18, 2)), N'asc - 389001', N'acsd', N'asx', NULL, N'kam asx', N'9106490735', 1, N'ByCredit', 1, 0, 13, CAST(0x0000AB82001C8CB6 AS DateTime), 13, CAST(0x0000AB82001C8CB6 AS DateTime), CAST(30000.00 AS Decimal(18, 2)), N'4', N'', N'', NULL, NULL, NULL, 4, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (5, 14, CAST(5594.16 AS Decimal(18, 2)), N'Ananf', N'Gujarat', N'Anand', NULL, N'kkrk shh', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9B00591DCD AS DateTime), 14, CAST(0x0000AB9B00591DCD AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EdSRWEiRs0Ln8B', N'pay_EdSRro46gidmeA', N'153288ed2b86298c561ef2fe5d6dad2a50f332a8f76d0ee10f8b7cbf90e6123f', NULL, NULL, NULL, 5, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (6, 14, CAST(2153.76 AS Decimal(18, 2)), N'Ananf', N'Gujarat', N'Anand', NULL, N'kkrk shh', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9B005BD138 AS DateTime), 14, CAST(0x0000AB9B005BD138 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EdSbq5MoFIuEMq', N'pay_EdScG5icuUJkUH', N'90ba0074eb890e5c9c7d03c74e84582731f4e7fdb16a146a8679fbf620658af9', NULL, NULL, NULL, 6, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (7, 14, CAST(2153.76 AS Decimal(18, 2)), N'Ananf', N'Gujarat', N'Anand', NULL, N'kkrk shh', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9B0093FD9B AS DateTime), 14, CAST(0x0000AB9B0093FD9B AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EdW5PnoTjIC5qy', N'pay_EdW6IaB54OsjKt', N'a039982ae71ed24b6c9d7f92e5ab698431182e90d85a0eacd3f0965caa7fb5b8', CAST(110.00 AS Decimal(18, 2)), 2, NULL, 7, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (8, 14, CAST(2044.39 AS Decimal(18, 2)), N'ANand', N'Gujarat', N'Shop 1, M G ROad', N'380001', N'ramesh patel', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9B00E72453 AS DateTime), 14, CAST(0x0000AB9B00E72453 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EdbFc0hWnhqFwe', N'pay_EdbG7yslWCiiJM', N'83ffd73f3a0e6b839c211767a857bb8567929c3800fb7bf5968b06114179ed30', CAST(150.00 AS Decimal(18, 2)), 2, NULL, 8, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (9, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C00FE88DD AS DateTime), 9, CAST(0x0000AB9C00FE88DD AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'9', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 9, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (10, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C00FE8975 AS DateTime), 9, CAST(0x0000AB9C00FE8975 AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'10', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 10, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (11, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C00FE8A1A AS DateTime), 9, CAST(0x0000AB9C00FE8A1A AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'11', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 11, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (12, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C01090A7D AS DateTime), 9, CAST(0x0000AB9C01090A7D AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'12', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 12, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (13, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C01090B1D AS DateTime), 9, CAST(0x0000AB9C01090B1D AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'13', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 13, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (14, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C0109A6E7 AS DateTime), 9, CAST(0x0000AB9C0109A767 AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'14', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 14, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (15, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010A2282 AS DateTime), 9, CAST(0x0000AB9C010A2282 AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'15', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 15, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (16, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010A99CB AS DateTime), 9, CAST(0x0000AB9C010A99CB AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'16', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 16, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (17, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010A9BE4 AS DateTime), 9, CAST(0x0000AB9C010A9BE4 AS DateTime), CAST(1050.00 AS Decimal(18, 2)), N'17', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 17, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (18, 9, CAST(936.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010BEAA1 AS DateTime), 9, CAST(0x0000AB9C010BEAA1 AS DateTime), CAST(936.00 AS Decimal(18, 2)), N'18', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 18, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (19, 9, CAST(936.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010BF205 AS DateTime), 9, CAST(0x0000AB9C010BF205 AS DateTime), CAST(936.00 AS Decimal(18, 2)), N'19', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 19, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (20, 9, CAST(936.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010C7BA1 AS DateTime), 9, CAST(0x0000AB9C010C7BA1 AS DateTime), CAST(936.00 AS Decimal(18, 2)), N'20', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 20, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (21, 9, CAST(936.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010CB92C AS DateTime), 9, CAST(0x0000AB9C010CB92C AS DateTime), CAST(936.00 AS Decimal(18, 2)), N'21', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 21, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (22, 9, CAST(1050.00 AS Decimal(18, 2)), N'ANand - 388770', N'Gujarat', N'Shop 1, M G ROad', N'388770', N'ramesh patel', N'8897987877', 1, N'ByCredit', 1, 0, 9, CAST(0x0000AB9C010D7DAB AS DateTime), 9, CAST(0x0000AB9C010D7DAB AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'22', N'', N'', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 22, N'2020-21')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (10016, 14, CAST(1093.61 AS Decimal(18, 2)), N'ANand', N'Gujarat', N'Shop 1, M G ROad', N'380001', N'ramesh patel', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9D012EBA06 AS DateTime), 14, CAST(0x0000AB9D012EBA06 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EeSl6rjXwSrtBA', N'pay_EeSleFPFeCelRD', N'6ea0b399dafa4a5e8ba22d2252e5447ac0a782d1b1d031f6b23f88b308b0bb58', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 23, NULL)
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [ShippingCharge], [ShippingStatus], [PointsUsed], [InvoiceNo], [InvoiceYear]) VALUES (10017, 14, CAST(2028.00 AS Decimal(18, 2)), N'ANand', N'Gujarat', N'Shop 1, M G ROad', N'380001', N'ramesh patel', N'7016232958', 1, N'upi', 1, 0, 14, CAST(0x0000AB9D01309794 AS DateTime), 14, CAST(0x0000AB9D01309811 AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_EeSqw3iD4doJHt', N'pay_EeSr13MoFuYjX4', N'4f613d2c100063c6d6e7b442fa310ac107f7ddc4366076f4d33b16016ccde69b', CAST(0.00 AS Decimal(18, 2)), 1, NULL, 24, N'2020-2021')
SET IDENTITY_INSERT [dbo].[tbl_Orders] OFF
SET IDENTITY_INSERT [dbo].[tbl_PaymentHistory] ON 

INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (1, CAST(4500.00 AS Decimal(18, 2)), CAST(4500.00 AS Decimal(18, 2)), CAST(0x0000AB7E017C8E9F AS DateTime), 1, 10, CAST(0x0000AB7E017C8E9F AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (2, CAST(5672.00 AS Decimal(18, 2)), CAST(5672.00 AS Decimal(18, 2)), CAST(0x0000AB7F002DC052 AS DateTime), 2, 6, CAST(0x0000AB7F002DC052 AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (3, CAST(5594.16 AS Decimal(18, 2)), CAST(5594.16 AS Decimal(18, 2)), CAST(0x0000AB9B00591E49 AS DateTime), 5, 14, CAST(0x0000AB9B00591E49 AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (4, CAST(2153.76 AS Decimal(18, 2)), CAST(2153.76 AS Decimal(18, 2)), CAST(0x0000AB9B005BD139 AS DateTime), 6, 14, CAST(0x0000AB9B005BD139 AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (5, CAST(2153.76 AS Decimal(18, 2)), CAST(2153.76 AS Decimal(18, 2)), CAST(0x0000AB9B0093FDAF AS DateTime), 7, 14, CAST(0x0000AB9B0093FDAF AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (6, CAST(2044.39 AS Decimal(18, 2)), CAST(2044.39 AS Decimal(18, 2)), CAST(0x0000AB9B00E7245A AS DateTime), 8, 14, CAST(0x0000AB9B00E7245A AS DateTime), N'upi', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (7, CAST(150.00 AS Decimal(18, 2)), CAST(150.00 AS Decimal(18, 2)), CAST(0x0000AB9B011AC49A AS DateTime), 8, 14, CAST(0x0000AB9B011AC49A AS DateTime), N'upi', N'order_EdeSQGs1KUDxS8', N'pay_EdeSVmz0sHv4eS', N'ffce58ae8b3ad0f1f8f79aeb6019b64184782f2c38156f764eb4e344665fd936', N'ShippingCharge')
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (8, CAST(5000.00 AS Decimal(18, 2)), CAST(8700.00 AS Decimal(18, 2)), CAST(0x0000AB9C00000000 AS DateTime), 3, 1, CAST(0x0000AB9C014EF239 AS DateTime), N'Cash', NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (9, CAST(500.00 AS Decimal(18, 2)), CAST(1050.00 AS Decimal(18, 2)), CAST(0x0000AB9C012C8BFA AS DateTime), 22, 9, CAST(0x0000AB9C012C8BFA AS DateTime), N'upi', N'order_Ee45pjJmdZnZB9', N'pay_Ee468cNKI7vaRh', N'160d545b02efe2b50e42aa895e20dbcf8bf8cc08f44893502944943980327fef', N'Order Amount')
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (10, CAST(550.00 AS Decimal(18, 2)), CAST(550.00 AS Decimal(18, 2)), CAST(0x0000AB9C012CEE8A AS DateTime), 22, 9, CAST(0x0000AB9C012CEE8A AS DateTime), N'upi', N'order_Ee47Jy5CAt7yAh', N'pay_Ee47crn7d3Litz', N'058d492ecb1ca1f63645255243dc93942e45ae4ea0afafacc2398c5e77be3a98', N'Order Amount')
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (10009, CAST(1093.61 AS Decimal(18, 2)), CAST(1093.61 AS Decimal(18, 2)), CAST(0x0000AB9D012EBA10 AS DateTime), 10016, 14, CAST(0x0000AB9D012EBA10 AS DateTime), N'upi', N'order_EeSl6rjXwSrtBA', N'pay_EeSleFPFeCelRD', N'6ea0b399dafa4a5e8ba22d2252e5447ac0a782d1b1d031f6b23f88b308b0bb58', N'OrderPayment')
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature], [PaymentFor]) VALUES (10010, CAST(2028.00 AS Decimal(18, 2)), CAST(2028.00 AS Decimal(18, 2)), CAST(0x0000AB9D01309CA3 AS DateTime), 10017, 14, CAST(0x0000AB9D01309CA4 AS DateTime), N'upi', N'order_EeSqw3iD4doJHt', N'pay_EeSr13MoFuYjX4', N'4f613d2c100063c6d6e7b442fa310ac107f7ddc4366076f4d33b16016ccde69b', N'OrderPayment')
SET IDENTITY_INSERT [dbo].[tbl_PaymentHistory] OFF
SET IDENTITY_INSERT [dbo].[tbl_PointDetails] ON 

INSERT [dbo].[tbl_PointDetails] ([PointId], [Points], [ClientUserId], [ExpiryDate], [CreatedDate], [CreatedBy], [UsedPoints]) VALUES (1, CAST(100.00 AS Decimal(18, 2)), 6, CAST(0x0000AC5100000000 AS DateTime), CAST(0x0000AB9A00000000 AS DateTime), 6, CAST(0.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_PointDetails] ([PointId], [Points], [ClientUserId], [ExpiryDate], [CreatedDate], [CreatedBy], [UsedPoints]) VALUES (3, CAST(100.00 AS Decimal(18, 2)), 14, CAST(0x0000AC5100F39F33 AS DateTime), CAST(0x0000AB9A00F39F33 AS DateTime), 14, CAST(100.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_PointDetails] ([PointId], [Points], [ClientUserId], [ExpiryDate], [CreatedDate], [CreatedBy], [UsedPoints]) VALUES (4, CAST(120.00 AS Decimal(18, 2)), 14, CAST(0x0000AC52009A04A0 AS DateTime), CAST(0x0000AB9B009A04A0 AS DateTime), 1, CAST(120.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[tbl_PointDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_ProductItemImages] ON 

INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (16, 4, N'f3adb265-e782-49b9-a339-ec6c2647ba0a-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(0x0000AB7E01265390 AS DateTime), 1, CAST(0x0000AB7E01265390 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (17, 4, N'481ae0fb-2a12-411d-9eec-9e7664006d3b-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(0x0000AB7E012654A3 AS DateTime), 1, CAST(0x0000AB7E012654A3 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (18, 5, N'fa1e464a-fc19-4c45-8ac4-96830f296ae0-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(0x0000AB7F004E73FE AS DateTime), 1, CAST(0x0000AB7F004E73FE AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (19, 5, N'd7499156-ca08-4561-8811-15192c366852-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(0x0000AB7F004E7407 AS DateTime), 1, CAST(0x0000AB7F004E7407 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (20, 6, N'414854d6-301c-4923-9563-8e1ce66f969e-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(0x0000AB7F004F8597 AS DateTime), 1, CAST(0x0000AB7F004F8597 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (21, 6, N'e38d9aed-44ee-43d2-b616-c9a3d67227b3-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(0x0000AB7F004F859C AS DateTime), 1, CAST(0x0000AB7F004F859C AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (22, 6, N'35fafa75-822c-4ccf-aafe-ce501c808617-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(0x0000AB7F004F85A0 AS DateTime), 1, CAST(0x0000AB7F004F85A0 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (23, 7, N'e0e7d6e8-1e5e-4c2d-8e20-7b30f9abed40-as.jpg', 1, 0, 1, CAST(0x0000AB7F007EDD1D AS DateTime), 1, CAST(0x0000AB7F007EDD1D AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (24, 7, N'5fb6152c-59d1-4007-be8c-b199a60d98ec-as.jpg', 1, 0, 1, CAST(0x0000AB7F007EDD27 AS DateTime), 1, CAST(0x0000AB7F007EDD27 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (25, 7, N'b0994802-1d9d-4766-a1c1-5fed21790557-as.jpg', 1, 0, 1, CAST(0x0000AB7F007EDD27 AS DateTime), 1, CAST(0x0000AB7F007EDD27 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (26, 7, N'1cf3dc44-65c5-4c98-93a1-91d804c70efc-as.jpg', 1, 0, 1, CAST(0x0000AB7F007EDD30 AS DateTime), 1, CAST(0x0000AB7F007EDD30 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (27, 8, N'2549d05c-c56d-4e75-ab2c-d5791343ba83-mbl.png', 1, 0, 1, CAST(0x0000AB7F007FAF1F AS DateTime), 1, CAST(0x0000AB7F007FAF1F AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (28, 8, N'7f276bd5-a6a2-42a0-b298-51b415831347-mbl.png', 1, 0, 1, CAST(0x0000AB7F007FAF23 AS DateTime), 1, CAST(0x0000AB7F007FAF23 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (29, 8, N'c20680e6-9b90-4d44-9d08-802c69bf3838-mbl.png', 1, 0, 1, CAST(0x0000AB7F007FAF23 AS DateTime), 1, CAST(0x0000AB7F007FAF23 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (30, 9, N'3d7197b6-8d20-4377-8630-ae0e17c898fc-turmeric.jpg', 1, 0, 1, CAST(0x0000AB7F0080081C AS DateTime), 1, CAST(0x0000AB7F0080081C AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (31, 10, N'45a8bc9d-eb95-446e-93b2-84d4836e946b-3.jpg', 1, 0, 1, CAST(0x0000AB7F008B98AA AS DateTime), 1, CAST(0x0000AB7F008B98AA AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (32, 11, N'98f0853b-6204-4c9c-aa25-1b01fab202f3-3.jpg', 1, 0, 1, CAST(0x0000AB7F008E8DCF AS DateTime), 1, CAST(0x0000AB7F008E8DCF AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (33, 12, N'a9a16837-2912-46e8-ba4f-06853211dcd7-favicon.ico', 1, 0, 1, CAST(0x0000AB7F008F139B AS DateTime), 1, CAST(0x0000AB7F008F139B AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (34, 13, N'f0678724-2ee1-4321-bcfa-5e04c1acedeb-ext.jpg', 1, 0, 1, CAST(0x0000AB7F00903CE1 AS DateTime), 1, CAST(0x0000AB7F00903CE1 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (35, 14, N'35d67d77-ab4e-4838-978e-45d4873bf8e9-1.jpg', 1, 0, 1, CAST(0x0000AB7F00914E00 AS DateTime), 1, CAST(0x0000AB7F00914E00 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (36, 15, N'53676158-b8fa-40c3-a913-aad92a7fecf4-mbl.png', 1, 0, 1, CAST(0x0000AB7F00920171 AS DateTime), 1, CAST(0x0000AB7F00920171 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (37, 16, N'cccc289d-65d6-4fe9-96f3-2943d0c1898d-3.jpg', 1, 0, 1, CAST(0x0000AB7F009286D6 AS DateTime), 1, CAST(0x0000AB7F009286D6 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (38, 17, N'e7ac4b93-4eae-4ea7-b2fc-62398f482188-master.jpg', 1, 0, 1, CAST(0x0000AB7F009390F7 AS DateTime), 1, CAST(0x0000AB7F009390F7 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (39, 18, N'd34b189e-ef73-4176-8781-1c9e26fa2f59-te1.jpg', 1, 0, 1, CAST(0x0000AB7F009476FB AS DateTime), 1, CAST(0x0000AB7F009476FB AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (40, 19, N'279c2ade-7145-4a2a-ad8c-cbf99d809c99-ext.jpg', 1, 0, 1, CAST(0x0000AB7F00952EAC AS DateTime), 1, CAST(0x0000AB7F00952EAC AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (41, 20, N'7c90d588-a58c-4052-8ff5-0e0debc3845a-images.jpg', 1, 0, 1, CAST(0x0000AB7F0095F21F AS DateTime), 1, CAST(0x0000AB7F0095F21F AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (42, 21, N'586017d7-970d-4ee9-9752-5cef14f6d85c-images.jpg', 1, 0, 1, CAST(0x0000AB7F009600C5 AS DateTime), 1, CAST(0x0000AB7F009600C5 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (43, 22, N'a706b54a-af28-4840-b96f-9b8564da15d3-3.jpg', 1, 0, 1, CAST(0x0000AB7F0096AF99 AS DateTime), 1, CAST(0x0000AB7F0096AF99 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (44, 23, N'a4843db5-d098-4acf-80cf-f35ef4ce02ac-823x350xgst-1-823x350.jpg.pagespeed.ic.QE-r9hvrwL.jpg', 1, 0, 1, CAST(0x0000AB7F00973F1E AS DateTime), 1, CAST(0x0000AB7F00973F1E AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (45, 24, N'be1ef08a-20fc-4f9e-8035-9d128caad93b-ext.jpg', 1, 0, 1, CAST(0x0000AB7F0097D771 AS DateTime), 1, CAST(0x0000AB7F0097D771 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (46, 25, N'd969e377-10c8-4376-957d-9cddd367e041-3.jpg', 1, 0, 1, CAST(0x0000AB7F00991108 AS DateTime), 1, CAST(0x0000AB7F00991108 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (47, 26, N'4438ca7c-2d97-45df-a319-7e58a7bc564b-ext.jpg', 1, 0, 1, CAST(0x0000AB8100A68C96 AS DateTime), 1, CAST(0x0000AB8100A68C96 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (48, 26, N'b0dfc33c-e34d-44c2-b496-79aa81218ddc-ext.jpg', 1, 0, 1, CAST(0x0000AB8100A68CA0 AS DateTime), 1, CAST(0x0000AB8100A68CA0 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (49, 26, N'fc8538ff-1a7f-42a6-9496-eb5825414ee5-ext.jpg', 1, 0, 1, CAST(0x0000AB8100A68CA4 AS DateTime), 1, CAST(0x0000AB8100A68CA4 AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (50, 26, N'dd6ea484-bb4e-4408-accb-f765f27cd8d5-ext.jpg', 1, 0, 1, CAST(0x0000AB8100A68CA4 AS DateTime), 1, CAST(0x0000AB8100A68CA4 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ProductItemImages] OFF
SET IDENTITY_INSERT [dbo].[tbl_ProductItems] ON 

INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (1, 1, 1, 1, N'Abcd Sanitory 11 ', N'sample description for sanitaty item', CAST(300.00 AS Decimal(18, 2)), CAST(230.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, N'13d507c7-5a4a-45aa-9163-b59b8f7714d9-sanitary1.jpg', N'Test', N'ITM444', 1, 1, 0, 1, CAST(0x0000AB7E00C55AF2 AS DateTime), 1, CAST(0x0000AB9A009D23D5 AS DateTime), CAST(0.00 AS Decimal(18, 2)), NULL, 3, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (2, 1, 1, 1, N'Item2 Test', N'Test Item Tank', CAST(560.00 AS Decimal(18, 2)), CAST(480.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), NULL, NULL, N'Sample text', N'i444', 0, 1, 0, 1, CAST(0x0000AB7E00C5A845 AS DateTime), 1, CAST(0x0000AB7E00C5A845 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (3, 1, 2, 2, N'Item2 PPPPPPP', N'Test Item Tank desccc', CAST(350.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), CAST(330.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, N'Test', N'ITM441099', 1, 1, 0, 1, CAST(0x0000AB7E00C78987 AS DateTime), 1, CAST(0x0000AB7E00C78987 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (4, 1, 1, 1, N'item aaaa', N'aaaa item desc', CAST(75.00 AS Decimal(18, 2)), CAST(55.00 AS Decimal(18, 2)), CAST(60.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), NULL, N'42a7f614-e82e-4988-94dd-b469cfad6732-7af1ea40865febba6e3eb51e1582e78b.jpg', N'this is notification text', N'456', 1, 1, 0, 1, CAST(0x0000AB7E012651D9 AS DateTime), 1, CAST(0x0000AB7E0134E7F8 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (5, 2, 3, 3, N'Orange Item 1', N'''Orange is the colour between yellow and red on the spectrum of visible light. Human eyes perceive orange when observing light with a dominant wavelength between roughly 585 and 620 nanometres.', CAST(1000.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(900.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'33272a60-5f3e-4c11-9ab1-97e5c42b26e2-7af1ea40865febba6e3eb51e1582e78b.jpg', N'you will get this after 2 days', N'4554', 1, 1, 0, 1, CAST(0x0000AB7F004E73F0 AS DateTime), 1, CAST(0x0000AB7F004E73F0 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (6, 3, 5, 0, N'Solar Panel', N'this is description of Solar Panel', CAST(2000.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), CAST(1900.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'14cd47ed-e6d0-4c6d-9cfe-bcb82be0bbf6-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', N'this will get in 5 days', N'8721', 0, 1, 0, 1, CAST(0x0000AB7F004F858E AS DateTime), 1, CAST(0x0000AB7F004F858E AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (7, 4, 7, 0, N'PILLER COCK', N'Polytetramethylene Terephthalate (PTMT) PTMT is a thermoplastic, further classified as a polyester plastic. The graph bars on the material properties cards below compare PTMT to: polyester plastics (t', CAST(1000.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(900.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'9fb60919-a069-4a1d-ba88-1b11cc77c80d-as.jpg', N'AS', N'P1', 1, 1, 0, 1, CAST(0x0000AB7F007EDD14 AS DateTime), 1, CAST(0x0000AB7F007EDD14 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (8, 4, 7, 0, N'SUPER', N'DETILS', CAST(2000.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), CAST(1900.00 AS Decimal(18, 2)), CAST(9.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'39062b6f-f1a5-44d3-962a-9cd5c2608fdf-mbl.png', N'NOTIFACTION', N'P2', 0, 1, 0, 1, CAST(0x0000AB7F007FAF1A AS DateTime), 1, CAST(0x0000AB7F007FAF1A AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (9, 4, 6, 0, N'SHEET COVER ITEM', N'DESC', CAST(3000.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), CAST(2900.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'c1ba161c-4126-41ef-b5ca-5ed99b9bdbea-turmeric.jpg', N'STOCK', N'P3', 0, 1, 0, 1, CAST(0x0000AB7F00800817 AS DateTime), 1, CAST(0x0000AB7F00800817 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (10, 5, 8, 4, N'BIB COCK ', N'WALL MAOUNT', CAST(252.00 AS Decimal(18, 2)), CAST(167.00 AS Decimal(18, 2)), CAST(189.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'9177d62c-7324-48f2-a230-82950b3e4172-3.jpg', N'STOCK', N'ED950', 0, 1, 0, 1, CAST(0x0000AB7F008B97FC AS DateTime), 1, CAST(0x0000AB7F008B97FC AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (11, 5, 8, 4, N'PILLAR COCK  NEW', N'FLOOR MOUNT', CAST(324.00 AS Decimal(18, 2)), CAST(214.00 AS Decimal(18, 2)), CAST(243.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e03a4a23-25e1-4178-b71c-3a5695af2e96-3.jpg', N'NOTIFACTION', N'ED952', 0, 1, 0, 1, CAST(0x0000AB7F008E8DC6 AS DateTime), 1, CAST(0x0000AB7F008E8DC6 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (12, 5, 8, 5, N'BIB COCK', N'WALL MAOUNT', CAST(252.00 AS Decimal(18, 2)), CAST(167.00 AS Decimal(18, 2)), CAST(189.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'8a1de2f8-3d63-4db0-8361-adca4c3eac82-favicon.ico', N'STOCK', N'ED980', 0, 1, 0, 1, CAST(0x0000AB7F008F1397 AS DateTime), 1, CAST(0x0000AB7F008F1397 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (13, 5, 8, 5, N'PILLER COCK', N'FLOOR MOUNT', CAST(324.00 AS Decimal(18, 2)), CAST(214.00 AS Decimal(18, 2)), CAST(243.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'a7e1ca35-7589-4dad-b645-e2a88ac6794e-ext.jpg', N'NOTIFACTION', N'EP982', 0, 1, 0, 1, CAST(0x0000AB7F00903CDC AS DateTime), 1, CAST(0x0000AB7F00903CDC AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (14, 5, 8, 6, N'BIB COCK', N'WALL MAOUNT', CAST(292.00 AS Decimal(18, 2)), CAST(193.00 AS Decimal(18, 2)), CAST(219.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'a486978e-a1d7-4182-9e85-ee7ca540b0d5-1.jpg', N'NOTIFACTION', N'SUP591', 0, 1, 0, 1, CAST(0x0000AB7F00914DFB AS DateTime), 1, CAST(0x0000AB7F00914DFB AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (15, 5, 9, 7, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1980.00 AS Decimal(18, 2)), CAST(1287.00 AS Decimal(18, 2)), CAST(1386.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'636b5775-e2be-4456-a591-210b06241a87-mbl.png', N'NOTIFACTION', N'ARA2103', 0, 1, 0, 1, CAST(0x0000AB7F0092016D AS DateTime), 1, CAST(0x0000AB7F0092016D AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (16, 5, 9, 7, N'PILLER COCK ', N'FLOOR MOUNT (CP BRASS)', CAST(2190.00 AS Decimal(18, 2)), CAST(1424.00 AS Decimal(18, 2)), CAST(1533.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'db14e602-f3f1-41cd-876f-8e4ed9f2545b-3.jpg', N'NOTIFACTION', N'ARA1101', 0, 1, 0, 1, CAST(0x0000AB7F009286D1 AS DateTime), 1, CAST(0x0000AB7F009286D1 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (17, 5, 9, 8, N'PILLER COCK', N'FLOOR MOUNT (CP BRASS)', CAST(2245.00 AS Decimal(18, 2)), CAST(1460.00 AS Decimal(18, 2)), CAST(1571.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'f2390f5e-62f1-4e1e-a8c2-72ba3c60dba0-master.jpg', N'NOTIFACTION', N'CFT1101', 0, 1, 0, 1, CAST(0x0000AB7F009390F2 AS DateTime), 1, CAST(0x0000AB86012E277D AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (18, 5, 9, 9, N'PILLAR COCK  NEW', N'FLOOR MOUNT (CP BRASS)', CAST(1880.00 AS Decimal(18, 2)), CAST(1222.00 AS Decimal(18, 2)), CAST(1316.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'd0033fd9-47ad-4876-8e6d-67e798656c83-te1.jpg', N'NOTIFACTION', N'WAV 1101', 0, 1, 0, 1, CAST(0x0000AB7F009476FB AS DateTime), 1, CAST(0x0000AB7F009476FB AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (19, 5, 9, 9, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1865.00 AS Decimal(18, 2)), CAST(1212.00 AS Decimal(18, 2)), CAST(1305.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'6c1b6107-5ae2-4885-a6d0-e0248107309d-ext.jpg', N'NOTIFACTION', N'WAV2103', 0, 1, 0, 1, CAST(0x0000AB7F00952EA3 AS DateTime), 1, CAST(0x0000AB7F00952EA3 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (20, 5, 9, 8, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(2070.00 AS Decimal(18, 2)), CAST(1345.00 AS Decimal(18, 2)), CAST(1449.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e24cbcf7-0cd5-42d0-8f21-f615060f3e02-images.jpg', N'NOTIFACTION', N'CFT2103', 0, 1, 0, 1, CAST(0x0000AB7F0095F215 AS DateTime), 1, CAST(0x0000AB7F0095F215 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (21, 5, 9, 8, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(2070.00 AS Decimal(18, 2)), CAST(1345.00 AS Decimal(18, 2)), CAST(1449.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'262816d7-69e9-49ad-83bf-9f02221abfd1-images.jpg', N'NOTIFACTION', N'CFT2103', 0, 1, 0, 1, CAST(0x0000AB7F009600C0 AS DateTime), 1, CAST(0x0000AB7F009600C0 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (22, 5, 10, 10, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1630.00 AS Decimal(18, 2)), CAST(978.00 AS Decimal(18, 2)), CAST(1060.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'0f4238eb-49e9-4722-b3a8-71c9c6d93ead-3.jpg', N'NOTIFACTION', N'TY102', 0, 1, 0, 1, CAST(0x0000AB7F0096AF95 AS DateTime), 1, CAST(0x0000AB7F0096AF95 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (23, 5, 10, 10, N'PILLAR COCK  NEW', N'FLOOR MOUNT (CP BRASS)', CAST(1910.00 AS Decimal(18, 2)), CAST(1146.00 AS Decimal(18, 2)), CAST(1241.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'ec2c3e1a-91a4-43f1-9166-e2bee0381b51-823x350xgst-1-823x350.jpg.pagespeed.ic.QE-r9hvrwL.jpg', N'NOTIFACTION', N'TY101', 0, 1, 0, 1, CAST(0x0000AB7F00973F19 AS DateTime), 1, CAST(0x0000AB7F00973F19 AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (24, 5, 10, 12, N'BIB COCK1', N'WALL MAOUNT (CP BRASS)', CAST(1560.00 AS Decimal(18, 2)), CAST(936.00 AS Decimal(18, 2)), CAST(1014.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e357aea4-ce2c-4963-a82c-4f5319c88a58-ext.jpg', N'NOTIFACTION', N'XO102', 0, 1, 0, 1, CAST(0x0000AB7F0097D76D AS DateTime), 1, CAST(0x0000AB9A00F671B4 AS DateTime), CAST(60.00 AS Decimal(18, 2)), NULL, 2, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (25, 5, 10, 12, N'PILLAR COCK  NEW1', N'FLOOR MOUNT (CP BRASS)', CAST(1750.00 AS Decimal(18, 2)), CAST(1050.00 AS Decimal(18, 2)), CAST(1138.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'149526b8-202f-4c80-819a-40026fcd90d6-3.jpg', N'NOTIFACTION', N'XO101', 0, 1, 0, 1, CAST(0x0000AB7F009910FA AS DateTime), 1, CAST(0x0000AB9D01392679 AS DateTime), CAST(50.00 AS Decimal(18, 2)), NULL, 3, N'HSN110')
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags], [GodownId], [HSNCode]) VALUES (26, 7, 11, 15, N'10 gm 24 carat bis', N'Want to enjoy a truly immersive gaming experience on a gaming machine that has striking good looks? Get your hands on the ROG Strix GL553. It comes with Windows 10 pre-installed. The 7th-generation Intel Core i7 processor and the discrete NVIDIA GeForce GTX 1050 graphics card. ROG Strix GL553 is specifically made for gaming and primed for creativity.', CAST(50000.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'032e1af3-8eb3-4e21-912f-f939306a86b0-ext.jpg', N'OUT OF STOCK', N'p001', 1, 1, 0, 1, CAST(0x0000AB8100A68C88 AS DateTime), 1, CAST(0x0000AB9A009D70FA AS DateTime), CAST(0.00 AS Decimal(18, 2)), NULL, 2, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ProductItems] OFF
SET IDENTITY_INSERT [dbo].[tbl_Products] ON 

INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Sanitory 1', 1, N'ed6b4fee-b8d1-4e2f-a7d2-e4398bfa33ed-sanitaty2.jpg', 1, 0, 1, CAST(0x0000AB7E00C34D44 AS DateTime), 1, CAST(0x0000AB7E00C35E9B AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Watertanks', 1, NULL, 1, 0, 1, CAST(0x0000AB7E00C3971D AS DateTime), 1, CAST(0x0000AB7E00C3971D AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Orange', 2, N'6446839b-7003-40fa-a42c-02fbbd6f7f5e-user7-128x128.jpg', 1, 0, 1, CAST(0x0000AB7F004C3720 AS DateTime), 1, CAST(0x0000AB7F004C3720 AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'Red', 2, N'a160fbc8-576d-424d-b14d-797c414cd1a6-user8-128x128.jpg', 1, 0, 1, CAST(0x0000AB7F004C568C AS DateTime), 1, CAST(0x0000AB7F004C568C AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'Abcd', 3, N'd6e5300f-a0df-4ba1-9867-1865c1702daf-photo3.jpg', 1, 0, 1, CAST(0x0000AB7F004CBF5B AS DateTime), 1, CAST(0x0000AB7F004CBF5B AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'WC SHEET COVER', 4, N'8f7ba049-6a9c-489f-acd7-8758b31a301e-2.jpg', 1, 0, 1, CAST(0x0000AB7F007C13F4 AS DateTime), 1, CAST(0x0000AB7F007C13F4 AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'BIB COCK PTMT', 4, N'889cde85-a40f-4a74-8870-a8d9350363d7-3.jpg', 1, 0, 1, CAST(0x0000AB7F007C4D98 AS DateTime), 1, CAST(0x0000AB7F007C4D98 AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'ELEGANT PTMT FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(0x0000AB7F0085D32D AS DateTime), 1, CAST(0x0000AB7F0085D32D AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'PLUMBER FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(0x0000AB7F0085EDFC AS DateTime), 1, CAST(0x0000AB7F0085EDFC AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'V-MAC FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(0x0000AB7F00860E3F AS DateTime), 1, CAST(0x0000AB7F00860E3F AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'PLATINUM', 7, N'ecfbe346-993a-4179-bcd9-dc362c589f4b-1.jpg', 1, 0, 1, CAST(0x0000AB8100A4B32B AS DateTime), 1, CAST(0x0000AB8100A4B32B AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'ROSE GOLD', 7, N'112ecfec-b1fc-49fc-b1a6-4c162c7fbc3f-3.jpg', 1, 0, 1, CAST(0x0000AB8100A4C660 AS DateTime), 1, CAST(0x0000AB8100A4C660 AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Products] OFF
SET IDENTITY_INSERT [dbo].[tbl_SubProducts] ON 

INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Xyz Sanitory', 1, 1, N'9345abb9-bea9-48b5-8a27-0b137abd6836-Shower2.jpg', 1, 0, 1, CAST(0x0000AB7E00C3B9C6 AS DateTime), 1, CAST(0x0000AB7E00C3B9C6 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Orange Water Tank', 1, 2, NULL, 1, 0, 1, CAST(0x0000AB7E00C4EFAE AS DateTime), 1, CAST(0x0000AB7E00C4EFAE AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Orange 1', 2, 3, N'29dd06de-4f2f-4cbb-b775-ce05532d120d-avatar5.png', 1, 0, 1, CAST(0x0000AB7F004D3B44 AS DateTime), 1, CAST(0x0000AB7F004D3B44 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'EDGE PTMT FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(0x0000AB7F008636A9 AS DateTime), 1, CAST(0x0000AB7F008636A9 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'EDGE PRIME PTMT FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(0x0000AB7F00865588 AS DateTime), 1, CAST(0x0000AB7F00865588 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'SUPERB FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(0x0000AB7F008677F4 AS DateTime), 1, CAST(0x0000AB7F008677F4 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'AURA FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(0x0000AB7F0086955D AS DateTime), 1, CAST(0x0000AB7F0086955D AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'CHORUS F FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(0x0000AB7F0086B44E AS DateTime), 1, CAST(0x0000AB7F0086B44E AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'WAVE FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(0x0000AB7F0086E0F7 AS DateTime), 1, CAST(0x0000AB7F0086E0F7 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'TROY FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(0x0000AB7F008704BF AS DateTime), 1, CAST(0x0000AB7F008704BF AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'PHILO FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(0x0000AB7F008727E2 AS DateTime), 1, CAST(0x0000AB7F008727E2 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'XERO FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(0x0000AB7F00874168 AS DateTime), 1, CAST(0x0000AB7F00874168 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (13, N'ORCHID FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(0x0000AB7F0087A95B AS DateTime), 1, CAST(0x0000AB7F0087A95B AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (14, N'PALM FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(0x0000AB7F0087C55F AS DateTime), 1, CAST(0x0000AB7F0087C55F AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (15, N'10GM', 7, 11, N'34b1ffd0-9586-4e1a-9c9b-5005eacef30f-dm.jpg', 1, 0, 1, CAST(0x0000AB8100A526B0 AS DateTime), 1, CAST(0x0000AB8100A526B0 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (16, N'20GM', 7, 11, N'1b08594a-3127-46c8-b669-af2acf7affe1-ext.jpg', 1, 0, 1, CAST(0x0000AB8100A542C2 AS DateTime), 1, CAST(0x0000AB8100A542C2 AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (17, N'25GM', 7, 12, N'5855dc1b-ed80-472b-9720-412ac3e6369e-te1.jpg', 1, 0, 1, CAST(0x0000AB8100A5563E AS DateTime), 1, CAST(0x0000AB8100A5563E AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_SubProducts] OFF
ALTER TABLE [dbo].[tbl_AdminRolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_tbl_AdminRolePermissions_tbl_AdminRoleModules] FOREIGN KEY([AdminRoleModuleId])
REFERENCES [dbo].[tbl_AdminRoleModules] ([AdminRoleModuleId])
GO
ALTER TABLE [dbo].[tbl_AdminRolePermissions] CHECK CONSTRAINT [FK_tbl_AdminRolePermissions_tbl_AdminRoleModules]
GO
ALTER TABLE [dbo].[tbl_AdminRolePermissions]  WITH CHECK ADD  CONSTRAINT [FK_tbl_AdminRolePermissions_tbl_AdminRolePermissions1] FOREIGN KEY([AdminRoleId])
REFERENCES [dbo].[tbl_AdminRoles] ([AdminRoleId])
GO
ALTER TABLE [dbo].[tbl_AdminRolePermissions] CHECK CONSTRAINT [FK_tbl_AdminRolePermissions_tbl_AdminRolePermissions1]
GO
USE [master]
GO
ALTER DATABASE [DB_krupagallery] SET  READ_WRITE 
GO
