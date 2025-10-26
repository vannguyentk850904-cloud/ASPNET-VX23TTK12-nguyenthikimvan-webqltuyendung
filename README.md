# 🧩 ASPNET-VX23TTK12-NguyenThiKimVan-WebQLTuyenDung  
### Recruitment Management MVC  

Ứng dụng Web **quản lý tuyển dụng** được xây dựng trên **ASP.NET MVC Framework**, cho phép:  

- 🏢 Nhà tuyển dụng đăng tin tuyển dụng  
- 👨‍🎓 Ứng viên đăng ký tài khoản và ứng tuyển  
- 👑 Quản trị viên duyệt tài khoản nhà tuyển dụng  

---

## 🚀 Công nghệ sử dụng  

| Công nghệ | Phiên bản |
|-----------|-----------|
| ASP.NET MVC | 5.2.9 |
| .NET Framework | 4.8 |
| Entity Framework | 6.5.1 |
| SQL Server Express | ✅ |
| Bootstrap UI | 5.2.3 |

---

## 📌 Yêu cầu chạy dự án  

- Visual Studio 2022  
- .NET Framework Developer Pack 4.8  
- SQL Server Express (hoặc LocalDB nếu tự tạo CSDL)  
- Quyền tạo database trong SQL Server  

---

## 📥 Cài đặt & Khởi chạy  

### 1️⃣ Clone dự án  

```bash
git clone https://github.com/your-repo/RecruitmentManagementMVC.git
```

Hoặc tải file ZIP và giải nén.  
Mở file `RecruitmentManagementMVC.sln` bằng **Visual Studio 2022**.

---

### 2️⃣ Khôi phục NuGet Packages  

Trong Visual Studio:  
```
Tools → NuGet Package Manager → Restore NuGet Packages
```

---

### 3️⃣ Kết nối cơ sở dữ liệu  

Tùy nhu cầu, bạn có thể **chạy trực tiếp với CSDL online** hoặc **tạo CSDL riêng trên máy**.

---

#### 🔹 Trường hợp 1: Dùng sẵn CSDL Online (Somee.com ✅)  

Nếu bạn chỉ muốn chạy web **ngay lập tức**, không cần tạo DB, hãy giữ nguyên cấu hình sau trong file `Web.config`:

```xml
<connectionStrings>
  <add name="RecruitmentDbEntities"
       connectionString="metadata=res://*/Models.RecruitmentModel.csdl|res://*/Models.RecruitmentModel.ssdl|res://*/Models.RecruitmentModel.msl;
       provider=System.Data.SqlClient;
       provider connection string=&quot;data source=SQL5108.site4now.net;
       initial catalog=db_aadf65_recruitment2025;
       user id=db_aadf65_recruitment2025_admin;
       password=yourpassword;
       MultipleActiveResultSets=True;
       App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

> 👉 Kết nối này dùng **CSDL online** đang hoạt động trên Somee.com.  
> Bạn chỉ cần tải source về là chạy được ngay.

---

#### 🔹 Trường hợp 2: Tạo cơ sở dữ liệu riêng (Local SQL Server)  

Nếu bạn muốn chạy độc lập trên máy:  

1. Mở **SQL Server Management Studio (SSMS)**  
2. Chạy file **`RecruitmentDb.sql`** có sẵn trong dự án  
   → Tạo database tên: `RecruitmentDb`  
3. Cập nhật `connectionString` trong **`Web.config`**:  

```xml
<connectionStrings>
  <add name="RecruitmentDbEntities"
       connectionString="metadata=res://*/Models.RecruitmentModel.csdl|res://*/Models.RecruitmentModel.ssdl|res://*/Models.RecruitmentModel.msl;
       provider=System.Data.SqlClient;
       provider connection string=&quot;data source=DESKTOP-ABC123\SQLEXPRESS;
       initial catalog=RecruitmentDb;
       integrated security=True;
       encrypt=False;
       MultipleActiveResultSets=True;
       App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

> ⚙️ Thay `DESKTOP-ABC123\SQLEXPRESS` bằng tên SQL Server thật của bạn.  
> (Kiểm tra bằng SSMS → Connect → Server Name.)

---

### 4️⃣ Chạy ứng dụng  

- Trong **Solution Explorer**, click phải vào dự án `RecruitmentManagementMVC`  
  → Chọn **Set as Startup Project**  
- Nhấn **F5** hoặc **Ctrl + F5** để chạy web  

Mặc định chạy tại:  
```
http://localhost:<port>/
```

---

## 🔑 Tài khoản đăng nhập mẫu  

### 👑 Quản trị viên (Admin)  

| Email | Mật khẩu |
|------|:------:|
| admin@gmail.com | admin123 |

### 🏢 Nhà tuyển dụng (Employer)  

| Email | Mật khẩu |
|------|:------:|
| ungvien1@gmail.com | 123456 |
| ungvien2@gmail.com | 123456 |

### 👨‍🎓 Ứng viên (Candidate)  

| Email | Mật khẩu |
|------|:------:|
| nhatuyendung1@gmail.com | 123456 |
| nhatuyendung2@gmail.com | 123456 |

---

## ✅ Chức năng chính  

| Vai trò | Quyền hạn |
|--------|-----------|
| **Admin** | Duyệt tài khoản, quản lý người dùng và dữ liệu |
| **Employer** | Đăng bài tuyển dụng khi được duyệt |
| **Candidate** | Nộp đơn ứng tuyển các công việc |

---

## 🐞 Lỗi thường gặp & Cách xử lý  

| Lỗi | Cách khắc phục |
|-----|----------------|
| Không kết nối được DB | Kiểm tra lại `connectionString` |
| Lỗi CHECK constraint Role | Role hợp lệ: Admin, Employer, Candidate |
| HTTP 500 khi đăng ký | Chưa chạy đúng file SQL |
| Đăng nhập sai dù đúng mật khẩu | Employer chưa được duyệt (`IsApproved = false`) |

---

## 📂 Cấu trúc thư mục dự án  

```
RecruitmentManagementMVC/
├── Controllers/
├── Models/
├── Views/
├── Scripts/
├── App_Start/
└── Web.config
```

---

## 🌐 Website demo  

Nếu bạn không thể chạy dự án cục bộ, có thể truy cập bản online tại:  

👉 **http://recruitment2025.somee.com/**  

---

## 🧠 Ghi chú  

- Một số trình duyệt có thể hiển thị cảnh báo “Không bảo mật” do **Somee.com bản miễn phí không hỗ trợ HTTPS**.  
- Đây chỉ là **cảnh báo giao thức**, không ảnh hưởng chức năng website.  

---

🎯 **Chúc bạn chạy web thành công và bảo vệ đồ án thuận lợi! 
Nếu có vấn đề gì liên hệ mail: vanntk040985@sv-onuni.edu.vn hoặc vannguyentk850904@gmail.com
**
