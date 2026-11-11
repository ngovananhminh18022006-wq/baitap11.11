CREATE DATABASE HRMDB;
GO
USE HRMDB;
GO

-- 1. Bảng Phòng ban
CREATE TABLE PhongBan (
    MaPB INT IDENTITY(1,1) PRIMARY KEY,
    TenPB NVARCHAR(100) NOT NULL,
    Mota NVARCHAR(255)
);

-- 2. Bảng Hồ sơ nhân viên
CREATE TABLE NhanVien (
    MaNV INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATETIME,
    CCCD VARCHAR(20) UNIQUE,
    MaPB INT FOREIGN KEY REFERENCES PhongBan(MaPB),
    LuongCoBan DECIMAL(18,2) CHECK (LuongCoBan > 0),
    HeSoLuong FLOAT DEFAULT 1.0
);

-- 3. Bảng Chấm công tháng
CREATE TABLE BangChamCong (
    MaCong INT IDENTITY(1,1) PRIMARY KEY,
    MaNV INT FOREIGN KEY REFERENCES NhanVien(MaNV),
    ThangNam VARCHAR(7), -- VD: '11/2026'
    SoNgayCong FLOAT CHECK (SoNgayCong <= 31),
    SoGioOT FLOAT DEFAULT 0
);

-- 4. Bảng Lương chi tiết
CREATE TABLE BangLuong (
    MaLuong INT IDENTITY(1,1) PRIMARY KEY,
    MaNV INT FOREIGN KEY REFERENCES NhanVien(MaNV),
    ThangNam VARCHAR(7),
    ThucLinh DECIMAL(18,2),
    CONSTRAINT UNQ_BangLuong UNIQUE(MaNV, ThangNam) -- Đảm bảo 1 NV chỉ có 1 bảng lương 1 tháng
);
