# ASPNET-VX23TTK12-nguyenthikimvan-webqltuyendung

# Recruitment Management MVC

Ứng dụng Web quản lý tuyển dụng được xây dựng trên ASP.NET MVC Framework:

- Nhà tuyển dụng đăng tin tuyển dụng
- Ứng viên đăng ký tài khoản và ứng tuyển
- Quản trị viên duyệt tài khoản tuyển dụng

---

## 🚀 Công nghệ sử dụng

| Công nghệ | Phiên bản |
|---------|-----------|
| ASP.NET MVC | 5.2.9 |
| .NET Framework | 4.8 |
| Entity Framework | 6.5.1 |
| SQL Server Express | ✅ |
| Bootstrap UI | 5.2.3 |

---

## 📌 Yêu cầu chạy dự án

- Visual Studio 2022
- .NET Framework Developer Pack 4.8
- SQL Server Express (hoặc LocalDB với chỉnh sửa connection string)
- Quyền tạo database trong SQL Server

---

## 📥 Cài đặt & Khởi chạy

### 1️⃣ Clone dự án

```
git clone https://github.com/your-repo/RecruitmentManagementMVC.git
```

Hoặc tải file ZIP và giải nén.

Mở file `RecruitmentManagementMVC.sln` bằng Visual Studio 2022.

---

### 2️⃣ Khôi phục NuGet Packages

Trong Visual Studio:

```
Tools → NuGet Package Manager → Restore NuGet Packages
```

Xong sẽ không còn lỗi thiếu thư viện.

---

### 3️⃣ Tạo cơ sở dữ liệu

Mở SQL Server Management Studio (SSMS):

- Chạy file **`RecruitmentDb.sql`**
- Đảm bảo database tên đúng: `RecruitmentDb`

---

### 4️⃣ Kiểm tra Connection String

Trong `Web.config`:

```xml
<connectionStrings>
  <add name="RecruitmentDbEntities"
       connectionString="metadata=res://*/Models.RecruitmentModel.csdl|res://*/Models.RecruitmentModel.ssdl|res://*/Models.RecruitmentModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=ANONYMOUS\SQLEXPRESS01;initial catalog=RecruitmentDb;integrated security=True;encrypt=False;MultipleActiveResultSets=True;App=EntityFramework&quot;" 
       providerName="System.Data.EntityClient" />
</connectionStrings>
```

Nếu SQL server khác tên, đổi phần:

```
data source= TÊN_MÁY\TÊN_SQL_INSTANCE
```

VD:  
`data source=DESKTOP-ABC123\SQLEXPRESS`

---


### 5️⃣ Chạy web

Chọn dự án:

```
RecruitmentManagementMVC → Set as Startup Project
```

Chạy:

```
F5 hoặc Ctrl + F5
```

Ứng dụng chạy tại:

```
http://localhost:<port>/
```

---

## 🔑 Tài khoản đăng nhập mẫu

### 👑 Admin

| Email | Mật khẩu |
|------|:------:|
| admin@gmail.com | admin123 |

---

### 🏢 Nhà tuyển dụng

| Email | Mật khẩu |
|------|:------:|
| camxucgoc12@gmail.com | 123456 |
| abc@gmail.com | 123456 |

---

### 👨‍🎓 Ứng viên

| Email | Mật khẩu |
|------|:------:|
| vana@gmail.com | 123456 |
| camxucgoc0@gmail.com | 123456 |

---

## ✅ Chức năng chính

| Vai trò | Quyền hạn |
|--------|-----------|
| Admin | Duyệt tài khoản, quản lý toàn bộ người dùng và dữ liệu |
| Employer | Đăng bài tuyển dụng khi được duyệt |
| Candidate | Nộp đơn ứng tuyển vào các công việc |

---


## 🐞 Lỗi thường gặp & cách xử lý

| Lỗi | Cách sửa |
|-----|----------|
| Không kết nối DB | Kiểm tra lại connection string |
| Lỗi CHECK constraint Role | Role phải thuộc: Admin, Employer, Candidate |
| HTTP 500 khi đăng ký | Chưa chạy đúng file SQL |
| Login sai mật khẩu ngay cả khi đúng | DB chưa bật `IsApproved` nếu là Employer |

---

## 📚 Cấu trúc thư mục

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

## ✅ Chức năng chính

- Đăng ký, đăng nhập, đăng xuất
- Admin quản lý users
- Nhà tuyển dụng đăng tin
- Ứng viên ứng tuyển
- Duyệt tài khoản Employer

---

Chúc bạn chạy web ngon lành. Nếu gặp khó khăn cứ báo cho mình!
