using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_06.Models;

using PagedList;
using PagedList.Mvc;
namespace WEB_06.Controllers
{
    public class WEB_06Controller : Controller
    {
        QLSPDataContext Data = new QLSPDataContext();
        private List<DONGHO> Laydonghomoi(int count)
        {
            return Data.DONGHOs.OrderByDescending(a => a.Ngaycapnhat).Take(count).ToList();
        }
        public ActionResult Index(int ? page)
        {
            int pageSize = 6; //tạo số sản phẩm trên trang
            int pageNum = (page ?? 1); // tạo phép gán nếu nó không có giá trị thì sẽ mặc định gán cho ta bằng 1
            var donghomoi = Laydonghomoi(20); // tạo view để lưu lại laydonghomoi
            return View(donghomoi.ToPagedList(pageNum, pageSize)); // trả về biến model phân trang topaselist
        }     
        public ActionResult LoaiSP()
        {
            var LoaiSP = from cd in Data.LOAISPs select cd;
            return PartialView(LoaiSP);
        }
        public ActionResult ThuongHieu()
        {
            var ThuongHieu = from cd in Data.THUONGHIEUs select cd;
            return PartialView(ThuongHieu);
        }
        public ActionResult SPTheoLoaiSP(int id, int ? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var Dongho = from s in Data.DONGHOs where s.MaLoai == id select s;
            return View(Dongho.ToPagedList(pageNum, pageSize));
        }
        public ActionResult SPTheoThuongHieu(int id, int ? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var Dongho = from s in Data.DONGHOs where s.MaTH == id select s;
            return View(Dongho.ToPagedList(pageNum, pageSize));
        }
        public ActionResult Details(int id)
        {
            var dongho = from s in Data.DONGHOs
                       where s.Madongho == id
                       select s;
            return View(dongho.Single());
        }
    }
}