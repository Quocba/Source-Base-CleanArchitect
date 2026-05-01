using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Config
{
    public class PolicyConfig
    {
        public static class ToanQuyen
        {
            public const string VIEW = "TOANQUYEN.VIEW";
            public const string CREATE = "TOANQUYEN.CREATE";
            public const string EDIT = "TOANQUYEN.EDIT";
            public const string DELETE = "TOANQUYEN.DELETE";
            public const string PRINT = "TOANQUYEN.PRINT";
            public const string SEND = "TOANQUYEN.SEND";
        }
        public static class ThongKe
        {
            public const string VIEW = "THONGKE.VIEW";
            public const string CREATE = "THONGKE.CREATE";
            public const string EDIT = "THONGKE.EDIT";
            public const string DELETE = "THONGKE.DELETE";
            public const string PRINT = "THONGKE.PRINT";
            public const string SEND = "THONGKE.SEND";
        }
        public static class PhieuNhapKho
        {
            public const string VIEW = "PHIEUNHAPKHO.VIEW";
            public const string CREATE = "PHIEUNHAPKHO.CREATE";
            public const string EDIT = "PHIEUNHAPKHO.EDIT";
            public const string DELETE = "PHIEUNHAPKHO.DELETE";
            public const string PRINT = "PHIEUNHAPKHO.PRINT";
            public const string SEND = "PHIEUNHAPKHO.SEND";
        }
        public static class QuanLyNguoiDung
        {
            public const string VIEW = "QUANLYNGUOIDUNG.VIEW";
            public const string CREATE = "QUANLYNGUOIDUNG.CREATE";
            public const string EDIT = "QUANLYNGUOIDUNG.EDIT";
            public const string DELETE = "QUANLYNGUOIDUNG.DELETE";
            public const string PRINT = "QUANLYNGUOIDUNG.PRINT";
            public const string SEND = "QUANLYNGUOIDUNG.SEND";
        }

        public static class ChuyenKhoanNoiBo
        {
            public const string VIEW = "CHUYENKHOANNOIBO.VIEW";
            public const string CREATE = "CHUYENKHOANNOIBO.CREATE";
            public const string EDIT = "CHUYENKHOANNOIBO.EDIT";
            public const string DELETE = "CHUYENKHOANNOIBO.DELETE";
            public const string PRINT = "CHUYENKHOANNOIBO.PRINT";
            public const string SEND = "CHUYENKHOANNOIBO.SEND";
        }
        public static class BaoGia
        {
            public const string VIEW = "BAOGIA.VIEW";
            public const string CREATE = "BAOGIA.CREATE";
            public const string EDIT = "BAOGIA.EDIT";
            public const string DELETE = "BAOGIA.DELETE";
            public const string PRINT = "BAOGIA.PRINT";
            public const string SEND = "BAOGIA.SEND";
        }


        // 🔹 Hợp Đồng
        public static class HopDong
        {
            public const string VIEW = "HOPDONG.VIEW";
            public const string CREATE = "HOPDONG.CREATE";
            public const string EDIT = "HOPDONG.EDIT";
            public const string DELETE = "HOPDONG.DELETE";
            public const string PRINT = "HOPDONG.PRINT";
            public const string SEND = "HOPDONG.SEND";
        }

        // 🔹 Nhóm Sản Phẩm
        public static class NhomSanPham
        {
            public const string VIEW = "NHOMSANPHAM.VIEW";
            public const string CREATE = "NHOMSANPHAM.CREATE";
            public const string EDIT = "NHOMSANPHAM.EDIT";
            public const string DELETE = "NHOMSANPHAM.DELETE";
            public const string PRINT = "NHOMSANPHAM.PRINT";
            public const string SEND = "NHOMSANPHAM.SEND";
        }

        // 🔹 Sản Phẩm
        public static class SanPham
        {
            public const string VIEW = "SANPHAM.VIEW";
            public const string CREATE = "SANPHAM.CREATE";
            public const string EDIT = "SANPHAM.EDIT";
            public const string DELETE = "SANPHAM.DELETE";
            public const string PRINT = "SANPHAM.PRINT";
            public const string SEND = "SANPHAM.SEND";
        }

        // 🔹 Đơn Vị Tính
        public static class DonViTinh
        {
            public const string VIEW = "DONVITINH.VIEW";
            public const string CREATE = "DONVITINH.CREATE";
            public const string EDIT = "DONVITINH.EDIT";
            public const string DELETE = "DONVITINH.DELETE";
            public const string PRINT = "DONVITINH.PRINT";
            public const string SEND = "DONVITINH.SEND";
        }

        // 🔹 Khách Hàng
        public static class KhachHang
        {
            public const string VIEW = "KHACHHANG.VIEW";
            public const string CREATE = "KHACHHANG.CREATE";
            public const string EDIT = "KHACHHANG.EDIT";
            public const string DELETE = "KHACHHANG.DELETE";
            public const string PRINT = "KHACHHANG.PRINT";
            public const string SEND = "KHACHHANG.SEND";
        }

        // 🔹 Phòng Ban
        public static class PhongBan
        {
            public const string VIEW = "PHONGBAN.VIEW";
            public const string CREATE = "PHONGBAN.CREATE";
            public const string EDIT = "PHONGBAN.EDIT";
            public const string DELETE = "PHONGBAN.DELETE";
            public const string PRINT = "PHONGBAN.PRINT";
            public const string SEND = "PHONGBAN.SEND";
        }

        // 🔹 Chức Vụ
        public static class ChucVu
        {
            public const string VIEW = "CHUCVU.VIEW";
            public const string CREATE = "CHUCVU.CREATE";
            public const string EDIT = "CHUCVU.EDIT";
            public const string DELETE = "CHUCVU.DELETE";
            public const string PRINT = "CHUCVU.PRINT";
            public const string SEND = "CHUCVU.SEND";
        }

        // 🔹 Nhân Viên
        public static class NhanVien
        {
            public const string VIEW = "NHANVIEN.VIEW";
            public const string CREATE = "NHANVIEN.CREATE";
            public const string EDIT = "NHANVIEN.EDIT";
            public const string DELETE = "NHANVIEN.DELETE";
            public const string PRINT = "NHANVIEN.PRINT";
            public const string SEND = "NHANVIEN.SEND";
        }

        // 🔹 Nhà Cung Cấp
        public static class NhaCungCap
        {
            public const string VIEW = "NHACUNGCAP.VIEW";
            public const string CREATE = "NHACUNGCAP.CREATE";
            public const string EDIT = "NHACUNGCAP.EDIT";
            public const string DELETE = "NHACUNGCAP.DELETE";
            public const string PRINT = "NHACUNGCAP.PRINT";
            public const string SEND = "NHACUNGCAP.SEND";
        }

        // 🔹 Tài Khoản
        public static class TaiKhoan
        {
            public const string VIEW = "TAIKHOAN.VIEW";
            public const string CREATE = "TAIKHOAN.CREATE";
            public const string EDIT = "TAIKHOAN.EDIT";
            public const string DELETE = "TAIKHOAN.DELETE";
            public const string PRINT = "TAIKHOAN.PRINT";
            public const string SEND = "TAIKHOAN.SEND";
        }

        // 🔹 Phiếu Thu
        public static class PhieuThu
        {
            public const string VIEW = "PHIEUTHU.VIEW";
            public const string CREATE = "PHIEUTHU.CREATE";
            public const string EDIT = "PHIEUTHU.EDIT";
            public const string DELETE = "PHIEUTHU.DELETE";
            public const string PRINT = "PHIEUTHU.PRINT";
            public const string SEND = "PHIEUTHU.SEND";
        }

        // 🔹 Phiếu Chi
        public static class PhieuChi
        {
            public const string VIEW = "PHIEUCHI.VIEW";
            public const string CREATE = "PHIEUCHI.CREATE";
            public const string EDIT = "PHIEUCHI.EDIT";
            public const string DELETE = "PHIEUCHI.DELETE";
            public const string PRINT = "PHIEUCHI.PRINT";
            public const string SEND = "PHIEUCHI.SEND";
        }

        // 🔹 Phiếu Xuất Kho
        public static class PhieuXuatKho
        {
            public const string VIEW = "PHIEUXUATKHO.VIEW";
            public const string CREATE = "PHIEUXUATKHO.CREATE";
            public const string EDIT = "PHIEUXUATKHO.EDIT";
            public const string DELETE = "PHIEUXUATKHO.DELETE";
            public const string PRINT = "PHIEUXUATKHO.PRINT";
            public const string SEND = "PHIEUXUATKHO.SEND";
        }

        // 🔹 Thông Tin Công Ty
        public static class ThongTinCongTy
        {
            public const string VIEW = "THONGTINCONGTY.VIEW";
            public const string CREATE = "THONGTINCONGTY.CREATE";
            public const string EDIT = "THONGTINCONGTY.EDIT";
            public const string DELETE = "THONGTINCONGTY.DELETE";
            public const string PRINT = "THONGTINCONGTY.PRINT";
            public const string SEND = "THONGTINCONGTY.SEND";
        }

        // 🔹 Quy Đổi Đơn Vị Tính
        public static class QuyDoiDonViTinh
        {
            public const string VIEW = "QUYDOIDONVITINH.VIEW";
            public const string CREATE = "QUYDOIDONVITINH.CREATE";
            public const string EDIT = "QUYDOIDONVITINH.EDIT";
            public const string DELETE = "QUYDOIDONVITINH.DELETE";
            public const string PRINT = "QUYDOIDONVITINH.PRINT";
            public const string SEND = "QUYDOIDONVITINH.SEND";
        }

        // 🔹 Phân Quyền
        public static class PhanQuyen
        {
            public const string VIEW = "PHANQUYEN.VIEW";
            public const string CREATE = "PHANQUYEN.CREATE";
            public const string EDIT = "PHANQUYEN.EDIT";
            public const string DELETE = "PHANQUYEN.DELETE";
            public const string PRINT = "PHANQUYEN.PRINT";
            public const string SEND = "PHANQUYEN.SEND";
        }
    }
}
