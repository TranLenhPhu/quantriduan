using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEB_06.Models
{
    public class Giohang
    {
        QLSPDataContext data = new QLSPDataContext();
        public int iMadongho { set; get; }
        public string sTendongho { set; get; }
        public string sAnhbia { set; get; }
        public Double dDongia { set; get; }
        public int iSoluong { set; get; }
        public Double dThanhtien
        {
            get { return iSoluong * dDongia; }
        }
        public Giohang(int Madongho)
        {
            iMadongho = Madongho;
            DONGHO dongho = data.DONGHOs.Single(n => n.Madongho == iMadongho);
            sTendongho = dongho.Tendongho;
            sAnhbia = dongho.Anhbia;
            dDongia = double.Parse(dongho.Giaban.ToString());
            iSoluong = 1;
        }
    }
}