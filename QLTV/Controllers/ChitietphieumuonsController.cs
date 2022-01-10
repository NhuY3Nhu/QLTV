using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QLTV.Models;

namespace QLTV.Controllers
{
    public class ChitietphieumuonsController : Controller
    {
        private qlthuvien db = new qlthuvien();

        // GET: Chitietphieumuons
        public ActionResult Index(string maphieu = null, string Maphieu = null)
        {

            var chitietphieumuons = db.Chitietphieumuons.Include(c => c.Sach).Where(x => x.Maphieu == maphieu);

            //Hiện mã và tên độc giả của chi tiết phiếu mượn
            using (qlthuvien db = new qlthuvien())
            {
                List<Docgia> docgia = db.Docgias.ToList();
                List<Phieumuon> phieumuon = db.Phieumuons.ToList();
                var main = from h in phieumuon
                           join k in docgia on h.Madg equals k.Madg
                           where (h.Maphieu == Maphieu)
                           select new ViewModel
                           {
                               phieumuon = h,
                               docgia = k
                           };
                //truyền đối tượng trên sang View
                ViewBag.Main = main;
                return View(chitietphieumuons.ToList());
            }    
        }

        // GET: Chitietphieumuons/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chitietphieumuon chitietphieumuon = db.Chitietphieumuons.Find(id);
            if (chitietphieumuon == null)
            {
                return HttpNotFound();
            }
            return View(chitietphieumuon);
        }

        // GET: Chitietphieumuons/Create
        public ActionResult Create()
        {
            var ketqua = db.Chitietphieumuons.OrderByDescending(p => p.MaCTPM);//Duyệt các mã chi tiết phiếu mượn
            string str = "";
            foreach (var item in ketqua)//Vòng lặp
            {
                str = item.MaCTPM;//Gán biến tạm cho chuỗi rỗng
                break;
            }
            string[] arrListStr = str.Split('T');//Tạo mảng chứa chuỗi đã được cắt
            int s = Convert.ToInt32(arrListStr[1]);//Lấy mảng vị trí thứ 1
            return View(new Chitietphieumuon() { MaCTPM = "CT" + (s + 1) });//Hiện chi tiết phiếu mượn vs mã tự tăng
        }

        //
        public void Messagebox(string xMessage)
        {
            Response.Write("<script>alert('" + xMessage + "')</script>");//Sửa dụng javascript để hiện bảng thông báo thông qua alert
        }
        // POST: Chitietphieumuons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaCTPM,Maphieu,Masach")] Chitietphieumuon chitietphieumuon)
        {

            if (ModelState.IsValid)//Kiểm tra tính hợp lệ của cơ sở dữ liệu
            {
                db.Chitietphieumuons.Add(chitietphieumuon);Thêm chi tiết phiếu mượn
                var sachs = db.Saches.Find(chitietphieumuon.Masach);//Xuống database tìm mã sách
                if (sachs.Soluong > 1)//nếu số lượng nhiều hơn 1 thì
                {
                    sachs.Soluong = sachs.Soluong - 1;//Số lượng sách giảm 1
                    db.SaveChanges();//Lưu lại trong database
                    
                    return RedirectToAction("Index");//quay trở lại trang Index

                }
                else
                {
                    Messagebox("Thông báo: Sách này đã cho mượn hết !");
                }
            }
            ViewBag.Maphieu = new SelectList(db.Phieumuons, "Maphieu", "Madg", chitietphieumuon.Maphieu);
            ViewBag.Masach = new SelectList(db.Saches, "Masach", "Tensach", chitietphieumuon.Masach);
            return View();// nếu không thể thêm thì trả về View như cũ
        }

        // GET: Chitietphieumuons/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chitietphieumuon chitietphieumuon = db.Chitietphieumuons.Find(id);//Tìm liên kết database theo id tương ứng
            if (chitietphieumuon == null)
            {
                return HttpNotFound();//Nếu không tìm thấy hiển thị kết quả không thấy
            }
            ViewBag.Maphieu = new SelectList(db.Phieumuons, "Maphieu", "Madg", chitietphieumuon.Maphieu);
            ViewBag.Masach = new SelectList(db.Saches, "Masach", "Tensach", chitietphieumuon.Masach);
            return View(chitietphieumuon);//Hiển thị nội dung
        }

        // POST: Chitietphieumuons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaCTPM,Maphieu,Masach")] Chitietphieumuon chitietphieumuon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chitietphieumuon).State = EntityState.Modified;//Chọn trạng thái thay đổi
                db.SaveChanges();
                return RedirectToAction("Index");//Sau khi cập nhật thì quay về trang chủ index
            }
            ViewBag.Maphieu = new SelectList(db.Phieumuons, "Maphieu", "Madg", chitietphieumuon.Maphieu);
            ViewBag.Masach = new SelectList(db.Saches, "Masach", "Tensach", chitietphieumuon.Masach);
            return View(chitietphieumuon);
        }

        // GET: Chitietphieumuons/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Chitietphieumuon chitietphieumuon = db.Chitietphieumuons.Find(id);
            if (chitietphieumuon == null)
            {
                return HttpNotFound();
            }
            return View(chitietphieumuon);
        }

        // POST: Chitietphieumuons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Chitietphieumuon chitietphieumuon = db.Chitietphieumuons.Find(id);//xuống database lấy chi tiết phiếu mượn theo id
            var sachs = db.Saches.Find(chitietphieumuon.Masach); //xuống database sách mượn theo mã sách trong chi tiết phiếu mượn
            sachs.Soluong = sachs.Soluong + 1;//Số lượng sách được cập nhật +1
            db.Chitietphieumuons.Remove(chitietphieumuon);//Xóa chi tiết phiếu mượn trong database
            db.SaveChanges();//lưu thay đổi
            return RedirectToAction("Index");//Hiện trang index đã thay đổi.
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();//Giải phóng tài nguyên không được quản lý
            }
            base.Dispose(disposing);
        }
    }
}
