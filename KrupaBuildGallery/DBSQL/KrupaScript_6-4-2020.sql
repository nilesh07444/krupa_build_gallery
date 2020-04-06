USE [master]
GO
/****** Object:  Database [DB_krupagallery]    Script Date: 06/04/2020 8:58:35 PM ******/
CREATE DATABASE [DB_krupagallery]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'DB_krupagallery', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\DB_krupagallery.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'DB_krupagallery_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\DB_krupagallery_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [DB_krupagallery] SET COMPATIBILITY_LEVEL = 140
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
ALTER DATABASE [DB_krupagallery] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [DB_krupagallery] SET  MULTI_USER 
GO
ALTER DATABASE [DB_krupagallery] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [DB_krupagallery] SET DB_CHAINING OFF 
GO
ALTER DATABASE [DB_krupagallery] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [DB_krupagallery] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [DB_krupagallery] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [DB_krupagallery] SET QUERY_STORE = OFF
GO
USE [DB_krupagallery]
GO
/****** Object:  Table [dbo].[tbl_AdminRoleModules]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_AdminRolePermissions]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_AdminRoles]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_AdminUsers]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_Cart]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_Categories]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ClientOtherDetails]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ClientRoles]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ClientUsers]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ContactFormData]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_DistributorRequestDetails]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_Godown]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_GSTMaster]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_HomeImages]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ItemStocks]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_Itemtext_master]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_LoginHistory]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_Offers]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_OrderItemDetails]    Script Date: 06/04/2020 8:58:35 PM ******/
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
 CONSTRAINT [PK_Tbl_OrderItemDetails] PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Orders]    Script Date: 06/04/2020 8:58:35 PM ******/
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
 CONSTRAINT [PK_Tbl_Orders] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_PaymentHistory]    Script Date: 06/04/2020 8:58:35 PM ******/
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
 CONSTRAINT [PK_tbl_PaymentHistory] PRIMARY KEY CLUSTERED 
(
	[PaymentHistory_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ProductItemImages]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_ProductItems]    Script Date: 06/04/2020 8:58:35 PM ******/
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
 CONSTRAINT [PK_Tbl_ProductItems] PRIMARY KEY CLUSTERED 
(
	[ProductItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Products]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_SubProducts]    Script Date: 06/04/2020 8:58:35 PM ******/
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
/****** Object:  Table [dbo].[tbl_WishList]    Script Date: 06/04/2020 8:58:35 PM ******/
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

INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Super Admin', NULL, 1, 0, -1, CAST(N'2020-02-02T00:00:00.000' AS DateTime), -1, CAST(N'2020-02-02T00:00:00.000' AS DateTime))
INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Staff', N'This is staff role for few module rights', 1, 0, 1, CAST(N'2020-03-28T14:03:03.480' AS DateTime), 1, CAST(N'2020-04-05T05:53:27.573' AS DateTime))
INSERT [dbo].[tbl_AdminRoles] ([AdminRoleId], [AdminRoleName], [AdminRoleDescription], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Manager', NULL, 1, 0, 1, CAST(N'2020-03-28T15:01:39.123' AS DateTime), 1, CAST(N'2020-04-05T08:57:45.833' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_AdminRoles] OFF
SET IDENTITY_INSERT [dbo].[tbl_AdminUsers] ON 

INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, N'Nilesh', N'Prajapati', N'prajapati.nileshbhai@gmail.com', N'12345', N'9824936252', N'9033550453', N'100 foot road,, Nani khodiyar,', N'Anand', N'Sr. Asp.net Developer', CAST(N'1989-10-04T00:00:00.000' AS DateTime), CAST(N'2020-04-04T00:00:00.000' AS DateTime), N'AB+', N'10 AM to 7 PM', N'1234 5678 9123', CAST(N'2020-05-31T00:00:00.000' AS DateTime), N'This is super admin user', N'avatar04-210a10e7-205f-4e1c-9da3-382dd4b72db5.png', 1, 0, -1, CAST(N'2020-02-02T00:00:00.000' AS DateTime), 1, CAST(N'2020-04-05T11:04:50.563' AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 1, N'Kamlesh', N'Lalwani', N'kamleshlalwani152@gmail.com', N'12345', N'9033550453', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, -1, CAST(N'2020-03-04T00:00:00.000' AS DateTime), -1, CAST(N'2020-03-04T00:00:00.000' AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, 3, N'Vanita', N'Prajapati', N'vanita007444@gmail.com', N'Admin@123', N'7575036252', N'9033550453', N'100 foot road,, Nani khodiyar,', N'Anand', N'Staff', CAST(N'1991-06-12T00:00:00.000' AS DateTime), CAST(N'2020-04-04T00:00:00.000' AS DateTime), N'O+', N'10 AM to 7 PM', N'0000 1111 2222 3333', NULL, N'testing', N'user2-160x160-e7478cd6-6bd9-4300-b4b4-679580240d19.jpg', 1, 0, 1, CAST(N'2020-04-04T23:05:14.467' AS DateTime), 1, CAST(N'2020-04-05T14:26:56.450' AS DateTime))
INSERT [dbo].[tbl_AdminUsers] ([AdminUserId], [AdminRoleId], [FirstName], [LastName], [Email], [Password], [MobileNo], [AlternateMobile], [Address], [City], [Designation], [Dob], [DateOfJoin], [BloodGroup], [WorkingTime], [AdharCardNo], [DateOfIdCardExpiry], [Remarks], [ProfilePicture], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, 2, N'test', N'test', N'test@gmail.com', N'9033550453', N'9999999999', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 1, 1, CAST(N'2020-04-04T23:52:08.187' AS DateTime), 1, CAST(N'2020-04-04T18:22:32.850' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_AdminUsers] OFF
SET IDENTITY_INSERT [dbo].[tbl_Cart] ON 

INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (15, 4, 1, N'98c308ae-104f-432a-80e5-78629dc8a3c2', 9, CAST(N'2020-03-15T12:05:24.590' AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (20, 1, 1, N'c4f19e00-6719-4699-aa0a-983a81558648', 0, CAST(N'2020-03-15T02:56:54.970' AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (21, 4, 3, N'c4f19e00-6719-4699-aa0a-983a81558648', 0, CAST(N'2020-03-15T02:57:13.330' AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (24, 17, 28, N'257cf5f6-20eb-4799-8c62-c84a714b3e32', 12, CAST(N'2020-03-15T03:33:03.840' AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (40, 3, 3, N'560c6e4b-19c8-411e-bf6c-c38cecbf1fa7', 1, CAST(N'2020-03-16T10:38:07.320' AS DateTime))
INSERT [dbo].[tbl_Cart] ([Cart_Id], [CartItemId], [CartItemQty], [CartSessionId], [ClientUserId], [CreatedDate]) VALUES (44, 26, 3, N'268e0d70-4853-4c9c-bfa2-dd0a179552c7', 6, CAST(N'2020-03-17T03:08:11.363' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Cart] OFF
SET IDENTITY_INSERT [dbo].[tbl_Categories] ON 

INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Catergory1', NULL, 1, 0, 1, CAST(N'2020-03-14T11:49:40.690' AS DateTime), 1, CAST(N'2020-03-14T11:49:40.690' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Color', N'9edffaa3-94a4-4bfd-a8c8-5e59420fda0f-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(N'2020-03-15T04:35:16.167' AS DateTime), 1, CAST(N'2020-03-15T04:35:40.870' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Water Tank', N'3209236e-695e-40ae-a4c7-b20df3d10722-asian-paints-blue-colour-500x500.png', 1, 0, 1, CAST(N'2020-03-15T04:36:44.400' AS DateTime), 1, CAST(N'2020-03-15T07:28:16.317' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'PTMP FAUCET', N'5e58ae27-54d9-4437-8e2c-96e3b82a60ce-1.jpg', 1, 0, 1, CAST(N'2020-03-15T07:30:21.817' AS DateTime), 1, CAST(N'2020-03-15T07:33:57.990' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'FAUCETS NEW', NULL, 1, 0, 1, CAST(N'2020-03-15T08:06:40.373' AS DateTime), 1, CAST(N'2020-03-15T08:06:40.373' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'Nilesh test', N'ca79335c-383e-45b0-9afa-6066607a5422-IMG-20200313-WA0008.jpg', 0, 0, 1, CAST(N'2020-03-15T15:21:09.600' AS DateTime), 1, CAST(N'2020-03-15T15:23:00.900' AS DateTime))
INSERT [dbo].[tbl_Categories] ([CategoryId], [CategoryName], [CategoryImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'GOLD', N'aee373b3-c111-44db-bb59-77856cd128eb-prehistorich.jpg', 1, 0, 1, CAST(N'2020-03-17T09:57:41.233' AS DateTime), 1, CAST(N'2020-03-17T09:57:41.233' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Categories] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientOtherDetails] ON 

INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (1, 1, NULL, NULL, NULL, N'Anand', N'Gujarat', N'B-101,Laksh Residency', NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (2, 2, NULL, NULL, NULL, N'Vadodara', N'Gujarat', N'A101,Prime Buglows', NULL, NULL, NULL, NULL, NULL, 1, 0, NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10003, 3, NULL, NULL, NULL, N'Nadiad', N'Gujarat', N'A5,Res Houese', NULL, NULL, NULL, CAST(5000.00 AS Numeric(18, 2)), NULL, 1, 0, NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10004, 4, NULL, NULL, NULL, N'Godhra', N'Gujarat', N'K Nivas,MG road', NULL, NULL, NULL, CAST(4000.00 AS Numeric(18, 2)), CAST(1200.00 AS Numeric(18, 2)), 1, 0, NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10005, 5, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 5, CAST(N'2020-03-10T12:33:26.893' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10006, 6, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 6, CAST(N'2020-03-11T02:20:16.303' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10007, 7, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 7, CAST(N'2020-03-11T02:23:39.447' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10008, 8, N'112121212', NULL, N'154544545', N'Anand', N'Gujarat', NULL, NULL, NULL, NULL, CAST(10000.00 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 1, CAST(N'2020-03-15T09:33:56.527' AS DateTime), 1, CAST(N'2020-03-15T09:33:56.527' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10009, 9, N'12345', NULL, NULL, N'Anand', N'Gujarat', NULL, NULL, NULL, NULL, CAST(20000.00 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 1, CAST(N'2020-03-15T09:37:04.513' AS DateTime), 1, CAST(N'2020-03-15T09:37:04.513' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10010, 10, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 0, 10, CAST(N'2020-03-14T22:36:27.573' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10011, 11, N'zx12345uhdhh', NULL, NULL, N'GODHRA', N'GUJARAT', NULL, NULL, NULL, NULL, CAST(10000.00 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 1, CAST(N'2020-03-14T22:55:06.590' AS DateTime), 1, CAST(N'2020-03-14T22:55:06.590' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10012, 12, N'123', NULL, NULL, N'godhra', N'gujarat', NULL, NULL, NULL, NULL, CAST(50000.00 AS Numeric(18, 2)), CAST(8700.00 AS Numeric(18, 2)), 1, 0, 1, CAST(N'2020-03-15T03:07:38.423' AS DateTime), 1, CAST(N'2020-03-15T03:07:38.423' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientOtherDetails] ([ClientOtherDetailsId], [ClientUserId], [Addharcardno], [Pancardno], [GSTno], [City], [State], [Address], [ShipCity], [ShipState], [ShipAddress], [CreditLimitAmt], [AmountDue], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShipFirstName], [ShipLastName], [ShipPhoneNumber], [ShipEmail], [ShipPostalcode], [Dob], [ShopPhoto], [ShopName], [PanCardPhoto], [GSTPhoto], [DistributorCode], [AddharPhoto]) VALUES (10013, 13, N'123455', NULL, NULL, N'GODHRA', N'GUJARAT', NULL, N'asc', N'acsd', N'asx', CAST(50000.00 AS Numeric(18, 2)), CAST(30000.00 AS Numeric(18, 2)), 1, 0, 1, CAST(N'2020-03-17T03:12:39.537' AS DateTime), 1, CAST(N'2020-03-17T03:12:39.537' AS DateTime), N'kam', N'asx', N'9106490735', N'kamleshlalwani152@gmail.com', N'389001', NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ClientOtherDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientRoles] ON 

INSERT [dbo].[tbl_ClientRoles] ([ClientRoleId], [ClientRoleName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Customer', 1, 0, 1, CAST(N'2020-02-03T18:02:27.017' AS DateTime), 1, CAST(N'2020-02-03T18:02:27.017' AS DateTime))
INSERT [dbo].[tbl_ClientRoles] ([ClientRoleId], [ClientRoleName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Distributor', 1, 0, 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ClientRoles] OFF
SET IDENTITY_INSERT [dbo].[tbl_ClientUsers] ON 

INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (1, N'John', N'Doe', N'johndoe', N'johndoe@gmail.com', N'1ZQRVQG3sX0=', N'9999898989', 1, NULL, NULL, 1, 0, 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (2, N'Ramesh', N'Patel', N'rameshpatel', N'ramesh@gmail.com', N'1ZQRVQG3sX0=', N'8787878787', 1, NULL, NULL, 1, 0, 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (3, N'Rakesh', N'Shah', N'rakeshshah11', N'rakesh@gmail.com', N'1ZQRVQG3sX0=', N'9998707070', 2, N'RK Center', NULL, 1, 0, 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (4, N'Nil', N'Thakkar', N'nil1212', N'nil1287@gmail.com', N'1ZQRVQG3sX0=', N'9977447744', 2, N'Nil Trader', NULL, 1, 0, 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), 1, CAST(N'2020-03-15T03:56:32.237' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (5, N'Nilesh', N'Prajapati', N'NileshPrajapati', N'nilesh007444@gmail.com', N'AOx_m27p__8=', N'09824936252', 1, NULL, N'', 1, 0, 0, CAST(N'2020-03-10T12:33:25.530' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (6, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'kamleshlalwani152@gmail.com', N'FczZJFnW_R0=', N'9106490735', 1, NULL, N'', 1, 0, 0, CAST(N'2020-03-11T02:20:16.273' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (7, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'kamleshlalwani152@yahoo.com', N'FczZJFnW_R0=', N'9510105513', 1, NULL, N'', 1, 0, 0, CAST(N'2020-03-11T02:23:39.430' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (8, N'Raj', N'Thakker', N'RajThakker', N'reaj@gmail.com', N'AOx_m27p__8=', N'878888788', 2, N'Raj Traders', NULL, 1, 0, 1, CAST(N'2020-03-15T09:33:55.180' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (9, N'Nilesh', N'Prajapati', N'NileshPrajapati', N'prajapati.nileshbhai@gmail.com', N'AOx_m27p__8=', N'09824936252', 2, N'Nilesh Software', NULL, 1, 0, 1, CAST(N'2020-03-15T09:37:03.570' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (10, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'abc@gmail.com', N'FczZJFnW_R0=', N'9106490735', 1, NULL, N'', 1, 0, 0, CAST(N'2020-03-14T22:36:27.540' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (11, N'kamlesh', N'LALWANI', N'kamleshLALWANI', N'redspeech@rediffmail.com', N'Sj3ERF8C/LY=', N'9106490735', 2, N'UNIVERSAL INFOTECH', NULL, 1, 0, 1, CAST(N'2020-03-14T22:55:06.577' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (12, N'ashwain', N'khatri', N'ashwainkhatri', N'prakashmagnani@gmail.com', N'KZ152Wn/Rqw=', N'9824143334', 2, N'HARI OM TREADERS', NULL, 1, 0, 1, CAST(N'2020-03-15T03:07:38.393' AS DateTime), NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_ClientUsers] ([ClientUserId], [FirstName], [LastName], [UserName], [Email], [Password], [MobileNo], [ClientRoleId], [CompanyName], [ProfilePicture], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AlternateMobileNo], [Prefix]) VALUES (13, N'BURHAN', N'BURHAN', N'BURHANBURHAN', N'kachwala63@gmail.com', N'KZ152Wn/Rqw=', N'9016050614', 2, N'BURHAN', NULL, 1, 0, 1, CAST(N'2020-03-17T03:12:39.503' AS DateTime), NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ClientUsers] OFF
SET IDENTITY_INSERT [dbo].[tbl_ContactFormData] ON 

INSERT [dbo].[tbl_ContactFormData] ([ContactForm_Id], [Name], [Message], [PhoneNumber], [Email], [FromWhere], [ClientUserId], [MessageDate], [IsDelete]) VALUES (3, N'kamlesh lalwani', N'd', N'9106490735', N'kamleshlalwani152@gmail.com', N'Web', 0, CAST(N'2020-03-14T22:59:19.483' AS DateTime), 0)
SET IDENTITY_INSERT [dbo].[tbl_ContactFormData] OFF
SET IDENTITY_INSERT [dbo].[tbl_DistributorRequestDetails] ON 

INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (1, N'Raj', N'Thakker', N'reaj@gmail.com', N'878888788', N'Raj Traders', N'Anand', N'Gujarat', N'112121212', NULL, N'154544545', 1, CAST(N'2020-02-03T18:21:22.537' AS DateTime), CAST(N'2020-02-03T18:21:22.537' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (2, N'Nilesh', N'Prajapati', N'prajapati.nileshbhai@gmail.com', N'09824936252', N'Nilesh Software', N'Anand', N'Gujarat', N'12345', NULL, NULL, 1, CAST(N'2020-03-15T09:31:30.767' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (3, N'kamlesh', N'LALWANI', N'redspeech@rediffmail.com', N'9106490735', N'UNIVERSAL INFOTECH', N'GODHRA', N'GUJARAT', N'zx12345uhdhh', NULL, NULL, 1, CAST(N'2020-03-14T22:50:43.467' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (4, N'ashwain', N'khatri', N'prakashmagnani@gmail.com', N'9824143334', N'HARI OM TREADERS', N'godhra', N'gujarat', N'123', NULL, NULL, 1, CAST(N'2020-03-15T02:57:52.783' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
INSERT [dbo].[tbl_DistributorRequestDetails] ([DistributorRequestId], [FirstName], [LastName], [Email], [MobileNo], [CompanyName], [City], [State], [AddharcardNo], [PanCardNo], [GSTNo], [IsDelete], [CreatedDate], [UpdatedDate], [UpdatedBy], [Dob], [AlternateMobileNo], [Prefix], [ShopName], [ShopPhoto], [PanCardPhoto], [GSTPhoto], [AddharPhoto], [ProfilePhoto]) VALUES (5, N'BURHAN', N'BURHAN', N'kachwala63@gmail.com', N'9016050614', N'BURHAN', N'GODHRA', N'GUJARAT', N'123455', NULL, NULL, 1, CAST(N'2020-03-17T03:11:24.487' AS DateTime), NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_DistributorRequestDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_Godown] ON 

INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'ABC1', 1, 1, 1, CAST(N'2020-03-28T06:06:06.810' AS DateTime), 1, CAST(N'2020-03-28T06:06:32.367' AS DateTime))
INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Godown 2', 1, 0, 1, CAST(N'2020-03-28T06:14:23.717' AS DateTime), 1, CAST(N'2020-03-28T06:28:23.013' AS DateTime))
INSERT [dbo].[tbl_Godown] ([GodownId], [GodownName], [IsActive], [IsDeleted], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Godown 1', 1, 0, 1, CAST(N'2020-03-28T06:27:46.250' AS DateTime), 1, CAST(N'2020-03-28T06:28:06.790' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Godown] OFF
SET IDENTITY_INSERT [dbo].[tbl_GSTMaster] ON 

INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (1, N'5%', CAST(5.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (2, N'12%', CAST(12.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (3, N'18%', CAST(18.00 AS Decimal(18, 2)))
INSERT [dbo].[tbl_GSTMaster] ([GST_Id], [GSTText], [GSTPer]) VALUES (4, N'28%', CAST(28.00 AS Decimal(18, 2)))
SET IDENTITY_INSERT [dbo].[tbl_GSTMaster] OFF
SET IDENTITY_INSERT [dbo].[tbl_HomeImages] ON 

INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'e3aa1a4a-eb1a-4868-8e35-1076f86be5f6-user4-128x128.jpg', N'ABC1', N'XYZ1', 1, 1, CAST(N'2020-03-27T17:10:11.720' AS DateTime), 1, CAST(N'2020-03-27T17:32:14.153' AS DateTime))
INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'00120c0d-6af4-45a7-9131-45da74c20d49-user7-128x128.jpg', N'ABC', N'XYZ', 1, 1, CAST(N'2020-03-28T06:54:50.293' AS DateTime), 1, CAST(N'2020-03-28T06:54:50.293' AS DateTime))
INSERT [dbo].[tbl_HomeImages] ([HomeImageId], [HomeImageName], [HeadingText1], [HeadingText2], [IsActive], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'2374a71a-076d-4505-af18-8c6a465f225d-20190428101259445950437.jpg', N'Hi', N'Hello', 1, 1, CAST(N'2020-04-02T18:19:52.407' AS DateTime), 1, CAST(N'2020-04-02T18:19:52.407' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_HomeImages] OFF
SET IDENTITY_INSERT [dbo].[tbl_ItemStocks] ON 

INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, 1, 1, 1, 50, 1, 0, 1, CAST(N'2020-03-14T12:11:10.243' AS DateTime), 1, CAST(N'2020-03-14T12:11:10.243' AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, 1, 2, 2, 3, 10, 1, 0, 1, CAST(N'2020-03-14T12:11:21.853' AS DateTime), 1, CAST(N'2020-03-14T12:11:21.853' AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, 3, 5, 0, 6, 100, 1, 0, 1, CAST(N'2020-03-15T04:51:15.357' AS DateTime), 1, CAST(N'2020-03-15T04:51:15.357' AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 4, 7, 0, 8, 50, 1, 0, 1, CAST(N'2020-03-15T07:47:09.370' AS DateTime), 1, CAST(N'2020-03-15T07:47:09.370' AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, 4, 7, 0, 8, 50, 1, 0, 1, CAST(N'2020-03-15T07:47:20.993' AS DateTime), 1, CAST(N'2020-03-15T07:47:20.993' AS DateTime))
INSERT [dbo].[tbl_ItemStocks] ([StockId], [CategoryId], [ProductId], [SubProductId], [ProductItemId], [Qty], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, 4, 7, 0, 8, 25, 1, 0, 1, CAST(N'2020-03-15T07:49:42.743' AS DateTime), 1, CAST(N'2020-03-15T07:49:42.743' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ItemStocks] OFF
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] ON 

INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (1, 1, CAST(N'2020-03-26T23:26:38.423' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (2, 1, CAST(N'2020-03-27T22:23:14.967' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (3, 1, CAST(N'2020-03-27T22:23:24.243' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (4, 1, CAST(N'2020-03-27T22:36:21.870' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (5, 1, CAST(N'2020-03-27T22:36:46.803' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (6, 1, CAST(N'2020-03-27T23:01:11.223' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (7, 1, CAST(N'2020-03-27T23:15:44.073' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (8, 1, CAST(N'2020-03-27T23:15:44.063' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (9, 1, CAST(N'2020-03-28T11:35:29.333' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (10, 1, CAST(N'2020-03-28T11:43:54.110' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (11, 1, CAST(N'2020-03-28T11:56:27.623' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (12, 1, CAST(N'2020-03-28T12:12:12.627' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (13, 1, CAST(N'2020-03-28T17:22:42.323' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (14, 1, CAST(N'2020-03-28T17:48:55.160' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (15, 1, CAST(N'2020-03-28T18:28:40.623' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (16, 1, CAST(N'2020-03-28T18:48:45.697' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (17, 1, CAST(N'2020-03-28T19:11:00.843' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (18, 1, CAST(N'2020-03-28T19:31:07.450' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (19, 1, CAST(N'2020-03-28T20:14:56.520' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (20, 1, CAST(N'2020-03-28T20:29:54.370' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (21, 1, CAST(N'2020-03-28T20:38:14.943' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (22, 1, CAST(N'2020-03-28T21:16:03.177' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (23, 1, CAST(N'2020-03-28T22:28:34.417' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (24, 1, CAST(N'2020-04-02T23:46:44.153' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (25, 1, CAST(N'2020-04-02T23:49:13.783' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (26, 1, CAST(N'2020-04-04T00:26:16.370' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (27, 1, CAST(N'2020-04-04T14:27:25.783' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (28, 1, CAST(N'2020-04-04T15:23:42.610' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (29, 1, CAST(N'2020-04-04T15:39:02.503' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (30, 1, CAST(N'2020-04-04T18:26:47.227' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (31, 1, CAST(N'2020-04-04T18:28:39.570' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (32, 1, CAST(N'2020-04-04T18:33:36.737' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (33, 1, CAST(N'2020-04-04T18:36:26.903' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (34, 1, CAST(N'2020-04-04T18:52:21.683' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (35, 1, CAST(N'2020-04-04T19:51:47.733' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (36, 1, CAST(N'2020-04-04T19:58:37.797' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (37, 1, CAST(N'2020-04-04T20:51:18.087' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (38, 1, CAST(N'2020-04-04T20:51:20.433' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (39, 1, CAST(N'2020-04-04T22:27:31.797' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (40, 1, CAST(N'2020-04-04T22:37:43.993' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (41, 1, CAST(N'2020-04-04T22:54:05.897' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (42, 1, CAST(N'2020-04-04T22:57:21.193' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (43, 1, CAST(N'2020-04-04T22:59:11.147' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (44, 1, CAST(N'2020-04-04T23:04:36.153' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (45, 1, CAST(N'2020-04-04T23:29:15.863' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (46, 1, CAST(N'2020-04-04T23:30:33.197' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (47, 1, CAST(N'2020-04-04T23:37:54.133' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (48, 1, CAST(N'2020-04-04T23:48:53.490' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (49, 1, CAST(N'2020-04-04T23:51:02.410' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (50, 1, CAST(N'2020-04-05T00:02:29.420' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (51, 1, CAST(N'2020-04-05T00:08:38.043' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (52, 1, CAST(N'2020-04-05T00:13:58.423' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (53, 5, CAST(N'2020-04-05T00:14:17.160' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (54, 1, CAST(N'2020-04-05T00:21:32.220' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (55, 1, CAST(N'2020-04-05T00:22:04.303' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (56, 5, CAST(N'2020-04-05T00:22:21.657' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (57, 1, CAST(N'2020-04-05T10:43:47.210' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (58, 1, CAST(N'2020-04-05T10:53:40.267' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (59, 1, CAST(N'2020-04-05T10:56:06.747' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (60, 1, CAST(N'2020-04-05T11:13:15.267' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (61, 1, CAST(N'2020-04-05T11:15:06.833' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (62, 5, CAST(N'2020-04-05T11:15:43.037' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (63, 5, CAST(N'2020-04-05T11:21:38.167' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (64, 1, CAST(N'2020-04-05T11:21:52.110' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (65, 1, CAST(N'2020-04-05T11:22:35.450' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (66, 1, CAST(N'2020-04-05T11:23:20.917' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (67, 1, CAST(N'2020-04-05T13:04:37.850' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (68, 1, CAST(N'2020-04-05T14:00:48.747' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (69, 5, CAST(N'2020-04-05T14:02:15.857' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (70, 5, CAST(N'2020-04-05T14:23:59.250' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (71, 1, CAST(N'2020-04-05T14:25:01.440' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (72, 5, CAST(N'2020-04-05T14:27:05.160' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (73, 1, CAST(N'2020-04-05T14:27:16.993' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (74, 5, CAST(N'2020-04-05T14:27:56.120' AS DateTime), N'::1', N'Login')
INSERT [dbo].[tbl_LoginHistory] ([LoginHistoryId], [UserId], [DateAction], [IPAddress], [Type]) VALUES (75, 1, CAST(N'2020-04-05T14:46:19.170' AS DateTime), N'::1', N'Login')
SET IDENTITY_INSERT [dbo].[tbl_LoginHistory] OFF
SET IDENTITY_INSERT [dbo].[tbl_Offers] ON 

INSERT [dbo].[tbl_Offers] ([OfferId], [OfferName], [CategoryId], [ProductId], [SubproductId], [ProductItemId], [OfferPrice], [OfferPriceforDistributor], [StartDate], [EndDate], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Offer Water Tank', 1, 2, 2, 3, CAST(200.00 AS Numeric(18, 2)), CAST(190.00 AS Numeric(18, 2)), CAST(N'2020-03-13T00:00:00.000' AS DateTime), CAST(N'2020-03-18T00:00:00.000' AS DateTime), 1, 0, 1, CAST(N'2020-03-14T12:07:38.570' AS DateTime), 1, CAST(N'2020-03-14T12:07:38.570' AS DateTime))
INSERT [dbo].[tbl_Offers] ([OfferId], [OfferName], [CategoryId], [ProductId], [SubproductId], [ProductItemId], [OfferPrice], [OfferPriceforDistributor], [StartDate], [EndDate], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'HEADING', 4, 6, 0, 9, CAST(900.00 AS Numeric(18, 2)), CAST(800.00 AS Numeric(18, 2)), CAST(N'2020-03-15T00:00:00.000' AS DateTime), CAST(N'2020-03-16T00:00:00.000' AS DateTime), 1, 0, 1, CAST(N'2020-03-15T07:53:44.543' AS DateTime), 1, CAST(N'2020-03-15T07:53:44.543' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Offers] OFF
SET IDENTITY_INSERT [dbo].[tbl_OrderItemDetails] ON 

INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, 1, 5, N'Orange Item 1', 5, CAST(900.00 AS Numeric(18, 2)), N'4554', CAST(686.44 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 10, CAST(N'2020-03-14T23:05:34.000' AS DateTime), 10, CAST(N'2020-03-14T23:05:34.000' AS DateTime))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, 2, 9, N'SHEET COVER ITEM', 1, CAST(2900.00 AS Numeric(18, 2)), N'P3', CAST(138.10 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 6, CAST(N'2020-03-15T02:46:34.530' AS DateTime), 6, CAST(N'2020-03-15T02:46:34.530' AS DateTime))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, 2, 15, N'BIB COCK', 2, CAST(1386.00 AS Numeric(18, 2)), N'ARA2103', CAST(422.84 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 6, CAST(N'2020-03-15T02:46:34.530' AS DateTime), 6, CAST(N'2020-03-15T02:46:34.530' AS DateTime))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, 3, 9, N'SHEET COVER ITEM', 3, CAST(2900.00 AS Numeric(18, 2)), N'P3', CAST(414.28 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 12, CAST(N'2020-03-15T03:30:53.497' AS DateTime), 12, CAST(N'2020-03-15T03:30:53.497' AS DateTime))
INSERT [dbo].[tbl_OrderItemDetails] ([OrderDetailId], [OrderId], [ProductItemId], [ItemName], [Qty], [Price], [Sku], [GSTAmt], [IGSTAmt], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, 4, 26, N'10 gm 24 carat bis', 1, CAST(30000.00 AS Numeric(18, 2)), N'p001', CAST(4576.28 AS Numeric(18, 2)), CAST(0.00 AS Numeric(18, 2)), 1, 0, 13, CAST(N'2020-03-18T01:43:56.813' AS DateTime), 13, CAST(N'2020-03-18T01:43:56.813' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_OrderItemDetails] OFF
SET IDENTITY_INSERT [dbo].[tbl_Orders] ON 

INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature]) VALUES (1, 10, CAST(4500.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 1, N'upi', 1, 0, 10, CAST(N'2020-03-14T23:05:33.937' AS DateTime), 10, CAST(N'2020-03-14T23:05:33.937' AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_ESOAFBLOJidBKr', N'pay_ESOBtkoArnI7p3', N'6d045ac62b9caef0f362559ac7a6426bb48c037a9c879f55d9c8fbc8b9e50831')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature]) VALUES (2, 6, CAST(5672.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 1, N'upi', 1, 0, 6, CAST(N'2020-03-15T02:46:34.483' AS DateTime), 6, CAST(N'2020-03-15T02:46:34.483' AS DateTime), CAST(0.00 AS Decimal(18, 2)), N'order_ESRwuMTRcw5bmH', N'pay_ESRxLuzvAUbwvq', N'3261bb465b1a0e8a635b3a3644158617ecb2303b8a97d184e37f4be48e10c604')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature]) VALUES (3, 12, CAST(8700.00 AS Decimal(18, 2)), N'godhra', N'gujarat', N'A', NULL, N'KAMLESH LALWANI', N'09106490735', 1, N'ByCredit', 1, 0, 12, CAST(N'2020-03-15T03:30:53.467' AS DateTime), 12, CAST(N'2020-03-15T03:30:53.467' AS DateTime), CAST(8700.00 AS Decimal(18, 2)), N'3', N'', N'')
INSERT [dbo].[tbl_Orders] ([OrderId], [ClientUserId], [OrderAmount], [OrderShipCity], [OrderShipState], [OrderShipAddress], [OrderShipPincode], [OrderShipClientName], [OrderShipClientPhone], [OrderStatusId], [PaymentType], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [AmountDue], [RazorpayOrderId], [RazorpayPaymentId], [RazorSignature]) VALUES (4, 13, CAST(30000.00 AS Decimal(18, 2)), N'asc - 389001', N'acsd', N'asx', NULL, N'kam asx', N'9106490735', 1, N'ByCredit', 1, 0, 13, CAST(N'2020-03-18T01:43:56.767' AS DateTime), 13, CAST(N'2020-03-18T01:43:56.767' AS DateTime), CAST(30000.00 AS Decimal(18, 2)), N'4', N'', N'')
SET IDENTITY_INSERT [dbo].[tbl_Orders] OFF
SET IDENTITY_INSERT [dbo].[tbl_PaymentHistory] ON 

INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy]) VALUES (1, CAST(4500.00 AS Decimal(18, 2)), CAST(4500.00 AS Decimal(18, 2)), CAST(N'2020-03-14T23:05:33.970' AS DateTime), 1, 10, CAST(N'2020-03-14T23:05:33.970' AS DateTime), N'upi')
INSERT [dbo].[tbl_PaymentHistory] ([PaymentHistory_Id], [AmountPaid], [AmountDue], [DateOfPayment], [OrderId], [CreatedBy], [CreatedDate], [PaymentBy]) VALUES (2, CAST(5672.00 AS Decimal(18, 2)), CAST(5672.00 AS Decimal(18, 2)), CAST(N'2020-03-15T02:46:34.513' AS DateTime), 2, 6, CAST(N'2020-03-15T02:46:34.513' AS DateTime), N'upi')
SET IDENTITY_INSERT [dbo].[tbl_PaymentHistory] OFF
SET IDENTITY_INSERT [dbo].[tbl_ProductItemImages] ON 

INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (16, 4, N'f3adb265-e782-49b9-a339-ec6c2647ba0a-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(N'2020-03-14T17:51:36.587' AS DateTime), 1, CAST(N'2020-03-14T17:51:36.587' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (17, 4, N'481ae0fb-2a12-411d-9eec-9e7664006d3b-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(N'2020-03-14T17:51:37.503' AS DateTime), 1, CAST(N'2020-03-14T17:51:37.503' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (18, 5, N'fa1e464a-fc19-4c45-8ac4-96830f296ae0-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(N'2020-03-15T04:45:38.340' AS DateTime), 1, CAST(N'2020-03-15T04:45:38.340' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (19, 5, N'd7499156-ca08-4561-8811-15192c366852-7af1ea40865febba6e3eb51e1582e78b.jpg', 1, 0, 1, CAST(N'2020-03-15T04:45:38.370' AS DateTime), 1, CAST(N'2020-03-15T04:45:38.370' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (20, 6, N'414854d6-301c-4923-9563-8e1ce66f969e-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(N'2020-03-15T04:49:31.810' AS DateTime), 1, CAST(N'2020-03-15T04:49:31.810' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (21, 6, N'e38d9aed-44ee-43d2-b616-c9a3d67227b3-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(N'2020-03-15T04:49:31.827' AS DateTime), 1, CAST(N'2020-03-15T04:49:31.827' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (22, 6, N'35fafa75-822c-4ccf-aafe-ce501c808617-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', 1, 0, 1, CAST(N'2020-03-15T04:49:31.840' AS DateTime), 1, CAST(N'2020-03-15T04:49:31.840' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (23, 7, N'e0e7d6e8-1e5e-4c2d-8e20-7b30f9abed40-as.jpg', 1, 0, 1, CAST(N'2020-03-15T07:41:53.803' AS DateTime), 1, CAST(N'2020-03-15T07:41:53.803' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (24, 7, N'5fb6152c-59d1-4007-be8c-b199a60d98ec-as.jpg', 1, 0, 1, CAST(N'2020-03-15T07:41:53.837' AS DateTime), 1, CAST(N'2020-03-15T07:41:53.837' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (25, 7, N'b0994802-1d9d-4766-a1c1-5fed21790557-as.jpg', 1, 0, 1, CAST(N'2020-03-15T07:41:53.837' AS DateTime), 1, CAST(N'2020-03-15T07:41:53.837' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (26, 7, N'1cf3dc44-65c5-4c98-93a1-91d804c70efc-as.jpg', 1, 0, 1, CAST(N'2020-03-15T07:41:53.867' AS DateTime), 1, CAST(N'2020-03-15T07:41:53.867' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (27, 8, N'2549d05c-c56d-4e75-ab2c-d5791343ba83-mbl.png', 1, 0, 1, CAST(N'2020-03-15T07:44:53.010' AS DateTime), 1, CAST(N'2020-03-15T07:44:53.010' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (28, 8, N'7f276bd5-a6a2-42a0-b298-51b415831347-mbl.png', 1, 0, 1, CAST(N'2020-03-15T07:44:53.023' AS DateTime), 1, CAST(N'2020-03-15T07:44:53.023' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (29, 8, N'c20680e6-9b90-4d44-9d08-802c69bf3838-mbl.png', 1, 0, 1, CAST(N'2020-03-15T07:44:53.023' AS DateTime), 1, CAST(N'2020-03-15T07:44:53.023' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (30, 9, N'3d7197b6-8d20-4377-8630-ae0e17c898fc-turmeric.jpg', 1, 0, 1, CAST(N'2020-03-15T07:46:08.947' AS DateTime), 1, CAST(N'2020-03-15T07:46:08.947' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (31, 10, N'45a8bc9d-eb95-446e-93b2-84d4836e946b-3.jpg', 1, 0, 1, CAST(N'2020-03-15T08:28:15.287' AS DateTime), 1, CAST(N'2020-03-15T08:28:15.287' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (32, 11, N'98f0853b-6204-4c9c-aa25-1b01fab202f3-3.jpg', 1, 0, 1, CAST(N'2020-03-15T08:39:01.383' AS DateTime), 1, CAST(N'2020-03-15T08:39:01.383' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (33, 12, N'a9a16837-2912-46e8-ba4f-06853211dcd7-favicon.ico', 1, 0, 1, CAST(N'2020-03-15T08:40:55.557' AS DateTime), 1, CAST(N'2020-03-15T08:40:55.557' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (34, 13, N'f0678724-2ee1-4321-bcfa-5e04c1acedeb-ext.jpg', 1, 0, 1, CAST(N'2020-03-15T08:45:09.230' AS DateTime), 1, CAST(N'2020-03-15T08:45:09.230' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (35, 14, N'35d67d77-ab4e-4838-978e-45d4873bf8e9-1.jpg', 1, 0, 1, CAST(N'2020-03-15T08:49:02.293' AS DateTime), 1, CAST(N'2020-03-15T08:49:02.293' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (36, 15, N'53676158-b8fa-40c3-a913-aad92a7fecf4-mbl.png', 1, 0, 1, CAST(N'2020-03-15T08:51:35.417' AS DateTime), 1, CAST(N'2020-03-15T08:51:35.417' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (37, 16, N'cccc289d-65d6-4fe9-96f3-2943d0c1898d-3.jpg', 1, 0, 1, CAST(N'2020-03-15T08:53:29.247' AS DateTime), 1, CAST(N'2020-03-15T08:53:29.247' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (38, 17, N'e7ac4b93-4eae-4ea7-b2fc-62398f482188-master.jpg', 1, 0, 1, CAST(N'2020-03-15T08:57:16.343' AS DateTime), 1, CAST(N'2020-03-15T08:57:16.343' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (39, 18, N'd34b189e-ef73-4176-8781-1c9e26fa2f59-te1.jpg', 1, 0, 1, CAST(N'2020-03-15T09:00:32.623' AS DateTime), 1, CAST(N'2020-03-15T09:00:32.623' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (40, 19, N'279c2ade-7145-4a2a-ad8c-cbf99d809c99-ext.jpg', 1, 0, 1, CAST(N'2020-03-15T09:03:09.373' AS DateTime), 1, CAST(N'2020-03-15T09:03:09.373' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (41, 20, N'7c90d588-a58c-4052-8ff5-0e0debc3845a-images.jpg', 1, 0, 1, CAST(N'2020-03-15T09:05:56.157' AS DateTime), 1, CAST(N'2020-03-15T09:05:56.157' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (42, 21, N'586017d7-970d-4ee9-9752-5cef14f6d85c-images.jpg', 1, 0, 1, CAST(N'2020-03-15T09:06:08.657' AS DateTime), 1, CAST(N'2020-03-15T09:06:08.657' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (43, 22, N'a706b54a-af28-4840-b96f-9b8564da15d3-3.jpg', 1, 0, 1, CAST(N'2020-03-15T09:08:37.843' AS DateTime), 1, CAST(N'2020-03-15T09:08:37.843' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (44, 23, N'a4843db5-d098-4acf-80cf-f35ef4ce02ac-823x350xgst-1-823x350.jpg.pagespeed.ic.QE-r9hvrwL.jpg', 1, 0, 1, CAST(N'2020-03-15T09:10:40.313' AS DateTime), 1, CAST(N'2020-03-15T09:10:40.313' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (45, 24, N'be1ef08a-20fc-4f9e-8035-9d128caad93b-ext.jpg', 1, 0, 1, CAST(N'2020-03-15T09:12:50.297' AS DateTime), 1, CAST(N'2020-03-15T09:12:50.297' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (46, 25, N'd969e377-10c8-4376-957d-9cddd367e041-3.jpg', 1, 0, 1, CAST(N'2020-03-15T09:17:17.893' AS DateTime), 1, CAST(N'2020-03-15T09:17:17.893' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (47, 26, N'4438ca7c-2d97-45df-a319-7e58a7bc564b-ext.jpg', 1, 0, 1, CAST(N'2020-03-17T10:06:23.220' AS DateTime), 1, CAST(N'2020-03-17T10:06:23.220' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (48, 26, N'b0dfc33c-e34d-44c2-b496-79aa81218ddc-ext.jpg', 1, 0, 1, CAST(N'2020-03-17T10:06:23.253' AS DateTime), 1, CAST(N'2020-03-17T10:06:23.253' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (49, 26, N'fc8538ff-1a7f-42a6-9496-eb5825414ee5-ext.jpg', 1, 0, 1, CAST(N'2020-03-17T10:06:23.267' AS DateTime), 1, CAST(N'2020-03-17T10:06:23.267' AS DateTime))
INSERT [dbo].[tbl_ProductItemImages] ([ProductItemImageId], [ProductItemId], [ItemImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (50, 26, N'dd6ea484-bb4e-4408-accb-f765f27cd8d5-ext.jpg', 1, 0, 1, CAST(N'2020-03-17T10:06:23.267' AS DateTime), 1, CAST(N'2020-03-17T10:06:23.267' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_ProductItemImages] OFF
SET IDENTITY_INSERT [dbo].[tbl_ProductItems] ON 

INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (1, 1, 1, 1, N'Abcd Sanitory 11 ', N'sample description for sanitaty item', CAST(300.00 AS Decimal(18, 2)), CAST(230.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, N'13d507c7-5a4a-45aa-9163-b59b8f7714d9-sanitary1.jpg', N'Test', N'ITM444', 1, 1, 0, 1, CAST(N'2020-03-14T11:58:32.913' AS DateTime), 1, CAST(N'2020-03-14T11:58:32.913' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (2, 1, 1, 1, N'Item2 Test', N'Test Item Tank', CAST(560.00 AS Decimal(18, 2)), CAST(480.00 AS Decimal(18, 2)), CAST(500.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), NULL, NULL, N'Sample text', N'i444', 0, 1, 0, 1, CAST(N'2020-03-14T11:59:38.897' AS DateTime), 1, CAST(N'2020-03-14T11:59:38.897' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (3, 1, 2, 2, N'Item2 PPPPPPP', N'Test Item Tank desccc', CAST(350.00 AS Decimal(18, 2)), CAST(300.00 AS Decimal(18, 2)), CAST(330.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), NULL, NULL, N'Test', N'ITM441099', 1, 1, 0, 1, CAST(N'2020-03-14T12:06:29.570' AS DateTime), 1, CAST(N'2020-03-14T12:06:29.570' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (4, 1, 1, 1, N'item aaaa', N'aaaa item desc', CAST(75.00 AS Decimal(18, 2)), CAST(55.00 AS Decimal(18, 2)), CAST(60.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), NULL, N'42a7f614-e82e-4988-94dd-b469cfad6732-7af1ea40865febba6e3eb51e1582e78b.jpg', N'this is notification text', N'456', 1, 1, 0, 1, CAST(N'2020-03-14T17:51:35.123' AS DateTime), 1, CAST(N'2020-03-14T18:44:41.573' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (5, 2, 3, 3, N'Orange Item 1', N'''Orange is the colour between yellow and red on the spectrum of visible light. Human eyes perceive orange when observing light with a dominant wavelength between roughly 585 and 620 nanometres.', CAST(1000.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(900.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'33272a60-5f3e-4c11-9ab1-97e5c42b26e2-7af1ea40865febba6e3eb51e1582e78b.jpg', N'you will get this after 2 days', N'4554', 1, 1, 0, 1, CAST(N'2020-03-15T04:45:38.293' AS DateTime), 1, CAST(N'2020-03-15T04:45:38.293' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (6, 3, 5, 0, N'Solar Panel', N'this is description of Solar Panel', CAST(2000.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), CAST(1900.00 AS Decimal(18, 2)), CAST(12.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'14cd47ed-e6d0-4c6d-9cfe-bcb82be0bbf6-180liter-Vacuum-Tube-Solar-Geyser-Solar-Water-Heater.jpg', N'this will get in 5 days', N'8721', 0, 1, 0, 1, CAST(N'2020-03-15T04:49:31.780' AS DateTime), 1, CAST(N'2020-03-15T04:49:31.780' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (7, 4, 7, 0, N'PILLER COCK', N'Polytetramethylene Terephthalate (PTMT) PTMT is a thermoplastic, further classified as a polyester plastic. The graph bars on the material properties cards below compare PTMT to: polyester plastics (t', CAST(1000.00 AS Decimal(18, 2)), CAST(800.00 AS Decimal(18, 2)), CAST(900.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'9fb60919-a069-4a1d-ba88-1b11cc77c80d-as.jpg', N'AS', N'P1', 1, 1, 0, 1, CAST(N'2020-03-15T07:41:53.773' AS DateTime), 1, CAST(N'2020-03-15T07:41:53.773' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (8, 4, 7, 0, N'SUPER', N'DETILS', CAST(2000.00 AS Decimal(18, 2)), CAST(1800.00 AS Decimal(18, 2)), CAST(1900.00 AS Decimal(18, 2)), CAST(9.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'39062b6f-f1a5-44d3-962a-9cd5c2608fdf-mbl.png', N'NOTIFACTION', N'P2', 0, 1, 0, 1, CAST(N'2020-03-15T07:44:52.993' AS DateTime), 1, CAST(N'2020-03-15T07:44:52.993' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (9, 4, 6, 0, N'SHEET COVER ITEM', N'DESC', CAST(3000.00 AS Decimal(18, 2)), CAST(2500.00 AS Decimal(18, 2)), CAST(2900.00 AS Decimal(18, 2)), CAST(5.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'c1ba161c-4126-41ef-b5ca-5ed99b9bdbea-turmeric.jpg', N'STOCK', N'P3', 0, 1, 0, 1, CAST(N'2020-03-15T07:46:08.930' AS DateTime), 1, CAST(N'2020-03-15T07:46:08.930' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (10, 5, 8, 4, N'BIB COCK ', N'WALL MAOUNT', CAST(252.00 AS Decimal(18, 2)), CAST(167.00 AS Decimal(18, 2)), CAST(189.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'9177d62c-7324-48f2-a230-82950b3e4172-3.jpg', N'STOCK', N'ED950', 0, 1, 0, 1, CAST(N'2020-03-15T08:28:14.707' AS DateTime), 1, CAST(N'2020-03-15T08:28:14.707' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (11, 5, 8, 4, N'PILLAR COCK  NEW', N'FLOOR MOUNT', CAST(324.00 AS Decimal(18, 2)), CAST(214.00 AS Decimal(18, 2)), CAST(243.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e03a4a23-25e1-4178-b71c-3a5695af2e96-3.jpg', N'NOTIFACTION', N'ED952', 0, 1, 0, 1, CAST(N'2020-03-15T08:39:01.353' AS DateTime), 1, CAST(N'2020-03-15T08:39:01.353' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (12, 5, 8, 5, N'BIB COCK', N'WALL MAOUNT', CAST(252.00 AS Decimal(18, 2)), CAST(167.00 AS Decimal(18, 2)), CAST(189.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'8a1de2f8-3d63-4db0-8361-adca4c3eac82-favicon.ico', N'STOCK', N'ED980', 0, 1, 0, 1, CAST(N'2020-03-15T08:40:55.543' AS DateTime), 1, CAST(N'2020-03-15T08:40:55.543' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (13, 5, 8, 5, N'PILLER COCK', N'FLOOR MOUNT', CAST(324.00 AS Decimal(18, 2)), CAST(214.00 AS Decimal(18, 2)), CAST(243.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'a7e1ca35-7589-4dad-b645-e2a88ac6794e-ext.jpg', N'NOTIFACTION', N'EP982', 0, 1, 0, 1, CAST(N'2020-03-15T08:45:09.213' AS DateTime), 1, CAST(N'2020-03-15T08:45:09.213' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (14, 5, 8, 6, N'BIB COCK', N'WALL MAOUNT', CAST(292.00 AS Decimal(18, 2)), CAST(193.00 AS Decimal(18, 2)), CAST(219.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'a486978e-a1d7-4182-9e85-ee7ca540b0d5-1.jpg', N'NOTIFACTION', N'SUP591', 0, 1, 0, 1, CAST(N'2020-03-15T08:49:02.277' AS DateTime), 1, CAST(N'2020-03-15T08:49:02.277' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (15, 5, 9, 7, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1980.00 AS Decimal(18, 2)), CAST(1287.00 AS Decimal(18, 2)), CAST(1386.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'636b5775-e2be-4456-a591-210b06241a87-mbl.png', N'NOTIFACTION', N'ARA2103', 0, 1, 0, 1, CAST(N'2020-03-15T08:51:35.403' AS DateTime), 1, CAST(N'2020-03-15T08:51:35.403' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (16, 5, 9, 7, N'PILLER COCK ', N'FLOOR MOUNT (CP BRASS)', CAST(2190.00 AS Decimal(18, 2)), CAST(1424.00 AS Decimal(18, 2)), CAST(1533.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'db14e602-f3f1-41cd-876f-8e4ed9f2545b-3.jpg', N'NOTIFACTION', N'ARA1101', 0, 1, 0, 1, CAST(N'2020-03-15T08:53:29.230' AS DateTime), 1, CAST(N'2020-03-15T08:53:29.230' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (17, 5, 9, 8, N'PILLER COCK', N'FLOOR MOUNT (CP BRASS)', CAST(2245.00 AS Decimal(18, 2)), CAST(1460.00 AS Decimal(18, 2)), CAST(1571.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'f2390f5e-62f1-4e1e-a8c2-72ba3c60dba0-master.jpg', N'NOTIFACTION', N'CFT1101', 0, 1, 0, 1, CAST(N'2020-03-15T08:57:16.327' AS DateTime), 1, CAST(N'2020-03-22T18:20:06.603' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (18, 5, 9, 9, N'PILLAR COCK  NEW', N'FLOOR MOUNT (CP BRASS)', CAST(1880.00 AS Decimal(18, 2)), CAST(1222.00 AS Decimal(18, 2)), CAST(1316.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'd0033fd9-47ad-4876-8e6d-67e798656c83-te1.jpg', N'NOTIFACTION', N'WAV 1101', 0, 1, 0, 1, CAST(N'2020-03-15T09:00:32.623' AS DateTime), 1, CAST(N'2020-03-15T09:00:32.623' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (19, 5, 9, 9, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1865.00 AS Decimal(18, 2)), CAST(1212.00 AS Decimal(18, 2)), CAST(1305.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'6c1b6107-5ae2-4885-a6d0-e0248107309d-ext.jpg', N'NOTIFACTION', N'WAV2103', 0, 1, 0, 1, CAST(N'2020-03-15T09:03:09.343' AS DateTime), 1, CAST(N'2020-03-15T09:03:09.343' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (20, 5, 9, 8, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(2070.00 AS Decimal(18, 2)), CAST(1345.00 AS Decimal(18, 2)), CAST(1449.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e24cbcf7-0cd5-42d0-8f21-f615060f3e02-images.jpg', N'NOTIFACTION', N'CFT2103', 0, 1, 0, 1, CAST(N'2020-03-15T09:05:56.123' AS DateTime), 1, CAST(N'2020-03-15T09:05:56.123' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (21, 5, 9, 8, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(2070.00 AS Decimal(18, 2)), CAST(1345.00 AS Decimal(18, 2)), CAST(1449.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'262816d7-69e9-49ad-83bf-9f02221abfd1-images.jpg', N'NOTIFACTION', N'CFT2103', 0, 1, 0, 1, CAST(N'2020-03-15T09:06:08.640' AS DateTime), 1, CAST(N'2020-03-15T09:06:08.640' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (22, 5, 10, 10, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1630.00 AS Decimal(18, 2)), CAST(978.00 AS Decimal(18, 2)), CAST(1060.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'0f4238eb-49e9-4722-b3a8-71c9c6d93ead-3.jpg', N'NOTIFACTION', N'TY102', 0, 1, 0, 1, CAST(N'2020-03-15T09:08:37.830' AS DateTime), 1, CAST(N'2020-03-15T09:08:37.830' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (23, 5, 10, 10, N'PILLAR COCK  NEW', N'FLOOR MOUNT (CP BRASS)', CAST(1910.00 AS Decimal(18, 2)), CAST(1146.00 AS Decimal(18, 2)), CAST(1241.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'ec2c3e1a-91a4-43f1-9166-e2bee0381b51-823x350xgst-1-823x350.jpg.pagespeed.ic.QE-r9hvrwL.jpg', N'NOTIFACTION', N'TY101', 0, 1, 0, 1, CAST(N'2020-03-15T09:10:40.297' AS DateTime), 1, CAST(N'2020-03-15T09:10:40.297' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (24, 5, 10, 12, N'BIB COCK', N'WALL MAOUNT (CP BRASS)', CAST(1560.00 AS Decimal(18, 2)), CAST(936.00 AS Decimal(18, 2)), CAST(1014.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'e357aea4-ce2c-4963-a82c-4f5319c88a58-ext.jpg', N'NOTIFACTION', N'XO102', 0, 1, 0, 1, CAST(N'2020-03-15T09:12:50.283' AS DateTime), 1, CAST(N'2020-03-15T09:12:50.283' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (25, 5, 10, 12, N'PILLAR COCK  NEW', N'FLOOR MOUNT (CP BRASS)', CAST(1750.00 AS Decimal(18, 2)), CAST(1050.00 AS Decimal(18, 2)), CAST(1138.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'149526b8-202f-4c80-819a-40026fcd90d6-3.jpg', N'NOTIFACTION', N'XO101', 0, 1, 0, 1, CAST(N'2020-03-15T09:17:17.847' AS DateTime), 1, CAST(N'2020-03-15T09:17:17.847' AS DateTime), NULL, NULL)
INSERT [dbo].[tbl_ProductItems] ([ProductItemId], [CategoryId], [ProductId], [SubProductId], [ItemName], [ItemDescription], [MRPPrice], [DistributorPrice], [CustomerPrice], [GST_Per], [IGST_Per], [Cess], [MainImage], [Notification], [Sku], [IsPopularProduct], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate], [ShippingCharge], [Tags]) VALUES (26, 7, 11, 15, N'10 gm 24 carat bis', N'Want to enjoy a truly immersive gaming experience on a gaming machine that has striking good looks? Get your hands on the ROG Strix GL553. It comes with Windows 10 pre-installed. The 7th-generation Intel Core i7 processor and the discrete NVIDIA GeForce GTX 1050 graphics card. ROG Strix GL553 is specifically made for gaming and primed for creativity.', CAST(50000.00 AS Decimal(18, 2)), CAST(30000.00 AS Decimal(18, 2)), CAST(40000.00 AS Decimal(18, 2)), CAST(18.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), NULL, N'032e1af3-8eb3-4e21-912f-f939306a86b0-ext.jpg', N'OUT OF STOCK', N'p001', 1, 1, 0, 1, CAST(N'2020-03-17T10:06:23.173' AS DateTime), 1, CAST(N'2020-03-17T10:16:20.927' AS DateTime), NULL, NULL)
SET IDENTITY_INSERT [dbo].[tbl_ProductItems] OFF
SET IDENTITY_INSERT [dbo].[tbl_Products] ON 

INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Sanitory 1', 1, N'ed6b4fee-b8d1-4e2f-a7d2-e4398bfa33ed-sanitaty2.jpg', 1, 0, 1, CAST(N'2020-03-14T11:51:04.333' AS DateTime), 1, CAST(N'2020-03-14T11:51:19.130' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Watertanks', 1, NULL, 1, 0, 1, CAST(N'2020-03-14T11:52:07.350' AS DateTime), 1, CAST(N'2020-03-14T11:52:07.350' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Orange', 2, N'6446839b-7003-40fa-a42c-02fbbd6f7f5e-user7-128x128.jpg', 1, 0, 1, CAST(N'2020-03-15T04:37:29.493' AS DateTime), 1, CAST(N'2020-03-15T04:37:29.493' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'Red', 2, N'a160fbc8-576d-424d-b14d-797c414cd1a6-user8-128x128.jpg', 1, 0, 1, CAST(N'2020-03-15T04:37:56.307' AS DateTime), 1, CAST(N'2020-03-15T04:37:56.307' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'Abcd', 3, N'd6e5300f-a0df-4ba1-9867-1865c1702daf-photo3.jpg', 1, 0, 1, CAST(N'2020-03-15T04:39:25.743' AS DateTime), 1, CAST(N'2020-03-15T04:39:25.743' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'WC SHEET COVER', 4, N'8f7ba049-6a9c-489f-acd7-8758b31a301e-2.jpg', 1, 0, 1, CAST(N'2020-03-15T07:31:45.240' AS DateTime), 1, CAST(N'2020-03-15T07:31:45.240' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'BIB COCK PTMT', 4, N'889cde85-a40f-4a74-8870-a8d9350363d7-3.jpg', 1, 0, 1, CAST(N'2020-03-15T07:32:34.427' AS DateTime), 1, CAST(N'2020-03-15T07:32:34.427' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'ELEGANT PTMT FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(N'2020-03-15T08:07:14.497' AS DateTime), 1, CAST(N'2020-03-15T08:07:14.497' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'PLUMBER FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(N'2020-03-15T08:07:37.373' AS DateTime), 1, CAST(N'2020-03-15T08:07:37.373' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'V-MAC FAUCETS NEW', 5, NULL, 1, 0, 1, CAST(N'2020-03-15T08:08:04.903' AS DateTime), 1, CAST(N'2020-03-15T08:08:04.903' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'PLATINUM', 7, N'ecfbe346-993a-4179-bcd9-dc362c589f4b-1.jpg', 1, 0, 1, CAST(N'2020-03-17T09:59:39.237' AS DateTime), 1, CAST(N'2020-03-17T09:59:39.237' AS DateTime))
INSERT [dbo].[tbl_Products] ([Product_Id], [ProductName], [CategoryId], [ProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'ROSE GOLD', 7, N'112ecfec-b1fc-49fc-b1a6-4c162c7fbc3f-3.jpg', 1, 0, 1, CAST(N'2020-03-17T09:59:55.627' AS DateTime), 1, CAST(N'2020-03-17T09:59:55.627' AS DateTime))
SET IDENTITY_INSERT [dbo].[tbl_Products] OFF
SET IDENTITY_INSERT [dbo].[tbl_SubProducts] ON 

INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (1, N'Xyz Sanitory', 1, 1, N'9345abb9-bea9-48b5-8a27-0b137abd6836-Shower2.jpg', 1, 0, 1, CAST(N'2020-03-14T11:52:36.927' AS DateTime), 1, CAST(N'2020-03-14T11:52:36.927' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (2, N'Orange Water Tank', 1, 2, NULL, 1, 0, 1, CAST(N'2020-03-14T11:57:01.380' AS DateTime), 1, CAST(N'2020-03-14T11:57:01.380' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (3, N'Orange 1', 2, 3, N'29dd06de-4f2f-4cbb-b775-ce05532d120d-avatar5.png', 1, 0, 1, CAST(N'2020-03-15T04:41:11.480' AS DateTime), 1, CAST(N'2020-03-15T04:41:11.480' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (4, N'EDGE PTMT FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(N'2020-03-15T08:08:39.390' AS DateTime), 1, CAST(N'2020-03-15T08:08:39.390' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (5, N'EDGE PRIME PTMT FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(N'2020-03-15T08:09:05.733' AS DateTime), 1, CAST(N'2020-03-15T08:09:05.733' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (6, N'SUPERB FAUCETS NEW', 5, 8, NULL, 1, 0, 1, CAST(N'2020-03-15T08:09:35.107' AS DateTime), 1, CAST(N'2020-03-15T08:09:35.107' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (7, N'AURA FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(N'2020-03-15T08:10:00.203' AS DateTime), 1, CAST(N'2020-03-15T08:10:00.203' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (8, N'CHORUS F FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(N'2020-03-15T08:10:26.607' AS DateTime), 1, CAST(N'2020-03-15T08:10:26.607' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (9, N'WAVE FAUCETS NEW', 5, 9, NULL, 1, 0, 1, CAST(N'2020-03-15T08:11:04.717' AS DateTime), 1, CAST(N'2020-03-15T08:11:04.717' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (10, N'TROY FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(N'2020-03-15T08:11:35.250' AS DateTime), 1, CAST(N'2020-03-15T08:11:35.250' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (11, N'PHILO FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(N'2020-03-15T08:12:05.233' AS DateTime), 1, CAST(N'2020-03-15T08:12:05.233' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (12, N'XERO FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(N'2020-03-15T08:12:27.013' AS DateTime), 1, CAST(N'2020-03-15T08:12:27.013' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (13, N'ORCHID FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(N'2020-03-15T08:13:55.717' AS DateTime), 1, CAST(N'2020-03-15T08:13:55.717' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (14, N'PALM FAUCETS NEW', 5, 10, NULL, 1, 0, 1, CAST(N'2020-03-15T08:14:19.623' AS DateTime), 1, CAST(N'2020-03-15T08:14:19.623' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (15, N'10GM', 7, 11, N'34b1ffd0-9586-4e1a-9c9b-5005eacef30f-dm.jpg', 1, 0, 1, CAST(N'2020-03-17T10:01:17.813' AS DateTime), 1, CAST(N'2020-03-17T10:01:17.813' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (16, N'20GM', 7, 11, N'1b08594a-3127-46c8-b669-af2acf7affe1-ext.jpg', 1, 0, 1, CAST(N'2020-03-17T10:01:41.767' AS DateTime), 1, CAST(N'2020-03-17T10:01:41.767' AS DateTime))
INSERT [dbo].[tbl_SubProducts] ([SubProductId], [SubProductName], [CategoryId], [ProductId], [SubProductImage], [IsActive], [IsDelete], [CreatedBy], [CreatedDate], [UpdatedBy], [UpdatedDate]) VALUES (17, N'25GM', 7, 12, N'5855dc1b-ed80-472b-9720-412ac3e6369e-te1.jpg', 1, 0, 1, CAST(N'2020-03-17T10:01:58.393' AS DateTime), 1, CAST(N'2020-03-17T10:01:58.393' AS DateTime))
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
