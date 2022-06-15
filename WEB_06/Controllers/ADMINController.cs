using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_06.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace WEB_06.Controllers
{
    public class ADMINController : Controller
    {
        QLSPDataContext db = new QLSPDataContext();
        // GET: ADMIN
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Login(FormCollection collection)
        {
            var tendn = collection["username"];
            var matkhau = collection["password"];
            if (String.IsNullOrEmpty(tendn))
            {
                ViewData["Loi1"] = " Phải nhập tên đăng nhập :";

            }
            else if (String.IsNullOrEmpty(matkhau))
            {
                ViewData["Loi2"] = "Phải nhập mật khẩu :";
            }
            else
            {
                Admin ad = db.Admins.SingleOrDefault(n => n.UserAdmin == tendn && n.PassAdmin == matkhau);
                if (ad != null)
                {
                    Session["UserAdmin"] = ad;
                    return RedirectToAction("Index", "ADMIN");
                }
                else
                {
                    ViewBag.Thongbao = " Tên đăng nhập hoặc mật khẩu không đúng!!!";
                }
            }
            return View();       
        }


        public ActionResult Dongho(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            return View(db.DONGHOs.ToList().OrderBy(n => n.Madongho).ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Themmoidongho()
        {
            // load dropdownlist mã loại và mã thương hiệu 
            ViewBag.MaLoai = new SelectList(db.LOAISPs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoidongho(DONGHO dongho, HttpPostedFileBase fileupload) // truyền tham số HttpPostedFileBase để nhận thông tin giá trị hình ảnh
        {
            ViewBag.MaLoai = new SelectList(db.LOAISPs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View();
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);// lấy tên hình ảnh 
                    var path = Path.Combine(Server.MapPath("~/ImagesSP"), fileName);// lấy hình ảnh chuyển vào thư viện
                    if (System.IO.File.Exists(path))// nếu thư mục đó chứa hình ảnh rồi thì xuất ra thông báo
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại !";
                    }
                    //lấy hình ảnh đưa vào thư mục ha
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    dongho.Anhbia = fileName;
                    db.DONGHOs.InsertOnSubmit(dongho);
                    db.SubmitChanges();
                }
                return RedirectToAction("Dongho");
            }

        }
        public ActionResult ChiTietDongho(int id) //truyền 1 pagarester lên router đó là id
        {
            DONGHO dongho = db.DONGHOs.SingleOrDefault(n => n.Madongho == id); // tìm phần tử là mã đồng hồ  
            ViewBag.Madongho = dongho.Madongho; // cho biết ta lấy mã đồng hồ ở bảng đồng hồ
            if (dongho == null) 
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongho); // nếu tồn tại sẽ truyền dongho vào trong model
        }
        [HttpGet]
        public ActionResult XoaDongho(int id)
        {
            DONGHO dongho = db.DONGHOs.SingleOrDefault(n => n.Madongho == id);
            ViewBag.Madongho = dongho.Madongho;
            if (dongho == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dongho);
        }
        [HttpPost, ActionName("XoaDongho")]
        public ActionResult Xacnhanxoa(int id)
        {
            DONGHO dongho = db.DONGHOs.SingleOrDefault(n => n.Madongho == id);
            ViewBag.Madongho = dongho.Madongho;
            if (dongho == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.DONGHOs.DeleteOnSubmit(dongho);
            db.SubmitChanges();// lưu toàn bộ về database
            return RedirectToAction("Dongho");// chuyển về trang index
        }
        [HttpGet]
        public ActionResult SuaDongho(int id)
        {
            DONGHO dongho = db.DONGHOs.SingleOrDefault(n => n.Madongho == id);
            if (dongho == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            //Lay danh sach tu table chu de, sap xep tang dan theo ten chu de , chon lay gia tri MaLoai,TenLoai và hien thi MaLoai
            ViewBag.MaLoai = new SelectList(db.LOAISPs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai", dongho.MaLoai);
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH", dongho.MaTH);
            return View(dongho);
        }
        [HttpPost, ActionName("SuaDongho")]
        [ValidateInput(false)]
        public ActionResult SuaDongho(DONGHO dongho, HttpPostedFileBase fileupload)
        {
            ViewBag.MaLoai = new SelectList(db.LOAISPs.ToList().OrderBy(n => n.TenLoai), "MaLoai", "TenLoai");
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            if (fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                return View(dongho);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var fileName = Path.GetFileName(fileupload.FileName);
                    var path = Path.Combine(Server.MapPath("~/ImagesSP"), fileName);
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại !";
                    }
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    dongho.Anhbia = fileName;
                    DONGHO sa = db.DONGHOs.SingleOrDefault(s => s.Madongho == dongho.Madongho);
                    sa.Anhbia = dongho.Anhbia;
                    sa.Tendongho = dongho.Tendongho;
                    sa.Mota = dongho.Mota;
                    sa.Ngaycapnhat = dongho.Ngaycapnhat;
                    sa.Soluongton = dongho.Soluongton;
                    sa.MaLoai = dongho.MaLoai;
                    sa.MaTH = dongho.MaTH;
                    sa.Giaban = dongho.Giaban;
                    db.SubmitChanges();
                }
                return RedirectToAction("Dongho");
            }

        }
        public ActionResult Thuonghieu(int? page)
        {
            int pageNumer = (page ?? 1);
            int pageSize = 7;
            return View(db.THUONGHIEUs.ToList().OrderBy(n => n.MaTH).ToPagedList(pageNumer, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoithuonghieu()
        {
        
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoithuonghieu(THUONGHIEU thuonghieu)
        {
            //Dua du lieu vao Dropdownlist
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            db.THUONGHIEUs.InsertOnSubmit(thuonghieu);
            db.SubmitChanges();

            return RedirectToAction("Thuonghieu");
        }
        [HttpGet]
        public ActionResult Suathuonghieu(int id)
        {
            THUONGHIEU thuonghieu = db.THUONGHIEUs.SingleOrDefault(n => n.MaTH == id);
            ViewBag.MaTH = thuonghieu.MaTH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH", thuonghieu.MaTH);

            return View(thuonghieu);
        }
        [HttpPost, ActionName("Suathuonghieu")]
        [ValidateInput(false)]
        public ActionResult Suathuonghieu(THUONGHIEU thuonghieu)
        {
            //Dua du lieu vao Dropdownlist
            ViewBag.MaTH = new SelectList(db.THUONGHIEUs.ToList().OrderBy(n => n.TenTH), "MaTH", "TenTH");
            THUONGHIEU sa = db.THUONGHIEUs.SingleOrDefault(n => n.MaTH == thuonghieu.MaTH);
            sa.Diachi = thuonghieu.Diachi;
            sa.Dienthoai = thuonghieu.Dienthoai;

            db.SubmitChanges();
            return RedirectToAction("Thuonghieu");
        }
        [HttpGet]
        public ActionResult Xoathuonghieu(int id)
        {

            THUONGHIEU thuonghieu = db.THUONGHIEUs.SingleOrDefault(n => n.MaTH == id);
            ViewBag.MaTH = thuonghieu.MaTH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(thuonghieu);
        }
        [HttpPost, ActionName("Xoathuonghieu")]
        public ActionResult Xoathuonnghieu(int id)
        {

            THUONGHIEU thuonghieu = db.THUONGHIEUs.SingleOrDefault(n => n.MaTH == id);
            ViewBag.MaTH = thuonghieu.MaTH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.THUONGHIEUs.DeleteOnSubmit(thuonghieu);
            db.SubmitChanges();
            return RedirectToAction("Thuonghieu");
        }
        public ActionResult Chitietthuonghieu(int id)
        {

            THUONGHIEU thuonghieu = db.THUONGHIEUs.SingleOrDefault(n => n.MaTH == id);
            ViewBag.MaTH = thuonghieu.MaTH;
            if (thuonghieu == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(thuonghieu);
        }
        public ActionResult Khachhang(int? page)
        {
            int pageNumer = (page ?? 1);
            int pageSize = 7;
            return View(db.KHACHHANGs.ToList().OrderBy(n => n.MaKH).ToPagedList(pageNumer, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoikhachhang()
        {

            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoikhachhang(KHACHHANG khachhang)
        {

            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen");
            db.KHACHHANGs.InsertOnSubmit(khachhang);
            db.SubmitChanges();

            return RedirectToAction("Khachhang");
        }
        [HttpGet]
        public ActionResult Suakhachhang(int id)
        {
            KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachhang.MaKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen", khachhang.MaKH);

            return View(khachhang);
        }
        [HttpPost, ActionName("Suakhachhang")]
        [ValidateInput(false)]
        public ActionResult Suakhachhang(KHACHHANG khachhang)
        {
            //Dua du lieu vao Dropdownlist
            ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.HoTen), "MaKH", "HoTen");
            KHACHHANG sa = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == khachhang.MaKH);

            sa.Matkhau = khachhang.Matkhau;
            sa.DiachiKH = khachhang.DiachiKH;
            sa.Email = khachhang.Email;
            sa.DienthoaiKH = khachhang.DienthoaiKH;

            db.SubmitChanges();
            return RedirectToAction("Khachhang");
        }
        [HttpGet]
        public ActionResult Xoakhachhang(int id)
        {

            KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachhang.MaKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }
        [HttpPost, ActionName("Xoakhachhang")]
        public ActionResult Xoakhachhang1(int id)
        {

            KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachhang.MaKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.KHACHHANGs.DeleteOnSubmit(khachhang);
            db.SubmitChanges();
            return RedirectToAction("Khachhang");
        }
        public ActionResult Chitietkhachhang(int id)
        {

            KHACHHANG khachhang = db.KHACHHANGs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = khachhang.MaKH;
            if (khachhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachhang);
        }


        public ActionResult Dondathang(int? page)
        {
            int pageNumer = (page ?? 1);
            int pageSize = 7;
            return View(db.DONDATHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumer, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoidondathang()
        {

            ViewBag.MaDonHang = new SelectList(db.DONDATHANGs.ToList().OrderBy(n => n.MaKH), "MaDonHang", "MaKH");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoidondathang(DONDATHANG dondathang)
        {

            ViewBag.MaDonHang = new SelectList(db.DONDATHANGs.ToList().OrderBy(n => n.MaKH), "MaDonHang", "MaKH");
            db.DONDATHANGs.InsertOnSubmit(dondathang);
            db.SubmitChanges();

            return RedirectToAction("Dondathang");
        }
        [HttpGet]
        public ActionResult Suadondathang(int id)
        {
            DONDATHANG dondathang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = dondathang.MaDonHang;
            if (dondathang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            ViewBag.MaDonHang = new SelectList(db.DONDATHANGs.ToList().OrderBy(n => n.MaKH), "MaDonHang", "MaKH", dondathang.MaDonHang);

            return View(dondathang);
        }
        [HttpPost, ActionName("Suadondathang")]
        [ValidateInput(false)]
        public ActionResult Suadondathang(DONDATHANG dondathang)
        {
            //Dua du lieu vao Dropdownlist
            ViewBag.MaDonHang = new SelectList(db.DONDATHANGs.ToList().OrderBy(n => n.MaKH), "MaDonHang", "MaKH");
            DONDATHANG sa = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == dondathang.MaDonHang);

            sa.Dathanhtoan = dondathang.Dathanhtoan;
            sa.Tinhtranggiaohang = dondathang.Tinhtranggiaohang;
            sa.Ngaydat = dondathang.Ngaydat;
            sa.Ngaygiao = dondathang.Ngaygiao;



            db.SubmitChanges();
            return RedirectToAction("Dondathang");
        }
        [HttpGet]
        public ActionResult Xoadondathang(int id)
        {

            DONDATHANG dondathang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = dondathang.MaDonHang;
            if (dondathang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dondathang);
        }
        [HttpPost, ActionName("Xoadondathang")]
        public ActionResult Xoadondathang1(int id)
        {

            DONDATHANG dondathang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = dondathang.MaDonHang;
            if (dondathang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.DONDATHANGs.DeleteOnSubmit(dondathang);
            db.SubmitChanges();
            return RedirectToAction("Dondathang");
        }
        public ActionResult Chitietdondathang(int id)
        {

            DONDATHANG dondathang = db.DONDATHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = dondathang.MaDonHang;
            if (dondathang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(dondathang);
        }



        public ActionResult Chitietdonhang(int? page)
        {
            int pageNumer = (page ?? 1);
            int pageSize = 7;
            return View(db.CHITIETDONHANGs.ToList().OrderBy(n => n.MaDonHang).ToPagedList(pageNumer, pageSize));
        }
        [HttpGet]
        public ActionResult Themmoichitietdonhang()
        {

            ViewBag.MaDonHang = new SelectList(db.CHITIETDONHANGs.ToList().OrderBy(n => n.MaDongHo), "MaDongHo", "MaDonHang");
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Themmoichitietdonhang(CHITIETDONHANG chitietdonhang)
        {

            ViewBag.MaDonHang = new SelectList(db.CHITIETDONHANGs.ToList().OrderBy(n => n.MaDongHo), "MaDongHo", "MaDonHang");
            db.CHITIETDONHANGs.InsertOnSubmit(chitietdonhang);
            db.SubmitChanges();

            return RedirectToAction("Chitietdonhang");
        }
        [HttpGet]
        public ActionResult Suachitietdonhang(int id)
        {
            CHITIETDONHANG chitietdonhang = db.CHITIETDONHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = chitietdonhang.MaDonHang;
            if (chitietdonhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            //Dua du lieu vao Dropdownlist
            ViewBag.MaDonHang = new SelectList(db.CHITIETDONHANGs.ToList().OrderBy(n => n.MaDongHo), "MaDongHo", "MaDonHang", chitietdonhang.MaDonHang);

            return View(chitietdonhang);
        }
        [HttpPost, ActionName("Suakhachhang")]
        [ValidateInput(false)]
        public ActionResult Suachitietdonhang(CHITIETDONHANG chitietdonhang)
        {
            //Dua du lieu vao Dropdownlist
            ViewBag.MaDonHang = new SelectList(db.CHITIETDONHANGs.ToList().OrderBy(n => n.MaDongHo), "MaDongHo", "MaDonHang");
            CHITIETDONHANG sa = db.CHITIETDONHANGs.SingleOrDefault(n => n.MaDonHang == chitietdonhang.MaDonHang);

            sa.SoLuong = chitietdonhang.SoLuong;
            sa.DonGia = chitietdonhang.DonGia;



            db.SubmitChanges();
            return RedirectToAction("Chitietdonhang");
        }
        [HttpGet]
        public ActionResult Xoachitietdonhang(int id)
        {

            CHITIETDONHANG chitietdonhang = db.CHITIETDONHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = chitietdonhang.MaDonHang;
            if (chitietdonhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitietdonhang);
        }
        [HttpPost, ActionName("Xoadondathang")]
        public ActionResult Xoadondathang2(int id)
        {

            CHITIETDONHANG chitietdonhang = db.CHITIETDONHANGs.SingleOrDefault(n => n.MaDonHang == id);
            ViewBag.MaDonHang = chitietdonhang.MaDonHang;
            if (chitietdonhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.CHITIETDONHANGs.DeleteOnSubmit(chitietdonhang);
            db.SubmitChanges();
            return RedirectToAction("Chitietdonhang");
        }
        public ActionResult Xemchitietdonhang(int id)
        {

            CHITIETDONHANG chitietdonhang = db.CHITIETDONHANGs.SingleOrDefault(n => n.MaDongHo == id);
            ViewBag.MaDongHo = chitietdonhang.MaDongHo;
            if (chitietdonhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(chitietdonhang);
        }

        public ActionResult Phanquyen(int? page)
        {
            int pageNumer = (page ?? 1);
            int pageSize = 7;
            return View(db.Admins.ToList().OrderBy(n => n.ChucVu).ToPagedList(pageNumer, pageSize));
        }

    }
}