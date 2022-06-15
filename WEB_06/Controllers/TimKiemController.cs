using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WEB_06.Models;

using PagedList.Mvc;
using PagedList;

namespace WEB_06.Controllers
{
    public class TimKiemController : Controller
    {
        QLSPDataContext Data = new QLSPDataContext();
       
        // GET: TimKiem
        [HttpGet]
        public ActionResult KetQuaTimKiem(string sTuKhoa, int? page)
        {
            ViewBag.TuKhoa = sTuKhoa;
            List<DONGHO> lstKQTK = Data.DONGHOs.Where(n => n.Tendongho.Contains(sTuKhoa)).ToList(); // tìm kiếm khi nào trùng với tên sản phẩm chứa từ khóa
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if (lstKQTK.Count == null)
            {
                ViewBag.Thongbao = "Không tìm thấy sản phẩm nào ";
                return View(Data.DONGHOs.OrderBy(n => n.Tendongho).ToPagedList(pageNumber, pageSize));//trả về model phân trang
            }
            ViewBag.Thongbao = "Đã tìm thấy  " + lstKQTK.Count + "  kết quả!";
            return View(lstKQTK.OrderBy(n => n.Tendongho).ToPagedList(pageNumber, pageSize));//hiển thị tên đồng hồ theo chiều tăng dần
        }
        [HttpPost]
        public ActionResult KetQuaTimKiem(FormCollection f, int ? page)
        {
            string sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            List<DONGHO> lstKQTK = Data.DONGHOs.Where(n => n.Tendongho.Contains(sTuKhoa)).ToList();
            int pageNumber = (page ?? 1);
            int pageSize = 6;
            if(lstKQTK.Count==null)
            {
                ViewBag.Thongbao = "Không tìm thấy sản phẩm nào ";
                return View(Data.DONGHOs.OrderBy(n=>n.Tendongho).ToPagedList(pageNumber,pageSize));

            }
            ViewBag.Thongbao = "Đã tìm thấy  " + lstKQTK.Count + "  kết quả!";
            return View(lstKQTK.OrderBy(n => n.Tendongho).ToPagedList(pageNumber, pageSize));
        }
       
    }
}